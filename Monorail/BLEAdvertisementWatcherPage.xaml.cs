using System;
using System.Collections.Generic;
using System.Diagnostics;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using System.ComponentModel;
using Windows.UI.Xaml.Data;
using System.Collections.ObjectModel;

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

        private ObservableCollection<BluetoothDevice> listBluetoothDevice = new ObservableCollection<BluetoothDevice>();

        BluetoothDevice bluetoothDevice;

        int resultsListViewSelectedIndex = -1;

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

            watcher.Stop();

            watcher.Received -= Watcher_Received;
            watcher.Stopped -= Watcher_Stopped;

            NotifyUser("Navigating away. Watcher stopped.", NotifyType.StatusMessage);

            base.OnNavigatingFrom(e);
        }

        private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

            watcher.Stop();

            watcher.Received -= Watcher_Received;
            watcher.Stopped -= Watcher_Stopped;

            NotifyUser("App suspending. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {

            watcher.Received += Watcher_Received;
            watcher.Stopped += Watcher_Stopped;

        }


        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {

            if( !isWatcherStarted )
            {

                isWatcherStarted = true;

                listBluetoothDevice.Clear();

                watcher.ScanningMode = BluetoothLEScanningMode.Active;

                watcher.Start();

                NotifyUser("Running... Watcher started.", NotifyType.StatusMessage);

            }

        }

        private bool FindBluetoothDevice(string id)
        {

            bool _isPresent = false;

            foreach (BluetoothDevice bluetoothDevice in listBluetoothDevice)
            {

                if (bluetoothDevice.Id == id)
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

                    BluetoothLEDevice bluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync( args.BluetoothAddress );

                    bluetoothDevice = new BluetoothDevice();

                    bluetoothDevice.Id = bluetoothLEDevice.DeviceId;
                    bluetoothDevice.Address = args.BluetoothAddress + "";
                    bluetoothDevice.Name = args.Advertisement.LocalName;
                    bluetoothDevice.Strength = args.RawSignalStrengthInDBm + "";

                    if( !FindBluetoothDevice( bluetoothDevice.Id ) )
                    {

                        listBluetoothDevice.Add(bluetoothDevice);
                    }

                }

            });

        }

        private async void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {

            // Notify the user that the watcher was stopped
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

                NotifyUser(string.Format("Watcher stopped or aborted: {0}", args.Error.ToString()), NotifyType.StatusMessage);

            });

        }

        private void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            resultsListViewSelectedIndex = ResultsListView.SelectedIndex;

        }

        private async void ResultsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            bluetoothDevice = (BluetoothDevice) e.ClickedItem;

            BluetoothLEDevice bluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(Convert.ToUInt64(bluetoothDevice.Address));
            
            // BluetoothLEDevice bluetoothLEDevice = await BluetoothLEDevice.FromIdAsync(bluetoothDevice.Address));


            DevicePairingResult dpr = await bluetoothLEDevice.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.None);
            GattDeviceService service = await GattDeviceService.FromIdAsync(bluetoothLEDevice.DeviceInformation.Id);

            if (resultsListViewSelectedIndex == ResultsListView.SelectedIndex && ResultsListView.SelectedIndex != -1)
            {

                rootPage.Navigate(typeof(MicrobitPage));

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

    public class BluetoothDevice : INotifyPropertyChanged
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
