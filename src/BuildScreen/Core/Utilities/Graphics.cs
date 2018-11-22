using System.Windows;
using System.Windows.Media;

namespace BuildScreen.Core.Utilities
{
    public static class Graphics
    {
        private static LinearGradientBrush _brushNeutral;
        private static LinearGradientBrush _brushSuccess;
        private static LinearGradientBrush _brushFailure;

        public static LinearGradientBrush BrushNeutral
        {
            get
            {
                if (_brushNeutral == null)
                {
                    LinearGradientBrush linearGradientBrush = new LinearGradientBrush { StartPoint = new Point(0.1, 0), EndPoint = new Point(0.1, 1) };
// ReSharper disable PossibleNullReferenceException
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#7F7F7F"), 0));
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#4B4B4B"), 1));
// ReSharper restore PossibleNullReferenceException

                    _brushNeutral = linearGradientBrush;
                }

                return _brushNeutral;
            }
        }

        public static LinearGradientBrush BrushSuccess
        {
            get
            {
                if (_brushSuccess == null)
                {
                    LinearGradientBrush linearGradientBrush = new LinearGradientBrush { StartPoint = new Point(0.1, 0), EndPoint = new Point(0.1, 1) };
// ReSharper disable PossibleNullReferenceException
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#83BE40"), 0));
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#4C7422"), 1));
// ReSharper restore PossibleNullReferenceException

                    _brushSuccess = linearGradientBrush;
                }

                return _brushSuccess;
            }
        }

        public static LinearGradientBrush BrushFailure
        {
            get
            {
                if (_brushFailure == null)
                {
                    LinearGradientBrush linearGradientBrush = new LinearGradientBrush { StartPoint = new Point(0.1, 0), EndPoint = new Point(0.1, 1) };
// ReSharper disable PossibleNullReferenceException
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#BE4040"), 0));
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#742222"), 1));
// ReSharper restore PossibleNullReferenceException

                    _brushFailure = linearGradientBrush;
                }

                return _brushFailure;
            }
        }
    }
}
