using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

using System.ComponentModel;

using System.Collections.ObjectModel;

using Windows.Storage.Streams;
using Windows.Security.Cryptography;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Monorail
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BLEAdvertisementWatcherPage : Page
    {

        BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();

        bool isWatcherStarted = false;

        private ObservableCollection<BluetoothLEDeviceDisplay> listBluetoothLEDeviceDisplay = new ObservableCollection<BluetoothLEDeviceDisplay>();

        BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay;
        BluetoothLEDevice bluetoothLEDevice;
        BluetoothDevice bluetoothDevice;

        private MainPage rootPage;

        public BLEAdvertisementWatcherPage()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            rootPage = MainPage.Current;

            watcher.Received += Watcher_Received;
            watcher.Stopped += Watcher_Stopped;

            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

            NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {

            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

            if (isWatcherStarted) { watcher.Stop(); }

            watcher.Received -= Watcher_Received;
            watcher.Stopped -= Watcher_Stopped;

            NotifyUser("Navigating away. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Suspending(object sender, object e)
        {

            if (isWatcherStarted) { watcher.Stop(); }

            watcher.Received -= Watcher_Received;
            watcher.Stopped -= Watcher_Stopped;
            
            NotifyUser("App suspending. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {

            watcher.Received += Watcher_Received;
            watcher.Stopped += Watcher_Stopped;

            NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {

            if( !isWatcherStarted )
            {

                isWatcherStarted = true;

                listBluetoothLEDeviceDisplay.Clear();

                watcher.ScanningMode = BluetoothLEScanningMode.Active;

                watcher.Start();

                NotifyUser("Running... Watcher started.", NotifyType.StatusMessage);

            }

        }

        private bool FindBluetoothDevice(string id)
        {

            bool _isPresent = false;

            foreach (BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay in listBluetoothLEDeviceDisplay)
            {

                if (bluetoothLEDeviceDisplay.Id == id)
                {
                    _isPresent = true;
                }

            }

            return _isPresent;

        }

        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                if (args.Advertisement.LocalName != "")
                {

                    try
                    {

                        BluetoothLEDevice bluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);

                        bluetoothLEDeviceDisplay = new BluetoothLEDeviceDisplay();

                        bluetoothLEDeviceDisplay.Id = bluetoothLEDevice.DeviceId;

                        bluetoothLEDeviceDisplay.Address = args.BluetoothAddress + "";
                        bluetoothLEDeviceDisplay.Name = args.Advertisement.LocalName;
                        bluetoothLEDeviceDisplay.Strength = args.RawSignalStrengthInDBm + "";

                        if (!FindBluetoothDevice(bluetoothLEDeviceDisplay.Id))
                        {

                            listBluetoothLEDeviceDisplay.Add(bluetoothLEDeviceDisplay);
                        }

                    }
                    catch (Exception e)
                    {

                        Debug.WriteLine("Exception : " + e.Message);

                    }

                }

            });

        }

        private async void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

                NotifyUser(string.Format("Watcher stopped or aborted: {0}", args.Error.ToString()), NotifyType.StatusMessage);

            });

        }

        private async void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {

                bluetoothLEDeviceDisplay = (BluetoothLEDeviceDisplay) ResultsListView.SelectedItem;

                bluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(Convert.ToUInt64(bluetoothLEDeviceDisplay.Address));

                if (bluetoothLEDevice.ConnectionStatus.Equals(BluetoothConnectionStatus.Disconnected))
                {

                    DevicePairingResult devicePairingResult = await bluetoothLEDevice.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.None);

                    rootPage.BluetoothLEDevice = bluetoothLEDevice;

                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> bluetoothLEDevice :");
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> bluetoothLEDevice : " + bluetoothLEDevice.BluetoothAddress);
                    Debug.WriteLine("");
                }

            }

        }

        public void NotifyUser(string strMessage, NotifyType type)
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
        }

        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };


    }


}
