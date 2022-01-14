using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

using Windows.UI.Core;

using Windows.UI.Xaml;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microbit
{

    public sealed partial class Scenario2_Device : Page
    {

        private DeviceWatcher watcher;

        bool isWatcherStarted = false;

        private ObservableCollection<DeviceInformationDisplay> listDeviceInformationDisplay = new ObservableCollection<DeviceInformationDisplay>();

        DeviceInformationDisplay deviceInformationDisplay;

        private MainPage rootPage;

        public Scenario2_Device()
        {

            this.InitializeComponent();

            rootPage = MainPage.Current;

            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable", "System.Devices.Aep.IsPaired" };

            watcher = DeviceInformation.CreateWatcher(aqsAllBluetoothLEDevices, requestedProperties, DeviceInformationKind.AssociationEndpoint);

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            rootPage = MainPage.Current;

            watcher.Added += Watcher_DeviceAdded;
            watcher.Updated += Watcher_DeviceUpdated;
            watcher.Removed += Watcher_DeviceRemoved;
            watcher.EnumerationCompleted += Watcher_EnumerationCompleted;
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

            watcher.Added -= Watcher_DeviceAdded;
            watcher.Updated -= Watcher_DeviceUpdated;
            watcher.Removed -= Watcher_DeviceRemoved;
            watcher.EnumerationCompleted -= Watcher_EnumerationCompleted;
            watcher.Stopped -= Watcher_Stopped;

            rootPage.NotifyUser("Navigating away. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

            if (isWatcherStarted)
            {
                watcher.Stop();
                isWatcherStarted = false;
            }

            watcher.Added -= Watcher_DeviceAdded;
            watcher.Updated -= Watcher_DeviceUpdated;
            watcher.Removed -= Watcher_DeviceRemoved;
            watcher.EnumerationCompleted -= Watcher_EnumerationCompleted;
            watcher.Stopped -= Watcher_Stopped;

            rootPage.NotifyUser("App suspending. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {

            watcher.Added += Watcher_DeviceAdded;
            watcher.Updated += Watcher_DeviceUpdated;
            watcher.Removed += Watcher_DeviceRemoved;
            watcher.EnumerationCompleted += Watcher_EnumerationCompleted;
            watcher.Stopped += Watcher_Stopped;

            rootPage.NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {

            if (!isWatcherStarted)
            {

                watcher.Start();

                isWatcherStarted = true;

                listDeviceInformationDisplay.Clear();

                rootPage.NotifyUser("Running... Watcher started.", NotifyType.StatusMessage);

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

        private async void Watcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInformation)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                lock (this)
                {

                    if (sender == watcher)
                    {

                        if (FindDeviceInformationDisplay(deviceInformation.Id ) == null )
                        {

                            if (deviceInformation.Name != string.Empty)
                            {

                                listDeviceInformationDisplay.Add(new DeviceInformationDisplay(deviceInformation));

                            }

                        }

                    }
                }

            });

        }

        private async void Watcher_DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                lock (this)
                {

                    if (sender == watcher)
                    {

                        DeviceInformationDisplay deviceInformationDisplay = FindDeviceInformationDisplay(deviceInformationUpdate.Id);

                        if ( deviceInformationDisplay != null )
                        {

                            deviceInformationDisplay.Update(deviceInformationUpdate);
                            return;
                        }

                    }

                }

            });

        }

        private async void Watcher_DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                lock (this)
                {

                    if (sender == watcher)
                    {

                        DeviceInformationDisplay deviceInformationDisplay = FindDeviceInformationDisplay(deviceInformationUpdate.Id);

                        if (deviceInformationDisplay != null)
                        {

                            listDeviceInformationDisplay.Remove( deviceInformationDisplay );

                        }

                    }

                }

            });

        }

        private async void Watcher_EnumerationCompleted(DeviceWatcher sender, object obj)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (sender == watcher)
                {

                    rootPage.NotifyUser($"{listDeviceInformationDisplay.Count} devices found. Enumeration completed.", NotifyType.StatusMessage);

                }

            });

        }

        private async void Watcher_Stopped(DeviceWatcher sender, object obj)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (sender == watcher)
                {
                    rootPage.NotifyUser($"No longer watching for devices.", sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);
                }

            });

        }

        private async void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {


            }

        }
        
    }

    public class DeviceInformationDisplay : INotifyPropertyChanged
    {

        public DeviceInformationDisplay(DeviceInformation deviceInformation)
        {

            DeviceInformation = deviceInformation;

        }

        public DeviceInformation DeviceInformation { get; private set; }

        public string Id => DeviceInformation.Id;
        public string Name => DeviceInformation.Name;
        public bool IsPaired => DeviceInformation.Pairing.IsPaired;
        public bool IsConnected => (bool?)DeviceInformation.Properties["System.Devices.Aep.IsConnected"] == true;
        public bool IsConnectable => (bool?)DeviceInformation.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"] == true;

        public IReadOnlyDictionary<string, object> Properties => DeviceInformation.Properties;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            DeviceInformation.Update(deviceInfoUpdate);

            OnPropertyChanged("Id");
            OnPropertyChanged("Name");
            OnPropertyChanged("DeviceInformation");
            OnPropertyChanged("IsPaired");
            OnPropertyChanged("IsConnected");
            OnPropertyChanged("Properties");
            OnPropertyChanged("IsConnectable");

        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
