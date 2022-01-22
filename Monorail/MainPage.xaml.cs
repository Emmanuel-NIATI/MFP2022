using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.UI.Core;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Monorail
{

    public sealed partial class MainPage : Page
    {

        // Zone commune
        public static MainPage Current;

        public const string FEATURE_NAME = "Gestion de la carte micro:bit";

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }

        List<Scenario> scenarios = new List<Scenario>
        {

            new Scenario() { Logo="\xE702", Title="Pairing the device", ClassType=typeof(Scenario1_PairingDevice) },
            new Scenario() { Logo="\xE702", Title="Managing micro:bit board", ClassType=typeof(Scenario2_ManagingMicrobit) }

            //new Scenario() { Logo="\xE787", Title="Gestion de la carte micro:bit", ClassType=typeof(Scenario3_Microbit) },
            //new Scenario() { Logo="\xE702", Title="Nearby BLE Advertisement", ClassType=typeof(Scenario1_Advertisement) },
            //new Scenario() { Logo="\xE702", Title="BLE Paired Device", ClassType=typeof(Scenario2_Device) },
            //new Scenario() { Logo="\xE787", Title="Gestion de la carte micro:bit", ClassType=typeof(Scenario3_Microbit) },
            //new Scenario() { Logo="\xE787", Title="Gestion de la carte micro:bit UART", ClassType=typeof(Scenario4_Microbit) }

        };

        // Zone Microbit

        String LocalSettingName;
        String LocalSettingAddress;
        String LocalSettingColor;

        public BluetoothLEDevice _BluetoothLEDevice { get; set; }
        public BluetoothLEDevice BluetoothLEDevice
        {

            get { return _BluetoothLEDevice; }
            set
            {

                _BluetoothLEDevice = value;
            }

        }
               
        public MainPage()
        {

            this.InitializeComponent();

            // Zone commune

            Current = this;

            App_Title.Text = FEATURE_NAME;

            // Zone Microbit
            this.ManageMicrobit();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            ScenarioControl.ItemsSource = scenarios;

            ScenarioControl.SelectedIndex = 0;

            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

        }

        protected async void App_Suspending(object sender, object e)
        {

            if (_BluetoothLEDevice != null)
            {

                _BluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(_BluetoothLEDevice.BluetoothAddress);

                if (_BluetoothLEDevice.DeviceInformation.Pairing.IsPaired)
                {

                    DeviceUnpairingResult deviceUnpairingResult = await _BluetoothLEDevice.DeviceInformation.Pairing.UnpairAsync();

                }

            }

            NotifyUser("App suspending.", NotifyType.StatusMessage);

        }

        protected void App_Resuming(object sender, object e)
        {

            NotifyUser("App resuming.", NotifyType.StatusMessage);

        }

        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            NotifyUser(String.Empty, NotifyType.StatusMessage);

            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;

            if (s != null)
            {

                ScenarioFrame.Navigate(s.ClassType);

                if (Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }

            }

        }

        public void Navigate(Type sourcePageType)
        {

            ScenarioFrame.Navigate(sourcePageType);

        }

        public void NotifyUser(string strMessage, NotifyType type)
        {

            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }

        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }

            StatusBlock.Text = strMessage;

            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;

            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }

            var peer = FrameworkElementAutomationPeer.FromElement(StatusBlock);

            if (peer != null)
            {
                peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            }

        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        // Zone Microbit
        public async void ManageMicrobit()
        {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            LocalSettingName = localSettings.Values["Name"] as string;
            LocalSettingAddress = localSettings.Values["Address"] as string;
            LocalSettingColor = localSettings.Values["Color"] as string;

            if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
            {

                if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                {

                    if( this.BluetoothLEDevice == null )
                    {

                        Debug.WriteLine(">>>>>>>>>>>>>>>>>>>> MainPage : BluetoothLEDevice == null au démarrage");

                        this.BluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync( Convert.ToUInt64(LocalSettingAddress) );

                        if (this.BluetoothLEDevice != null)
                        {

                            Debug.WriteLine(">>>>>>>>>>>>>>>>>>>> MainPage : BluetoothLEDevice non null après initialisation");

                        }
                        else
                        {

                            Debug.WriteLine(">>>>>>>>>>>>>>>>>>>> MainPage : BluetoothLEDevice toujours null après initialisation");

                        }

                    }

                }

            }

        }

    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public class Scenario
    {
        public string Logo { get; set; }
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

    public class ScenarioLogoBindingConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;

            return s.Logo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }

    }

    public class ScenarioBindingConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return (MainPage.Current.Scenarios.IndexOf(s) + 1) + ") " + s.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }

    }

}
