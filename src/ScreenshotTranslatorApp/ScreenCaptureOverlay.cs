using System.Drawing.Imaging;

namespace ScreenshotTranslatorApp;

public class ScreenCaptureOverlay : Form
{
    private Point startPoint;
    private Rectangle selectionRect;
    private Rectangle previousRect;
    private bool isSelecting = false;
    private bool hasSelection = false;

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
        
        // Invalidate only the affected region for better performance
        // Create a region that encompasses both the previous and current selection
        Rectangle invalidateRect = Rectangle.Union(previousRect, selectionRect);
        
        // Add some padding to ensure complete coverage of the border
        invalidateRect.Inflate(3, 3);
        
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

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
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

        try
        {
            // Create a bitmap of the selection area
            using var bitmap = new Bitmap(selectionRect.Width, selectionRect.Height);
            using var g = Graphics.FromImage(bitmap);
            
            // Get screen coordinates
            var screenPoint = this.PointToScreen(new Point(selectionRect.X, selectionRect.Y));
            
            // Capture the screen portion
            g.CopyFromScreen(screenPoint, Point.Empty, selectionRect.Size);
            
            // Copy the image to clipboard
            Clipboard.SetImage(bitmap);
            
            // Raise the event with the captured image
            ScreenshotCaptured?.Invoke(this, bitmap);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error capturing screenshot: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}