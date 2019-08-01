using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace BuildScreen.Helpers
{
    static public class AutoCloseWindow
    {
        public static Window CreateAutoCloseWindow(TimeSpan timeout)
        {
            Window window = new Window()
            {
                WindowStyle = WindowStyle.None,
                WindowState = System.Windows.WindowState.Maximized,
                Background = System.Windows.Media.Brushes.Transparent,
                AllowsTransparency = true,
                ShowInTaskbar = false,
                ShowActivated = true,
                Topmost = true
            };

            window.Show();

            IntPtr handle = new WindowInteropHelper(window).Handle;

            Task.Delay((int)timeout.TotalMilliseconds).ContinueWith(
                t => NativeMethods.SendMessage(handle, 0x10 /*WM_CLOSE*/, IntPtr.Zero, IntPtr.Zero));

            return window;
        }
    }

    static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
