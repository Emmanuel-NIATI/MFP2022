using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public BluetoothLEDevice BLEDevice { get; set; }

        public MainPage()
        {

            this.InitializeComponent();

            Current = this;

            MyFrame.Navigate(typeof(BLEAdvertisementWatcherPage));
                        
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {

            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (BLEAdvertisementWatcherListBoxItem.IsSelected)
            {

                MyFrame.Navigate(typeof(BLEAdvertisementWatcherPage));
            }
            else if (MicrobitListBoxItem.IsSelected)
            {

                MyFrame.Navigate(typeof(MicrobitPage));
            }

        }

        public void Navigate(Type sourcePageType)
        {
            MyFrame.Navigate(sourcePageType);
        }

    }

}
