using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib;

public static class CvUtilsOcr
{
    // Static cache for Tesseract instance
    private static Tesseract _tesseract;
    private static readonly object _tesseractLock = new();

    private static Tesseract _numericTesseract;

    public static string GetTextFromRegion(Image<Rgba32> region, bool isNumericOnly, Rectangle regionRect = new())
    {
        if (regionRect.IsEmpty)
        {
            regionRect = new Rectangle(0, 0, region.Width, region.Height);
        }

        string result;
        using Mat sourceMat = ConvertImageSharpToMat(region);
        using Mat regionMat = new Mat(sourceMat, regionRect);
        lock (_tesseractLock)
        {
            var tesseract = isNumericOnly ? GetNumericTesseract() : GetTesseractInstance();
            tesseract.SetImage(regionMat);
            tesseract.Recognize();
            result = tesseract.GetUTF8Text().Trim();
        }

        return result;
    }

    private static Tesseract GetTesseractInstance()
    {
        if (_tesseract != null) return _tesseract;

        lock (_tesseractLock)
        {
            if (_tesseract != null) return _tesseract;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string tessdataPath = Path.Combine(baseDirectory, "tessdata");

            if (!Directory.Exists(tessdataPath))
            {
                Directory.CreateDirectory(tessdataPath);
                throw new DirectoryNotFoundException(
                    $"Tessdata directory not found at: {tessdataPath}\n" +
                    "Please download eng.traineddata and place it in this directory."
                );
            }

            _tesseract = new Tesseract();
            _tesseract.SetVariable("user_defined_dpi", "70");
            _numericTesseract.PageSegMode = PageSegMode.SingleLine;
            _tesseract.Init(tessdataPath, "eng", OcrEngineMode.TesseractLstmCombined);


            return _tesseract;
        }
    }

    private static Tesseract GetNumericTesseract()
    {
        if (_numericTesseract != null) return _numericTesseract;

        lock (_tesseractLock)
        {
            if (_numericTesseract != null) return _numericTesseract;

            // Initialize with same path, but set numeric-specific vars
            string tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
            _numericTesseract = new Tesseract();
            _numericTesseract.Init(tessdataPath, "eng", OcrEngineMode.TesseractLstmCombined);

            // Maybe set DPI or any other variable
            _numericTesseract.SetVariable("user_defined_dpi", "70");
            // Restrict to digits
            _numericTesseract.SetVariable("tessedit_char_whitelist", "0123456789.$,");
            _numericTesseract.PageSegMode = PageSegMode.SingleLine;

            return _numericTesseract;
        }
    }

    private static Mat ConvertImageSharpToMat(Image<Rgba32> image)
    {
        int width = image.Width;
        int height = image.Height;

        // Prepare a byte array to store pixel data
        byte[] imageData = new byte[width * height * 4]; // 4 bytes per pixel for Rgba32

        // Copy pixel data to byte array
        image.CopyPixelDataTo(imageData);

        // Create an Emgu CV Mat with the RGBA data
        Mat mat = new Mat(height, width, DepthType.Cv8U, 4); // 8-bit, 4-channel

        // Set the pixel data to the Mat
        mat.SetTo(imageData);

        return mat;
    }
}