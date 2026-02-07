using log4net.Plugin;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using PropertyChanged;
using SimHub.Plugins;
using SimHub.Plugins.OutputPlugins.EditorControls;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xamarin.Forms.Xaml;

namespace User.CornerSpeed
{    
    [AddINotifyPropertyChangedInterface]
    public class iRacingCornerSpeeds : DrawableItem
    {
        public override string ComponentTypeName => "iRacingCornerSpeeds";
        
        [Browsable(false)]
        [JsonIgnore]
        public CornerSpeedPlugin Plugin { get; set; }

        public iRacingCornerSpeeds() : base()
        {
            Plugin = PluginManager.GetInstance().GetPlugin<CornerSpeedPlugin>();
        }

        [Browsable(false)]
        [JsonIgnore]
        public override bool FreezeWhenRunning => false;

        [Browsable(false)]
        [JsonIgnore]
        public string TrackId => Plugin.TrackId;

        //[Browsable(false)]
        //[JsonIgnore]
        //public ComparisonMode Mode => Plugin.Mode;
        
        [Browsable(false)]
        [JsonIgnore]
        public ObservableCollection<CornerViewModel> Corners { 
            get {
                if (IsDesignMode || IsEditMode)
                {
                    return new ObservableCollection<CornerViewModel> { 
                        new CornerViewModel(new Corner
                        {
                            name = "Turn 3",
                            start_speed = 90.56,
                            min_speed = 78.46,
                            end_speed = 80.46,
                            start_time = TimeSpan.FromSeconds(13.645),
                            end_time = TimeSpan.FromSeconds(18.645),
                            init = true
                        }, ComparisonMode.CompareToBestCarLap, new Dictionary<ComparisonMode, Corner> {
                            [ComparisonMode.CompareToBestCarLap] = new Corner
                        {
                            name = "Turn 3",
                            start_speed = 80.56,
                            min_speed = 76.46,
                            end_speed = 88.46,
                            start_time = TimeSpan.FromSeconds(12.645),
                            end_time = TimeSpan.FromSeconds(19.645),
                            init = true
                        } }),
                        new CornerViewModel(new Corner
                        {
                            name = "Turn 2",
                            start_speed = 90.56,
                            min_speed = 78.46,
                            end_speed = 80.46,
                            start_time = TimeSpan.FromSeconds(13.645),
                            end_time = TimeSpan.FromSeconds(28.645),
                            Valid = false
                        }, ComparisonMode.CompareToBestCarLap, new Dictionary<ComparisonMode, Corner> {
                            [ComparisonMode.CompareToBestCarLap] =new Corner
                        {
                            name = "Turn 2",
                            start_speed = 80.56,
                            min_speed = 76.46,
                            end_speed = 98.46,
                            start_time = TimeSpan.FromSeconds(12.645),
                            end_time = TimeSpan.FromSeconds(19.645)
                        } }),
                        new CornerViewModel(new Corner
                        {
                            name = "Turn 1",
                            start_speed = 70.56,
                            min_speed = 78.46,
                            end_speed = 80.46,
                            start_time = TimeSpan.FromSeconds(17.645),
                            end_time = TimeSpan.FromSeconds(18.645)
                        }, ComparisonMode.CompareToBestCarLap, new Dictionary<ComparisonMode, Corner> {
                            [ComparisonMode.CompareToBestCarLap] =new Corner
                        {
                            name = "Turn 1",
                            start_speed = 80.56,
                            min_speed = 76.46,
                            end_speed = 88.46,
                            start_time = TimeSpan.FromSeconds(12.645),
                            end_time = TimeSpan.FromSeconds(19.645)
                        } })
                    };
                } else
                {
                    return Plugin.CornerAttempts;
                }
            }
                //return 
        }

    }
}
