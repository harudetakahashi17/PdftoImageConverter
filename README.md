# PDF to Image Converter (WPF, .NET 10, offline)

Layers (dependencies point inward):
  Desktop (WPF/MVVM, composition root) -> UseCases -> Domain
  Infrastructure (PDFtoImage/PDFium + SkiaSharp) -> UseCases

Run:      dotnet run --project src/PdfConverter.Desktop
Portable: dotnet publish src/PdfConverter.Desktop -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
Solution:  open PdfConverter.sln in Visual Studio, or run `dotnet build PdfConverter.slnx`
Internet is only needed once, for `dotnet restore`.

# Feature

- 100% Offline
- Convert single or bulk
- Free

# Screenshot Sample

<img width="809" height="505" alt="Screenshot_1" src="https://github.com/user-attachments/assets/6c102250-cc0f-4bb4-a2d3-bd4c9c43ce90" />

# Note

This program cannot convert protected PDF. You may modify the code to your will.
