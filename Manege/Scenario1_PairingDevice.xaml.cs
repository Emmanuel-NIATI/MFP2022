using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Manege
{

    public sealed partial class Scenario1_PairingDevice : Page
    {

        // Zone commune
        private MainPage rootPage;

        // Zone Microbit
        String LocalSettingName;
        String LocalSettingAddress;
        String LocalSettingColor;

        // Zone Advertisement
        BluetoothLEAdvertisementWatcher bluetoothLEAdvertisementWatcher;
        bool isBluetoothLEAdvertisementWatcherStarted = false;
        private ObservableCollection<BluetoothLEDeviceDisplay> listBluetoothLEDeviceDisplay = new ObservableCollection<BluetoothLEDeviceDisplay>();
        BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay;

        // Zone Device
        private DeviceWatcher deviceWatcher;
        bool isDeviceWatcherStarted = false;
        private ObservableCollection<DeviceInformationDisplay> listDeviceInformationDisplay = new ObservableCollection<DeviceInformationDisplay>();
        DeviceInformationDisplay deviceInformationDisplay;

        public Scenario1_PairingDevice()
        {

            this.InitializeComponent();

            // Zone commune
            this.rootPage = MainPage.Current;

            // Zone Microbit
            this.ManageMicrobit();

            // Zone Advertisement
            bluetoothLEAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();
            bluetoothLEAdvertisementWatcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Zone Device
            deviceWatcher = DeviceInformation.CreateWatcher();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Zone commune
            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

            // Zone Microbit

            // Zone Advertisement
            bluetoothLEAdvertisementWatcher.Received += BluetoothLEAdvertisementWatcher_Received;
            bluetoothLEAdvertisementWatcher.Stopped += BluetoothLEAdvertisementWatcher_Stopped;

            // Zone Device
            deviceWatcher.Added += DeviceWatcher_DeviceAdded;
            deviceWatcher.Updated += DeviceWatcher_DeviceUpdated;
            deviceWatcher.Removed += DeviceWatcher_DeviceRemoved;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Zone notification
            rootPage.NotifyUser("Appairage de la carte micro:bit.", NotifyType.StatusMessage);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            // Zone commune
            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

            // Zone Microbit

            // Zone Advertisement

            if (isBluetoothLEAdvertisementWatcherStarted)
            {
                bluetoothLEAdvertisementWatcher.Stop();
                isBluetoothLEAdvertisementWatcherStarted = false;
            }

            bluetoothLEAdvertisementWatcher.Received -= BluetoothLEAdvertisementWatcher_Received;
            bluetoothLEAdvertisementWatcher.Stopped -= BluetoothLEAdvertisementWatcher_Stopped;

            // Zone Device

            if (isDeviceWatcherStarted)
            {
                deviceWatcher.Stop();
                isDeviceWatcherStarted = false;
            }

            deviceWatcher.Added -= DeviceWatcher_DeviceAdded;
            deviceWatcher.Updated -= DeviceWatcher_DeviceUpdated;
            deviceWatcher.Removed -= DeviceWatcher_DeviceRemoved;
            deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped -= DeviceWatcher_Stopped;

            // Zone notification
            rootPage.NotifyUser("A bientôt !", NotifyType.StatusMessage);

        }

        protected void App_Suspending(object sender, object e)
        {

            // Zone Advertisement

            if (isBluetoothLEAdvertisementWatcherStarted)
            {
                bluetoothLEAdvertisementWatcher.Stop();
                isBluetoothLEAdvertisementWatcherStarted = false;
            }

            bluetoothLEAdvertisementWatcher.Received -= BluetoothLEAdvertisementWatcher_Received;
            bluetoothLEAdvertisementWatcher.Stopped -= BluetoothLEAdvertisementWatcher_Stopped;

            // Zone Microbit

            // Zone Device

            if (isDeviceWatcherStarted)
            {
                deviceWatcher.Stop();
                isDeviceWatcherStarted = false;
            }

            deviceWatcher.Added -= DeviceWatcher_DeviceAdded;
            deviceWatcher.Updated -= DeviceWatcher_DeviceUpdated;
            deviceWatcher.Removed -= DeviceWatcher_DeviceRemoved;
            deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped -= DeviceWatcher_Stopped;

            // Zone notification
            rootPage.NotifyUser("A bientôt !", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {

            // Zone Advertisement

            bluetoothLEAdvertisementWatcher.Received += BluetoothLEAdvertisementWatcher_Received;
            bluetoothLEAdvertisementWatcher.Stopped += BluetoothLEAdvertisementWatcher_Stopped;

            // Zone Microbit

            // Zone Device

            deviceWatcher.Added += DeviceWatcher_DeviceAdded;
            deviceWatcher.Updated += DeviceWatcher_DeviceUpdated;
            deviceWatcher.Removed += DeviceWatcher_DeviceRemoved;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Zone notification
            rootPage.NotifyUser("Appairage de la carte micro:bit.", NotifyType.StatusMessage);

        }


        // Zone Microbit

        private async void ManageMicrobit()
        {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            LocalSettingName = localSettings.Values["Name"] as string;
            LocalSettingAddress = localSettings.Values["Address"] as string;
            LocalSettingColor = localSettings.Values["Color"] as string;

            if (LocalSettingName == null)
            {
                MicrobitName.Text = "";
            }

            if (LocalSettingAddress == null)
            {
                MicrobitAddress.Text = "";
            }

            if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
            {

                if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                {

                    MicrobitName.Text = LocalSettingName;

                    MicrobitAddress.Text = LocalSettingAddress;

                    if (LocalSettingColor.Equals("bleu")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_bleu.png")); }
                    if (LocalSettingColor.Equals("jaune")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_jaune.png")); }
                    if (LocalSettingColor.Equals("rouge")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_rouge.png")); }
                    if (LocalSettingColor.Equals("vert")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_vert.png")); }

                    if (this.rootPage.BluetoothLEDevice != null)
                    {

                        BluetoothConnectionStatus bluetoothConnectionStatus = this.rootPage.BluetoothLEDevice.ConnectionStatus;

                        if (bluetoothConnectionStatus.Equals(BluetoothConnectionStatus.Connected))
                        {

                            MicrobitDeviceConnected.Text = "Connected";

                        }
                        else if (bluetoothConnectionStatus.Equals(BluetoothConnectionStatus.Disconnected))
                        {

                            MicrobitDeviceConnected.Text = "Disconnected";
                        }

                    }

                }

            }
            else
            {

                localSettings.Values["Name"] = null;
                localSettings.Values["Address"] = null;
                localSettings.Values["Color"] = null;

            }

        }

        private void ButtonRed_Click(object sender, RoutedEventArgs e)
        {

            if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
            {

                if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                {

                    ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_rouge.png"));
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["Color"] = "rouge";

                }

            }

        }

        private void ButtonBlue_Click(object sender, RoutedEventArgs e)
        {

            if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
            {

                if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                {

                    ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_bleu.png"));

                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["Color"] = "bleu";

                }

            }

        }

        private void ButtonYellow_Click(object sender, RoutedEventArgs e)
        {

            if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
            {

                if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                {

                    ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_jaune.png"));

                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["Color"] = "jaune";

                }

            }

        }

        private void ButtonGreen_Click(object sender, RoutedEventArgs e)
        {

            if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
            {

                if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                {

                    ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_vert.png"));

                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["Color"] = "vert";

                }

            }

        }

        // Zone Advertisement

        private void EnumerateButtonAdvertisement_Click(object sender, RoutedEventArgs e)
        {

            if (!isBluetoothLEAdvertisementWatcherStarted)
            {

                bluetoothLEAdvertisementWatcher.Start();

                isBluetoothLEAdvertisementWatcherStarted = true;

                listBluetoothLEDeviceDisplay.Clear();

                rootPage.NotifyUser("Running... Advertisement Watcher started.", NotifyType.StatusMessage);

            }

        }

        private bool FindBluetoothLEDeviceDisplay(string id)
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

        private async void BluetoothLEAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {

                if (sender == bluetoothLEAdvertisementWatcher)
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

                                bluetoothLEDeviceDisplay.IsPaired = "Yes";

                            }
                            else
                            {

                                bluetoothLEDeviceDisplay.IsPaired = "No";

                            }

                            if (!FindBluetoothLEDeviceDisplay(bluetoothLEDeviceDisplay.Id))
                            {

                                if (LocalSettingName != null && !LocalSettingName.Equals(""))
                                {

                                    if (LocalSettingName.Equals(bluetoothLEDeviceDisplay.Name))
                                    {

                                        listBluetoothLEDeviceDisplay.Add(bluetoothLEDeviceDisplay);
                                    }

                                }
                                else
                                {
                                    listBluetoothLEDeviceDisplay.Add(bluetoothLEDeviceDisplay);
                                }

                                rootPage.NotifyUser("Advertisement Watcher received.", NotifyType.StatusMessage);

                            }

                        }
                        catch (Exception e)
                        {

                            Debug.WriteLine("Exception : " + e.Message);

                        }

                    }

                }

            });

        }

        private async void BluetoothLEAdvertisementWatcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (sender == bluetoothLEAdvertisementWatcher)
                {

                    rootPage.NotifyUser(string.Format("Advertisement Watcher stopped or aborted: {0}", args.Error.ToString()), NotifyType.StatusMessage);

                }

            });

        }

        private async void ResultsListViewAdvertisement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {

                bluetoothLEDeviceDisplay = (BluetoothLEDeviceDisplay)ResultsListViewAdvertisement.SelectedItem;

                BluetoothLEDevice bluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(Convert.ToUInt64(bluetoothLEDeviceDisplay.Address));

                if (!bluetoothLEDevice.DeviceInformation.Pairing.IsPaired)
                {

                    if (bluetoothLEDevice.DeviceInformation.Pairing.CanPair)
                    {

                        DevicePairingResult devicePairingResult = await bluetoothLEDevice.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.None);

                        if (devicePairingResult.Status.Equals(DevicePairingResultStatus.Paired))
                        {

                            // Zone commune
                            this.rootPage.BluetoothLEDevice = bluetoothLEDevice;
                            
                            // Zone Microbit

                            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                            localSettings.Values["Name"] = bluetoothLEDevice.Name;
                            localSettings.Values["Address"] = bluetoothLEDeviceDisplay.Address;
                            localSettings.Values["Color"] = "rouge";

                            this.ManageMicrobit();

                            // Zone Advertisement
                            if (isBluetoothLEAdvertisementWatcherStarted)
                            {

                                bluetoothLEAdvertisementWatcher.Stop();
                                listBluetoothLEDeviceDisplay.Clear();
                                ResultsListViewAdvertisement.ItemsSource = listBluetoothLEDeviceDisplay;
                                isBluetoothLEAdvertisementWatcherStarted = false;

                            }

                            // Zone Device
                            if (isDeviceWatcherStarted)
                            {

                                deviceWatcher.Stop();
                                listDeviceInformationDisplay.Clear();
                                ResultsListViewDevice.ItemsSource = listDeviceInformationDisplay;
                                isDeviceWatcherStarted = false;

                            }

                            // Zone notification
                            rootPage.NotifyUser("Device paired.", NotifyType.StatusMessage);

                        }

                    }

                }

            }

        }

        // Zone Device

        private void EnumerateButtonDevice_Click(object sender, RoutedEventArgs e)
        {

            if (!isDeviceWatcherStarted)
            {

                deviceWatcher.Start();

                isDeviceWatcherStarted = true;

                listDeviceInformationDisplay.Clear();

                rootPage.NotifyUser("Running... device Watcher started.", NotifyType.StatusMessage);

            }

        }

        private DeviceInformationDisplay FindDeviceInformationDisplay(string id)
        {

            foreach (DeviceInformationDisplay deviceInformationDisplay in listDeviceInformationDisplay)
            {

                if (deviceInformationDisplay.Id == id)
                {

                    return deviceInformationDisplay;

                }

            }

            return null;

        }

        private async void DeviceWatcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInformation)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                lock (this)
                {

                    if (sender == deviceWatcher)
                    {

                        if (FindDeviceInformationDisplay(deviceInformation.Id) == null)
                        {

                            if (deviceInformation.Name != string.Empty)
                            {

                                if(deviceInformation.Name.Contains("micro:bit"))
                                {

                                    if (deviceInformation.Pairing.IsPaired)
                                    {

                                        listDeviceInformationDisplay.Add(new DeviceInformationDisplay(deviceInformation));

                                        rootPage.NotifyUser("Device Watcher added.", NotifyType.StatusMessage);

                                    }

                                }

                            }

                        }

                    }
                }

            });

        }

        private async void DeviceWatcher_DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                lock (this)
                {

                    if (sender == deviceWatcher)
                    {

                        DeviceInformationDisplay deviceInformationDisplay = FindDeviceInformationDisplay(deviceInformationUpdate.Id);

                        if (deviceInformationDisplay != null)
                        {

                            deviceInformationDisplay.Update(deviceInformationUpdate);
                            rootPage.NotifyUser("Device Watcher updated.", NotifyType.StatusMessage);
                            return;
                        }

                    }

                }

            });

        }

        private async void DeviceWatcher_DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                lock (this)
                {

                    if (sender == deviceWatcher)
                    {

                        DeviceInformationDisplay deviceInformationDisplay = FindDeviceInformationDisplay(deviceInformationUpdate.Id);

                        if (deviceInformationDisplay != null)
                        {

                            listDeviceInformationDisplay.Remove(deviceInformationDisplay);
                            rootPage.NotifyUser("Device Watcher removed.", NotifyType.StatusMessage);
                        }

                    }

                }

            });

        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object obj)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (sender == deviceWatcher)
                {

                    rootPage.NotifyUser($"{listDeviceInformationDisplay.Count} devices found. Enumeration completed.", NotifyType.StatusMessage);

                }

            });

        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object obj)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (sender == deviceWatcher)
                {
                    rootPage.NotifyUser($"No longer watching for devices.", sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);
                }

            });

        }

        private async void ResultsListViewDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {

                deviceInformationDisplay = (DeviceInformationDisplay)ResultsListViewDevice.SelectedItem;

                if (deviceInformationDisplay.DeviceInformation.Pairing.IsPaired)
                {

                    DeviceUnpairingResult deviceUnpairingResult = await deviceInformationDisplay.DeviceInformation.Pairing.UnpairAsync();

                    if (deviceUnpairingResult.Status.Equals(DeviceUnpairingResultStatus.Unpaired))
                    {

                        // Zone commune
                        this.rootPage.BluetoothLEDevice = null;

                        // Zone Microbit

                        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                        localSettings.Values["Name"] = null;
                        localSettings.Values["Address"] = null;
                        localSettings.Values["Color"] = null;

                        this.ManageMicrobit();

                        // Zone Advertisement
                        if (isBluetoothLEAdvertisementWatcherStarted)
                        {

                            bluetoothLEAdvertisementWatcher.Stop();
                            listBluetoothLEDeviceDisplay.Clear();
                            ResultsListViewAdvertisement.ItemsSource = listBluetoothLEDeviceDisplay;
                            isBluetoothLEAdvertisementWatcherStarted = false;

                        }

                        // Zone Device
                        if (isDeviceWatcherStarted)
                        {

                            deviceWatcher.Stop();
                            listDeviceInformationDisplay.Clear();
                            ResultsListViewDevice.ItemsSource = listDeviceInformationDisplay;
                            isDeviceWatcherStarted = false;

                        }

                        rootPage.NotifyUser("Device unpaired.", NotifyType.StatusMessage);

                    }

                }

            }

        }

    }

    // Zone Advertisement

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

        public string _IsPaired { get; set; }
        public string IsPaired
        {

            get { return _IsPaired; }
            set
            {

                _IsPaired = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsPaired"));
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

    // Zone Device

    public class DeviceInformationDisplay : INotifyPropertyChanged
    {

        public DeviceInformationDisplay(DeviceInformation deviceInformation)
        {

            DeviceInformation = deviceInformation;

        }

        public DeviceInformation DeviceInformation { get; set; }

        public string Id => DeviceInformation.Id;
        public string Name => DeviceInformation.Name;
        public bool IsPaired => (bool?)DeviceInformation.Pairing.IsPaired == true;

        public void Update(DeviceInformationUpdate deviceInformationUpdate)
        {

            DeviceInformation.Update(deviceInformationUpdate);

            OnPropertyChanged(new PropertyChangedEventArgs("DeviceInformation"));

            OnPropertyChanged(new PropertyChangedEventArgs("Id"));
            OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsPaired"));

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
