using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using TrackOnMouse.Utility;
using static TrackOnMouse.Utility.WinApi.User32;
using static TrackOnMouse.Utility.WinApi.Gdi32;

namespace TrackOnMouse.Highlighter
{
    public sealed partial class HighlighterForm : Form
    {
        private readonly MouseHook _mouseHook;

        private int _highlightShapeSize;
        private float _highlightShapeStroke;
        private int _clickAnimationTicks;

        private const int ClickAnimationMaxTicks = 25;

        private Point WindowCenter => new Point(Width / 2, Height / 2);

        private Size CurrentHighlightShapeSize => new Size(HighlightShapeSize, HighlightShapeSize);

        public int HighlightShapeSize
        {
            get => _highlightShapeSize;
            set
            {
                _highlightShapeSize = value;
                UpdateFormSize();
            }
        }

        public float HighlightShapeStroke
        {
            get => _highlightShapeStroke;
            set
            {
                if (value > _highlightShapeSize)
                    _highlightShapeStroke = _highlightShapeSize;
                else
                    _highlightShapeStroke = value;

                UpdateFormSize();
            }
        }

        public double HighlightShapeOpacity { get; set; } = 0.7;

        public Color HighlightShapeColor { get; set; } = Color.Yellow;

        public Color RightClickShapeColor { get; set; } = Color.Blue;

        public Color LeftClickShapeColor { get; set; } = Color.Red;

        private bool IsLeftClick { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;

                createParams.ExStyle |= 0x80000;    //  WS_EX_LAYERED
                createParams.ExStyle |= 0x20;       //  WS_EX_TRANSPARENT
                createParams.Style |= (int) ControlStyles.DoubleBuffer;
                createParams.Style |= (int) ControlStyles.OptimizedDoubleBuffer;

                return createParams;
            }
        }

        public HighlighterForm()
        {
            InitializeComponent();

            ShowInTaskbar = false;

            _mouseHook = MouseHook.GetInstance();
            _mouseHook.Install();
            _mouseHook.LeftButtonDown += delegate
            {
                _clickAnimationTicks = ClickAnimationMaxTicks;
                IsLeftClick = true;
            };

            _mouseHook.RightButtonDown += delegate
            {
                _clickAnimationTicks = ClickAnimationMaxTicks;
                IsLeftClick = false;
            };

            Timer highlightTimer = new Timer {Interval = 1};
            highlightTimer.Tick += HighlightTimerOnTick;
            highlightTimer.Start();
        }

        ~HighlighterForm()
        {
            _mouseHook.Uninstall();
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            PaintCircle();
        }

        private void HighlightTimerOnTick(object sender, EventArgs e)
        {
            Point currentMousePosition = GetCursorPosition();
            int x = currentMousePosition.X - WindowCenter.X;
            int y = currentMousePosition.Y - WindowCenter.Y;
            Location = new Point(x, y);

            PaintCircle();
        }

        private void UpdateFormSize()
        {
            Width = Height = (int) (HighlightShapeSize + HighlightShapeStroke) + 5;
        }

        private void PaintCircle()
        {
            using Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            using Graphics g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(
                new Pen(
                    new SolidBrush(Color.FromArgb((int) (byte.MaxValue * HighlightShapeOpacity), HighlightShapeColor)),
                    HighlightShapeStroke),
                new Rectangle(
                    WindowCenter.X - CurrentHighlightShapeSize.Width / 2,
                    WindowCenter.Y - CurrentHighlightShapeSize.Height / 2,
                    CurrentHighlightShapeSize.Width, CurrentHighlightShapeSize.Height));

            if (_clickAnimationTicks > 0)
            {
                float clickCircleSize = HighlightShapeSize -
                                        HighlightShapeSize / (float) ClickAnimationMaxTicks * _clickAnimationTicks;
                g.DrawEllipse(
                    new Pen(
                        new SolidBrush(IsLeftClick ? LeftClickShapeColor : RightClickShapeColor), 3),
                    WindowCenter.X - clickCircleSize / 2,
                    WindowCenter.Y - clickCircleSize / 2,
                    clickCircleSize, clickCircleSize);

                _clickAnimationTicks -= 1;
            }

            IntPtr screenDc = GetDC(IntPtr.Zero);
            IntPtr compatibleDc = CreateCompatibleDC(screenDc);
            IntPtr hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            IntPtr oldBitmap = SelectObject(compatibleDc, hBitmap);

            BLENDFUNCTION blend = new BLENDFUNCTION
            {
                BlendOp = AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = byte.MaxValue,
                AlphaFormat = AC_SRC_ALPHA
            };

            Point location = new Point(Left, Top);
            Size size = new Size(bitmap.Width, bitmap.Height);
            Point pointSource = new Point(0, 0);
            UpdateLayeredWindow(Handle, screenDc, ref location, ref size, compatibleDc, ref pointSource, 0, ref blend, ULW_ALPHA);
            SelectObject(compatibleDc, oldBitmap);
            DeleteObject(hBitmap);
            DeleteDC(compatibleDc);
            DeleteDC(screenDc);
        }
    }
}