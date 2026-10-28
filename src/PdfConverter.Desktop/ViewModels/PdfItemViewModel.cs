using CommunityToolkit.Mvvm.ComponentModel;
using PdfConverter.Domain;

namespace PdfConverter.Desktop.ViewModels;

public partial class PdfItemViewModel : ObservableObject
{
    public PdfItemViewModel(string fullPath)
    {
        FullPath = fullPath;
        Name = System.IO.Path.GetFileName(fullPath);
    }

    public string FullPath { get; }
    public string Name { get; }

    [ObservableProperty] private ConversionStatus _status = ConversionStatus.Pending;
    [ObservableProperty] private string? _message;
}
