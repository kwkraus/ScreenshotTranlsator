using System.ComponentModel;

namespace ScreenshotTranslatorApp;

/// <summary>
/// Main form for the Screenshot Translator application
/// </summary>
/// <remarks>
/// Setup Instructions:
/// 1. Build and run the application using .NET 8.0
/// 2. The application will start minimized in the system tray
/// 3. Double-click the tray icon to show the main window
/// 4. Right-click the tray icon to access the context menu with Exit option
/// 
/// Future Development:
/// - Add custom icon for the tray notification
/// - Implement screenshot capture functionality
/// - Add translation features
/// </remarks>
public partial class Form1 : Form
{
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;

    public Form1()
    {
        InitializeComponent();
        InitializeNotifyIcon();
        
        // Add a simple "Hello World" label
        Label helloLabel = new Label
        {
            Text = "Hello World",
            AutoSize = true,
            Location = new Point(50, 50),
            Font = new Font(Font.FontFamily, 14)
        };
        
        this.Controls.Add(helloLabel);
        this.Text = "Screenshot Translator";
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(300, 200);
        
        // Hide form when minimized and on start
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
        this.FormClosing += Form1_FormClosing;
        this.Shown += Form1_Shown;
    }
    
    private void InitializeNotifyIcon()
    {
        // Create context menu with Exit option
        contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Exit", null, ExitMenuItem_Click);

        // Initialize NotifyIcon
        notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application, // Using system application icon for now
            Text = "Screenshot Translator Running",
            Visible = true,
            ContextMenuStrip = contextMenu
        };
        
        // Show form when the notify icon is double-clicked
        notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
    }

    private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
    {
        // Show the form and bring it to the foreground
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.Activate();
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        // Clean up and exit the application
        notifyIcon.Visible = false;
        Application.Exit();
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // If user closes the form, just minimize it to tray instead
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }
    }

    private void Form1_Shown(object? sender, EventArgs e)
    {
        // Hide the form on initial start
        this.Hide();
    }
}
