using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TrackOnMouse.Utility
{
    public static class Win32
    {
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

        }

        public const int ULW_ALPHA = 2;

        public const int ULW_COLORKEY = 1;

        public const byte AC_SRC_OVER = 0;

        public const byte AC_SRC_ALPHA = 1;

        [DllImport("user32.dll")]
        public static extern bool UpdateLayeredWindow(IntPtr handle, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr handle);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr handle, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        public static Point GetCursorPosition()
        {
            GetCursorPos(out Point lpPoint);
            return lpPoint;
        }
    }
}
