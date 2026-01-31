using PropertyChanged;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.CornerSpeed
{
    //[DesignerIcon("/SimHub.Plugins;component/Resources/comp_widget.png")]
    [AddINotifyPropertyChangedInterface]
    public class JackWidgetItem : ChartItem // : ChartItem //, IDashControl
    {
        public override string ComponentTypeName => "JackWidget";

        //public DashHostControls s;
        //public GroupPresenter s;

        public JackWidgetItem()
        {
            // should add to SimHub.Plugins.OutputPlugins.GraphicalDash.DashHostControls only
            //if (!IsEditMode && !IsDesignMode) { 
            //var dict = Application.LoadComponent(new Uri("/User.CornerSpeed;component/ControlsTemplates.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

            //if (Application.Current.Resources == null)
            //    Application.Current.Resources = new ResourceDictionary();
            //Application.Current.Resources.MergedDictionaries.Add(dict);
        //}

        //Owner.

        //var dict = Application.LoadComponent(new Uri("/User.CornerSpeed;component/ControlsTemplates.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

        // SimHub.Plugins.g.resources outputplugins/graphicaldash/render/controlsTemplates.baml

        //var resourceManager = new ResourceManager("SimHub.Plugins.g", Assembly.GetAssembly(typeof(DrawableItem)));
        //var resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        //var stream = resources.GetObject("outputplugins/graphicaldash/render/controlsTemplates.baml") as Stream;
        //new ResourceDictionary(stream!)


        //var res = Application.Current.TryFindResource(typeof(DashHostControls));
        //if (res != null)
        //{
        //}


        //foreach(string resourceName in Assembly.GetAssembly(typeof(DrawableItem)).GetManifestResourceNames())
        //{
        //    resourceName
        //}

        //if (Screen != null && Screen.Container != null && !Screen.Container.Resources.MergedDictionaries.Contains(dict))
        //var res = Application.Current.Resources.MergedDictionaries.First(x => x.Source == new Uri("/SimHub.Plugins;component/OutputPlugins/GraphicalDash/Render/ControlsTemplates.xaml"));

        //res.MergedDictionaries.Add(dict);

    }

        //public override void UpdateData(GameData data, Action<IBindable> postAction)
        //{
        //    base.UpdateData(data, postAction);
        //}
        
        //public override FrameworkElement ExtraEditControl => new JackChart();

        //private bool isweb;
        //private bool init;
        //private Queue<double> WebData = new Queue<double>();

        //public JackWidgetItem()
        //{
        //    Points = GetDefaultPoints();
        //}

        //private SimpleStreamChartPointsCollection GetDefaultPoints()
        //{
        //    SimpleStreamChartPointsCollection simpleStreamChartPointsCollection = new SimpleStreamChartPointsCollection();
        //    simpleStreamChartPointsCollection.MaxPoints = (int)2048;
        //    simpleStreamChartPointsCollection.AddPoint(4.0);
        //    simpleStreamChartPointsCollection.AddPoint(13.0);
        //    simpleStreamChartPointsCollection.AddPoint(15.0);
        //    simpleStreamChartPointsCollection.AddPoint(16.0);
        //    simpleStreamChartPointsCollection.AddPoint(12.0);
        //    simpleStreamChartPointsCollection.AddPoint(12.0);
        //    simpleStreamChartPointsCollection.AddPoint(4.0);
        //    simpleStreamChartPointsCollection.AddPoint(13.0);
        //    simpleStreamChartPointsCollection.AddPoint(15.0);
        //    simpleStreamChartPointsCollection.AddPoint(16.0);
        //    simpleStreamChartPointsCollection.AddPoint(12.0);
        //    simpleStreamChartPointsCollection.AddPoint(12.0);
        //    simpleStreamChartPointsCollection.AddPoint(4.0);
        //    simpleStreamChartPointsCollection.AddPoint(13.0);
        //    simpleStreamChartPointsCollection.AddPoint(15.0);
        //    simpleStreamChartPointsCollection.AddPoint(16.0);
        //    simpleStreamChartPointsCollection.AddPoint(12.0);
        //    simpleStreamChartPointsCollection.AddPoint(12.0);
        //    return simpleStreamChartPointsCollection;
        //}

        //[Category("Chart")]
        //public bool ChartSuspended { get; set; }

        //[Category("Chart")]
        //public bool ChartEnabled { get; set; } = true;
                
        //[WebIgnore]
        //[Category("Chart")]
        //public double CurrentValue { get; set; }

        
        //public void OnChartEnabledChanged()
        //{
        //    if (base.IsDesignMode && !ChartEnabled)
        //    {
        //        Points = new SimpleStreamChartPointsCollection();
        //    }
        //    else if (base.IsDesignMode)
        //    {
        //        Points = GetDefaultPoints();
        //    }
        //}

        //public override void UpdateData(GameData data, Action<IBindable> postAction)
        //{
        //    if (!init)
        //    {
        //        init = true;
        //        //Points.Clear();
        //    }

        //    base.UpdateData(data, postAction);
        //    if (!ChartSuspended && ChartEnabled)
        //    {
        //        Points.AddPoint(CurrentValue);
        //        if (isweb)
        //        {
        //            WebData.Enqueue(CurrentValue);
        //        }
        //        while ((double)WebData.Count > 2048)
        //        {
        //            WebData.Dequeue();
        //        }
        //    }

        //    if (!ChartEnabled)
        //    {
        //        //Points.Clear();
        //    }
        //}
        
        //public object GetWebData(object previous, bool visible)
        //{
        //    isweb = true;
        //    if (WebData.Count > 0)
        //    {
        //        var result = new
        //        {
        //            Points = WebData.ToArray()
        //        };
        //        WebData.Clear();
        //        return result;
        //    }

        //    return null;
        //}

        //public void IncomingData(string data)
        //{
        //}

        //[JsonIgnore]
        //[Browsable(false)]
        //public SimpleStreamChartPointsCollection Points { get; set; }

    }
}
