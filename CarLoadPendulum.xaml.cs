using Newtonsoft.Json;
using SimHub.Plugins;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace User.CornerSpeed
{
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
    }

    public partial class CarLoadPendulum : UserControl
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        private double _x;
        private double _y;

        public CarLoadPendulum()
        {
            InitializeComponent();
            Loaded += CarLoadPendulum_Loaded;
            Unloaded += CarLoadPendulum_Unloaded;
            _timer.Tick += Timer_Tick;
        }

        private void CarLoadPendulum_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void CarLoadPendulum_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdatePendulum();
        }

        private void UpdatePendulum()
        {
            var item = DataContext as CarLoadPendulumItem;
            if (item == null)
            {
                return;
            }

            var length = Math.Max(10.0, item.PendulumLength);
            var massWeight = Math.Max(0.1, item.MassWeight);
            var responsiveness = Math.Min(1.0, 0.25 / massWeight);
            var lateral = GetPropertyValue(item.LateralLoadProperty);
            var longitudinal = GetPropertyValue(item.LongitudinalLoadProperty);

            var targetX = Math.Max(-1.5, Math.Min(1.5, lateral)) / 1.5 * length;
            var targetY = Math.Max(-1.5, Math.Min(1.5, -longitudinal)) / 1.5 * length;

            _x += (targetX - _x) * responsiveness;
            _y += (targetY - _y) * responsiveness;

            var centerX = ActualWidth > 0 ? ActualWidth / 2.0 : 80.0;
            var centerY = ActualHeight > 0 ? ActualHeight / 2.0 : 80.0;

            var massSize = 20.0 + Math.Min(20.0, massWeight * 2.0);
            PendulumMass.Width = massSize;
            PendulumMass.Height = massSize;

            SetCenter(Pivot, centerX, centerY);
            SetCenter(PendulumMass, centerX + _x, centerY + _y);
            PendulumRod.X1 = centerX;
            PendulumRod.Y1 = centerY;
            PendulumRod.X2 = centerX + _x;
            PendulumRod.Y2 = centerY + _y;
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
                return Convert.ToDouble(PluginManager.GetPropertyValue(propertyName));
            }
            catch
            {
                return 0.0;
            }
        }
    }
}
