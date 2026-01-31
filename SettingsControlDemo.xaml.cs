using SimHub.Plugins.Devices.DevicesExtensionsDummy;
using SimHub.Plugins.Styles;
using System.Windows;
using System.Windows.Controls;
using WoteverLocalization;

namespace User.CornerSpeed
{
    /// <summary>
    /// Logique d'interaction pour SettingsControlDemo.xaml
    /// </summary>
    public partial class SettingsControlDemo : UserControl
    {
        public CornerSpeedPlugin Plugin { get; }

        public SettingsControlDemo()
        {
            InitializeComponent();
        }
                
        public static readonly DependencyProperty CornerSpeedPluginSettingsProperty =
            DependencyProperty.Register("CornerSpeedPluginSettings", typeof(CornerSpeedPluginSettings), typeof(SettingsControlDemo), new PropertyMetadata(null));

        
        public CornerSpeedPluginSettings CornerSpeedPluginSettings
        {
            get { return (CornerSpeedPluginSettings)GetValue(CornerSpeedPluginSettingsProperty); }
            set { SetValue(CornerSpeedPluginSettingsProperty, value); }
        }

        public SettingsControlDemo(CornerSpeedPlugin plugin) : this()
        {
            this.Plugin = plugin;
            CornerSpeedPluginSettings = plugin.Settings;
        }

        private async void StyledMessageBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var res = await SHMessageBox.Show("Message box", SLoc.GetValue("MyPlugin_LocalizedDialogTitle"), System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question);

            await SHMessageBox.Show(res.ToString());
        }

        private void DemoWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = new DemoWindow();

            window.Show();
        }

        private async void DemodialogWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialogWindow = new DemoDialogWindow();

            var res = await dialogWindow.ShowDialogWindowAsync(this);

            await SHMessageBox.Show(res.ToString());
        }

        private void SHButtonPrimary_Click(object sender, RoutedEventArgs e)
        {
            Plugin.LoadLapFile();
        }

        private void SHButtonPrimary_Click2(object sender, RoutedEventArgs e)
        {
            Plugin.Settings.LapFile = "";
            Plugin.LoadLapFile();
        }

        private void SHButtonSecondary_Click(object sender, RoutedEventArgs e)
        {
            Plugin.Settings.LapFile = (string)((Button)sender).Content;
            Plugin.LoadLapFile();
        }
    }
}