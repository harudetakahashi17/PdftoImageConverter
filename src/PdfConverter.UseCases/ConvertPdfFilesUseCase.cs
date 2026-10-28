using PdfConverter.Domain;

namespace PdfConverter.UseCases;

public sealed class ConvertPdfFilesUseCase
{
    private readonly IPdfImageConverter _converter;

    public ConvertPdfFilesUseCase(IPdfImageConverter converter) => _converter = converter;

    public async Task ExecuteAsync(
        IReadOnlyList<string> pdfPaths,
        ConversionOptions options,
        IProgress<ConversionUpdate> progress,
        CancellationToken ct)
    {
        foreach (var pdf in pdfPaths)
        {
            ct.ThrowIfCancellationRequested();
            progress.Report(new(pdf, ConversionStatus.Processing));

            try
            {
                var baseDir = string.IsNullOrWhiteSpace(options.OutputDirectory)
                    ? Path.GetDirectoryName(pdf)!
                    : options.OutputDirectory;
                var target = Path.Combine(baseDir, Path.GetFileNameWithoutExtension(pdf));

                var pages = await _converter.ConvertAsync(pdf, target, options, ct);
                progress.Report(new(pdf, ConversionStatus.Completed, $"{pages} page(s) saved"));
            }
            catch (OperationCanceledException)
            {
                progress.Report(new(pdf, ConversionStatus.Pending, "Cancelled"));
                throw;
            }
            catch (Exception ex)
            {
                progress.Report(new(pdf, ConversionStatus.Failed, ex.Message));
            }
        }
    }
}
