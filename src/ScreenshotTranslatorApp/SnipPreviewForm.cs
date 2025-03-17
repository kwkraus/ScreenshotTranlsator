namespace ScreenshotTranslatorApp
{
    public partial class SnipPreviewForm : Form
    {
        private PictureBox pictureBox = new PictureBox();
        private Bitmap? capturedImage;

        public SnipPreviewForm(Bitmap image)
        {
            capturedImage = image;
            InitializeFormProperties();
            InitializePictureBox();
            UpdateImage();
        }

        private void InitializeFormProperties()
        {
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.DoubleBuffered = true;
            this.Resize += SnipPreviewForm_Resize;
        }

        private void InitializePictureBox()
        {
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackColor = Color.Black;
            this.Controls.Add(pictureBox);
        }

        private void UpdateImage()
        {
            if (capturedImage != null)
            {
                pictureBox.Image = new Bitmap(capturedImage);
            }
        }

        private void SnipPreviewForm_Resize(object? sender, EventArgs e)
        {
            UpdateImage();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && capturedImage != null)
            {
                capturedImage.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}