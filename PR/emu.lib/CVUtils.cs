using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.OCR;

namespace emu.lib;
public static class CVUtils
{
    public static Rectangle GetRefinedRectangle(Bitmap selectedRegion, Rectangle initialSelection)
    {
        // Convert the selected region to Mat format
        using (Mat sourceMat = selectedRegion.ToMat())
        using (Mat grayMat = new Mat())
        using (Mat edgesMat = new Mat())
        {
            // Convert to grayscale
            CvInvoke.CvtColor(sourceMat, grayMat, ColorConversion.Bgr2Gray);

            // Apply Gaussian blur to reduce noise
            CvInvoke.GaussianBlur(grayMat, grayMat, new Size(3, 3), 0);

            // Apply Canny edge detection
            CvInvoke.Canny(grayMat, edgesMat, 100, 200);

            // Find contours
            using VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(edgesMat, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Find the largest contour (likely our main object)
            if (contours.Size > 0)
            {
                double maxArea = 0;
                Rectangle bestFit = initialSelection;

                for (int i = 0; i < contours.Size; i++)
                {
                    Rectangle bounds = CvInvoke.BoundingRectangle(contours[i]);
                    double area = bounds.Width * bounds.Height;
                    if (area > maxArea)
                    {
                        maxArea = area;
                        bestFit = bounds;
                    }
                }

                // Calculate the differences and apply them to the initial selection
                return new Rectangle(
                    initialSelection.X + bestFit.X,
                    initialSelection.Y + bestFit.Y,
                    bestFit.Width,
                    bestFit.Height
                );
            }
        }

        // Return original selection if no better fit found
        return initialSelection;
    }

    public static string GetTextFromRegion(Bitmap region, Rectangle regionRect)
    {
        using (var tesseract = new Tesseract())
        {
            // Initialize tesseract with English language
            // You'll need to provide the path to tessdata
            tesseract.SetVariable("user_defined_dpi", "70");
            tesseract.Init("path_to_tessdata", "eng", OcrEngineMode.TesseractLstmCombined);
            
            // Convert Bitmap to Mat and extract region
            using (Mat sourceMat = region.ToMat())
            using (Mat regionMat = new Mat(sourceMat, regionRect))
            {
                tesseract.SetImage(regionMat);
                tesseract.Recognize();
                
                return tesseract.GetUTF8Text().Trim();
            }
        }
    }
}