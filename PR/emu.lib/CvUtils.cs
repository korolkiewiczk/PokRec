using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace emu.lib;

public static class CvUtils
{
    public static Image<Gray, byte> ConvertToEmguCvGray(Image<Rgba32> imageSharp)
    {
        var width = imageSharp.Width;
        var height = imageSharp.Height;

        // Initialize the 3D array for Emgu CV
        var data = new byte[height, width, 1];

        // Process each row using ProcessPixelRows
        imageSharp.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);

                for (var x = 0; x < width; x++)
                {
                    var pixel = pixelRow[x];

                    // Convert to grayscale
                    var grayValue = (byte) (
                        0.299f * pixel.R +
                        0.587f * pixel.G +
                        0.114f * pixel.B
                    );

                    data[y, x, 0] = grayValue;
                }
            }
        });

        // Create the Emgu CV image
        return new Image<Gray, byte>(data);
    }

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
                return bestFit with {X = initialSelection.X + bestFit.X, Y = initialSelection.Y + bestFit.Y};
            }
        }

        // Return original selection if no better fit found
        return initialSelection;
    }

    public static Image<Rgba32> GenerateRegionImage(Rectangle rectangle, Image<Rgba32> mainImage)
    {
        var regionImage = new Image<Rgba32>(rectangle.Width, rectangle.Height);
        var sourceRectangle =
            new SixLabors.ImageSharp.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        regionImage.Mutate(ctx => ctx
            .DrawImage(mainImage, new Point(0, 0), sourceRectangle, PixelColorBlendingMode.Normal,
                PixelAlphaCompositionMode.SrcOver, 1.0f));

        return regionImage;
    }
}