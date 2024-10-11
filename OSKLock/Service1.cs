using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace OSKLock
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private const int DesiredX = -6;       // Desired X position
        private const int DesiredY = 778;      // Desired Y position
        private const int DesiredWidth = 1933; // Desired Width
        private const int DesiredHeight = 308; // Desired Height

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Log service start
            EventLog.WriteEntry("OSKLockService", "Service started", EventLogEntryType.Information);

            // Start the timer to check and move the OSK window every 500ms
            timer = new Timer(MoveOskWindow, null, 0, 500);
        }

        protected override void OnStop()
        {
            // Log service stop
            EventLog.WriteEntry("OSKLockService", "Service stopped", EventLogEntryType.Information);

            // Dispose of the timer when the service stops
            timer.Dispose();
        }

        private void MoveOskWindow(object state)
        {
            // Find the On-Screen Keyboard window by its class name
            IntPtr oskHandle = FindWindow("OSKMainClass", null);
            if (oskHandle != IntPtr.Zero)
            {
                // Log if the window is found
                EventLog.WriteEntry("OSKLockService", $"OSK Handle: {oskHandle}", EventLogEntryType.Information);

                // Get the current window position
                RECT rect;
                GetWindowRect(oskHandle, out rect);

                // Check if the window is already at the desired position
                if (rect.Left != DesiredX || rect.Top != DesiredY ||
                    (rect.Right - rect.Left) != DesiredWidth ||
                    (rect.Bottom - rect.Top) != DesiredHeight)
                {
                    // Move and resize the window to the desired position and size
                    bool result = MoveWindow(oskHandle, DesiredX, DesiredY, DesiredWidth, DesiredHeight, true);

                    // Log whether the move was successful
                    EventLog.WriteEntry("OSKLockService", $"MoveWindow result: {result}", EventLogEntryType.Information);
                }
            }
            else
            {
                // Log if the window is not found
                EventLog.WriteEntry("OSKLockService", "OSK window not found", EventLogEntryType.Warning);
            }
        }

        // P/Invoke declarations to interact with Windows API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);



        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
