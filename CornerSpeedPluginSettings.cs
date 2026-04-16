using GameReaderCommon;
using Newtonsoft.Json;
using PropertyChanged;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Documents;

namespace User.CornerSpeed
{
    /// <summary>
    /// Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    //[AddINotifyPropertyChangedInterface]
    public class CornerSpeedPluginSettings : INotifyPropertyChanged
    {
        public string LapFile { get; set; }

        public ObservableCollection<string> AvailableFiles { get; set; }
                
        public ComparisonMode Mode { get; set; } = ComparisonMode.CompareToBestCarLap;

        private bool[] wholeLapDelta = new bool[] { true, false };

        public event PropertyChangedEventHandler PropertyChanged;

        public DynamicButtonAction CustomButton1Action { get; set; } = new DynamicButtonAction("CustomButton1");

        [JsonIgnore]
        [AlsoNotifyFor("LiveDelta")]
        public bool[] WholeLapDeltaArray
        {
            get { return wholeLapDelta; }
        }
        
        [JsonIgnore]
        public int WholeLapDelta
        {
            get { return Array.IndexOf(wholeLapDelta, true); }
        }

        public string GetLapFilesFolder(string trackId)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, "iRacing", "lapfiles", trackId);
        }

        public void UpdateAvailableLapFiles(string trackId)
        {
            AvailableFiles = new ObservableCollection<string>(GetLapFiles(trackId));
        }

        private List<string> GetLapFiles(string trackId)
        {
            var files = new List<string>();
            var folder = GetLapFilesFolder(trackId);
            if (Directory.Exists(folder))
            {
                files = GetFiles(folder);
            }
            return files;
        }

        private List<string> GetFiles(string folder)
        {
            var o = new List<string>();
            string[] files = Directory.GetFiles(folder);
            foreach(string file in files)
            {
                if (Directory.Exists(file)) { 
                    o.AddRange(GetFiles(file));
                } else if (File.Exists(file)) {
                    o.Add(file);
                }
            }
            return o;
        }
    }
}