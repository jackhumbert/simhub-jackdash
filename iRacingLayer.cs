using PropertyChanged;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 
// SimHub.Plugins, Version=1.0.9524.22766, Culture=neutral, PublicKeyToken=null
// SimHub.Plugins.OutputPlugins.GraphicalDash.SelectionTemplateSelector
using System.Windows;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;

public override DataTemplate SelectTemplate(object item, DependencyObject container)
{
	FrameworkElement frameworkElement = container as FrameworkElement;
	if (item is Layer)
	{
		return frameworkElement.FindResource("layer") as DataTemplate;
	}
	if (item is ContainerItemBase)
	{
		return frameworkElement.FindResource("container") as DataTemplate;
	}
	GDashItemBindingBase obj = item as GDashItemBindingBase;
	if (obj != null && obj.Owner?.Editor?.IsEditMode == true)
	{
		return frameworkElement.FindResource("selectable") as DataTemplate;
	}
	return frameworkElement.FindResource("notselectable") as DataTemplate;
}

 */

namespace User.CornerSpeed
{
    //[DesignerIcon("/SimHub.Plugins;component/Resources/comp_widget.png")]
    [AddINotifyPropertyChangedInterface]
    public class iRacingLayer : Layer // : ChartItem //, IDashControl
    {
        public override string ComponentTypeName => "iRacingLayer";

        public iRacingLayer() : base()
        {

        }
    }
}
