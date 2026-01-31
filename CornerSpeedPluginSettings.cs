using System.Collections.Generic;
using System.Windows.Documents;
using PropertyChanged;
using SimHub.Plugins;

namespace User.CornerSpeed
{
    /// <summary>
    /// Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class CornerSpeedPluginSettings
    {
        public string LapFile { get; set; } = "C:\\Users\\Jack\\Documents\\iRacing\\lapfiles\\spa 2024 up\\Kyle Hayne - 02.16.603 - BLAP - Porsche 911 GT3 R (992) - Spa-Francorchamps (GP Pits) (Garage 61 - 01KG7PBB6CXS2530FHWJ6KVP3D).blap";
    }
}