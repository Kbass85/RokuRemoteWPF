using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RokuTools;

namespace RokuRemoteWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            RefreshDeviceList(null,null);
        }

        public static List<RokuTools.RokuTools.RokuDevice> rokuDevices { get; set; }

        public string ipAddress;
        public string manualIp { get; set; } = "Enter IP Address...";
        public string manualCommand { get; set; } = "Enter Command...";

        private void KeyPress(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            RokuTools.RokuTools.SendKey(ipAddress, btn.Name);
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            RokuTools.RokuTools.SendLaunch(ipAddress, btn.Uid);
        }

        private void ComboSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            RokuTools.RokuTools.RokuDevice device = combo.SelectedItem as RokuTools.RokuTools.RokuDevice; 
            ipAddress = device.IpAddress;  
        }

        private async void RefreshDeviceList(object sender, RoutedEventArgs e)
        {
            rokuDevices = await Task.Run(() => RokuTools.RokuTools.Disco2().ToList());
                            //Update ComboBox
            comboOne.ItemsSource = rokuDevices;
        }

        private void SendManualCommand(object sender, RoutedEventArgs e)
        {
            RokuTools.RokuTools.ManualEntry(manualIp, manualCommand);
        }
    }
}
