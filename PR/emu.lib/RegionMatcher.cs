using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
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

        private class WeightedImages : IComparable<WeightedImages>
        {
            public string ImagePath { get; set; }
            public double Score { get; set; }

            public int CompareTo(WeightedImages other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;

                return Score.CompareTo(other.Score);
            }
        }

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
                var regionImage = GenerateRegionImage();

                if (_isOcr)
                {
                    var swOcr = new Stopwatch();
                    swOcr.Start();
                    var recognizedText = CVUtilsOcr.GetTextFromRegion(regionImage);
                    swOcr.Stop();
                    log.Info($"OCR result '{recognizedText}' in {swOcr.ElapsedMilliseconds} ms");
                    return !string.IsNullOrWhiteSpace(recognizedText)
                        ? [recognizedText]
                        : [];
                }

                var sw = new Stopwatch();
                sw.Start();
                ProcessFolder(regionImage);
                sw.Stop();

                log.Info(
                    $"{string.Join(";", _imgList.Select(x => $"{Path.GetFileNameWithoutExtension(x.ImagePath)} {x.Score}"))} in {sw.ElapsedMilliseconds} ms");

                var result = _imgList.OrderByDescending(x => x.Score)
                    .Where(x => x.Score > _threshold)
                    .Take(_take)
                    .Select(x => Path.GetFileNameWithoutExtension(x.ImagePath))
                    .ToList();

                return result.Any() ? result : new List<string>();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return [];
            }
        }

        private void ProcessFolder(Image<Rgba32> regionImage)
        {
            var checkFurther = true;
            if (_toCheckFirst != null && _toCheckFirst.Any())
            {
                foreach (var classImage in _toCheckFirst.Select(x => Path.Combine(_classPath, x)))
                {
                    if (!ProcessImage(regionImage, classImage + ".png")) break;
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
                    if (!ProcessImage(regionImage, classImage)) break;
                }
            }
        }
        
        private Image<Gray, byte> ConvertToEmguCvGray(Image<Rgba32> imageSharp)
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
                        var grayValue = (byte)(
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

        private bool ProcessImage(Image<Rgba32> regionImage, string classImagePath)
        {
            using var regionImageCv = ConvertToEmguCvGray(regionImage);
            // Load class image using Emgu CV
            using var classImageFull = new Image<Gray, byte>(classImagePath);
            
            var numExtraColumns = (int) (classImageFull.Width * 0.05);

            if (numExtraColumns >= classImageFull.Width)
            {
                return true;
            }

            var cropRect = new Rectangle(numExtraColumns, 0, classImageFull.Width - numExtraColumns,
                classImageFull.Height);

            using var classImage = classImageFull.Copy(cropRect);
            
            if (classImage.Width > regionImage.Width || classImage.Height > regionImage.Height)
                return true;

            var result = regionImageCv.MatchTemplate(classImage, TemplateMatchingType.CcoeffNormed);

            result.MinMax(out _, out var maxValues, out _, out _);

            var similarity = maxValues[0];

            if (similarity * 100 >= _threshold)
            {
                _imgList.Add(new WeightedImages
                {
                    ImagePath = classImagePath,
                    Score = similarity * 100
                });
            }

            return true;
        }

        private Image<Rgba32> GenerateRegionImage()
        {
            var rectangle = (Rectangle)new RectangleConverter().ConvertFromString(
                File.ReadAllText($"{Path.Combine(_regionPath, _regionName)}.txt"))!;

            var regionImage = new Image<Rgba32>(rectangle.Width, rectangle.Height);
            var sourceRectangle = new SixLabors.ImageSharp.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

            regionImage.Mutate(ctx => ctx
                .DrawImage(_mainImage, new SixLabors.ImageSharp.Point(0, 0), sourceRectangle, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcOver, 1.0f));

            return regionImage;
        }
    }
}