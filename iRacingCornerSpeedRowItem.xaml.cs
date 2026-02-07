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
            //ViewModel = DataContext;
        }

        public CornerViewModel ViewModel => DataContext as CornerViewModel;

        //public CornerViewModel ViewModel
        //{
        //    get { return (CornerViewModel)GetValue(ViewModelProperty); }
        //    set { SetValue(ViewModelProperty, value); }
        //}

        //public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CornerViewModel), typeof(iRacingCornerSpeedRowItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnViewModelChanged)));

        //private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ////throw new NotImplementedException();
        //    //if (e.OldValue is CornerViewModel cvm2)
        //    //{
        //    //    ;
        //    //}
        //    //var cvm = (CornerViewModel)e.NewValue;
        //    //cvm.PropertyChanged += (sender, args) =>
        //    //{
        //    //    var self = (iRacingCornerSpeedRowItem)d;
        //    //    self.Dispatcher.Invoke(() =>
        //    //    {
        //    //        var formatter = new TimeSpanFormatter();
        //    //        self.Duration.Text = (string)formatter.Convert(cvm.Duration, typeof(string), null, null);
        //    //    });
        //    //};
        //}

        //private static void Cvm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (sender is CornerViewModel corner)
        //    {
        //        corner.
        //    }
        //}

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            switch (Plugin.Mode)
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
