using PdfConverter.Domain;

namespace PdfConverter.UseCases;

/// <param name="OutputDirectory">Null/empty = next to each source PDF.</param>
public sealed record ConversionOptions(
    OutputFormat Format,
    string? OutputDirectory,
    int Dpi = 150,
    int JpegQuality = 90);

public sealed record ConversionUpdate(string PdfPath, ConversionStatus Status, string? Message = null);
