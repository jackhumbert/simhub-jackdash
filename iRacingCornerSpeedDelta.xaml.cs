using Newtonsoft.Json;
using PropertyChanged;
using SimHub.Plugins;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace User.CornerSpeed
{
    //public class DeltaColorConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is TimeSpan timeSpan)
    //        {
    //            string positiveFormat = timeSpan.Negate().ToString(@"s\.fff");
    //            if (timeSpan.TotalMinutes > 1.0) { 
    //                positiveFormat = timeSpan.Negate().ToString(@"m\:ss\.fff");
    //            }
    //            // Check if the TimeSpan is negative
    //            if (timeSpan < TimeSpan.Zero)
    //            {
    //                // Format as positive and prepend the minus sign manually
    //                // The "g" standard format specifier can work well, or a custom format
    //                return "-" + positiveFormat;
    //            }
    //            return positiveFormat;
    //        }
    //        return value; // Or return a default value/error indicator
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    [AddINotifyPropertyChangedInterface]
    public class iRacingCornerSpeedDeltaItem : DrawableItem
    {
        public override string ComponentTypeName => "iRacingCornerSpeedDeltaItem";
        
        [Browsable(false)]
        [JsonIgnore]
        public CornerSpeedPlugin Plugin { get; set; }

        public iRacingCornerSpeedDeltaItem() : base()
        {
            Plugin = PluginManager.GetInstance().GetPlugin<CornerSpeedPlugin>();
        }

        [Browsable(false)]
        [JsonIgnore]
        public override bool FreezeWhenRunning => false;

        public double Delta { get; set; }
        
        public double Range { get; set; }

        public double DDelta { get; set; }

    }

    /// <summary>
    /// Interaction logic for iRacingCornerSpeedDelta.xaml
    /// </summary>
    public partial class iRacingCornerSpeedDelta : UserControl
    {
        public CornerSpeedPlugin Plugin { get; set; }

        public iRacingCornerSpeedDelta()
        {
            InitializeComponent();
            Plugin = PluginManager.GetInstance().GetPlugin<CornerSpeedPlugin>();
        }
    }
}
