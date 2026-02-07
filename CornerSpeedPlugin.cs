using FMOD.Studio;
using GameReaderCommon;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyChanged;
using SimHub.Plugins;
using SimHub.Plugins.ChartingWindow;
using SimHub.Plugins.Dashstudio.Behaviors.Core.Interfaces;
using SimHub.Plugins.DataPlugins.DataCore;
using SimHub.Plugins.Devices.DevicesExtensionsDummy;
using SimHub.Plugins.Devices.Registry.Impl.TurtleBeach.UI;
using SimHub.Plugins.OutputPlugins.Dash.WPFUI;
using SimHub.Plugins.OutputPlugins.EditorControls;
using SimHub.Plugins.OutputPlugins.GraphicalDash;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Models;
using SimHub.Plugins.OutputPlugins.GraphicalDash.Render;
using SimHub.Plugins.OutputPlugins.GraphicalDash.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using User.CornerSpeed;
using WoteverCommon.Extensions;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using static User.CornerSpeed.CornerSpeedPlugin;

namespace User.CornerSpeed
{

    //public class CustomEntry : FormulaPropertyEntry
    //{
    //    public CustomEntry(string Name, string syntax, string description) : base(Name, syntax, description)
    //    {
    //    }
    //}

    public class iRacingTurnNumbers
    {
        public List<Turn> turns = [];
    };

    public class Turn
    {
        public string name;
        public float start; // meters
        public float end; // meters

        public float distance_per_marker = 2.0f;
    };
    
    //[AddINotifyPropertyChangedInterface]
    public class Corner : Turn, INotifyPropertyChanged
    {

        private static PluginManager PluginManager;
        private static int IncidentCount = -1;

        public Corner()
        {
            PluginManager ??= PluginManager.GetInstance();
        }

        public Corner(Turn turn, int index) : this()
        {
            name = turn.name;
            start = turn.start;
            end = turn.end;
            Index = index;
        }

        [JsonIgnore]
        public int Index;

        public event PropertyChangedEventHandler PropertyChanged;

        public double start_speed { get; set;} = 0.0;
        public double end_speed { get; set; } = 0.0;
        public double min_speed { get; set; } = 999.9;

        public bool init { get; set; } = false;

        public TimeSpan start_time { get; set; }

        public TimeSpan end_time { get; set; }
        
        /// <summary>
        /// The session time when the min_speed occured
        /// </summary>
        public TimeSpan min_time { get; set; }
        
        /// <summary>
        /// The track dist where the min_speed occured
        /// </summary>
        public double min_dist { get; set; }

        [JsonIgnore]
        public TimeSpan Duration => end_time - start_time;

        [JsonIgnore]
        public bool Active { get; set; } = false;

        [JsonIgnore]
        public bool Valid { get; set; } = true;

        public ObservableCollection<TimeSpan> markers = [];
        
        public void Start()
        {
            Active = true;
            start_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));
            start_speed = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameData.SpeedLocal"));

            // reset vars
            end_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));
            min_speed = 999.9;
            Valid = true;

            markers = [];

            //markers = [.. new TimeSpan[((int)((end - start) / distance_per_marker))]];
        }

        public void Update()
        {
            var session_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));
            var time = session_time - start_time;
            var dist = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LapDist"));
            var length = dist - start;
            if (start > end)
            {
                length += Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameData.TrackLength"));
            }
            var index = (int)((length) / distance_per_marker);
            //markers[(int)((dist - start) / distance_per_marker)] = time;
            if (markers.Count < index)
            {
                markers.Add(time);
            }

            var incidentCount = Convert.ToInt32(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.PlayerCarMyIncidentCount"));
            if (incidentCount > IncidentCount && IncidentCount != -1)
            {
                Valid = false;
            }
            IncidentCount = incidentCount;
            var black_flag = Convert.ToInt32(PluginManager.GetPropertyValue("DataCorePlugin.GameData.Flag_Black"));
            if (black_flag > 0)
            {
                Valid = false;
            }

            var speed = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameData.SpeedLocal"));
            if (speed < min_speed)
            {
                min_speed = speed;
                min_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));
                min_dist = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LapDist"));
            }

            end_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));
            end_speed = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameData.SpeedLocal"));
        }

        public void End()
        {
            Active = false;
            init = true;
            var time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));
            
            end_time = time;
            var speed = Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameData.SpeedLocal"));            
            end_speed = speed;
        }

        public Corner Copy()
        {
            return new Corner()
            {
                name = name,
                Index = Index,
                start = start,
                end = end,
                Valid = Valid,
                start_time = start_time,
                end_time = end_time,
                min_time = min_time,
                min_dist = min_dist,
                start_speed = start_speed,
                end_speed = end_speed,
                min_speed = min_speed,
                init = init,
                Active = Active,
                markers = markers
            };
        }
    };
    
    //[AddINotifyPropertyChangedInterface]
    public class CornerViewModel : INotifyPropertyChanged
    {
        public Corner Corner { get; set; }
        public Dictionary<ComparisonMode, Corner> ComparisonCorners { get; set; }

        public ComparisonMode Mode { get; set;} = ComparisonMode.CompareToBestCarLap;

        public int TurnIndex => Corner.Index;
        public string TurnName => Corner.name;
        public double StartSpeed => Corner.start_speed;
        public double MinSpeed => Corner.min_speed;
        public double EndSpeed => Corner.end_speed;

        public double StartSpeedDelta => Corner.start_speed - (ComparisonCorners[Mode]?.start_speed ?? 0.0);
        public double MinSpeedDelta => Corner.min_speed - (ComparisonCorners[Mode]?.min_speed ?? 0.0);
        public double EndSpeedDelta => Corner.end_speed - (ComparisonCorners[Mode]?.end_speed ?? 0.0);

        public TimeSpan Duration => Corner.Duration;
        public TimeSpan DurationDelta => (IsActive) ? LiveDelta : Corner.Duration - (ComparisonCorners[Mode]?.Duration ?? TimeSpan.Zero);
        public TimeSpan Apex => Corner.min_time - Corner.start_time;
        public TimeSpan ApexDelta => (Corner.min_time - Corner.start_time) - ((ComparisonCorners[Mode]?.min_time ?? TimeSpan.Zero) - (ComparisonCorners[Mode]?.start_time ?? TimeSpan.Zero));
        public double ApexDist => Corner.min_dist - Corner.start;
        public double ApexDistDelta => (Corner.min_dist - Corner.start) - ((ComparisonCorners[Mode]?.min_dist ?? 0.0) - (ComparisonCorners[Mode]?.start ?? 0.0));
        public bool WasShortestDuration => !IsActive && Corner.Duration < (ComparisonCorners[Mode]?.Duration ?? TimeSpan.Zero);
        public bool IsNewEntry => !ComparisonCorners[Mode]?.init ?? true;
        public bool IsInvalid => !Corner.Valid;
        public bool IsActive => Corner.Active;
        public bool Latest { get; set; } = true;

        public TimeSpan LiveDelta { get {
                if (Corner.markers.Count > 0) {
                    var comp = ComparisonCorners[Mode];
                    var index = Corner.markers.Count - 1;
                    if (comp != null && comp.markers.Count > index) {
                        return Corner.markers[index] - comp.markers[index];
                    }
                }
                return TimeSpan.Zero;
            }
        }

        public CornerViewModel()
        {
        }

        public Corner ComparisonCorner => ComparisonCorners[Mode];
        
        public CornerViewModel(Corner corner, ComparisonMode mode, Dictionary<ComparisonMode, Corner> comparisonCorners) : this()
        {
            Corner = corner;
            Mode = mode;
            ComparisonCorners = comparisonCorners;

            Corner.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Corner) {
                PluginManager.GetInstance().UIUpdateDispatcher.Dispatcher.Invoke(() => 
                {
                    if (e.PropertyName == "Duration") {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Duration"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DurationDelta"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WasShortestDuration"));
                    }
                    if (e.PropertyName == "marker") {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DurationDelta"));
                    }
                    else if (e.PropertyName == "Valid")
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsInvalid"));
                    }
                    else if (e.PropertyName == "Active")
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsActive"));
                    }
                    else if (e.PropertyName == "min_speed")
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinSpeed"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinSpeedDelta"));
                    }
                    else if (e.PropertyName == "min_time")
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Apex"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApexDelta"));
                    }
                    else if (e.PropertyName == "min_dist")
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApexDist"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ApexDistDelta"));
                    }
                    else if (e.PropertyName == "end_speed")
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndSpeed"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndSpeedDelta"));
                    }
                    else if (e.PropertyName == "init")
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsNewEntry"));
                    }
                });
            } else {
                PluginManager.GetInstance().UIUpdateDispatcher.Dispatcher.Invoke(() =>
                { 
                    PropertyChanged?.Invoke(this, e);
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    };

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

    public static class MyEnumExtensions
    {
        public static string ToDescriptionString(this ComparisonMode val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    } 

    public enum ComparisonMode
    {
        [Description("vs Best in Car")]
        CompareToBestCarLap,
        [Description("vs Session Best in Car")]
        CompareToBestSessionCarLap,
        [Description("vs Comparison Lap")]
        CompareToComparisonLap,

        Count
    };

    [PluginDescription("My plugin description")]
    [PluginAuthor("Jack Humbert")]
    [PluginName("Corner Speed")]
    [AddINotifyPropertyChangedInterface]
    public class CornerSpeedPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        private static readonly Program program = new Program();

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

        public int NumberOfCorners = 20;

        public int NumberOfLaps = 10;

        public int CurrentLap = 0;

        //public int CurrentCorner = -1;
        //public int LastCorner = -1;

        public List<List<Corner>> CornerSpeeds;
        
        public TimeSpan CornerDelta;

        //public TimeSpan OverallDelta { get
        //    {

        //    }
        //}
        
        
        [AlsoNotifyFor("OptimalCornerSpeedsLap")]
        public CornerSpeedsStorage BestCarCornerSpeeds { get; set; }
        
        //public CornerSpeedsStorage BestCarClassCornerSpeeds;

        [AlsoNotifyFor("OptimalCornerSpeedsLap")]
        public ObservableCollection<Corner> BestSessionCornerSpeeds;

        public ObservableCollection<Corner> CurrentCornerSpeeds;

        private ComparisonMode _mode = ComparisonMode.CompareToBestCarLap;

        
        [AlsoNotifyFor("OptimalCornerSpeedsLap")]
        public ComparisonMode Mode { 
            get => _mode; 
            set { 
                //var oldCorners = CornersToCompare;
                _mode = value;
                //var new_attempts = new List<CornerViewModel>();
                for (int i = 0; i < CornerAttempts.Count; i++)
                {
                    //var turn_index = CornerAttempts[i].TurnIndex;
                    //CornerAttempts[i] = new CornerViewModel(CornerAttempts[i], oldCorners[turn_index], CornersToCompare[turn_index]);
                    CornerAttempts[i].Mode = value;
                    //CornerAttempts[i].UpdateComparison(CornersToCompare[turn_index]);
                    //new_attempts.Add(new CornerViewModel(CurrentCornerSpeeds[turn_index], CornersToCompare[turn_index]));
                }
                //CornerAttempts.Clear();
                //CornerAttempts.AddAll(new_attempts);
            } 
        }

        public ObservableCollection<Corner> CornersToCompare { get
            {
                switch (Mode)
                {
                    case ComparisonMode.CompareToComparisonLap: 
                        if (ComparisonLapSectors.Count > 0) 
                            return ComparisonLapCorners;
                        goto default;
                    default:
                    case ComparisonMode.CompareToBestCarLap: return BestCarCornerSpeeds?[TrackId] ?? new ObservableCollection<Corner>();
                    case ComparisonMode.CompareToBestSessionCarLap: return BestSessionCornerSpeeds;
                }
            }    
        }

        public ObservableCollection<CornerViewModel> CornerAttempts = new ObservableCollection<CornerViewModel>();
        
        public List<List<TimeSpan>> SectorTimes;
        public List<TimeSpan> SectorTimesBest;
        public List<TimeSpan> LapTimes;
        public List<int> LapNumbers;
        public int NumberOfSectors = 10;
        public int CurrentSector = -1;
        public string CarId;

        public string TrackId { get; set; }
               
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
        public ObservableCollection<Corner> ComparisonLapCorners;

        public bool GamePaused = false;

        public iRacingTurnNumbers TurnNumbers;

        public void UpdateTurnNumbers(string trackId)
        {
            var file = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                "iracing-turn-numbers", 
                trackId + ".json"
            );
            if (File.Exists(file))
            {
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    TurnNumbers = JsonConvert.DeserializeObject<iRacingTurnNumbers>(json);
                    return;
                }
            } 
            else
            {
                using (StreamWriter w = new StreamWriter(file))
                {
                    TurnNumbers = new iRacingTurnNumbers();
                    string json = JsonConvert.SerializeObject(TurnNumbers);
                    w.Write(json);
                    return;
                }
            }
            
        }

        public TimeSpan OptimalCornerSpeedsLap {
            get {
                var sum = TimeSpan.Zero;

                foreach (TimeSpan value in CornersToCompare.Select(o => o.Duration)) {
                    sum += value;
                }
                return sum;
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
                    var timeValid = true;
                    //if (data.OldData.LapInvalidated || data.NewData.LapInvalidated)
                        //timeValid = false;
                    if (data.NewData.Rpms < 300)
                        timeValid = false;
                    //if (GamePaused)
                        //timeValid = false;
                    if (Math.Abs(data.NewData.TrackPositionMeters - data.OldData.TrackPositionMeters) > 100 && data.NewData.CurrentLap == CurrentLap)
                        timeValid = false;
                    if (data.NewData.IsInPit > 0 || data.NewData.IsInPitLane > 0)
                        timeValid = false;
                    if (CarId != data.NewData.CarId)
                    {
                        CarId = data.NewData.CarId;
                        BestCarCornerSpeeds = this.ReadCommonSettings(CarId, () => new CornerSpeedsStorage());
                    }
                    if (TrackId != data.NewData.TrackId)
                    {
                        TrackId = data.NewData.TrackId;
                        Settings.UpdateAvailableLapFiles(TrackId);
                        UpdateTurnNumbers(TrackId);

                        if (!BestCarCornerSpeeds.ContainsKey(TrackId) && TurnNumbers.turns.Count > 0)
                        {
                            BestCarCornerSpeeds[TrackId] = new ObservableCollection<Corner>(TurnNumbers.turns.Select((o, i) => new Corner(o, i)));
                        }
                        if (ComparisonLapSectors.Count > 0 && TurnNumbers.turns.Count > 0)
                        {
                            ComparisonLapCorners = GetCornersFromComparisonLapSectors();
                        }
                        if (TurnNumbers.turns.Count > 0)
                        {
                            CurrentCornerSpeeds = new ObservableCollection<Corner>(TurnNumbers.turns.Select((o, i) => new Corner(o, i)));
                            BestSessionCornerSpeeds = new ObservableCollection<Corner>(TurnNumbers.turns.Select((o, i) => new Corner(o, i)));
                        }
                        Mode = _mode;
                    }


                    // look at DataCorePlugin.GameRawData.Telemetry.TrackWetness too
                    // mabye DataCorePlugin.GameRawData.Telemetry.TrackTemp as well

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

                    //var lapChanged = false;
                    if (data.NewData.CurrentLap != CurrentLap)
                    {
                        //lapChanged = true;
                        //LapTimes[CurrentLap % NumberOfLaps] = data.NewData.CurrentLapTime;
                        //LapTimes[CurrentLap % NumberOfLaps] = data.NewData.LastLapTime;
                        LapNumbers[CurrentLap % NumberOfLaps] = CurrentLap;

                        CurrentLap = data.NewData.CurrentLap;
                        for (int corner_i = 0; corner_i < NumberOfCorners; corner_i++) {
                            if (corner_i < TurnNumbers.turns.Count)
                            {
                                CornerSpeeds[CurrentLap % NumberOfLaps][corner_i] = new Corner(TurnNumbers.turns[corner_i], corner_i);
                            } else
                            {
                                CornerSpeeds[CurrentLap % NumberOfLaps][corner_i] = new Corner();
                            }
                        }
                        for (int sector_i = 0; sector_i < NumberOfSectors; sector_i++) {
                            SectorTimes[CurrentLap % NumberOfLaps][sector_i] = new TimeSpan();
                        }
                        
                        foreach (var corner in CurrentCornerSpeeds.Where(o => o.Active))
                        {
                            if (corner.end > corner.start)
                            {
                                corner.Valid = false;
                            }
                        }
                    }
                    
                    LapTimes[CurrentLap % NumberOfLaps] = data.NewData.CurrentLapTime;
                    
                    SectorTimes[CurrentLap % NumberOfLaps][CurrentSector] = data.NewData.CurrentLapTime - sectorsSum;

                    //foreach (var corner in CurrentCornerSpeeds.Where(o => !o.Active))
                    for (int i = 0; i < CurrentCornerSpeeds.Count; i++)
                    {
                        var corner = CurrentCornerSpeeds[i];
                        if (corner.Active)
                            continue;
                        if ((corner.end > corner.start && data.NewData.TrackPositionMeters >= corner.start && data.NewData.TrackPositionMeters < corner.end) || 
                            (corner.end < corner.start && data.NewData.TrackPositionMeters >= corner.start))
                        {
                            CurrentCornerSpeeds[i] = corner.Copy();
                            corner = CurrentCornerSpeeds[i];
                            corner.Start();
                            if ((corner.end > corner.start && ( 
                                data.OldData.TrackPositionMeters >= corner.start && 
                                data.OldData.TrackPositionMeters < corner.end
                                )
                            ) || 
                            (corner.end < corner.start && (
                                data.OldData.TrackPositionMeters >= corner.start
                                )
                            ))
                                corner.Valid = false;
                            if (!timeValid)
                                corner.Valid = false;
                            
                            pluginManager.UIUpdateDispatcher.Dispatcher.Invoke(() => { 
                                if (CornerAttempts.Count > 1)
                                    CornerAttempts[1].Latest = false;
                                CornerAttempts.Insert(0, new CornerViewModel(corner, Mode, new Dictionary<ComparisonMode, Corner> {
                                    [ComparisonMode.CompareToBestCarLap] = BestCarCornerSpeeds[TrackId][corner.Index].Copy(),
                                    [ComparisonMode.CompareToBestSessionCarLap] = BestSessionCornerSpeeds[corner.Index].Copy(),
                                    [ComparisonMode.CompareToComparisonLap] = ComparisonLapCorners?[corner.Index].Copy() ?? null
                                }));
                                // show last corner to last lap's same corner
                                while (CornerAttempts.Count > (TurnNumbers.turns.Count + 1))
                                {
                                    CornerAttempts.RemoveAt(CornerAttempts.Count - 1);
                                }
                            });
                        }
                    }
                    
                    for (int i = 0; i < CurrentCornerSpeeds.Count; i++)
                        //foreach (var corner in CurrentCornerSpeeds.Where(o => o.Active))
                    {
                        var corner = CurrentCornerSpeeds[i];
                        if (!corner.Active)
                            continue;
                        if (!timeValid) {
                            corner.Valid = false;
                            corner.End();
                            continue;
                        }
                        if ((corner.end > corner.start && data.NewData.TrackPositionMeters >= corner.end) ||
                            (corner.end < corner.start && data.NewData.TrackPositionMeters < corner.start && data.NewData.TrackPositionMeters >= corner.end))
                        {
                            if ((corner.end > corner.start && data.OldData.TrackPositionMeters >= corner.end) ||
                            (corner.end < corner.start && data.OldData.TrackPositionMeters < corner.start && data.OldData.TrackPositionMeters >= corner.end))
                                corner.Valid = false;
                            if (!timeValid)
                                corner.Valid = false;
                            corner.End();

                            //pluginManager.UIUpdateDispatcher.Dispatcher.Invoke(() => { 
                            //    CornerAttempts.Insert(0, new CornerViewModel(corner.Copy(), new Dictionary<ComparisonMode, Corner> {
                            //        [ComparisonMode.CompareToBestCarLap] = BestCarCornerSpeeds[TrackId][corner.Index].Copy(),
                            //        [ComparisonMode.CompareToBestSessionCarLap] = BestSessionCornerSpeeds[corner.Index].Copy(),
                            //        [ComparisonMode.CompareToComparisonLap] = ComparisonLapCorners?[corner.Index].Copy() ?? null
                            //    }));
                            //    // show last corner to last lap's same corner
                            //    while (CornerAttempts.Count > (TurnNumbers.turns.Count + 1))
                            //    {
                            //        CornerAttempts.RemoveAt(CornerAttempts.Count - 1);
                            //    }
                            //});
                            if (corner.Valid) {
                                if (!BestSessionCornerSpeeds[corner.Index].init || BestSessionCornerSpeeds[corner.Index].Duration > corner.Duration)
                                {
                                    BestSessionCornerSpeeds[corner.Index] = corner.Copy();
                                }
                                if (!BestCarCornerSpeeds[TrackId][corner.Index].init || BestCarCornerSpeeds[TrackId][corner.Index].Duration > corner.Duration) 
                                {
                                    BestCarCornerSpeeds[TrackId][corner.Index] = corner.Copy();
                                    pluginManager.UIUpdateDispatcher.Dispatcher.Invoke(() => { 
                                        this.SaveCommonSettings(CarId, BestCarCornerSpeeds);
                                    });
                                    this.TriggerEvent("NewBestCornerTime");
                                }
                            }
                        }
                    }
                    
                    for (int i = 0; i < CurrentCornerSpeeds.Count; i++)
                    //foreach (var corner in CurrentCornerSpeeds.Where(o => o.Active))
                    {
                        //pluginManager.UIUpdateDispatcher.Dispatcher.Invoke(() => { 
                        if (CurrentCornerSpeeds[i].Active)
                            CurrentCornerSpeeds[i].Update();
                        //});
                    }

                    /*

                    //if (CurrentCorner != -1)
                    for (int i = 0; i < TurnNumbers.turns.Count; i++)
                    {
                        if (data.NewData.TrackPositionMeters >= TurnNumbers.turns[i].start && data.NewData.TrackPositionMeters < TurnNumbers.turns[i].end)
                        {
                            newCorner = i;
                            break;
                        }
                    }
                    if (CurrentCorner != newCorner) {
                        if (CurrentCorner != -1) {
                            // end corner
                            CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].end_speed = data.NewData.SpeedLocal;
                            //CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].end_time = data.NewData.CurrentLapTime;
                            CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].end_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));

                            this.TriggerEvent("CornerEnd");

                            CornerDelta = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].Duration - BestSessionCornerSpeeds[CurrentCorner].Duration;
                            bool shortest = false;
                            if (BestSessionCornerSpeeds[CurrentCorner].Duration > CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].Duration)
                            {
                                BestSessionCornerSpeeds[CurrentCorner] = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner];
                            }
                            var comparisonCorner = CornersToCompare[CurrentCorner];
                            bool isNewEntry = comparisonCorner.end_time == TimeSpan.MaxValue;

                            if (BestCarCornerSpeeds[TrackId][CurrentCorner].Duration > CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].Duration)
                            {
                                BestCarCornerSpeeds[TrackId][CurrentCorner] = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner];
                                this.SaveCommonSettings(CarId, BestCarCornerSpeeds);
                                this.TriggerEvent("NewBestCornerTime");
                                shortest = true;
                            }
                            pluginManager.UIUpdateDispatcher.Dispatcher.Invoke(() => { 
                                CornerAttempts.Insert(0, new CornerViewModel
                                {
                                    TurnName = TurnNumbers.turns[CurrentCorner].name,
                                    StartSpeed = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].start_speed,
                                    MinSpeed = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].min_speed,
                                    EndSpeed = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].end_speed,
                                    StartSpeedDelta = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].start_speed - comparisonCorner.start_speed,
                                    MinSpeedDelta = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].min_speed - comparisonCorner.min_speed,
                                    EndSpeedDelta = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].end_speed - comparisonCorner.end_speed,
                                    Duration = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].Duration,
                                    DurationDelta = CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].Duration - comparisonCorner.Duration,
                                    WasShortestDuration = shortest,
                                    IsNewEntry = isNewEntry
                                });
                                // show last corner to last lap's same corner
                                while (CornerAttempts.Count > (TurnNumbers.turns.Count + 1))
                                {
                                    CornerAttempts.RemoveAt(CornerAttempts.Count - 1);
                                }
                            });
                        }
                        if (newCorner != -1) {
                            // start corner
                            CornerSpeeds[CurrentLap % NumberOfLaps][newCorner] = new Corner
                            {
                                start_speed = data.NewData.SpeedLocal,
                                //start_time = data.NewData.CurrentLapTime
                                start_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")))
                            };

                            this.TriggerEvent("CornerStart");
                        }
                        LastCorner = CurrentCorner;
                        CurrentCorner = newCorner;
                    }
                    // update corner min
                    if (CurrentCorner != -1 && data.NewData.SpeedLocal < CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].min_speed)
                    {
                        CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].min_speed = data.NewData.SpeedLocal;
                        //CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].min_time = data.NewData.CurrentLapTime;
                        CornerSpeeds[CurrentLap % NumberOfLaps][CurrentCorner].min_time = TimeSpan.FromSeconds(Convert.ToDouble(PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.SessionTime")));
                    }
                    */

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
                    
                    if (ComparisonLapSectors.Count > 0 && TurnNumbers.turns.Count > 0)
                    {
                        ComparisonLapCorners = GetCornersFromComparisonLapSectors();
                    }
                }
            }
        }

        public ObservableCollection<Corner> GetCornersFromComparisonLapSectors()
        {
            double total_sector_time_s = 0.0;
            double distance_m = 0.0;
            int current_turn_i = -1;
            var corners = new ObservableCollection<Corner>(TurnNumbers.turns.Select((o, i) => new Corner(o, i)));
            foreach (Sector sector in ComparisonLapSectors)
            {
                var total_time_s = total_sector_time_s;
                foreach (Marker marker in sector.Markers)
                {
                    int turn_i = -1;
                    for (int i = 0; i < TurnNumbers.turns.Count; i++)
                    {
                        if (distance_m >= TurnNumbers.turns[i].start && distance_m < TurnNumbers.turns[i].end)
                        {
                            turn_i = i;
                            break;
                        }
                    }
                    var current_speed = sector.distance_per_marker / (total_sector_time_s + marker.sector_time - total_time_s) * 2.237; // dumb mps to mph conversion
                    if (turn_i != current_turn_i)
                    {
                        if (current_turn_i != -1)
                        {
                            // end turn
                            corners[current_turn_i].end_speed = current_speed;
                            corners[current_turn_i].end_time = TimeSpan.FromSeconds(total_time_s);
                            corners[current_turn_i].init = true;
                        }
                        if (turn_i != -1)
                        {
                            // start turn
                            corners[turn_i].start_speed = current_speed;
                            corners[turn_i].start_time = TimeSpan.FromSeconds(total_time_s);
                        }
                        current_turn_i = turn_i;
                    }
                    if (current_turn_i != -1 && current_speed < corners[current_turn_i].min_speed) 
                    {
                        corners[current_turn_i].min_speed = current_speed;
                        corners[current_turn_i].min_time = TimeSpan.FromSeconds(total_time_s);
                    }
                    distance_m += sector.distance_per_marker;
                    total_time_s = total_sector_time_s + marker.sector_time;
                }
                total_sector_time_s += sector.time;
            }
            return corners;
        }

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

            //JackDashResource = Application.LoadComponent(new Uri("/User.CornerSpeed;component/ControlsTemplates.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
            
            //MessageBox.Show("Init", "Question", MessageBoxButton.YesNo);

            // Load settings
            Settings = this.ReadCommonSettings<CornerSpeedPluginSettings>("GeneralSettings", () => new CornerSpeedPluginSettings());
                        
            ComparisonLapSectors = new List<Sector>();
            //Settings.UpdateLapFiles();
                        
            LoadLapFile();

            CornerSpeeds = new List<List<Corner>>();
            BestSessionCornerSpeeds = new ObservableCollection<Corner>();
            SectorTimes = new List<List<TimeSpan>>();
            SectorTimesBest = new List<TimeSpan>();
            LapTimes = new List<TimeSpan>();
            LapNumbers = new List<int>();
            //CornerSpeeds = new List<List<double>>(NumberOfLaps);

            NumberOfCorners = CornerPositions.Length;
            
            this.AttachDelegate(name: "NumberOfCorners", valueProvider: () => NumberOfCorners);
            //this.AttachDelegate(name: "CurrentCorner", valueProvider: () => CurrentCorner);
            //this.AttachDelegate(name: "LastCorner", valueProvider: () => LastCorner);
            this.AttachDelegate(name: "CornerDelta", valueProvider: () => CornerDelta);
            this.AttachDelegate(name: "CornerAttempts", valueProvider: () => CornerAttempts);

            this.AttachDelegate(name: "TurnNumber", valueProvider: () => { 
                var pct = PluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LapDist");
                //var pct = PluginManager.GetPropertyValue("TrackPositionMeters");
                if (pct != null && TurnNumbers != null) { 
                    var dist = Convert.ToDouble(pct);
                    for (int i = 0; i < TurnNumbers.turns.Count; i++)
                    {
                        if (TurnNumbers.turns[i].start <= dist && dist < TurnNumbers.turns[i].end)
                        {
                            return TurnNumbers.turns[i].name;
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
                CornerSpeeds.Add(new List<Corner>());
                LapTimes.Add(TimeSpan.Zero);
                LapNumbers.Add(-1);
                var ilap_s = lap.ToString().PadLeft(2, '0');
                int ilap_i = lap;
                this.AttachDelegate(name: $"LapTimes_{ilap_s}", valueProvider: () => LapTimes[(CurrentLap + NumberOfLaps - ilap_i) % NumberOfLaps]);
                this.AttachDelegate(name: $"LapNumbers_{ilap_s}", valueProvider: () => LapNumbers[(CurrentLap + NumberOfLaps - ilap_i) % NumberOfLaps]);

                // turnNumbers.turns.Count
                for (int corner = 0; corner < 20; corner++) {
                    CornerSpeeds[lap].Add(new Corner());
                    var lap_s = lap.ToString().PadLeft(2, '0');
                    var corner_s = corner.ToString().PadLeft(2, '0');
                    int lap_i = lap;
                    int corner_i = corner;
                    this.AttachDelegate(name: $"CornerSpeed_{lap_s}_{corner_s}", valueProvider: () => CornerSpeeds[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][corner_i].min_speed);
                    this.AttachDelegate(name: $"CornerSpeedStart_{lap_s}_{corner_s}", valueProvider: () => CornerSpeeds[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][corner_i].start_speed);
                    this.AttachDelegate(name: $"CornerSpeedEnd_{lap_s}_{corner_s}", valueProvider: () => CornerSpeeds[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][corner_i].end_speed);
                    this.AttachDelegate(name: $"CornerSpeedRating_{lap_s}_{corner_s}", valueProvider: () => {
                        var min = 999.9;
                        var max = 0.0;
                        for (int lap_j = 0; lap_j < NumberOfLaps; lap_j++)
                        {
                            if (CornerSpeeds[lap_j][corner_i].min_speed == 0.0 || CornerSpeeds[lap_j][corner_i].min_speed == 999.9)
                                continue;
                            if (CornerSpeeds[lap_j][corner_i].min_speed < min)
                            {
                                min = CornerSpeeds[lap_j][corner_i].min_speed;
                            }
                            if (CornerSpeeds[lap_j][corner_i].min_speed > max)
                            {
                                max = CornerSpeeds[lap_j][corner_i].min_speed;
                            }
                        }
                        if (min != 999.9 && max != 0.0) { 
                            return (CornerSpeeds[(CurrentLap + NumberOfLaps - lap_i) % NumberOfLaps][corner_i].min_speed - min) / (max - min);
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
            this.AddEvent(eventName: "CornerStart");
            this.AddEvent(eventName: "CornerEnd");

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