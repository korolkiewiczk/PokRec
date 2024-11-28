using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib;
public static class CVUtilsOcr
{
    // Static cache for Tesseract instance
    private static Tesseract _tesseract;
    private static readonly object _tesseractLock = new object();

    // Cache for OCR results using a simple dictionary
    private static readonly Dictionary<string, string> _ocrCache = new();
    private static readonly object _cacheLock = new object();
    private const int MAX_CACHE_SIZE = 1000; // Adjust based on your needs

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
        // // Generate a cache key based on the bitmap data and region
        // string cacheKey = GenerateCacheKey(region, regionRect);

        // // Check cache first
        // lock (_cacheLock)
        // {
        //     if (_ocrCache.TryGetValue(cacheKey, out string cachedResult))
        //     {
        //         return cachedResult;
        //     }
        // }

        string result;
        using (Mat sourceMat = ConvertImageSharpToMat(region))
        using (Mat regionMat = new Mat(sourceMat, regionRect))
        {
            var tesseract = GetTesseractInstance();
            tesseract.SetImage(regionMat);
            tesseract.Recognize();
            result = tesseract.GetUTF8Text().Trim();
        }

        // if (!string.IsNullOrWhiteSpace(result))
        // {
        //     // Add to cache
        //     lock (_cacheLock)
        //     {
        //         // Implement simple cache eviction if needed
        //         if (_ocrCache.Count >= MAX_CACHE_SIZE)
        //         {
        //             _ocrCache.Clear(); // Simple strategy: clear all when full
        //         }
        //         _ocrCache[cacheKey] = result;
        //     }
        // }

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

    private static string GenerateCacheKey(Bitmap bitmap, Rectangle rect)
    {
        // Generate a more robust hash from the bitmap data and rectangle
        using (var ms = new MemoryStream())
        {
            // Lock bitmap bits to access raw pixel data
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bitmap.PixelFormat);
            
            try
            {
                // Create hash of raw pixel data
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] pixelData = new byte[bitmapData.Stride * bitmap.Height];
                    System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);
                    byte[] hash = sha256.ComputeHash(pixelData);
                    return $"{Convert.ToBase64String(hash)}_{rect.X}_{rect.Y}_{rect.Width}_{rect.Height}";
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}