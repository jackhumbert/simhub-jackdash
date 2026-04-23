using Newtonsoft.Json;
using SimHub.Plugins;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace User.CornerSpeed
{
    public enum PendulumTraceRenderMode
    {
        Segments,
        SmoothPath,
    }

    public class CarLoadPendulumItem : DrawableItem, INeedLoadedDynamically
    {
        public override string ComponentTypeName => "CarLoadPendulum";

        [Browsable(false)]
        [JsonIgnore]
        public override bool FreezeWhenRunning => false;

        [Category("Pendulum")]
        public double PendulumLength { get; set; } = 60.0;

        [Category("Pendulum")]
        public double MassWeight { get; set; } = 1.0;

        [Category("Pendulum")]
        public string LateralLoadProperty { get; set; } = "DataCorePlugin.GameRawData.Telemetry.LatAccel";

        [Category("Pendulum")]
        public string LongitudinalLoadProperty { get; set; } = "DataCorePlugin.GameRawData.Telemetry.LongAccel";

        [Category("Pendulum")]
        public PendulumTraceRenderMode TraceRenderMode { get; set; } = PendulumTraceRenderMode.Segments;
    }

    public partial class CarLoadPendulum : UserControl
    {
        private const double MinPendulumLength = 10.0;
        private const double MinMassWeight = 0.1;
        private const double MaxAccelerationG = 1.5;
        private const double BaseMassSize = 20.0;
        private const double MaxAdditionalMassSize = 20.0;
        private const double MassSizeScaleFactor = 2.0;
        private const double DefaultCenterOffset = 80.0;
        private const int UpdateIntervalMs = 33;
        private const double TraceFadeDurationSeconds = 2.0;
        private const double TraceStrokeThickness = 2.0;
        private const double MinTracePointDistanceSquared = 0.25;
        private const int SmoothTraceBandCount = 4;

        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(UpdateIntervalMs) };
        private readonly List<TracePoint> _tracePoints = new();
        private double _x;
        private double _y;

        public CarLoadPendulum()
        {
            InitializeComponent();
            Loaded += CarLoadPendulum_Loaded;
            Unloaded += CarLoadPendulum_Unloaded;
            SizeChanged += CarLoadPendulum_SizeChanged;
            _timer.Tick += Timer_Tick;
        }

        private void CarLoadPendulum_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void CarLoadPendulum_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            ClearTrace();
        }

        private void CarLoadPendulum_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClearTrace();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePendulum();
        }

        private void UpdatePendulum()
        {
            if (DataContext is not CarLoadPendulumItem item)
            {
                ClearTrace();
                return;
            }

            var length = Math.Max(MinPendulumLength, item.PendulumLength);
            var massWeight = Math.Max(MinMassWeight, item.MassWeight);
            var responsiveness = Math.Min(1.0, 0.25 / massWeight);
            var lateral = GetPropertyValue(item.LateralLoadProperty);
            var longitudinal = GetPropertyValue(item.LongitudinalLoadProperty);

            var targetX = Math.Max(-MaxAccelerationG, Math.Min(MaxAccelerationG, lateral)) / MaxAccelerationG * length;
            var targetY = Math.Max(-MaxAccelerationG, Math.Min(MaxAccelerationG, -longitudinal)) / MaxAccelerationG * length;

            _x += (targetX - _x) * responsiveness;
            _y += (targetY - _y) * responsiveness;

            var centerX = ActualWidth > 0 ? ActualWidth / 2.0 : DefaultCenterOffset;
            var centerY = ActualHeight > 0 ? ActualHeight / 2.0 : DefaultCenterOffset;

            var massSize = BaseMassSize + Math.Min(MaxAdditionalMassSize, massWeight * MassSizeScaleFactor);
            PendulumMass.Width = massSize;
            PendulumMass.Height = massSize;

            SetCenter(Pivot, centerX, centerY);
            SetCenter(PendulumMass, centerX + _x, centerY + _y);
            PendulumRod.X1 = centerX;
            PendulumRod.Y1 = centerY;
            PendulumRod.X2 = centerX + _x;
            PendulumRod.Y2 = centerY + _y;

            UpdateTrace(centerX + _x, centerY + _y, item.TraceRenderMode);
        }

        private void UpdateTrace(double traceX, double traceY, PendulumTraceRenderMode traceRenderMode)
        {
            var now = DateTime.UtcNow;

            if (_tracePoints.Count == 0)
            {
                _tracePoints.Add(new TracePoint(traceX, traceY, now));
            }
            else
            {
                var lastPoint = _tracePoints[_tracePoints.Count - 1];
                var deltaX = traceX - lastPoint.X;
                var deltaY = traceY - lastPoint.Y;

                if ((deltaX * deltaX) + (deltaY * deltaY) >= MinTracePointDistanceSquared)
                {
                    _tracePoints.Add(new TracePoint(traceX, traceY, now));
                }
                else
                {
                    _tracePoints[_tracePoints.Count - 1] = new TracePoint(traceX, traceY, now);
                }
            }

            var cutoff = now - TimeSpan.FromSeconds(TraceFadeDurationSeconds);
            _tracePoints.RemoveAll(point => point.Timestamp < cutoff);
            RenderTrace(now, traceRenderMode);
        }

        private void RenderTrace(DateTime now, PendulumTraceRenderMode traceRenderMode)
        {
            TraceLayer.Children.Clear();

            if (_tracePoints.Count < 2)
            {
                return;
            }

            if (traceRenderMode == PendulumTraceRenderMode.SmoothPath)
            {
                RenderSmoothTrace(now);
                return;
            }

            RenderSegmentTrace(now);
        }

        private void RenderSegmentTrace(DateTime now)
        {
            for (var index = 1; index < _tracePoints.Count; index++)
            {
                var start = _tracePoints[index - 1];
                var end = _tracePoints[index];
                var ageSeconds = (now - end.Timestamp).TotalSeconds;
                var opacity = 1.0 - (ageSeconds / TraceFadeDurationSeconds);

                if (opacity <= 0.0)
                {
                    continue;
                }

                TraceLayer.Children.Add(new Line
                {
                    X1 = start.X,
                    Y1 = start.Y,
                    X2 = end.X,
                    Y2 = end.Y,
                    Stroke = Brushes.Red,
                    StrokeThickness = TraceStrokeThickness,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Opacity = opacity,
                    IsHitTestVisible = false,
                });
            }
        }

        private void RenderSmoothTrace(DateTime now)
        {
            var bandDuration = TraceFadeDurationSeconds / SmoothTraceBandCount;

            for (var bandIndex = 0; bandIndex < SmoothTraceBandCount; bandIndex++)
            {
                var minAgeSeconds = bandIndex * bandDuration;
                var maxAgeSeconds = (bandIndex + 1) * bandDuration;
                var bandPoints = GetTracePointsForBand(now, minAgeSeconds, maxAgeSeconds);

                if (bandPoints.Count < 2)
                {
                    continue;
                }

                var bandMidpointAge = minAgeSeconds + (bandDuration / 2.0);
                var opacity = 1.0 - (bandMidpointAge / TraceFadeDurationSeconds);

                if (opacity <= 0.0)
                {
                    continue;
                }

                TraceLayer.Children.Add(new Path
                {
                    Data = CreateSmoothGeometry(bandPoints),
                    Stroke = Brushes.Red,
                    StrokeThickness = TraceStrokeThickness,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeLineJoin = PenLineJoin.Round,
                    Opacity = opacity,
                    IsHitTestVisible = false,
                });
            }
        }

        private List<Point> GetTracePointsForBand(DateTime now, double minAgeSeconds, double maxAgeSeconds)
        {
            var points = new List<Point>();

            for (var index = 0; index < _tracePoints.Count; index++)
            {
                var tracePoint = _tracePoints[index];
                var ageSeconds = (now - tracePoint.Timestamp).TotalSeconds;

                if (ageSeconds < minAgeSeconds || ageSeconds > maxAgeSeconds)
                {
                    continue;
                }

                if (points.Count == 0 && index > 0)
                {
                    var previousPoint = _tracePoints[index - 1];
                    points.Add(new Point(previousPoint.X, previousPoint.Y));
                }

                points.Add(new Point(tracePoint.X, tracePoint.Y));
            }

            return points;
        }

        private static Geometry CreateSmoothGeometry(IReadOnlyList<Point> points)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(points[0], false, false);

                if (points.Count == 2)
                {
                    context.LineTo(points[1], true, false);
                }
                else
                {
                    for (var index = 1; index < points.Count - 1; index++)
                    {
                        var controlPoint = points[index];
                        var nextPoint = points[index + 1];
                        var midPoint = new Point(
                            (controlPoint.X + nextPoint.X) / 2.0,
                            (controlPoint.Y + nextPoint.Y) / 2.0);

                        context.QuadraticBezierTo(controlPoint, midPoint, true, false);
                    }

                    context.LineTo(points[points.Count - 1], true, false);
                }
            }

            geometry.Freeze();
            return geometry;
        }

        private void ClearTrace()
        {
            _tracePoints.Clear();
            TraceLayer.Children.Clear();
        }

        private static void SetCenter(FrameworkElement element, double centerX, double centerY)
        {
            Canvas.SetLeft(element, centerX - element.Width / 2.0);
            Canvas.SetTop(element, centerY - element.Height / 2.0);
        }

        private static double GetPropertyValue(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return 0.0;
            }

            try
            {
                return Convert.ToDouble(PluginManager.GetInstance().GetPropertyValue(propertyName));
            }
            catch
            {
                return 0.0;
            }
        }

        private readonly struct TracePoint
        {
            public TracePoint(double x, double y, DateTime timestamp)
            {
                X = x;
                Y = y;
                Timestamp = timestamp;
            }

            public double X { get; }

            public double Y { get; }

            public DateTime Timestamp { get; }
        }
    }
}
