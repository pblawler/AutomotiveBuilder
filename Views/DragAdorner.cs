using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes; // Add this using directive at the top of the file to explicitly use System.Windows.Media.Brushes

// Explicitly qualify Point usage to System.Windows.Point to resolve ambiguity
namespace AutomotiveBuilder.Views
{
    public class DragAdorner : Adorner
    {
        private readonly ContentPresenter _contentPresenter;
        private System.Windows.Point _offset;

        public DragAdorner(UIElement adornedElement, object data, System.Windows.Point offset) : base(adornedElement)
        {
            _offset = offset;
            IsHitTestVisible = false;

            _contentPresenter = new ContentPresenter
            {
                Content = data,
                ContentTemplate = CreateDragTemplate(),
                Opacity = 0.8
            };
        }

        private static DataTemplate CreateDragTemplate()
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(System.Windows.Controls.Border));
            factory.SetValue(System.Windows.Controls.Border.BackgroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 255, 255, 255)));
            factory.SetValue(System.Windows.Controls.Border.BorderBrushProperty, Brushes.DodgerBlue);
            factory.SetValue(System.Windows.Controls.Border.BorderThicknessProperty, new Thickness(2));
            factory.SetValue(System.Windows.Controls.Border.CornerRadiusProperty, new CornerRadius(4));
            factory.SetValue(System.Windows.Controls.Border.PaddingProperty, new Thickness(8, 4, 8, 4));

            var textFactory = new FrameworkElementFactory(typeof(System.Windows.Controls.TextBlock));
            textFactory.SetBinding(System.Windows.Controls.TextBlock.TextProperty, new System.Windows.Data.Binding("Name"));
            textFactory.SetValue(System.Windows.Controls.TextBlock.FontWeightProperty, FontWeights.SemiBold);
            textFactory.SetValue(System.Windows.Controls.TextBlock.ForegroundProperty, Brushes.Black);

            factory.AppendChild(textFactory);
            template.VisualTree = factory;
            return template;
        }

        public void UpdatePosition(System.Windows.Point position)
        {
            _offset = position;
            InvalidateVisual();
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => _contentPresenter;
        protected override int VisualChildrenCount => 1;

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(_offset.X + 15, _offset.Y + 15));
            if (transform != null)
                result.Children.Add(transform);
            return result;
        }
    }
}