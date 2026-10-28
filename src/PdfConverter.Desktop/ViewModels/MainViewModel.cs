using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfConverter.Desktop.Services;
using PdfConverter.Domain;
using PdfConverter.UseCases;

namespace PdfConverter.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const int Dpi = 150;
    private const int JpegQuality = 90;

    private readonly ConvertPdfFilesUseCase _useCase;
    private readonly IDialogService _dialogs;
    private CancellationTokenSource? _cts;

    public MainViewModel(ConvertPdfFilesUseCase useCase, IDialogService dialogs)
    {
        _useCase = useCase;
        _dialogs = dialogs;
    }

    public ObservableCollection<PdfItemViewModel> Files { get; } = new();
    public OutputFormat[] Formats { get; } = Enum.GetValues<OutputFormat>();

    [ObservableProperty] private OutputFormat _selectedFormat = OutputFormat.Png;
    [ObservableProperty] private string? _outputDirectory;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConvertAllCommand))]
    [NotifyCanExecuteChangedFor(nameof(ConvertSelectedCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddFilesCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveSelectedCommand))]
    [NotifyCanExecuteChangedFor(nameof(ClearCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isBusy;

    private bool CanEdit() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void AddFiles()
    {
        foreach (var path in _dialogs.PickPdfFiles())
        {
            if (!Files.Any(f => string.Equals(f.FullPath, path, StringComparison.OrdinalIgnoreCase)))
                Files.Add(new PdfItemViewModel(path));
        }
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void RemoveSelected(IList? selected)
    {
        foreach (var item in selected?.Cast<PdfItemViewModel>().ToList() ?? new())
            Files.Remove(item);
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Clear() => Files.Clear();

    [RelayCommand]
    private void BrowseOutput()
    {
        var folder = _dialogs.PickFolder();
        if (folder is not null) OutputDirectory = folder;
    }

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private Task ConvertAllAsync() => RunAsync(Files.ToList());

    [RelayCommand(CanExecute = nameof(CanEdit))]
    private Task ConvertSelectedAsync(IList? selected)
        => RunAsync(selected?.Cast<PdfItemViewModel>().ToList() ?? new());

    [RelayCommand(CanExecute = nameof(IsBusy))]
    private void Cancel() => _cts?.Cancel();

    private async Task RunAsync(List<PdfItemViewModel> items)
    {
        if (items.Count == 0) return;

        IsBusy = true;
        _cts = new CancellationTokenSource();

        foreach (var i in items) { i.Status = ConversionStatus.Pending; i.Message = null; }

        var lookup = items.ToDictionary(i => i.FullPath, StringComparer.OrdinalIgnoreCase);
        var progress = new Progress<ConversionUpdate>(u =>
        {
            if (!lookup.TryGetValue(u.PdfPath, out var item)) return;
            item.Status = u.Status;
            item.Message = u.Message;
        });

        var options = new ConversionOptions(SelectedFormat, OutputDirectory, Dpi, JpegQuality);

        try
        {
            await _useCase.ExecuteAsync(items.Select(i => i.FullPath).ToList(), options, progress, _cts.Token);
        }
        catch (OperationCanceledException) { /* remaining items simply stay Pending */ }
        finally
        {
            _cts.Dispose();
            _cts = null;
            IsBusy = false;
        }
    }
}
