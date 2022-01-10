using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Microbit
{

    public partial class MainPage : Page
    {

        public const string FEATURE_NAME = "Bluetooth Low Energy C# Sample";

        List<Scenario> scenarios = new List<Scenario>
        {

            new Scenario() { Title="Nearby BLE Advertisement", ClassType=typeof(Scenario1_Advertisement) },
            new Scenario() { Title="BLE Paired Device", ClassType=typeof(Scenario2_Device) },

        };

        public string SelectedBleDeviceId;
        public string SelectedBleDeviceName = "No device selected";

    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

}
