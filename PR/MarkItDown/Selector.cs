using System.Drawing;
using System.Windows.Forms;
using Common;

namespace MarkItDown
{
    public class Selector : Form
    {
        protected const int BtnSize = 40;
        protected const int BtnPadding = 45;
        protected const int BtnStart = 20;
        
        protected PictureBox PictureBox;

        protected void CreatePictureBox()
        {
            PictureBox = new PictureBox();
            var image = Image.FromFile(Paths.TempImg);

            PictureBox.Image = image;

            PictureBox.Size = image.Size;

            Controls.Add(PictureBox);

            Text = image.Size.ToString();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PictureBox.Image.Dispose();
                PictureBox.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}