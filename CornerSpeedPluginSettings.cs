using System.Collections.Generic;
using System.Windows.Documents;

namespace User.CornerSpeed
{
    /// <summary>
    /// Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    public class CornerSpeedPluginSettings
    {
        public int SpeedWarningLevel = 100;
    }
}