using Microsoft.Win32;

namespace PdfConverter.Desktop.Services;

public sealed class DialogService : IDialogService
{
    public string[] PickPdfFiles()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select PDF files",
            Filter = "PDF files (*.pdf)|*.pdf",
            Multiselect = true
        };
        return dlg.ShowDialog() == true ? dlg.FileNames : Array.Empty<string>();
    }

    public string? PickFolder()
    {
        var dlg = new OpenFolderDialog { Title = "Select output folder" };
        return dlg.ShowDialog() == true ? dlg.FolderName : null;
    }
}
