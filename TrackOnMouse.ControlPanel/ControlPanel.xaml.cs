using System.Windows;
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
        }

        private void ControlPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
