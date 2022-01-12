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

        private DeviceWatcher watcher = DeviceInformation.CreateWatcher();

        bool isWatcherStarted = false;

        private ObservableCollection<BluetoothLEDeviceDisplay> listBluetoothLEDeviceDisplay = new ObservableCollection<BluetoothLEDeviceDisplay>();

        BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay;
        BluetoothLEDevice bluetoothLEDevice;
        BluetoothDevice bluetoothDevice;

        private MainPage rootPage;

        public Scenario2_Device()
        {
            this.InitializeComponent();
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

            if (isWatcherStarted) { watcher.Stop(); }

            watcher.Added -= Watcher_DeviceAdded;
            watcher.Updated -= Watcher_DeviceUpdated;
            watcher.Removed -= Watcher_DeviceRemoved;
            watcher.EnumerationCompleted -= Watcher_EnumerationCompleted;
            watcher.Stopped -= Watcher_Stopped;

            rootPage.NotifyUser("Navigating away. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

            if (isWatcherStarted) { watcher.Stop(); }

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

                isWatcherStarted = true;

                listBluetoothLEDeviceDisplay.Clear();



                watcher.Start();

                rootPage.NotifyUser("Running... Watcher started.", NotifyType.StatusMessage);

            }

        }

        private async void Watcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {

            var selector = BluetoothLEDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selector);

            Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Devices : ");

            Debug.WriteLine("");

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {

                if (isWatcherStarted)
                {

                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Watcher_DeviceAdded : ");
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Id : " + deviceInfo.Id);
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Name : " + deviceInfo.Name);
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Pairing : " + deviceInfo.Pairing.IsPaired);
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Kind : " + deviceInfo.Kind);
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Type : " + deviceInfo.GetType().ToString());
                    Debug.WriteLine("");

                }

            });

        }

        private async void Watcher_DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {

                /*
                if (IsWatcherStarted(sender))
                {
                    // Find the corresponding updated DeviceInformation in the collection and pass the update object
                    // to the Update method of the existing DeviceInformation. This automatically updates the object
                    // for us.
                    foreach (DeviceInformationDisplay deviceInfoDisp in resultCollection)
                    {
                        if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
                        {
                            deviceInfoDisp.Update(deviceInfoUpdate);
                            RaiseDeviceChanged(sender, deviceInfoUpdate.Id);
                            break;
                        }
                    }
                }
                */

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
