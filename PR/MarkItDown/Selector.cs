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
        
        protected PictureBox _pictureBox;

        protected void CreatePictureBox()
        {
            _pictureBox = new PictureBox();
            var image = Image.FromFile(Paths.TempImg);

            _pictureBox.Image = image;

            _pictureBox.Size = image.Size;

            this.Controls.Add(_pictureBox);

            Text = image.Size.ToString();

            
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pictureBox.Image.Dispose();
                _pictureBox.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}