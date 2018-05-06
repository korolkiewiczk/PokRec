using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AForge.Imaging;
using Image = System.Drawing.Image;

namespace emu
{
    public class RegionMatcher
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Custom class to store image scores
        private class WeightedImages : IComparable<WeightedImages>
        {
            public string ImagePath { get; set; }
            public long Score { get; set; }

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

        public RegionMatcher(string regionPath, string regionName, Image mainImage, string classPath, int take,
            int threshold)
        {
            _regionPath = regionPath;
            _regionName = regionName;
            _mainImage = mainImage;
            _classPath = classPath;
            _take = take;
            _threshold = threshold;
        }

        public List<string> Process()
        {
            GenerateRegionImage();
            Stopwatch sw=new Stopwatch();
            
            sw.Start();
            ProcessFolder(_classPath, RegionFilename);
            sw.Stop();

            log.Info(string.Join(";",
                         _imgList.Select(x => Path.GetFileNameWithoutExtension(x.ImagePath) + " " + x.Score)) + " in " +
                     sw.ElapsedMilliseconds);

            var result = _imgList.OrderByDescending(x => x.Score)
                .Where(x => x.Score > _threshold)
                .Take(_take)
                .Select(x => Path.GetFileNameWithoutExtension(x.ImagePath))
                .ToList();

            if (!result.Any())
            {
                return new List<string>();
            }
            else
            {
                return result;
            }
        }

        private void ProcessFolder(string classesFolder, string mainImage)
        {
            foreach (var classImage in Directory.GetFiles(classesFolder))
                ProcessImage(mainImage, classImage);
        }

        /// <summary>
        /// Process single image: calculate score then add the occurence to imgList List<WeightedImage>
        /// </summary>
        private void ProcessImage(string mainImage, string classImage)
        {
            if (classImage == mainImage) return;

            float similarityThreshold = 0.0f;
            
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(similarityThreshold);

            // Compare two images
            var m = Image.FromFile(mainImage);
            var c = Image.FromFile(classImage);
            TemplateMatch[] matchings = tm.ProcessImage(
                new Bitmap(m).Clone(new Rectangle(0, 0, m.Width, m.Height), PixelFormat.Format8bppIndexed),
                new Bitmap(c).Clone(new Rectangle(0, 0, c.Width, c.Height), PixelFormat.Format8bppIndexed));

            if (matchings.Length == 0) return;

            _imgList.Add(new WeightedImages
            {
                ImagePath = classImage,
                Score = (long) (matchings[0].Similarity * 100)
            });
        }

        private void GenerateRegionImage()
        {
            Rectangle rectangle =
                (Rectangle) new RectangleConverter().ConvertFromString(
                    File.ReadAllText(Path.Combine(_regionPath, _regionName) + ".txt"));

            using (Bitmap bm = new Bitmap(rectangle.Width, rectangle.Height))
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