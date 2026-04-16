using PropertyChanged;
using SimHub.Plugins.OutputPlugins.EditorControls;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.CornerSpeed
{
    //[DesignerIcon("/SimHub.Plugins;component/Resources/comp_widget.png")]
    [AddINotifyPropertyChangedInterface]
    public class iRacingGroupItem : GroupItem, INeedLoadedDynamically // : ChartItem //, IDashControl
    {
        public override string ComponentTypeName => "iRacingGroup";

        public iRacingGroupItem() : base()
        {

        }
        
        [Category("Group")]
        public double Scale { get; set; } = 1.0;

        [Category("Group")]
        public string Header { get; set; }
    }

    //[DesignerIcon("/SimHub.Plugins;component/Resources/comp_widget.png")]
    [AddINotifyPropertyChangedInterface]
    public class iRacingSubGroupItem : GroupItem, INeedLoadedDynamically // : ChartItem //, IDashControl
    {
        public override string ComponentTypeName => "iRacingSubGroup";

        public iRacingSubGroupItem() : base()
        {

        }
        
        //[Category("Group")]
        //public double Scale { get; set; } = 1.0;
    }
}