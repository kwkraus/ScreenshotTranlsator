namespace ScreenshotTranslatorApp;

public class ScreenCaptureOverlay : Form
{
    private Point startPoint;
    private Rectangle selectionRect;
    private Rectangle previousRect;
    private bool isSelecting = false;
    private bool hasSelection = false;
    private Region? invalidRegion;
    private Bitmap? originalScreenImage;
    private Rectangle virtualScreenBounds; // Added for virtual screen dimensions

    public event EventHandler<Bitmap>? ScreenshotCaptured;

    public ScreenCaptureOverlay()
    {
        InitializeOverlay();
    }

    private void InitializeOverlay()
    {
        // Capture the composite screen image first
        CaptureOriginalScreen();

        // Set up the overlay form to cover all screens
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.Black;
        this.Opacity = 0.3;
        this.Cursor = Cursors.Cross;
        this.TopMost = true;

        // Position and size the form to cover the entire virtual screen area
        this.StartPosition = FormStartPosition.Manual;
        this.Location = new Point(virtualScreenBounds.Left, virtualScreenBounds.Top);
        this.Size = new Size(virtualScreenBounds.Width, virtualScreenBounds.Height);
        this.ShowInTaskbar = false;

        // Improve rendering performance
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

        // Handle mouse events
        this.MouseDown += OnMouseDown;
        this.MouseMove += OnMouseMove;
        this.MouseUp += OnMouseUp;
        this.KeyDown += OnKeyDown;

        // Add handler for proper cleanup when form closes
        this.FormClosing += OnFormClosing;
    }

    private void CaptureOriginalScreen()
    {
        try
        {
            // Get the bounds of the virtual screen (all monitors combined)
            virtualScreenBounds = SystemInformation.VirtualScreen;

            // Create a bitmap that can hold all screens
            originalScreenImage = new Bitmap(virtualScreenBounds.Width, virtualScreenBounds.Height);

            using (Graphics g = Graphics.FromImage(originalScreenImage))
            {
                // Copy from the entire virtual screen area
                g.CopyFromScreen(
                    virtualScreenBounds.Left, virtualScreenBounds.Top, // Source point (virtual screen origin)
                    0, 0,                                             // Destination point (bitmap origin)
                    new Size(virtualScreenBounds.Width, virtualScreenBounds.Height) // Area to copy
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error capturing screen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isSelecting = true;
            hasSelection = true;
            startPoint = e.Location;
            selectionRect = new Rectangle();
            previousRect = Rectangle.Empty;
            invalidRegion?.Dispose();
            invalidRegion = null;
            this.Invalidate();
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (!isSelecting) return;

        // Store the previous rectangle for efficient invalidation
        previousRect = selectionRect;

        // Calculate the new selection rectangle
        int x = Math.Min(startPoint.X, e.X);
        int y = Math.Min(startPoint.Y, e.Y);
        int width = Math.Abs(e.X - startPoint.X);
        int height = Math.Abs(e.Y - startPoint.Y);

        selectionRect = new Rectangle(x, y, width, height);

        // Create a region that encompasses both the previous and current selection
        Rectangle invalidateRect = Rectangle.Union(previousRect, selectionRect);

        // Add some padding to ensure complete coverage of the border
        invalidateRect.Inflate(3, 3);

        // Use region for more efficient invalidation
        invalidRegion?.Dispose();
        invalidRegion = new Region(invalidateRect);
        this.Invalidate(invalidateRect);
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

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        // Clean up resources and unregister event handlers
        this.MouseDown -= OnMouseDown;
        this.MouseMove -= OnMouseMove;
        this.MouseUp -= OnMouseUp;
        this.KeyDown -= OnKeyDown;
        this.FormClosing -= OnFormClosing;

        // Clean up any unmanaged resources
        invalidRegion?.Dispose();
        invalidRegion = null;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Only paint if we have a valid selection with dimensions
        if (hasSelection && selectionRect.Width > 0 && selectionRect.Height > 0)
        {
            // Create a semi-transparent white fill for the selection area
            using var selectionBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255));
            e.Graphics.FillRectangle(selectionBrush, selectionRect);

            // Draw a border - white inner border with a dark outer border for contrast
            using var whitePen = new Pen(Color.White, 1.5f);
            e.Graphics.DrawRectangle(whitePen, selectionRect);

            // Draw second border (outer) to improve visibility against different backgrounds
            using var darkPen = new Pen(Color.FromArgb(150, 0, 0, 0), 1);
            Rectangle outerRect = new Rectangle(
                selectionRect.X - 1,
                selectionRect.Y - 1,
                selectionRect.Width + 2,
                selectionRect.Height + 2);
            e.Graphics.DrawRectangle(darkPen, outerRect);
        }
    }

    private void CaptureScreenshot()
    {
        if (selectionRect.Width <= 5 || selectionRect.Height <= 5)
        {
            MessageBox.Show("Selection area is too small. Please make a larger selection.",
                "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Bitmap? capturedBitmap = null;
        try
        {
            // Create a bitmap of the selection area
            capturedBitmap = new Bitmap(selectionRect.Width, selectionRect.Height);

            using (var g = Graphics.FromImage(capturedBitmap))
            {
                // Configure graphics for better quality
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Extract the selected portion from our original screen capture
                if (originalScreenImage != null)
                {
                    g.DrawImage(originalScreenImage,
                        new Rectangle(0, 0, selectionRect.Width, selectionRect.Height),
                        selectionRect,
                        GraphicsUnit.Pixel);
                }
            }

            // Copy the image to clipboard
            Clipboard.SetImage(capturedBitmap);

            // Create a copy for the event handler if needed
            if (ScreenshotCaptured != null)
            {
                // Create a copy of the bitmap for the event handler
                var bitmapCopy = new Bitmap(capturedBitmap);
                // Raise the event with the copied image
                ScreenshotCaptured?.Invoke(this, bitmapCopy);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error capturing screenshot: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            // Always dispose of the bitmap to prevent resource leaks
            capturedBitmap?.Dispose();
        }
    }

    // Ensure proper disposal when the form is disposed
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            invalidRegion?.Dispose();
            originalScreenImage?.Dispose();
        }
        base.Dispose(disposing);
    }
}