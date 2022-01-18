using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Monorail
{

    public sealed partial class Scenario1_PairingDevice : Page
    {


        BluetoothLEAdvertisementWatcher watcher;

        bool isWatcherStarted = false;

        private ObservableCollection<BluetoothLEDeviceDisplay> listBluetoothLEDeviceDisplay = new ObservableCollection<BluetoothLEDeviceDisplay>();

        BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay;
        BluetoothLEDevice bluetoothLEDevice;

        private MainPage rootPage;

        public Scenario1_PairingDevice()
        {

            this.InitializeComponent();

            rootPage = MainPage.Current;

            watcher = new BluetoothLEAdvertisementWatcher();
            watcher.ScanningMode = BluetoothLEScanningMode.Active;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            watcher.Received += Watcher_Received;
            watcher.Stopped += Watcher_Stopped;

            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

            rootPage.NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

            if (isWatcherStarted)
            {
                watcher.Stop();
                isWatcherStarted = false;
            }

            watcher.Received -= Watcher_Received;
            watcher.Stopped -= Watcher_Stopped;

            rootPage.NotifyUser("Navigating away. Watcher stopped.", NotifyType.StatusMessage);

        }

        protected void App_Suspending(object sender, object e)
        {

            if (isWatcherStarted)
            {
                watcher.Stop();
                isWatcherStarted = false;
            }

            watcher.Received -= Watcher_Received;
            watcher.Stopped -= Watcher_Stopped;

            rootPage.NotifyUser("App suspending. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {

            watcher.Received += Watcher_Received;
            watcher.Stopped += Watcher_Stopped;

            rootPage.NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {

            if (!isWatcherStarted)
            {

                watcher.Start();

                isWatcherStarted = true;

                listBluetoothLEDeviceDisplay.Clear();

                rootPage.NotifyUser("Running... Watcher started.", NotifyType.StatusMessage);

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

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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

                        if (bluetoothLEDevice.DeviceInformation.Pairing.IsPaired)
                        {

                            bluetoothLEDeviceDisplay.Paired = "Yes";

                        }
                        else
                        {
                            bluetoothLEDeviceDisplay.Paired = "No";
                        }

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

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                rootPage.NotifyUser(string.Format("Watcher stopped or aborted: {0}", args.Error.ToString()), NotifyType.StatusMessage);

            });

        }

        private async void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {

                bluetoothLEDeviceDisplay = (BluetoothLEDeviceDisplay)ResultsListView.SelectedItem;

                bluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(Convert.ToUInt64(bluetoothLEDeviceDisplay.Address));

                if (!bluetoothLEDevice.DeviceInformation.Pairing.IsPaired)
                {

                    if (bluetoothLEDevice.DeviceInformation.Pairing.CanPair)
                    {

                        DevicePairingResult devicePairingResult = await bluetoothLEDevice.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.None);

                    }

                }

                rootPage.BluetoothLEDevice = bluetoothLEDevice;

            }

        }

    }

    public class BluetoothLEDeviceDisplay : INotifyPropertyChanged
    {

        public string _Id { get; set; }
        public string Id
        {

            get { return _Id; }
            set
            {

                _Id = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Id"));
            }

        }

        public string _Address { get; set; }
        public string Address
        {

            get { return _Address; }
            set
            {

                _Address = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Address"));
            }

        }

        public string _Name { get; set; }
        public string Name
        {

            get { return _Name; }
            set
            {

                _Name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }

        }

        public string _Strength { get; set; }
        public string Strength
        {

            get { return _Strength; }
            set
            {

                _Strength = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Strength"));
            }

        }

        public string _Paired { get; set; }
        public string Paired
        {

            get { return _Paired; }
            set
            {

                _Paired = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Paired"));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

    }

}
