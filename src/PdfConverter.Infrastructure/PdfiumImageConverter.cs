using PDFtoImage;
using PdfConverter.Domain;
using PdfConverter.UseCases;
using SkiaSharp;

namespace PdfConverter.Infrastructure;

public sealed class PdfiumImageConverter : IPdfImageConverter
{
    public Task<int> ConvertAsync(string pdfPath, string outputDirectory, ConversionOptions options, CancellationToken ct)
        => Task.Run(() =>
        {
            Directory.CreateDirectory(outputDirectory);

            var bytes = File.ReadAllBytes(pdfPath);
            var name = Path.GetFileNameWithoutExtension(pdfPath);

            // A fresh stream per call: the library disposes the stream it is given.
            var pageCount = Conversion.GetPageCount(new MemoryStream(bytes));

            var isPng = options.Format == OutputFormat.Png;
            var ext = isPng ? "png" : "jpg";
            var skFormat = isPng ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
            var digits = Math.Max(3, pageCount.ToString().Length);

            var render = new RenderOptions(
                Dpi: options.Dpi,
                WithAnnotations: true,
                BackgroundColor: SKColors.White);

            for (var i = 0; i < pageCount; i++)
            {
                ct.ThrowIfCancellationRequested();

                // Pass a System.Index (not an int) so the non-obsolete overload is used.
                using var rendered = Conversion.ToImage(new MemoryStream(bytes), new Index(i), options: render);

                // View the same pixels as fully opaque so a stray alpha channel from PDFium
                // can never make the page look blank (PNG) or black (JPG).
                using var opaque = new SKBitmap();
                var info = new SKImageInfo(rendered.Width, rendered.Height, rendered.ColorType, SKAlphaType.Opaque);
                if (!opaque.InstallPixels(info, rendered.GetPixels(), rendered.RowBytes))
                    throw new InvalidOperationException($"Could not read rendered page {i + 1}.");

                var file = Path.Combine(outputDirectory, $"{name}_page_{(i + 1).ToString().PadLeft(digits, '0')}.{ext}");
                using var fs = File.Create(file);
                if (!opaque.Encode(fs, skFormat, options.JpegQuality))
                    throw new InvalidOperationException($"Could not encode page {i + 1}.");
            }

            return pageCount;
        }, ct);
}
