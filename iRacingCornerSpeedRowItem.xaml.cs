using log4net.Plugin;
using PostSharp.Aspects.Advices;
using PropertyChanged;
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
    /// Interaction logic for iRacingCornerSpeedRowItem.xaml
    /// </summary>
    public partial class iRacingCornerSpeedRowItem : UserControl
    {
        public CornerSpeedPlugin Plugin { get; set; }

        public iRacingCornerSpeedRowItem()
        {
            InitializeComponent();
            Plugin = PluginManager.GetInstance().GetPlugin<CornerSpeedPlugin>();
        }

        public CornerViewModel ViewModel => DataContext as CornerViewModel;

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            switch (Plugin.Settings.Mode)
            {
                case ComparisonMode.CompareToBestCarLap:
                    Plugin.BestCarCornerSpeeds[Plugin.TrackId][ViewModel.TurnIndex] = ViewModel.ComparisonCorner;
                    break;
                case ComparisonMode.CompareToBestSessionCarLap:
                    Plugin.BestSessionCornerSpeeds[ViewModel.TurnIndex] = ViewModel.ComparisonCorner;
                    break;
                default:
                    break;
            }
        }
    }
}
