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
    private static Tesseract _numericTesseract;
    private static readonly object _tesseractLock = new();

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
        return GetTesseract(ref _tesseract, false);
    }

    private static Tesseract GetNumericTesseract()
    {
        return GetTesseract(ref _numericTesseract, true);
    }

    private static Tesseract GetTesseract(ref Tesseract tesseract, bool isNumericOnly)
    {
        if (tesseract != null) return tesseract;

        lock (_tesseractLock)
        {
            if (tesseract != null) return tesseract;

            string tessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

            if (!Directory.Exists(tessdataPath))
            {
                Directory.CreateDirectory(tessdataPath);
                throw new DirectoryNotFoundException(
                    $"Tessdata directory not found at: {tessdataPath}\n" +
                    "Please download eng.traineddata and place it in this directory."
                );
            }

            tesseract = new Tesseract();
            tesseract.Init(tessdataPath, "eng", OcrEngineMode.TesseractLstmCombined);
            tesseract.SetVariable("user_defined_dpi", "70");

            if (isNumericOnly)
            {
                tesseract.SetVariable("tessedit_char_whitelist", "0123456789.$,");
            }
            else
            {
                tesseract.SetVariable("tessedit_char_whitelist",
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.$,");
            }

            tesseract.PageSegMode = PageSegMode.SingleWord;

            return tesseract;
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