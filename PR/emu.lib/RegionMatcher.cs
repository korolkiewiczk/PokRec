using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace emu.lib
{
    public class RegionMatcher
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Custom class to store image scores
        private class WeightedImages : IComparable<WeightedImages>
        {
            public string ImagePath { get; set; }
            public double Score { get; set; } // Changed to double for precision

            public int CompareTo(WeightedImages other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;

                return Score.CompareTo(other.Score);
            }
        }

        // A List<> which contains processed images scores
        private readonly List<WeightedImages> _imgList = new List<WeightedImages>();

        private readonly string _regionPath;
        private readonly string _regionName;
        private readonly Image<Rgba32> _mainImage;
        private readonly string _classPath;
        private readonly int _take;
        private readonly int _threshold;
        private readonly List<string> _toCheckFirst;
        private readonly bool _isOcr;

        public RegionMatcher(string regionPath, string regionName, Image<Rgba32> mainImage, string classPath, int take,
            int threshold, List<string> toCheckFirst, bool isOcr = false)
        {
            _regionPath = regionPath;
            _regionName = regionName;
            _mainImage = mainImage;
            _classPath = classPath;
            _take = take;
            _threshold = threshold;
            _toCheckFirst = toCheckFirst;
            _isOcr = isOcr;
        }

        public List<string> Process()
        {
            try
            {
                GenerateRegionImage();

                if (_isOcr)
                {
                    Stopwatch swOcr = new Stopwatch();
                    swOcr.Start();
                    using var image = Image.Load(RegionFilename).CloneAs<Rgba32>();
                    string recognizedText = CVUtilsOcr.GetTextFromRegion(image);
                    swOcr.Stop();
                    log.Info($"OCR result '{recognizedText}' in {swOcr.ElapsedMilliseconds} ms");
                    return !string.IsNullOrWhiteSpace(recognizedText)
                        ? new List<string> {recognizedText}
                        : [];
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();
                ProcessFolder();
                sw.Stop();

                log.Info(
                    $"{string.Join(";", _imgList.Select(x => $"{Path.GetFileNameWithoutExtension(x.ImagePath)} {x.Score}"))} in {sw.ElapsedMilliseconds} ms");

                var result = _imgList.OrderByDescending(x => x.Score)
                    .Where(x => x.Score > _threshold)
                    .Take(_take)
                    .Select(x => Path.GetFileNameWithoutExtension(x.ImagePath))
                    .ToList();

                return result.Any() ? result : [];
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return [];
            }
        }

        private void ProcessFolder()
        {
            bool checkFurther = true;
            if (_toCheckFirst != null && _toCheckFirst.Any())
            {
                foreach (var classImage in _toCheckFirst.Select(x => Path.Combine(_classPath, x)))
                {
                    if (!ProcessImage(RegionFilename, classImage + ".png")) break;
                }

                if (_imgList.All(x => Math.Abs(x.Score - 100) < 1))
                {
                    checkFurther = false;
                }
            }

            if (checkFurther)
            {
                _imgList.Clear();
                foreach (var classImage in Directory.GetFiles(_classPath))
                {
                    if (!ProcessImage(RegionFilename, classImage)) break;
                }
            }
        }

        /// <summary>
        /// Process single image: calculate score then add the occurrence to imgList List<WeightedImage>
        /// </summary>
        private bool ProcessImage(string mainImagePath, string classImagePath)
        {
            if (classImagePath == mainImagePath) return true;

            // Load images using Emgu CV
            using (var mainImage = new Image<Gray, byte>(mainImagePath))
            using (var classImageFull = new Image<Gray, byte>(classImagePath))
            {
                // Calculate the number of extra columns to ignore (5% of the image width)
                int numExtraColumns = (int) (classImageFull.Width * 0.05);

                // Ensure that cropping does not result in an invalid region
                if (numExtraColumns >= classImageFull.Width)
                {
                    // Handle the error appropriately, e.g., skip processing or throw an exception
                    return true; // Or handle accordingly
                }

                // Define the cropping rectangle to exclude the left 5% of the class image
                Rectangle cropRect = new Rectangle(numExtraColumns, 0, classImageFull.Width - numExtraColumns,
                    classImageFull.Height);

                // Crop the template image
                using (var classImage = classImageFull.Copy(cropRect))
                {
                    // Ensure template size is smaller than source image
                    if (classImage.Width > mainImage.Width || classImage.Height > mainImage.Height)
                        return true; // Or handle accordingly

                    // Use template matching
                    var result = mainImage.MatchTemplate(classImage, TemplateMatchingType.CcoeffNormed);

                    // Find the best match position
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    // The maximum value corresponds to the best match
                    double similarity = maxValues[0];

                    // Only add to list if above threshold
                    if (similarity * 100 >= _threshold)
                    {
                        _imgList.Add(new WeightedImages
                        {
                            ImagePath = classImagePath,
                            Score = similarity * 100 // Convert to percentage
                        });
                    }
                }
            }

            return true; // Always continue processing
        }

        private bool ProcessImage3(string mainImagePath, string classImagePath)
        {
            using (var mainImage = new Image<Gray, byte>(mainImagePath))
            using (var classImage = new Image<Gray, byte>(classImagePath))
            {
                // Initialize ORB detector
                var orb = new ORB();

                // Containers for keypoints and descriptors
                var keyPointsMain = new VectorOfKeyPoint();
                var keyPointsClass = new VectorOfKeyPoint();
                var descriptorsMain = new Mat();
                var descriptorsClass = new Mat();

                // Detect and compute keypoints and descriptors
                orb.DetectAndCompute(mainImage, null, keyPointsMain, descriptorsMain, false);
                orb.DetectAndCompute(classImage, null, keyPointsClass, descriptorsClass, false);

                // Check for empty descriptors
                if (descriptorsMain.IsEmpty || descriptorsMain.Rows == 0 || descriptorsMain.Cols == 0 ||
                    descriptorsClass.IsEmpty || descriptorsClass.Rows == 0 || descriptorsClass.Cols == 0)
                {
                    Console.WriteLine("One or both descriptor matrices are empty. Skipping matching.");
                    return false;
                }

                // Convert descriptors to the correct type if needed
                if (descriptorsMain.Depth != Emgu.CV.CvEnum.DepthType.Cv8U)
                {
                    var temp = new Mat();
                    descriptorsMain.ConvertTo(temp, Emgu.CV.CvEnum.DepthType.Cv8U);
                    descriptorsMain = temp;
                }

                if (descriptorsClass.Depth != Emgu.CV.CvEnum.DepthType.Cv8U)
                {
                    var temp = new Mat();
                    descriptorsClass.ConvertTo(temp, Emgu.CV.CvEnum.DepthType.Cv8U);
                    descriptorsClass = temp;
                }

                // Initialize BFMatcher with Hamming distance
                var matcher = new BFMatcher(DistanceType.Hamming);

                // Match descriptors
                var matches = new VectorOfDMatch();
                matcher.Match(descriptorsMain, descriptorsClass, matches);

                // Optional: Filter matches by distance
                double maxDist = matches.ToArray().Max(m => m.Distance);
                var goodMatches = matches.ToArray()
                    .Where(m => m.Distance < 0.2 * maxDist) // Adjust threshold
                    .ToList();

                // Check if there are enough good matches
                if (goodMatches.Count >= 20) // Define _minMatches
                {
                    _imgList.Add(new WeightedImages
                    {
                        ImagePath = classImagePath,
                        Score = (double) goodMatches.Count / matches.Size * 100 // Match percentage
                    });
                }
            }

            return true;
        }


        /// <summary>
        /// Process single image: calculate score then add the occurrence to imgList List<WeightedImage>
        /// </summary>
        private bool ProcessImage2(string mainImagePath, string classImagePath)
        {
            if (classImagePath == mainImagePath) return true;

            // Load images using Emgu CV
            using (var m = new Image<Gray, byte>(mainImagePath))
            using (var c = new Image<Gray, byte>(classImagePath))
            {
                // Ensure template size is smaller than source image
                if (c.Width > m.Width || c.Height > m.Height)
                    return true; // Or handle accordingly

                // Use template matching
                var result = m.MatchTemplate(c, TemplateMatchingType.CcoeffNormed);

                // Find the best match position
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                // The maximum value corresponds to the best match
                double similarity = maxValues[0];

                // Only add to list if above abandon threshold, but don't stop processing
                if (similarity * 100 >= _threshold)
                {
                    _imgList.Add(new WeightedImages
                    {
                        ImagePath = classImagePath,
                        Score = similarity * 100 // Convert to percentage
                    });
                }
            }

            return true; // Always continue processing
        }

        private void GenerateRegionImage()
        {
            // Read the rectangle dimensions from the file
            var rectangle = (Rectangle)new RectangleConverter().ConvertFromString(
                File.ReadAllText($"{Path.Combine(_regionPath, _regionName)}.txt"));

            // Create a new blank image with the dimensions of the rectangle
            using (var regionImage = new Image<Rgba32>(rectangle.Width, rectangle.Height))
            {
                // Crop the region from the main image and draw it on the regionImage
                var sourceRectangle = new SixLabors.ImageSharp.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

                regionImage.Mutate(ctx => ctx
                    .DrawImage(_mainImage,new SixLabors.ImageSharp.Point(0, 0), sourceRectangle, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcOver, 1.0f));

                // Save the result to a file
                regionImage.Save(RegionFilename);
            }
        }

        private string RegionFilename
        {
            get { return _regionName + ".png"; }
        }
    }
}