using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Manege
{

    public sealed partial class MainPage : Page
    {

        public const string FEATURE_NAME = "Le manège de la fête foraine avec micro:bit";

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }

        List<Scenario> scenarios = new List<Scenario>
        {

            new Scenario() { Logo="\xE702", Title="Pairing the device", ClassType=typeof(Scenario1_PairingDevice) }

        };

    }

    public class Scenario
    {
        public string Logo { get; set; }
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

}
