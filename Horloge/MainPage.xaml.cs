﻿using LCDDisplayDriver;
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

        private GpioPin _pin27;
        private GpioPin _pin05;
        private GpioPin _pin13;
        private GpioPin _pin19;
        private GpioPin _pin26;
        private GpioPin _pin21;

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

            // Initialisation du Timer
            this.TravauxTimer();

        }

        private void InitVariable()
        {

            ecran.LoadImage(rgb_chat01, "ms-appx:///Pictures/chat01.png");
            
        }

        private void InitGpio()
        {

            // Configuration du contrôleur du GPIO par défaut
            _gpc = GpioController.GetDefault();

            // Bouton Gris sur GPIO 27 en entrée
            _pin27 = _gpc.OpenPin(27);
            _pin27.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _pin27.DebounceTimeout = new TimeSpan(10000);

            // Bouton Blanc sur GPIO 5 en entrée
            _pin05 = _gpc.OpenPin(5);
            _pin05.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _pin05.DebounceTimeout = new TimeSpan(10000);

            // Bouton Vert sur GPIO 13 en entrée
            _pin13 = _gpc.OpenPin(13);
            _pin13.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _pin13.DebounceTimeout = new TimeSpan(10000);

            // Bouton Bleu sur GPIO 19 en entrée
            _pin19 = _gpc.OpenPin(19);
            _pin19.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _pin19.DebounceTimeout = new TimeSpan(10000);

            // Bouton Jaune sur GPIO 26 en entrée
            _pin26 = _gpc.OpenPin(26);
            _pin26.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _pin26.DebounceTimeout = new TimeSpan(10000);

            // Buzzer sur sur GPIO 21 en sortie
            _pin21 = _gpc.OpenPin(21);
            _pin21.SetDriveMode(GpioPinDriveMode.Output);
            _pin21.Write(GpioPinValue.Low);

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

        private void TravauxTimer()
        {


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }

}
