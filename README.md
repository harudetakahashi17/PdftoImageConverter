# PDF to Image Converter (WPF, .NET 10, offline)

Layers (dependencies point inward):
  Desktop (WPF/MVVM, composition root) -> UseCases -> Domain
  Infrastructure (PDFtoImage/PDFium + SkiaSharp) -> UseCases

Run:      dotnet run --project src/PdfConverter.Desktop
Portable: dotnet publish src/PdfConverter.Desktop -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
Solution:  open PdfConverter.sln in Visual Studio, or run `dotnet build PdfConverter.slnx`
Internet is only needed once, for `dotnet restore`.
