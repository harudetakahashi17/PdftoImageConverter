using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PdfConverter.Desktop.Services;
using PdfConverter.Desktop.ViewModels;
using PdfConverter.Infrastructure;
using PdfConverter.UseCases;

namespace PdfConverter.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Composition root: the only place that knows every layer.
        var services = new ServiceCollection();
        services.AddSingleton<IPdfImageConverter, PdfiumImageConverter>();
        services.AddSingleton<ConvertPdfFilesUseCase>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<MainViewModel>();
        var provider = services.BuildServiceProvider();

        new MainWindow { DataContext = provider.GetRequiredService<MainViewModel>() }.Show();
    }
}
