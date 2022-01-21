using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Monorail
{

    public sealed partial class Scenario2_ManagingMicrobit : Page
    {

        // Zone commune
        private MainPage rootPage;

        // Zone Microbit
        String LocalSettingName;
        String LocalSettingAddress;
        String LocalSettingColor;

        public Scenario2_ManagingMicrobit()
        {
            this.InitializeComponent();

            // Zone commune
            this.rootPage = MainPage.Current;

            // Zone Microbit
            if( this.rootPage.BluetoothLEDevice != null )
            {



            }




        }






















        private async void ButtonA_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void ButtonB_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void ButtonAB_Click(object sender, RoutedEventArgs e)
        {
        }

    }

}
