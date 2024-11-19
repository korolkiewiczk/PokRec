using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Image = System.Drawing.Image;

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
        private readonly Image _mainImage;
        private readonly string _classPath;
        private readonly int _take;
        private readonly int _threshold;
        private readonly List<string> _toCheckFirst;

        public RegionMatcher(string regionPath, string regionName, Image mainImage, string classPath, int take,
            int threshold, List<string> toCheckFirst)
        {
            _regionPath = regionPath;
            _regionName = regionName;
            _mainImage = mainImage;
            _classPath = classPath;
            _take = take;
            _threshold = threshold;
            _toCheckFirst = toCheckFirst;
        }

        public List<string> Process()
        {
            try
            {
                GenerateRegionImage();
                Stopwatch sw = new Stopwatch();

                sw.Start();
                ProcessFolder();
                sw.Stop();

                log.Info($"{string.Join(";", _imgList.Select(x => $"{Path.GetFileNameWithoutExtension(x.ImagePath)} {x.Score}"))} in {sw.ElapsedMilliseconds} ms");

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

        private void ProcessFolder()
        {
            bool checkFurther = true;
            if (_toCheckFirst != null && _toCheckFirst.Any())
            {
                foreach (var classImage in _toCheckFirst.Select(x => Path.Combine(_classPath, x)))
                {
                    if (!ProcessImage(RegionFilename, classImage + ".png")) break;
                }

                if (_imgList.All(x => x.Score == 100))
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

            return true;  // Always continue processing
        }

        private void GenerateRegionImage()
        {
            Rectangle rectangle =
                (Rectangle)new RectangleConverter().ConvertFromString(
                    File.ReadAllText($"{Path.Combine(_regionPath, _regionName)}.txt"));

            using (Bitmap bm = new(rectangle.Width, rectangle.Height))
            {
                using (var mainGraphics = Graphics.FromImage(bm))
                {
                    mainGraphics.DrawImage(_mainImage, new Rectangle(0, 0, rectangle.Width, rectangle.Height),
                        rectangle, GraphicsUnit.Pixel);
                }

                bm.Save(RegionFilename);
            }
        }

        private string RegionFilename
        {
            get { return _regionName + ".png"; }
        }
    }
}