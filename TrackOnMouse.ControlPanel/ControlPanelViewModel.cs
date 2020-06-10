using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using TrackOnMouse.Highlighter;

namespace TrackOnMouse.ControlPanel
{
    public class ControlPanelViewModel : BindableBase
    {
        private readonly HighlighterForm _highlighter = new HighlighterForm();
        private float _circleSize = 100;
        private float _circleStroke = 15;

        public float CircleSize
        {
            get => _circleSize;
            set 
            {
                SetProperty(ref _circleSize, value);
                _highlighter.CircleSize = (int) _circleSize;
            }
        }

        public float CircleSizeMax { get; } = 500;

        public float CircleStroke
        {
            get => _circleStroke;
            set
            {
                SetProperty(ref _circleStroke, value);
                _highlighter.CircleThickness = _circleStroke;
            }
        }

        public float CircleStrokeMax => CircleSizeMax / 2;

        public ICommand CloseCommand { get; }

        public ICommand MinimizeCommand { get; }

        public ControlPanelViewModel()
        {
            CloseCommand = new DelegateCommand(() => Close?.Invoke());
            MinimizeCommand = new DelegateCommand(() => Minimize?.Invoke());

            _highlighter.Show();
        }

        public event Action Close;
        public event Action Minimize;
    }
}