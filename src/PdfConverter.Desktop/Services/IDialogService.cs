namespace PdfConverter.Desktop.Services;

public interface IDialogService
{
    string[] PickPdfFiles();
    string? PickFolder();
}
