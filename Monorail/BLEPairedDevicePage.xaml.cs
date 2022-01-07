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
using System.Collections;
using Windows.UI.Core;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Monorail
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class BLEPairedDevicePage : Page
    {

        private DeviceWatcher watcher = DeviceInformation.CreateWatcher(BluetoothDevice.GetDeviceSelectorFromPairingState(false) );

        bool isWatcherStarted = false;

        private ObservableCollection<BluetoothLEDeviceDisplay> listBluetoothLEDeviceDisplay = new ObservableCollection<BluetoothLEDeviceDisplay>();

        BluetoothLEDeviceDisplay bluetoothLEDeviceDisplay;
        BluetoothLEDevice bluetoothLEDevice;

        private MainPage rootPage;

        public BLEPairedDevicePage()
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

            NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {

            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

            watcher.Stop();

            watcher.Added -= Watcher_DeviceAdded;
            watcher.Updated -= Watcher_DeviceUpdated;
            watcher.Removed -= Watcher_DeviceRemoved;
            watcher.EnumerationCompleted -= Watcher_EnumerationCompleted;
            watcher.Stopped -= Watcher_Stopped;

            NotifyUser("Navigating away. Watcher stopped.", NotifyType.StatusMessage);

            base.OnNavigatingFrom(e);
        }

        private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

            watcher.Added -= Watcher_DeviceAdded;
            watcher.Updated -= Watcher_DeviceUpdated;
            watcher.Removed -= Watcher_DeviceRemoved;
            watcher.EnumerationCompleted -= Watcher_EnumerationCompleted;
            watcher.Stopped -= Watcher_Stopped;

            NotifyUser("App suspending. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {

            watcher.Added += Watcher_DeviceAdded;
            watcher.Updated += Watcher_DeviceUpdated;
            watcher.Removed += Watcher_DeviceRemoved;
            watcher.EnumerationCompleted += Watcher_EnumerationCompleted;
            watcher.Stopped += Watcher_Stopped;

            NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {

            if (!isWatcherStarted)
            {

                isWatcherStarted = true;

                listBluetoothLEDeviceDisplay.Clear();

        

                watcher.Start();

                NotifyUser("Running... Watcher started.", NotifyType.StatusMessage);

            }

        }


        private async void Watcher_DeviceAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {

            Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Watcher_DeviceAdded : ");

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {

                if (isWatcherStarted)
                {

                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Watcher_DeviceAdded : ");
                    Debug.WriteLine("");
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Id : " + deviceInfo.Id);
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Name : " + deviceInfo.Name);
                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>> Pairing : " + deviceInfo.Pairing.IsPaired);
                    //resultCollection.Add(new DeviceInformationDisplay(deviceInfo));

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
