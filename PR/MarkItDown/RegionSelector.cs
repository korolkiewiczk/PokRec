using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MarkItDown
{
    public class RegionSelector : Selector
    {
        private const int BtnPerLine = 16;
        private const string RegionsFolder = "regions\\";
        private readonly string _rootFolder;
        private readonly Rectangle _originRect;
        private readonly List<string> _regions;

        public RegionSelector(string rootFolder, Rectangle originRect)
        {
            _rootFolder = rootFolder;
            _originRect = originRect;

            CreatePictureBox();

            _regions = File.ReadAllLines(Common.Paths.Regions).ToList();

            BuildRegionsButtons();
        }
        
        private string RegionsDir => _rootFolder + "\\";

        private void BuildRegionsButtons()
        {
            int x = BtnStart, y = _pictureBox.Size.Height + BtnStart;
            var btnSize = BtnPadding * BtnPerLine + BtnStart * 2;

            var countItemsSize = _regions.Count / BtnPerLine + 2;
            var btnLineSize = countItemsSize * BtnPadding + y;

            Size = new Size(Math.Max(btnSize, _pictureBox.Size.Width), btnLineSize);

            var tempImgExtension = Path.GetExtension(Common.Paths.TempImg);
            int lineCount = 0;

            foreach (var region in _regions)
            {
                Button b = new Button();
                b.Text = region;
                if (File.Exists(RegionsDir + region + tempImgExtension))
                {
                    b.BackColor = Color.Gray;
                }
                b.Size = new Size(BtnSize, BtnSize);
                b.Location = new Point(x, y);
                b.Tag = region;
                if (region.Length >= 9)
                {
                    b.Font = new Font(b.Font.FontFamily, b.Font.Size - 2);
                }
                b.Click += (sender, e) =>
                {
                    var tag = ((Button) sender).Tag as string;
                    var regionDir = RegionsDir;
                    if (!Directory.Exists(regionDir))
                    {
                        Directory.CreateDirectory(regionDir);
                    }

                    File.Copy(Common.Paths.TempImg, RegionsDir + tag + tempImgExtension, true);
                    
                    File.WriteAllText(RegionsDir + tag + ".txt", new RectangleConverter().ConvertToString(_originRect));

                    Close();
                };
                Controls.Add(b);

                x += BtnPadding;
                lineCount++;
                if (lineCount >= BtnPerLine)
                {
                    x = BtnStart;
                    y += BtnPadding;
                    lineCount = 0;
                }
            }
        }
    }
}