using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        private ObservableCollection<BluetoothLEDeviceDisplay> listBluetoothLEDeviceDisplay = new ObservableCollection<BluetoothLEDeviceDisplay>();

        BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay;
        BluetoothLEDevice bluetoothLEDevice;
        BluetoothDevice bluetoothDevice;

        private MainPage rootPage;

        public Scenario2_Device()
        {

            this.InitializeComponent();

            rootPage = MainPage.Current;

            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.IsPaired" };

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

        private async void Watcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {

                if(deviceInfo.Name != "")
                {

                    BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay = new BluetoothLEDeviceDisplay();

                    bluetoothLEDeviceDisplay.Id = deviceInfo.Id;
                    bluetoothLEDeviceDisplay.Name = deviceInfo.Name;
                    
                    if(deviceInfo.Pairing.IsPaired)
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

            });

        }

        private async void Watcher_DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {


                if (isWatcherStarted)
                {

                }

            });

        }

        private async void Watcher_DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {

                /*
                // Watcher may have stopped while we were waiting for our chance to run.
                if (IsWatcherStarted(sender))
                {
                    // Find the corresponding DeviceInformation in the collection and remove it
                    foreach (DeviceInformationDisplay deviceInfoDisp in resultCollection)
                    {
                        if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            resultCollection.Remove(deviceInfoDisp);
                            break;
                        }
                    }

                    RaiseDeviceChanged(sender, deviceInfoUpdate.Id);
                }
                */

            });

        }

        private async void Watcher_EnumerationCompleted(DeviceWatcher sender, object obj)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {

            });

        }

        private async void Watcher_Stopped(DeviceWatcher sender, object obj)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {

            });

        }

        private async void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {


            }

        }

    }

}
