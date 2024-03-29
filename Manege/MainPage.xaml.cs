﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://github.com/lzhengwei/UWP_Nordic_Uart_Transmitter
// https://lancaster-university.github.io/microbit-docs/ble/profile/
// https://lancaster-university.github.io/microbit-docs/resources/bluetooth/bluetooth_profile.html
// https://www.bluetooth.com/blog/bbc-microbit-inspiring-generation-get-creative-coding/
// https://www.bluetooth.com/blog/bluetooth-bbc-microbit/

namespace Manege
{

    public sealed partial class MainPage : Page
    {

        // Zone commune
        public static MainPage Current;

        // Zone Microbit

        public BluetoothLEDevice _BluetoothLEDevice { get; set; }
        public BluetoothLEDevice BluetoothLEDevice
        {

            get { return _BluetoothLEDevice; }
            set
            {

                _BluetoothLEDevice = value;
            }

        }

        String LocalSettingName;
        String LocalSettingAddress;
        String LocalSettingColor;

        // Generic Access
        private string SelectedServiceGenericAccessUUID = "00001800-0000-1000-8000-00805f9b34fb";
        private string SelectedCharacteristicDeviceNameUUID = "00002a00-0000-1000-8000-00805f9b34fb";
        private string SelectedCharacteristicAppearanceUUID = "00002a01-0000-1000-8000-00805f9b34fb";
        private string SelectedCharacteristicPeripheralUUID = "00002a04-0000-1000-8000-00805f9b34fb";

        // Generic Attribute
        private string SelectedServiceGenericAttributeUUID = "00001801-0000-1000-8000-00805f9b34fb";

        private GattDeviceService SelectedServiceGenericAccess;
        private GattCharacteristic SelectedCharacteristicDeviceName;
        private GattCharacteristic SelectedCharacteristicAppearance;
        private GattCharacteristic SelectedCharacteristicPeripheral;

        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df);

        public MainPage()
        {

            this.InitializeComponent();

            // Zone commune

            Current = this;

            ApplicationTitle.Text = APPLICATION_TITLE;

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
                    StatusBorder.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
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

        private async void ManageMicrobit()
        {

            try
            {

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                LocalSettingName = localSettings.Values["Name"] as string;
                LocalSettingAddress = localSettings.Values["Address"] as string;
                LocalSettingColor = localSettings.Values["Color"] as string;

                if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
                {

                    if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                    {

                        if (this.BluetoothLEDevice == null)
                        {

                            this.BluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(Convert.ToUInt64(LocalSettingAddress));

                            if (this.BluetoothLEDevice != null)
                            {

                                Debug.WriteLine(">>>>>>>>>> MainPage : BluetoothLEDevice not null");

                                IReadOnlyList<GattDeviceService> ListGattDeviceService = this.BluetoothLEDevice.GattServices;

                                Debug.WriteLine(">>>>>>>>>> ListGattDeviceService : " + ListGattDeviceService.Count);

                            }
                            else
                            {

                                NotifyUser("Impossible to find the bluetooth device from the micro:bit board known.", NotifyType.ErrorMessage);

                            }

                        }

                    }

                }

            }
            catch (Exception e)
            {

                Debug.WriteLine(">>>>>>>>>> Exception : " + e.Message);

            }

        }

    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

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

    public class ScenarioTitleBindingConverter : IValueConverter
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
