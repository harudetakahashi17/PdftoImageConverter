namespace PdfConverter.UseCases;

/// <summary>Port implemented by the infrastructure layer.</summary>
public interface IPdfImageConverter
{
    /// <returns>Number of pages written.</returns>
    Task<int> ConvertAsync(string pdfPath, string outputDirectory, ConversionOptions options, CancellationToken ct);
}
