using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Devices.Bluetooth;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Monorail
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
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

        public BluetoothLEDevice bluetoothLEDevice;

        public MainPage()
        {

            this.InitializeComponent();

            Current = this;

            MyFrame.Navigate(typeof(BLEPairedDevicePage));

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {

            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

        }

        private void App_Suspending(object sender, object e)
        {



        }

        private void App_Resuming(object sender, object e)
        {



        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {

            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            if (BLEPairedDeviceListBoxItem.IsSelected)
            {

                MyFrame.Navigate(typeof(BLEPairedDevicePage));
            }
            else if (BLEAdvertisementWatcherListBoxItem.IsSelected)
            {

                MyFrame.Navigate(typeof(BLEAdvertisementWatcherPage));
            }
            else if (MicrobitListBoxItem.IsSelected)
            {

                MyFrame.Navigate(typeof(MicrobitPage));
            }
            else if (TextToSpeechListBoxItem.IsSelected)
            {

                MyFrame.Navigate(typeof(TextToSpeechPage));
            }



        }

        public void Navigate(Type sourcePageType)
        {
            MyFrame.Navigate(sourcePageType);
        }

    }

}
