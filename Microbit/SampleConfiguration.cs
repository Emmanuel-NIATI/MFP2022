using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Microbit
{

    public partial class MainPage : Page
    {

        public const string FEATURE_NAME = "Gestion de la carte micro:bit";

        List<Scenario> scenarios = new List<Scenario>
        {

            new Scenario() { Logo="\xE702", Title="Nearby BLE Advertisement", ClassType=typeof(Scenario1_Advertisement) },
            new Scenario() { Logo="\xE702", Title="BLE Paired Device", ClassType=typeof(Scenario2_Device) },
            new Scenario() { Logo="\xE787", Title="Gestion de la carte micro:bit", ClassType=typeof(Scenario3_Microbit) },
            new Scenario() { Logo="\xE787", Title="Gestion de la carte micro:bit UART", ClassType=typeof(Scenario4_Microbit) }

        };

    }

    public class Scenario
    {
        public string Logo { get; set; }
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

}
