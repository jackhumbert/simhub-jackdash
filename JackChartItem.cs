using PropertyChanged;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace User.CornerSpeed
{
    //[DesignerIcon("/SimHub.Plugins;component/Resources/comp_widget.png")]
    [AddINotifyPropertyChangedInterface]
    public class JackChartItem : ChartItem, INeedLoadedDynamically // : ChartItem //, IDashControl
    {
        public override string ComponentTypeName => "JackWidget";

        //public DashHostControls s;
        //public GroupPresenter s;
                
        [Category("Chart")]
        public Color FillColor { get; set; }

        [Category("Chart")]
        public Thickness Padding { get; set; } = new(10);

        [Category("Chart")]
        public bool LabelChanges { get; set; } = false;

        [Category("Chart")]
        public bool DiscreteValues { get; set; } = false;

        public JackChartItem()
        {
        }
    }
}
