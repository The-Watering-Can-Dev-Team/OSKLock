using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace OSKTrayApp
{
    public partial class Form1 : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private Timer moveTimer;

        private const int DesiredHeight = 300; // Fixed height for the OSK

        private const uint SWP_NOZORDER = 0x0004; // Flag to ignore Z-order
        private const uint SWP_NOACTIVATE = 0x0010; // Flag to prevent activation
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const int GWL_STYLE = -16; // Index for retrieving window styles
        private const long WS_SYSMENU = 0x00080000L; // System Menu (Minimize/Close buttons)
        private const long WS_MINIMIZEBOX = 0x00020000L; // Minimize button


        public Form1()
        {
            InitializeComponent();

            // Create a simple tray menu with an Exit option
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon
            trayIcon = new NotifyIcon()
            {
                Text = "OSK Mover",
                Icon = new Icon(SystemIcons.Application, 40, 40),
                ContextMenu = trayMenu,
                Visible = true
            };

            // Initialize the timer to move OSK periodically and hide Minimize/Close buttons
            moveTimer = new Timer();
            moveTimer.Interval = 100; // Move OSK every 0.1 seconds (adjust as needed)
            moveTimer.Tick += MoveOskAutomatically;
            moveTimer.Start();

            // Hide the form
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        // Automatically move the OSK and hide minimize/close buttons at regular intervals
        private void MoveOskAutomatically(object sender, EventArgs e)
        {
            IntPtr oskHandle = IntPtr.Zero;

            // Enumerate all windows and find the OSK window
            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                GetClassName(hWnd, sb, sb.Capacity);

                // Check if the window's class name is "OSKMainClass"
                if (sb.ToString().Contains("OSKMainClass"))
                {
                    oskHandle = hWnd;
                    return false; // Stop enumerating
                }

                return true; // Continue enumerating
            }, IntPtr.Zero);

            if (oskHandle != IntPtr.Zero)
            {
                // Get the working area of the screen (excluding the taskbar)
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

                // Set the desired size and position
                int oskHeight = 300; // Desired height for the OSK window
                int oskWidth = workingArea.Width; // Full screen width, without covering the taskbar
                int oskX = workingArea.X; // X position (left side of the screen)
                int oskY = workingArea.Bottom - oskHeight; // Y position just above the taskbar

                // Move and resize the OSK window
                bool result = SetWindowPos(oskHandle, IntPtr.Zero, oskX, oskY, oskWidth, oskHeight, SWP_NOZORDER | SWP_SHOWWINDOW);

                // Hide the Minimize and Close buttons
                long style = GetWindowLong(oskHandle, GWL_STYLE);
                style &= ~WS_SYSMENU;  // Remove the system menu (close button)
                style &= ~WS_MINIMIZEBOX; // Remove the minimize button
                SetWindowLong(oskHandle, GWL_STYLE, style);

                // Force the OSK window to redraw with new styles
                SetWindowPos(oskHandle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOZORDER | SWP_SHOWWINDOW | 0x0027);
            }
        }

        // Exit the application when the user selects "Exit"
        private void OnExit(object sender, EventArgs e)
        {
            moveTimer.Stop();
            trayIcon.Visible = false;
            Application.Exit();
        }

        // P/Invoke declarations for interacting with Windows API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}