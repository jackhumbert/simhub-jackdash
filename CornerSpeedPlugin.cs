using GameReaderCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyChanged;
using SimHub.Plugins;
using SimHub.Plugins.Dashstudio.Behaviors.Core.Interfaces;
using SimHub.Plugins.DataPlugins.DataCore;
using SimHub.Plugins.Devices.DevicesExtensionsDummy;
using SimHub.Plugins.Devices.Registry.Impl.TurtleBeach.UI;
using SimHub.Plugins.OutputPlugins.EditorControls;
using SimHub.Plugins.OutputPlugins.GraphicalDash;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Render;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static User.CornerSpeed.CornerSpeedPlugin;


//         <ResourceDictionary Source="/SimHub.Plugins;component/OutputPlugins/GraphicalDash/Render/ControlsTemplates.xaml" />

//namespace SimHub.Plugins.OutputPlugins.GraphicalDash.Models
//{

namespace User.CornerSpeed
{ 
    //public static class MyExt
    //{
    //    public static int ieni(this DashHostControls dashHostControls) => 0;

    //}
    
    public class TurnNumbers
    {
        public List<Turn> turns;
    };

    public class Turn
    {
        public string name;
        public float start;
        public float end;
    };

    [PluginDescription("My plugin description")]
    [PluginAuthor("Jack Humbert")]
    [PluginName("Corner Speed")]
    public class CornerSpeedPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public CornerSpeedPluginSettings Settings;

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        //public SimpleChart item;
        //public SimHub.Plugins.OutputPlugins.GraphicalDash.SelectionTemplateSelector selector;

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "Corner Speed";

        public int NumberOfCorners = 5;

        public int NumberOfLaps = 10;

        public int CurrentLap = 0;

        public int CurrentCorner = -1;

        public List<List<double>> CornerSpeeds;
        
        public List<List<TimeSpan>> SectorTimes;
        public List<TimeSpan> SectorTimesBest;
        public List<TimeSpan> LapTimes;
        public List<int> LapNumbers;
        public int NumberOfSectors = 10;
        public int CurrentSector = -1;
        public string TrackId;
               
        public double[] CornerPositions = {
            0.055,
            0.150,
            0.166,
            0.183,
            0.348,
            0.358,
            0.380,
            0.431,
            0.439,
            0.471,
            0.550,
            0.584,
            0.647,
            0.664,
            0.707,
            0.745,
            0.887,
            0.966,
            0.972
        };
        
        public List<Sector> ComparisonLapSectors;

        public bool GamePaused = false;

        public TurnNumbers turnNumbers;

        public void UpdateTurnNumbers(string trackId)
        {
            var file = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                "TurnNumbers", 
                trackId + ".json"
            );
            if (File.Exists(file))
            {
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    turnNumbers = JsonConvert.DeserializeObject<TurnNumbers>(json);
                }
            }
        }

        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        ///
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {

             //PluginManager.GetPropertyValue("{prop}");
            // Define the value of our property (declared in init)
            if (data.GameRunning)
            {
                if (data.OldData != null && data.NewData != null)
                {
                    if (TrackId != data.NewData.TrackId)
                    {
                        TrackId = data.NewData.TrackId;
                        Settings.UpdateAvailableLapFiles(TrackId);
                        UpdateTurnNumbers(TrackId);
                    }

                    if (data.NewData.SectorsCount != NumberOfSectors)
                    {
                        NumberOfSectors = (int)data.NewData.SectorsCount;
                        //for (int i = 0; i < NumberOfLaps; i++) {
                            //SectorTimes[i] = new List<TimeSpan>(NumberOfSectors); 
                        //}
                    }
                    
                    if (CurrentSector != (data.NewData.CurrentSectorIndex - 1))
                    {
                        if (CurrentSector != -1) { 
                            if (SectorTimes[CurrentLap % NumberOfLaps][CurrentSector] < SectorTimesBest[CurrentSector])
                            {
                                SectorTimesBest[CurrentSector] = SectorTimes[CurrentLap % NumberOfLaps][CurrentSector];
                            }
                        }
                        CurrentSector = (data.NewData.CurrentSectorIndex - 1);
                    }
                    TimeSpan sectorsSum = TimeSpan.Zero;
                    for (int i = 0; i < CurrentSector; i++)
                    {
                        sectorsSum += SectorTimes[CurrentLap % NumberOfLaps][i];
                    }

                    if (data.NewData.CurrentLap != CurrentLap)
                    {
                        //LapTimes[CurrentLap % NumberOfLaps] = data.NewData.CurrentLapTime;
                        //LapTimes[CurrentLap % NumberOfLaps] = data.NewData.LastLapTime;
                        LapNumbers[CurrentLap % NumberOfLaps] = CurrentLap;

                        CurrentLap = data.NewData.CurrentLap;
                        for (int corner_i = 0; corner_i < NumberOfCorners; corner_i++) {
                            CornerSpeeds[CurrentLap % NumberOfLaps][corner_i] = 999.9;
                        }
                        for (int sector_i = 0; sector_i < NumberOfSectors; sector_i++) {
                            SectorTimes[CurrentLap % NumberOfLaps][sector_i] = new TimeSpan();
                        }
                    }
                    
                    LapTimes[CurrentLap % NumberOfLaps] = data.NewData.CurrentLapTime;
                    
                    SectorTimes[CurrentLap % NumberOfLaps][CurrentSector] = data.NewData.CurrentLapTime - sectorsSum;

                    CurrentCorner = -1;
                    for (int i = 0; i < CornerPositions.Length; i++)
                    {
                        if (Math.Abs(data.NewData.TrackPositionPercent - CornerPositions[i]) < 0.010)
                        {
                            CurrentCorner = i;
                            break;
                        }
                    }
                    //corner = (int)((data.NewData.TrackPositionPercent + (0.5 / NumberOfCorners)) % 1.0 * NumberOfCorners);
                    if (CurrentCorner != -1 && data.NewData.SpeedLocal < CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner])
                    {
                        //this.TriggerEvent("NewMinSpeed");
                        CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner] = data.NewData.SpeedLocal;
                    }

                }
            }
        }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControlDemo(this);
        }
        
        public class Sector
        {
            public float start;
            public float end;
            public uint num_markers;
            public float distance_per_marker;
            public float sector_start_info;
            public float sector_end_info;
            public float time;
            public uint flags;

            public List<Marker> Markers;
        };

        public class Marker
        {
            public float sector_time;
            public float timing_line_position;
            public float YawNorth; // <read=Str("%0.2fdeg (%0.6f)", this*57.2958, this)>;
            public float Pitch; // <read=Str("%0.2fdeg (%0.6f)", this*57.2958, this)>;
            public float Roll; // <read=Str("%0.2fdeg (%0.6f)", this*57.2958, this)>;
            public uint flags; // marker->unk14, off-track?
            public byte Throttle; // <read=Str("%d", this)>;
            public byte Brake; // <read=Str("%d", this)>;
            public byte Clutch; // <read=Str("%d", this)>; // inverse
            public byte Gear; // <read=Str("%d", this - 1)>; // gear plus one
        };

        public void LoadLapFile()
        {
            if (!File.Exists(Settings.LapFile)) { 
                ComparisonLapSectors.Clear();
                return;
            }
            using (FileStream fs = File.OpenRead(Settings.LapFile))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                { 
                    var header = reader.ReadBytes(1468);
                    var num_sectors = reader.ReadInt32();
                    ComparisonLapSectors = new List<Sector>();
                    for (int i = 0; i < num_sectors; i++)
                    {
                        ComparisonLapSectors.Add(new Sector {
                            start = reader.ReadSingle(),
                            end = reader.ReadSingle(),
                            num_markers = reader.ReadUInt32(),
                            distance_per_marker = reader.ReadSingle(),
                            sector_start_info = reader.ReadSingle(),
                            sector_end_info = reader.ReadSingle(),
                            time = reader.ReadSingle(),
                            flags = reader.ReadUInt32(),
                            Markers = new List<Marker>()
                        });
                    }
                    for (int i = 0; i < num_sectors; i++)
                    {
                        for (int j = 0; j < ComparisonLapSectors[i].num_markers; j++) 
                        {
                            ComparisonLapSectors[i].Markers.Add(new Marker
                            {
                                sector_time = reader.ReadSingle(),
                                timing_line_position = reader.ReadSingle(),
                                YawNorth = reader.ReadSingle(),
                                Pitch = reader.ReadSingle(),
                                Roll = reader.ReadSingle(),
                                flags = reader.ReadUInt32(),
                                Throttle = reader.ReadByte(),
                                Brake = reader.ReadByte(),
                                Clutch = reader.ReadByte(),
                                Gear = reader.ReadByte()
                            });
                        }
                    }
                }
            }
        }

        private static void OnDashHostControlsLoaded(object sender, RoutedEventArgs e)
        {
            var control = (FrameworkElement)sender;

            control.Resources.MergedDictionaries.Insert(0, JackDashResource);
        }

        public static ResourceDictionary JackDashResource;

        public double LapDist;
        private Marker _currentMarker;

        public Marker GetCurrentMarker() {
            if (ComparisonLapSectors.Count == 0)
                return null;
            var dist = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LapDist"));
            if (LapDist != dist) {
                LapDist = dist;
                var sector_i = 0;
                var marker_i = 0;
                var dist_cum = 0.0;
                while (dist_cum < dist) {
                    dist_cum += ComparisonLapSectors[sector_i].distance_per_marker;
                    marker_i++;
                    if (marker_i >= ComparisonLapSectors[sector_i].num_markers)
                    {
                        sector_i++;
                        marker_i = 0;
                    }
                }
                _currentMarker = ComparisonLapSectors[sector_i].Markers[marker_i];
            }
            return _currentMarker;
        }
                
        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting plugin");

            JackDashResource = Application.LoadComponent(new Uri("/User.CornerSpeed;component/ControlsTemplates.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
            
            EventManager.RegisterClassHandler(typeof(DashHostControls), FrameworkElement.LoadedEvent, new RoutedEventHandler(OnDashHostControlsLoaded));
            EventManager.RegisterClassHandler(typeof(DashHostControlsRendering), FrameworkElement.LoadedEvent, new RoutedEventHandler(OnDashHostControlsLoaded));

            // Load settings
            Settings = this.ReadCommonSettings<CornerSpeedPluginSettings>("GeneralSettings", () => new CornerSpeedPluginSettings());

            //Settings.UpdateLapFiles();
                        
            LoadLapFile();

            CornerSpeeds = new List<List<double>>();
            SectorTimes = new List<List<TimeSpan>>();
            SectorTimesBest = new List<TimeSpan>();
            LapTimes = new List<TimeSpan>();
            LapNumbers = new List<int>();
            //CornerSpeeds = new List<List<double>>(NumberOfLaps);

            NumberOfCorners = CornerPositions.Length;
            
            this.AttachDelegate(name: "NumberOfCorners", valueProvider: () => NumberOfCorners);
            this.AttachDelegate(name: "CurrentCorner", valueProvider: () => CurrentCorner);

            this.AttachDelegate(name: "TurnNumber", valueProvider: () => { 
                //var pct = PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LapDistPct");
                var pct = PluginManager.GetPropertyValue("TrackPositionPercent");
                if (pct != null && turnNumbers != null) { 
                    var dist = Convert.ToDouble(pct);
                    for (int i = 0; i < turnNumbers.turns.Count; i++)
                    {
                        if (turnNumbers.turns[i].start <= dist && dist < turnNumbers.turns[i].end)
                        {
                            return turnNumbers.turns[i].name;
                        }
                    }
                }
                return null;
            });

            this.AttachDelegate(name: "NumberOfSectors", valueProvider: () => NumberOfSectors);
            this.AttachDelegate(name: "CurrentSector", valueProvider: () => CurrentSector);
            
            this.AttachDelegate(name: "GamePaused", valueProvider: () => GamePaused);

            this.AttachDelegate(name: "HasComparisonLap", valueProvider: () => ComparisonLapSectors.Count != 0);
            
            this.AttachDelegate(name: "ComparisonLapThrottle", valueProvider: () => {
                if (GetCurrentMarker() != null)
                    return GetCurrentMarker().Throttle;
                return 0.0;
            });
            this.AttachDelegate(name: "ComparisonLapBrake", valueProvider: () => {
                if (GetCurrentMarker() != null)
                    return GetCurrentMarker().Brake;
                return 0.0;
            });
            this.AttachDelegate(name: "ComparisonLapGear", valueProvider: () => {
                if (GetCurrentMarker() != null)
                    return GetCurrentMarker().Gear;
                return 0.0;
            });
            this.AttachDelegate(name: "ComparisonLapDelta", valueProvider: () => {
                if (ComparisonLapSectors.Count == 0)
                    return 0.0;
                var dist = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LapDist"));
                var sector_i = 0;
                var marker_i = 0;
                var dist_cum = 0.0;
                var time_cum = 0.0;
                while (dist_cum < dist) {
                    dist_cum += ComparisonLapSectors[sector_i].distance_per_marker;
                    marker_i++;
                    if (marker_i >= ComparisonLapSectors[sector_i].num_markers)
                    {
                        time_cum += ComparisonLapSectors[sector_i].Markers[marker_i-1].sector_time;
                        sector_i++;
                        marker_i = 0;
                    }
                }
                //var time = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LapCurrentLapTime"));
                return time_cum + ComparisonLapSectors[sector_i].Markers[marker_i].sector_time;
            });

            // Declare a property available in the property list, this gets evaluated "on demand" (when shown or used in formulas)
            for (int lap = 0; lap < NumberOfLaps; lap++) {
                //CornerSpeeds.Insert(lap, new List<double>(NumberOfCorners));
                SectorTimes.Add(new List<TimeSpan>());
                CornerSpeeds.Add(new List<double>());
                LapTimes.Add(TimeSpan.Zero);
                LapNumbers.Add(-1);
                var ilap_s = lap.ToString().PadLeft(2, '0');
                int ilap_i = lap;
                this.AttachDelegate(name: $"LapTimes_{ilap_s}", valueProvider: () => LapTimes[(CurrentLap + NumberOfLaps - ilap_i) % NumberOfLaps]);
                this.AttachDelegate(name: $"LapNumbers_{ilap_s}", valueProvider: () => LapNumbers[(CurrentLap + NumberOfLaps - ilap_i) % NumberOfLaps]);


                for (int corner = 0; corner < NumberOfCorners; corner++) {
                    CornerSpeeds[lap].Add(999.9);
                    var lap_s = lap.ToString().PadLeft(2, '0');
                    var corner_s = corner.ToString().PadLeft(2, '0');
                    int lap_i = lap;
                    int corner_i = corner;
                    this.AttachDelegate(name: $"CornerSpeed_{lap_s}_{corner_s}", valueProvider: () => CornerSpeeds[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][corner_i]);
                    this.AttachDelegate(name: $"CornerSpeedRating_{lap_s}_{corner_s}", valueProvider: () => {
                        var min = 999.9;
                        var max = 0.0;
                        for (int lap_j = 0; lap_j < NumberOfLaps; lap_j++)
                        {
                            if (CornerSpeeds[lap_j][corner_i] == 0.0 || CornerSpeeds[lap_j][corner_i] == 999.9)
                                continue;
                            if (CornerSpeeds[lap_j][corner_i] < min)
                            {
                                min = CornerSpeeds[lap_j][corner_i];
                            }
                            if (CornerSpeeds[lap_j][corner_i] > max)
                            {
                                max = CornerSpeeds[lap_j][corner_i];
                            }
                        }
                        if (min != 999.9 && max != 0.0) { 
                            return (CornerSpeeds[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][corner_i] - min) / (max - min);
                        } else
                        {
                            return 0.0;
                        }
                    });
                }
                
                for (int sector = 0; sector < NumberOfSectors; sector++) {
                    SectorTimes[lap].Add(TimeSpan.Zero);
                    var lap_s = lap.ToString().PadLeft(2, '0');
                    var sector_s = sector.ToString().PadLeft(2, '0');
                    int lap_i = lap;
                    int sector_i = sector;
                    this.AttachDelegate(name: $"SectorTime_{lap_s}_{sector_s}", valueProvider: () => SectorTimes[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][sector_i]);
                    
                    this.AttachDelegate(name: $"SectorTimeRating_{lap_s}_{sector_s}", valueProvider: () => {
                        var min = 999.9;
                        var max = 0.0;
                        for (int lap_j = 0; lap_j < NumberOfLaps; lap_j++)
                        {
                            if (SectorTimes[lap_j][sector_i] == TimeSpan.Zero)
                                continue;
                            if (SectorTimes[lap_j][sector_i].TotalSeconds < min)
                            {
                                min = SectorTimes[lap_j][sector_i].TotalSeconds;
                            }
                            if (SectorTimes[lap_j][sector_i].TotalSeconds > max)
                            {
                                max = SectorTimes[lap_j][sector_i].TotalSeconds;
                            }
                        }
                        if (min != 999.9 && max != 0.0) { 
                            return (SectorTimes[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][sector_i].TotalSeconds - min) / (max - min);
                        } else
                        {
                            return 0.0;
                        }
                    });
                }
            }

            
            for (int sector = 0; sector < NumberOfSectors; sector++) {
                SectorTimesBest.Add(TimeSpan.MaxValue);
                var sector_s = sector.ToString().PadLeft(2, '0');
                int sector_i = sector;
                this.AttachDelegate(name: $"SectorTimeBest_{sector_s}", valueProvider: () => SectorTimesBest[sector_i]);
            }

            // Declare an event
            //this.AddEvent(eventName: "SpeedWarning");
            //this.AddEvent(eventName: "NewMinSpeed");

            // Declare an action which can be called
            //this.AddAction(
            //    actionName: "IncrementSpeedWarning",
            //    actionStart: (a, b) =>
            //    {
            //        Settings.SpeedWarningLevel++;
            //        SimHub.Logging.Current.Info("Speed warning changed");
            //    });

            // Declare an action which can be called, actions are meant to be "triggered" and does not reflect an input status (pressed/released ...)
            this.AddAction(
                actionName: "PauseGame",
                actionStart: (a, b) =>
                {
                    GamePaused = true;
                });

            this.AddAction(
                actionName: "UnpauseGame",
                actionStart: (a, b) =>
                {
                    GamePaused = false;
                });
            
            this.AddAction(
                actionName: "ToggleGamePaused",
                actionStart: (a, b) =>
                {
                    GamePaused = !GamePaused;
                });

            // Declare an input which can be mapped, inputs are meant to be keeping state of the source inputs,
            // they won't trigger on inputs not capable of "holding" their state.
            // Internally they work similarly to AddAction, but are restricted to a "during" behavior
            //this.AddInputMapping(
            //    inputName: "InputPressed",
            //    inputPressed: (a, b) => {/* One of the mapped input has been pressed   */},
            //    inputReleased: (a, b) => {/* One of the mapped input has been released */}
            //);

        }
    }
}