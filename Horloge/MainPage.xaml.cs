using LCDDisplayDriver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
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

namespace Horloge
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // Variables liées aux librairies
        private static readonly ILI9341 ecran = new ILI9341();

        // Variables liées au GPIO
        private GpioController _gpc;

        // Variables liées à l'affichage LCD
        uint[] rgb_chat01 = new uint[ 240 * 320 ];





        public MainPage()
        {

            this.InitializeComponent();

            // Initialisation des variables
            this.InitVariable();

            // Initialisation du GPIO
            this.InitGpio();

            // Initialisation de l'affichage
            this.InitSpiDisplay();

        }

        private void InitVariable()
        {

            ecran.LoadImage(rgb_chat01, 240, 320, "ms-appx:///Pictures/chat01.png");

        }

        private void InitGpio()
        {

            // Configuration du contrôleur du GPIO par défaut
            _gpc = GpioController.GetDefault();

        }

        private async void InitSpiDisplay()
        {

            // Initialisation de l'affichage
            await ecran.PowerOnSequence();

            // Effacer l'écran
            ecran.ClearScreen();

            

            ecran.DrawPicture(rgb_chat01, 0, 0, 240, 320);

            ecran.PlaceCursor(0, 0);

            ecran.Print("0", 4, ILI9341.COLOR_PINK_PAULINE);

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }

}
