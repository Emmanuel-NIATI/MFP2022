﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Microbit
{

    public sealed partial class MainPage : Page
    {

        public static MainPage Current;

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

            Current = this;
            App_Title.Text = FEATURE_NAME;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            ScenarioControl.ItemsSource = scenarios;

            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }

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

            if ( _BluetoothLEDevice != null )
            {

                _BluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync( _BluetoothLEDevice.BluetoothAddress );

                if (_BluetoothLEDevice.DeviceInformation.Pairing.IsPaired )
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

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
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
