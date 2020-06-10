using System.Windows.Input;

namespace TrackOnMouse.ControlPanel
{
    public partial class ControlPanel
    {
        public ControlPanel()
        {
            InitializeComponent();
            DataContext = new ControlPanelViewModel();
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
