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

        private string RegionsDir => _rootFolder + "\\";

        private readonly RegionsMarker _regionsMarker;

        public RegionSelector(string rootFolder, Rectangle originRect)
        {
            _rootFolder = rootFolder;
            _originRect = originRect;

            CreatePictureBox();

            _regionsMarker = new RegionsMarker();
            _regions = _regionsMarker.Regions;

            BuildRegionsButtons();

            PictureBox.Paint += PictureBox_Paint;
        }

        private void BuildRegionsButtons()
        {
            int x = BtnStart, y = PictureBox.Size.Height + BtnStart;
            var btnSize = BtnPadding * BtnPerLine + BtnStart * 2;

            var countItemsSize = _regions.Count / BtnPerLine + 2;
            var btnLineSize = countItemsSize * BtnPadding + y;

            Size = new Size(Math.Max(btnSize, PictureBox.Size.Width), btnLineSize);

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
                    var tag = ((Button)sender)?.Tag as string;
                    if (!Directory.Exists(RegionsDir))
                    {
                        Directory.CreateDirectory(RegionsDir);
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
            AutoScrollMinSize = new Size(Math.Max(btnSize, PictureBox.Size.Width), btnLineSize);
        }

        private void SaveRegion(string tag)
        {
            File.WriteAllText(RegionsDir + tag + ".txt", new RectangleConverter().ConvertToString(_originRect));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.S:
                    _regionsMarker.MarkRegionsOnScreen(_rootFolder);
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            _regionsMarker.PaintRegions(e);
            PictureBox.Invalidate();
        }
    }
}