using System;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace TrackOnMouse.ControlPanel
{
    public partial class ControlPanel
    {
        private ControlPanelViewModel ViewModel => DataContext as ControlPanelViewModel;

        public ControlPanel()
        {
            InitializeComponent();
            DataContext = new ControlPanelViewModel();
            ViewModel.Close += Close;
            ViewModel.Minimize += () => WindowState = WindowState.Minimized;

            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.AppIcon,
                Visible = true,
                Text = @"TrackOnMouse"     //  Temporary literal string
            };

            notifyIcon.DoubleClick += delegate 
            {
                Show();
                WindowState = WindowState.Normal;
            };
        }

        private void ControlPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }
    }
}
