using SimHub.Plugins.OutputPlugins.GraphicalDash;
using MahApps.Metro.Controls;
using SimHub.Plugins.OutputPlugins.GraphicalDash.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SimHub.Plugins.OutputPlugins.Dash.WPFUI;

namespace User.CornerSpeed
{

public static class TreeHelper
{
    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
    {
        // Get the number of children for the current parent object
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            // Get the child at the specified index
            var child = VisualTreeHelper.GetChild(parent, i);
            
            // Check if the child is the desired type T
            if (child is T typedChild)
            {
                yield return typedChild;
            }

            // Recursively call the function for all children
            foreach (var other in FindVisualChildren<T>(child))
            {
                yield return other;
            }
        }
    }
}

internal class Program
{
    public Program()
    {
        //SimHub.Plugins.OutputPlugins.Dash.TemplatingCommon.NCalcEngineBase.AvailableFunctions.Add(new FormulaPropertyEntry(
        //    "if", 
        //    "if(condition,trueResult, falseResult)", 
        //    "Returs trueResult if condition is true otherwise returns false"
        //));
            
        Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/User.CornerSpeed;component/App.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary);

        EventManager.RegisterClassHandler(typeof(DashHostControls), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(SimHubControlLoaded));
        EventManager.RegisterClassHandler(typeof(DashHostControlsRendering), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(SimHubControlLoaded));
        EventManager.RegisterClassHandler(typeof(SelectedItemPreview), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(SimHubControlLoaded));

    }
        
    private static void SimHubControlLoaded(object sender, RoutedEventArgs e)
    {
        var control = (ContentControl)sender;
        
        { 
            DataTemplateKey dtKey = new DataTemplateKey(typeof(iRacingGroupItem));
            if (control.Resources[dtKey] == null)
                control.Resources.Add(dtKey, JackDashResource[dtKey]);
        }
        { 
            DataTemplateKey dtKey = new DataTemplateKey(typeof(iRacingSubGroupItem));
            if (control.Resources[dtKey] == null)
                control.Resources.Add(dtKey, JackDashResource[dtKey]);
        }
        { 
            DataTemplateKey dtKey = new DataTemplateKey(typeof(JackWidgetItem));
            if (control.Resources[dtKey] == null)
                control.Resources.Add(dtKey, JackDashResource[dtKey]);
        }
        { 
            DataTemplateKey dtKey = new DataTemplateKey(typeof(iRacingCornerSpeeds));
            if (control.Resources[dtKey] == null)
                control.Resources.Add(dtKey, JackDashResource[dtKey]);
        }
        { 
            DataTemplateKey dtKey = new DataTemplateKey(typeof(iRacingCornerSpeedDeltaItem));
            if (control.Resources[dtKey] == null)
                control.Resources.Add(dtKey, JackDashResource[dtKey]);
        }

        //foreach (var child in TreeHelper.FindVisualChildren<ItemsControl>(control))
        //{
        //    //MessageBox.Show("Injecting", "Question", MessageBoxButton.YesNo);
        //    //if (child.Resources[dtKey] != null)
        //    //    continue;

        //    //child.Resources.Add(dtKey, JackDashResource[dtKey]);
        //    var tems = child.ItemTemplateSelector;
        //    child.ItemTemplateSelector = null;
        //    child.ItemTemplateSelector = tems;
        //}

        if (control is DashHostControls dhc)
        {
            var ic = dhc.FindChild<ItemsControl>("icAdorners");
            var dts = ic.ItemTemplateSelector;
            ic.ItemTemplateSelector = null;
            ic.ItemTemplateSelector = dts;
            
            {
                var c = dhc.FindChild<Control>("GBackgroundContainer");
                var tem = c.Template;
                c.Template = null;
                c.Template = tem;
            }
            {
                var c = dhc.FindChild<Control>("GContainer");
                var tem = c.Template;
                c.Template = null;
                c.Template = tem;
            }
            {
                var c = dhc.FindChild<Control>("GContainerForeground");
                var tem = c.Template;
                c.Template = null;
                c.Template = tem;
            }
        }
        if (control is DashHostControlsRendering dhcr)
        {
            {
                var ic = dhcr.FindChild<Control>("GContainerScreen");
                var tem = ic.Template;
                ic.Template = null;
                ic.Template = tem;

            }
            {
                var ic = dhcr.FindChild<Control>("GContainerOverlay");
                if (ic != null)
                {
                    var tem = ic.Template;
                    ic.Template = null;
                    ic.Template = tem;
                }
            }
            {
                var ic = dhcr.FindChild<Control>("GContainerBackground");
                var tem = ic.Template;
                ic.Template = null;
                ic.Template = tem;
            }
            {
                var ic = dhcr.FindChild<Control>("GContainerForeground");
                var tem = ic.Template;
                ic.Template = null;
                ic.Template = tem;
            }
        }
    }

    public static ResourceDictionary JackDashResource = Application.LoadComponent(new Uri("/User.CornerSpeed;component/ControlsTemplates.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
}
}
