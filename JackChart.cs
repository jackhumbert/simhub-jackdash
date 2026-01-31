using SimHub.Plugins.OutputPlugins.GraphicalDash.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace User.CornerSpeed
{
    public class JackChart : SimpleChart
    {

        public JackChart() : base()
        {
            FillBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }
       
        protected override void Render()
        {
            if (base.ActualHeight <= 0.0 || base.ActualWidth <= 0.0)
            {
                return;
            }

            double num = Minimum;
            double num2 = Maximum;
            if (AutoMaximum)
            {
                SimpleStreamChartPointsCollection dataPoints = DataPoints;
                if (dataPoints != null && (dataPoints.Points?.Any()).GetValueOrDefault())
                {
                    num2 = DataPoints.Points.Max((ChartPoint i) => i.Y);
                }
            }

            if (AutoMinimum)
            {
                SimpleStreamChartPointsCollection dataPoints2 = DataPoints;
                if (dataPoints2 != null && (dataPoints2.Points?.Any()).GetValueOrDefault())
                {
                    num = DataPoints.Points.Min((ChartPoint i) => i.Y);
                }
            }

            if (num == num2 && AutoMaximum && AutoMinimum)
            {
                double num3 = num;
                num2 = num3 + 1.0;
                num = num3 - 1.0;
            }
            else if (num == num2 && AutoMaximum)
            {
                num2 = num + 1.0;
            }

            int y_padding = 10;
            int x_padding = 10;
            int valueOrDefault = (DataPoints?.Points?.Count).GetValueOrDefault();
            DrawingContext drawingContext = backingStore.Open();
            try
            {
                Rect rect = new Rect(x_padding, Math.Max(0.0, (double)y_padding - LineThickness), Math.Max(0.0, base.ActualWidth - (double)(x_padding * 2)), Math.Max(0.0, Math.Min(base.ActualHeight, base.ActualHeight - (double)(y_padding * 2) + LineThickness * 2.0)));
                if (rect.Width <= 0.0 || rect.Height <= 0.0)
                {
                    return;
                }

                drawingContext.PushClip(new RectangleGeometry(rect));
                drawingContext.PushTransform(new TranslateTransform(x_padding, base.ActualHeight - (double)y_padding));
                drawingContext.PushTransform(new ScaleTransform(1.0, -1.0));
                SimpleStreamChartPointsCollection dataPoints3 = DataPoints;
                if (dataPoints3 != null && (dataPoints3.Points?.Any()).GetValueOrDefault())
                {
                    double x_scalar = (base.ActualWidth - (double)(x_padding * 2)) / (double)valueOrDefault;
                    double y_scalar = (base.ActualHeight - (double)(y_padding * 2)) / (num2 - num);
                    Point point = new Point(0.0 * x_scalar, (DataPoints.Points[0].Y - num) * y_scalar);
                    Point point2 = point;
                    PathFigure pathFigure = new PathFigure
                    {
                        //StartPoint = point,
                        StartPoint = new Point(0.0, 0.0),
                        IsClosed = false
                    };
                    PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
                    //pathSegmentCollection.Add(new LineSegment(new Point(0.0, 0.0), true));
                    bool flag = false;
                    int num8 = Math.Min(DataPoints.Points.Count, valueOrDefault);
                    for (int j = 1; j < num8; j++)
                    {
                        Point point3 = new Point((double)j * x_scalar, (DataPoints.Points[j].Y - num) * y_scalar);
                        if ((point3.Y != point.Y && point3.X - point.X > 1.0) || j == num8 - 1)
                        {
                            if (flag)
                            {
                                pathSegmentCollection.Add(new LineSegment(point2, isStroked: true));
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
                    pathSegmentCollection.Add(new LineSegment(new Point(ActualWidth - x_padding, 0.0), true));

                    pathFigure.Segments = pathSegmentCollection;
                    PathFigureCollection figures = new PathFigureCollection { pathFigure };
                    PathGeometry geometry = new PathGeometry
                    {
                        Figures = figures
                    };
                    //drawingContext.DrawGeometry(null, new Pen(new SolidColorBrush(LineColor), LineThickness), geometry);
                    //drawingContext.DrawGeometry(new SolidColorBrush(LineColor), new Pen(new SolidColorBrush(LineColor), LineThickness), geometry);
                    drawingContext.DrawGeometry(new SolidColorBrush(LineColor), null, geometry);
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
