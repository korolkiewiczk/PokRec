using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib;
public static class CVUtilsOcr
{
    // Static cache for Tesseract instance
    private static Tesseract _tesseract;
    private static readonly object _tesseractLock = new();

    // Cache for OCR results using a simple dictionary

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
            _tesseract.Init(tessdataPath, "eng", OcrEngineMode.TesseractLstmCombined);

            return _tesseract;
        }
    }

    public static string GetTextFromRegion(Image<Rgba32> region, Rectangle regionRect = new Rectangle())
    {
        // If not in cache, perform OCR
        if (regionRect.IsEmpty)
        {
            regionRect = new Rectangle(0, 0, region.Width, region.Height);
        }
       
        string result;
        using (Mat sourceMat = ConvertImageSharpToMat(region))
        using (Mat regionMat = new Mat(sourceMat, regionRect))
        {
            var tesseract = GetTesseractInstance();
            tesseract.SetImage(regionMat);
            tesseract.Recognize();
            result = tesseract.GetUTF8Text().Trim();
        }

        return result;
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