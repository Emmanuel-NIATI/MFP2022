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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Microbit
{

    public sealed partial class Scenario3_Microbit : Page
    {

        private MainPage rootPage;

        public Scenario3_Microbit()
        {

            this.InitializeComponent();

            rootPage = MainPage.Current;

        }

        private void AButton_Click(object sender, RoutedEventArgs e)
        {

            rootPage.NotifyUser("Envoi A.", NotifyType.StatusMessage);
        }

        private void BButton_Click(object sender, RoutedEventArgs e)
        {

            rootPage.NotifyUser("Envoi B.", NotifyType.StatusMessage);
        }

        private void ABButton_Click(object sender, RoutedEventArgs e)
        {

            rootPage.NotifyUser("Envoi A+B.", NotifyType.StatusMessage);
        }

    }

}
