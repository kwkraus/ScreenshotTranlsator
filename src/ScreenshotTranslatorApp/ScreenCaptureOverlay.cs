using System.Drawing.Imaging;

namespace ScreenshotTranslatorApp;

public class ScreenCaptureOverlay : Form
{
    private Point startPoint;
    private Rectangle selectionRect;
    private bool isSelecting = false;

    public event EventHandler<Bitmap>? ScreenshotCaptured;

    public ScreenCaptureOverlay()
    {
        InitializeOverlay();
    }

    private void InitializeOverlay()
    {
        // Set up the overlay form
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.Black;
        this.Opacity = 0.3;
        this.Cursor = Cursors.Cross;
        this.TopMost = true;

        // Handle mouse events
        this.MouseDown += OnMouseDown;
        this.MouseMove += OnMouseMove;
        this.MouseUp += OnMouseUp;
        this.KeyDown += OnKeyDown;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isSelecting = true;
            startPoint = e.Location;
            selectionRect = new Rectangle();
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (!isSelecting) return;

        int x = Math.Min(startPoint.X, e.X);
        int y = Math.Min(startPoint.Y, e.Y);
        int width = Math.Abs(e.X - startPoint.X);
        int height = Math.Abs(e.Y - startPoint.Y);

        selectionRect = new Rectangle(x, y, width, height);
        this.Invalidate();
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && isSelecting)
        {
            isSelecting = false;
            CaptureScreenshot();
            this.Close();
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            this.Close();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (isSelecting)
        {
            // Clear the selection area to make it transparent
            using var clearBrush = new SolidBrush(Color.FromArgb(0, 0, 0, 0));
            e.Graphics.FillRectangle(clearBrush, selectionRect);

            // Draw the red border
            using var pen = new Pen(Color.Red, 2);
            e.Graphics.DrawRectangle(pen, selectionRect);

            // Draw the dimmed area outside the selection
            using var dimBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            e.Graphics.FillRectangle(dimBrush, new Rectangle(0, 0, this.Width, selectionRect.Top)); // Top
            e.Graphics.FillRectangle(dimBrush, new Rectangle(0, selectionRect.Bottom, this.Width, this.Height - selectionRect.Bottom)); // Bottom
            e.Graphics.FillRectangle(dimBrush, new Rectangle(0, selectionRect.Top, selectionRect.Left, selectionRect.Height)); // Left
            e.Graphics.FillRectangle(dimBrush, new Rectangle(selectionRect.Right, selectionRect.Top, this.Width - selectionRect.Right, selectionRect.Height)); // Right
        }
    }

    private void CaptureScreenshot()
    {
        if (selectionRect.Width <= 0 || selectionRect.Height <= 0) return;

        try
        {
            // Create a bitmap of the selection area
            using var bitmap = new Bitmap(selectionRect.Width, selectionRect.Height);
            using var g = Graphics.FromImage(bitmap);
            
            // Get screen coordinates
            var screenPoint = this.PointToScreen(new Point(selectionRect.X, selectionRect.Y));
            
            // Capture the screen portion
            g.CopyFromScreen(screenPoint, Point.Empty, selectionRect.Size);
            
            // Raise the event with the captured image
            ScreenshotCaptured?.Invoke(this, bitmap);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error capturing screenshot: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}