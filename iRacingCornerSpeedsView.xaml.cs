using FMOD;
using GongSolutions.Wpf.DragDrop.Utilities;
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
        
        //public CornerSpeedPlugin Plugin { get; set; }
        public CornerSpeedPluginSettings Settings { get; set; }

        public iRacingCornerSpeedsView()
        {
            InitializeComponent();
            //Plugin = PluginManager.GetInstance().GetPlugin<CornerSpeedPlugin>();
            Settings = PluginManager.GetInstance().GetPlugin<CornerSpeedPlugin>().Settings;
        }

        private void Mode_Prev(object sender, RoutedEventArgs e)
        {
            Settings.Mode = (ComparisonMode)(((int)Settings.Mode + (int)ComparisonMode.Count - 1) % (int)ComparisonMode.Count);
        }

        private void Mode_Next(object sender, RoutedEventArgs e)
        {
            Settings.Mode = (ComparisonMode)(((int)Settings.Mode + 1) % (int)ComparisonMode.Count);
        }

        //private bool isMoving = false;

        private void iRacingBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //isMoving = true;
            if (e.ChangedButton == MouseButton.Left)
                this.GetVisualAncestor<Window>().DragMove();
        }

        private void iRacingBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //isMoving = false;
        }

        private void iRacingBorder_MouseMove(object sender, MouseEventArgs e)
        {
            //ViewModel.Owner.BaseWidth += e.
        }
    }
}
