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
        private readonly string _rootFolder;
        private readonly Rectangle _originRect;
        private readonly List<string> _regions;

        private bool _showRegions;
        private Dictionary<string, Rectangle> _parsedRegions;

        public RegionSelector(string rootFolder, Rectangle originRect)
        {
            _rootFolder = rootFolder;
            _originRect = originRect;

            CreatePictureBox();

            _regions = File.ReadAllLines(Common.Paths.Regions).ToList();

            BuildRegionsButtons();

            _pictureBox.Paint += PictureBox_Paint;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.S)
            {
                _showRegions = !_showRegions;

                if (_showRegions && _parsedRegions == null)
                {
                    _parsedRegions = new Dictionary<string, Rectangle>();
                    foreach (var region in _regions)
                    {
                        var rect = LoadRegion(region);
                        if (rect != Rectangle.Empty)
                        {
                            _parsedRegions.Add(region, rect);
                        }
                    }
                }

                _pictureBox.Invalidate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
                    var tag = ((Button)sender).Tag as string;
                    var regionDir = RegionsDir;
                    if (!Directory.Exists(regionDir))
                    {
                        Directory.CreateDirectory(regionDir);
                    }

                    File.Copy(Common.Paths.TempImg, RegionsDir + tag + tempImgExtension, true);

                    SaveRegion(tag);

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

            AutoScroll = true;
            AutoScrollMinSize = new Size(Math.Max(btnSize, _pictureBox.Size.Width), btnLineSize);
        }

        private void SaveRegion(string tag)
        {
            File.WriteAllText(RegionsDir + tag + ".txt", new RectangleConverter().ConvertToString(_originRect));
        }

        private Rectangle LoadRegion(string tag)
        {
            try
            {
                var rectStr = File.ReadAllText(RegionsDir + tag + ".txt");
                var rect = (Rectangle)new RectangleConverter().ConvertFromString(rectStr);
                return rect;
            }
            catch
            {
                return Rectangle.Empty;
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_showRegions && _parsedRegions != null)
            {
                Color color = Color.Magenta;
                using (var brush = new SolidBrush(color))
                using (var pen = new Pen(brush, 3f))
                {
                    foreach (var rect in _parsedRegions)
                    {
                        e.Graphics.DrawRectangle(pen, rect.Value);
                        using (var font = new Font("Arial", 6, FontStyle.Regular, GraphicsUnit.Point))
                        {
                            e.Graphics.DrawString(rect.Key, font, brush, rect.Value.Location);
                        }

                    }
                }
            }
        }
    }
}