using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Gigabyte
{

    public sealed partial class MainPage : Page
    {

        public const string APPLICATION_TITLE = "Gigabyte PC Case BLE Devices";

        public const string SCENARIO_01_TITLE = "Pairing the device";
        public const string SCENARIO_02_TITLE = "Managing micro:bit board (BlockyTalky)";
        public const string SCENARIO_03_TITLE = "Managing BLE earsphone";

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }

        List<Scenario> scenarios = new List<Scenario>
        {

            new Scenario() { Logo="\xE702", Title=SCENARIO_01_TITLE, ClassType=typeof(Scenario1_PairingDevice) },
            new Scenario() { Logo="\xE702", Title=SCENARIO_02_TITLE, ClassType=typeof(Scenario2_ManagingDevice01) },
            new Scenario() { Logo="\xE702", Title=SCENARIO_03_TITLE, ClassType=typeof(Scenario3_ManagingDevice02) }

        };

    }

    public class Scenario
    {
        public string Logo { get; set; }
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

}
