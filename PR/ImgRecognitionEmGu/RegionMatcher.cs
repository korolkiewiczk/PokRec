using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;

namespace ImgRecognitionEmGu
{
    public class RegionMatcher
    {
        // Custom class to store image scores
        class WeightedImages : IComparable<WeightedImages>
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
        List<WeightedImages> imgList = new List<WeightedImages>();

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

            ProcessFolder(_classPath, RegionFilename);

            var result = imgList.OrderByDescending(x => x.Score)
                .Where(x => x.Score > _threshold)
                .Take(_take)
                .Select(x => Path.GetFileNameWithoutExtension(x.ImagePath))
                .ToList();

            if (!result.Any())
            {
                return new List<string> {"-"};
            }
            else
            {
                return result;
            }
        }

        public void Draw()
        {
            //string imgPath = imgList.Max().ImagePath;
            foreach (var imgPath in Directory.GetFiles(_classPath))
            {
                long score;
                long matchTime;

                using (Mat modelImage = CvInvoke.Imread(imgPath, ImreadModes.Color))
                using (Mat observedImage = CvInvoke.Imread(RegionFilename, ImreadModes.Color))
                {
                    var result = DrawMatches.Draw(modelImage, observedImage, out matchTime, out score);
                    var iv = new emImageViewer(result, score);
                    iv.Show();
                }
            }
        }

        private void ProcessFolder(string classesFolder, string mainImage)
        {
            using (var observedImage = CvInvoke.Imread(mainImage, ImreadModes.Color))
            {
                foreach (var classImage in Directory.GetFiles(classesFolder))
                    ProcessImage(mainImage, classImage, observedImage);
            }
        }

        /// <summary>
        /// Process single image: calculate score then add the occurence to imgList List<WeightedImage>
        /// </summary>
        private void ProcessImage(string mainImage, string classImage, Mat observedImage)
        {
            if (classImage == mainImage) return;

            try
            {
                long score;
                long matchTime;
                using (Mat modelImage = CvInvoke.Imread(classImage, ImreadModes.Color))
                {
                    Mat homography;
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfKeyPoint observedKeyPoints;

                    using (var matches = new VectorOfVectorOfDMatch())
                    {
                        Mat mask;
                        DrawMatches.FindMatch(modelImage, observedImage, out matchTime, out modelKeyPoints,
                            out observedKeyPoints, matches,
                            out mask, out homography, out score);
                    }

                    imgList.Add(new WeightedImages() {ImagePath = classImage, Score = score});
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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