using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using TrackOnMouse.Highlighter;

namespace TrackOnMouse.ControlPanel
{
    public class ControlPanelViewModel : BindableBase
    {
        private readonly HighlighterForm _highlighter;
        private float _shapeSize = 100;
        private float _shapeStroke = 15;
        private float _shapeOpacityPercentage = 70;

        public static float ShapeSizeMax => 500;
        public static float ShapeOpacityMax => 100;
        public float ShapeStrokeMax => ShapeSize;

        public float ShapeSize
        {
            get => _shapeSize;
            set
            {
                if (value < 0) return;
                if (value > ShapeSizeMax)
                {
                    value = ShapeSizeMax;
                }

                SetProperty(ref _shapeSize, value);
                
                if (ShapeStroke > _shapeSize)
                {
                    ShapeStroke = _shapeSize;
                }

                RaisePropertyChanged(nameof(ShapeStrokeMax));

                _highlighter.ShapeSize = (int) _shapeSize;
            }
        }

        public float ShapeStroke
        {
            get => _shapeStroke;
            set
            {
                if (value < 0) return;
                if (value > ShapeStrokeMax)
                {
                    value = ShapeStrokeMax;
                }
                
                SetProperty(ref _shapeStroke, value);
                _highlighter.ShapeStroke = _shapeStroke;
            }
        }

        public float ShapeOpacityPercentage
        {
            get => _shapeOpacityPercentage;
            set
            {
                if (value < 0) return;
                if (value > ShapeOpacityMax)
                {
                    value = ShapeOpacityMax;
                }

                SetProperty(ref _shapeOpacityPercentage, value);
                _highlighter.ShapeOpacity = _shapeOpacityPercentage / 100f;
            }
        }

        public ICommand CloseCommand { get; }
        public ICommand MinimizeCommand { get; }

        public ControlPanelViewModel()
        {
            CloseCommand = new DelegateCommand(() => Close?.Invoke());
            MinimizeCommand = new DelegateCommand(() => Minimize?.Invoke());

            _highlighter = new HighlighterForm
            {
                ShapeSize = (int) _shapeSize, 
                ShapeStroke = _shapeStroke,
                ShapeOpacity = _shapeOpacityPercentage / 100f
            };

            _highlighter.Show();
        }

        public event Action Close;
        public event Action Minimize;
    }
}