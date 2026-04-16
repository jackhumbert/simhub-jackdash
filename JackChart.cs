using SimHub.Plugins.Devices.Registry.Impl.TurtleBeach.UI;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Render;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace User.CornerSpeed
{
    public class JackChart : SimpleChart
    {

        private static readonly Typeface Font = new Typeface("Inter");

        public SolidColorBrush LineBrush => new(LineColor);

        public new SolidColorBrush FillBrush => new(FillColor);

        public JackChart() : base()
        {
            //LineBrush = new SolidColorBrush(LineColor);
            //FillBrush = new SolidColorBrush(FillColor);
        }

        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register("FillColor", typeof(Color), typeof(JackChart), new FrameworkPropertyMetadata(Color.FromArgb(0x7F, 0xFF, 0x00, 0x00), FrameworkPropertyMetadataOptions.AffectsRender));

        public Color FillColor
        {
            get
            {
                return (Color)GetValue(FillColorProperty);
            }
            set
            {
                SetValue(FillColorProperty, value);
            }
        }

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(JackChart), new FrameworkPropertyMetadata(new Thickness(10), FrameworkPropertyMetadataOptions.AffectsRender));

        public Thickness Padding
        {
            get
            {
                return (Thickness)GetValue(PaddingProperty);
            }
            set
            {
                SetValue(PaddingProperty, value);
            }
        }

        public static readonly DependencyProperty LabelChangesProperty = DependencyProperty.Register("LabelChanges", typeof(bool), typeof(JackChart), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool LabelChanges
        {
            get
            {
                return (bool)GetValue(LabelChangesProperty);
            }
            set
            {
                SetValue(LabelChangesProperty, value);
            }
        }

        public static readonly DependencyProperty DiscreteValuesProperty = DependencyProperty.Register("DiscreteValues", typeof(bool), typeof(JackChart), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool DiscreteValues
        {
            get
            {
                return (bool)GetValue(DiscreteValuesProperty);
            }
            set
            {
                SetValue(DiscreteValuesProperty, value);
            }
        }
       
        protected override void Render()
        {
            if (base.ActualHeight <= 0.0 || base.ActualWidth <= 0.0)
            {
                return;
            }

            double min_value = Minimum;
            double max_value = Maximum;
            if (AutoMaximum)
            {
                SimpleStreamChartPointsCollection dataPoints = DataPoints;
                if (dataPoints != null && (dataPoints.Points?.Any()).GetValueOrDefault())
                {
                    max_value = DataPoints.Points.Max((ChartPoint i) => i.Y);
                }
            }

            if (AutoMinimum)
            {
                SimpleStreamChartPointsCollection dataPoints2 = DataPoints;
                if (dataPoints2 != null && (dataPoints2.Points?.Any()).GetValueOrDefault())
                {
                    min_value = DataPoints.Points.Min((ChartPoint i) => i.Y);
                }
            }

            if (min_value == max_value && AutoMaximum && AutoMinimum)
            {
                double num3 = min_value;
                max_value = num3 + 1.0;
                min_value = num3 - 1.0;
            }
            else if (min_value == max_value && AutoMaximum)
            {
                max_value = min_value + 1.0;
            }

            DrawingContext drawingContext = backingStore.Open();
            try
            {
                Rect rect = new Rect(Padding.Left, Math.Max(0.0, Padding.Top - LineThickness), Math.Max(0.0, base.ActualWidth - (Padding.Left + Padding.Right)), Math.Max(0.0, Math.Min(base.ActualHeight, base.ActualHeight - (Padding.Top + Padding.Bottom) + LineThickness * 2.0)));
                if (rect.Width <= 0.0 || rect.Height <= 0.0)
                {
                    return;
                }

                drawingContext.PushClip(new RectangleGeometry(rect));
                //drawingContext.PushTransform(new TranslateTransform(Padding.Left, base.ActualHeight - Padding.Top));
                //drawingContext.PushTransform(new ScaleTransform(1.0, -1.0));
                SimpleStreamChartPointsCollection dataPoints3 = DataPoints;
                if (dataPoints3 != null && (dataPoints3.Points?.Any()).GetValueOrDefault())
                {
                    double x_scalar = (ActualWidth - (Padding.Left + Padding.Right)) / DataPoints.MaxPoints;
                    double y_scalar = (ActualHeight - (Padding.Top + Padding.Bottom)) / (max_value - min_value);
                    double y_max = (ActualHeight - (Padding.Top + Padding.Bottom));
                    Point point = new(0.0 * x_scalar, y_max - (DataPoints.Points[0].Y - min_value) * y_scalar);
                    Point point2 = point;
                    PathFigure pathFigure = new PathFigure
                    {
                        //StartPoint = point,
                        StartPoint = new Point(0.0, y_max),
                        //IsClosed = false
                    };
                    PathSegmentCollection pathSegmentCollection = [];
                    pathSegmentCollection.Add(new LineSegment(new Point(Padding.Left, y_max), false));
                    pathSegmentCollection.Add(new LineSegment(point, false));
                    bool flag = false;
                    //for (int j = 1; j < (DataPoints.MaxPoints - DataPoints.Points.Count); j++)
                    //{
                    //    Point point3 = new(j * x_scalar, y_max - (min_value * y_scalar));

                    //    if ((point3.X - point.X > 1.0) || j == DataPoints.MaxPoints - 1)
                    //    {
                    //        if (flag)
                    //        {
                    //            pathSegmentCollection.Add(new LineSegment(point2, isStroked: true));
                    //        }

                    //        pathSegmentCollection.Add(new LineSegment(point3, isStroked: true));
                    //        point = point3;
                    //        flag = false;
                    //    }
                    //    else
                    //    {
                    //        point2 = point3;
                    //        flag = true;
                    //    }
                    //}
                    for (int j = 1; j < DataPoints.Points.Count; j++)
                    {
                        Point point3 = new(((DataPoints.MaxPoints - DataPoints.Points.Count) + j) * x_scalar, y_max - (DataPoints.Points[j].Y - min_value) * y_scalar);

                        if (((point3.Y != point.Y) && (point3.X - point.X > 1.0)) || j == DataPoints.MaxPoints - 1)
                        {
                            if (point3.Y != point.Y && LabelChanges)
                            {
                                var text_point = point3;
                                if (text_point.Y - 20 < 0)
                                    text_point.Y += 2;
                                else
                                    text_point.Y -= 20;
                                text_point.X += 2;
                                var text = new FormattedText(((int)DataPoints.Points[j].Y).ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, Font, 16, LineBrush, 1.0);
                                drawingContext.DrawText(text, text_point);
                            }
                            if (flag)
                            {
                                if (DiscreteValues)
                                {
                                    var corner = point2;
                                    corner.X = point3.X;
                                    pathSegmentCollection.Add(new LineSegment(point2, isStroked: true));
                                    pathSegmentCollection.Add(new LineSegment(corner, isStroked: true));
                                } else { 
                                    pathSegmentCollection.Add(new LineSegment(point2, isStroked: true));
                                }
                            }

                            pathSegmentCollection.Add(new LineSegment(point3, isStroked: true));
                            point = point3;
                            flag = false;
                        }
                        else
                        {
                            point2 = point3;
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        pathSegmentCollection.Add(new LineSegment(point2, isStroked: true));
                    }
                    //pathSegmentCollection.Add(new LineSegment(point, true));
                    pathSegmentCollection.Add(new LineSegment(new Point(ActualWidth - Padding.Left, y_max), false));

                    pathFigure.Segments = pathSegmentCollection;
                    PathFigureCollection figures = new() { pathFigure };
                    PathGeometry geometry = new()
                    {
                        Figures = figures
                    };
                    drawingContext.DrawGeometry(FillBrush, new Pen(LineBrush, LineThickness), geometry);
                }

                drawingContext.Pop();
            }
            finally
            {
                drawingContext.Close();
            }
        }

    }

}
