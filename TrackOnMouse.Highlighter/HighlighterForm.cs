using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using TrackOnMouse.Utility;

namespace TrackOnMouse.Highlighter
{
    public sealed partial class HighlighterForm : Form
    {
        private int _circleSize = 100;

        private Point CurrentMousePosition { get; set; }

        private Point WindowCenter => new Point(Width / 2, Height / 2);

        private Size CurrentCircleSize => new Size(CircleSize, CircleSize);

        public int CircleSize
        {
            get => _circleSize;
            set
            {
                _circleSize = value;

                Width = (int)(_circleSize * 1.5);
                Height = (int)(_circleSize * 1.5);
            }
        }

        public float CircleThickness { get; set; } = 15;

        public double CircleOpacity { get; set; } = 0.7;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;

                createParams.ExStyle |= 0x80000;    //  WS_EX_LAYERED
                createParams.ExStyle |= 0x20;       //  WS_EX_TRANSPARENT
                createParams.Style |= (int)ControlStyles.DoubleBuffer;
                createParams.Style |= (int)ControlStyles.OptimizedDoubleBuffer;

                return createParams;
            }
        }

        public HighlighterForm()
        {
            InitializeComponent();

            ShowInTaskbar = false;

            Timer highlightTimer = new Timer { Interval = 1 };
            highlightTimer.Tick += HighlightTimerOnTick;
            highlightTimer.Start();
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            PaintCircle();
        }

        private void HighlightTimerOnTick(object sender, EventArgs e)
        {
            CurrentMousePosition = Win32.GetCursorPosition();

            int x = CurrentMousePosition.X - WindowCenter.X;
            int y = CurrentMousePosition.Y - WindowCenter.Y;
            Location = new Point(x, y);

            PaintCircle();
        }

        private void PaintCircle()
        {
            using Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            using Graphics g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(
                new Pen(new SolidBrush(Color.FromArgb((int) (byte.MaxValue * CircleOpacity), Color.Yellow)),
                    CircleThickness),
                new Rectangle(
                    WindowCenter.X - CurrentCircleSize.Width / 2, WindowCenter.Y - CurrentCircleSize.Height / 2,
                    CurrentCircleSize.Width, CurrentCircleSize.Height));


            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr compatibleDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            IntPtr oldBitmap = Win32.SelectObject(compatibleDc, hBitmap);

            Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION
            {
                BlendOp = Win32.AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = byte.MaxValue,
                AlphaFormat = Win32.AC_SRC_ALPHA
            };

            Point location = new Point(Left, Top);
            Size size = new Size(bitmap.Width, bitmap.Height);
            Point pointSource = new Point(0, 0);
            Win32.UpdateLayeredWindow(Handle, screenDc, ref location, ref size, compatibleDc, ref pointSource, 0,
                ref blend, Win32.ULW_ALPHA);
            Win32.SelectObject(compatibleDc, oldBitmap);
            Win32.DeleteObject(hBitmap);
            Win32.DeleteDC(compatibleDc);
            Win32.DeleteDC(screenDc);
        }
    }
}
