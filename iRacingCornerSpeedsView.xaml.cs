using FMOD;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace User.CornerSpeed
{
    /// <summary>
    /// Interaction logic for iRacingCornerSpeeds.xaml
    /// </summary>
    public partial class iRacingCornerSpeedsView : UserControl
    {
        public iRacingCornerSpeeds ViewModel => DataContext as iRacingCornerSpeeds;

        public CornerSpeedPlugin Plugin { get; set; }

        public iRacingCornerSpeedsView()
        {
            InitializeComponent();
            Plugin = PluginManager.GetInstance().GetPlugin<CornerSpeedPlugin>();
        }

        private void Mode_Prev(object sender, RoutedEventArgs e)
        {
            Plugin.Mode = (ComparisonMode)(((int)Plugin.Mode + (int)ComparisonMode.Count - 1) % (int)ComparisonMode.Count);
        }

        private void Mode_Next(object sender, RoutedEventArgs e)
        {
            Plugin.Mode = (ComparisonMode)(((int)Plugin.Mode + 1) % (int)ComparisonMode.Count);
        }
    }
}
