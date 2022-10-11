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

        private GpioPin _gpio27;
        private GpioPin _gpio05;
        private GpioPin _gpio13;
        private GpioPin _gpio19;
        private GpioPin _gpio26;
        private GpioPin _gpio21;

        // Variables liées à l'affichage LCD

        uint[] rgb_black = new uint[240 * 64];
        uint[] rgb_win10iot = new uint[240 * 256];

        uint[] rgb_chat01 = new uint[ 240 * 320 ];

        uint[] rgb_admin = new uint[240 * 256];
        uint[] rgb_blast = new uint[240 * 256];
        uint[] rgb_csa = new uint[240 * 256];
        uint[] rgb_gamer = new uint[240 * 256];

        String rgb = "";

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

            ecran.LoadImage(rgb_black, "ms-appx:///Pictures/black.png");
            ecran.LoadImage(rgb_win10iot, "ms-appx:///Pictures/win10iot.png");

            ecran.LoadImage(rgb_chat01, "ms-appx:///Pictures/chat01.png");

            ecran.LoadImage(rgb_admin, "ms-appx:///Pictures/admin.png");
            ecran.LoadImage(rgb_blast, "ms-appx:///Pictures/blast.png");
            ecran.LoadImage(rgb_csa, "ms-appx:///Pictures/csa.png");
            ecran.LoadImage(rgb_gamer, "ms-appx:///Pictures/gamer.png");

        }

        private void InitGpio()
        {

            // Configuration du contrôleur du GPIO par défaut
            _gpc = GpioController.GetDefault();

            // Bouton Gris sur GPIO 27 en entrée
            _gpio27 = _gpc.OpenPin(27);
            _gpio27.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _gpio27.DebounceTimeout = new TimeSpan(10000);

            // Bouton Blanc sur GPIO 5 en entrée
            _gpio05 = _gpc.OpenPin(5);
            _gpio05.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _gpio05.DebounceTimeout = new TimeSpan(10000);

            // Bouton Vert sur GPIO 13 en entrée
            _gpio13 = _gpc.OpenPin(13);
            _gpio13.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _gpio13.DebounceTimeout = new TimeSpan(10000);

            // Bouton Bleu sur GPIO 19 en entrée
            _gpio19 = _gpc.OpenPin(19);
            _gpio19.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _gpio19.DebounceTimeout = new TimeSpan(10000);

            // Bouton Jaune sur GPIO 26 en entrée
            _gpio26 = _gpc.OpenPin(26);
            _gpio26.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _gpio26.DebounceTimeout = new TimeSpan(10000);

            // Buzzer sur sur GPIO 21 en sortie
            _gpio21 = _gpc.OpenPin(21);
            _gpio21.SetDriveMode(GpioPinDriveMode.Output);
            _gpio21.Write(GpioPinValue.Low);

        }

        private void afficherHorloge( string _hh, string _mm, string _ss, string _dow, string _day, string _month, string _year)
        {

            // Affiche HH:MM:SS

            ecran.PlaceCursor(0, 34 * 8);
            ecran.Print( _hh + ":" + _mm, 4, ILI9341.COLOR_BLUE_WINDOWS);

            ecran.PlaceCursor(20 * 6, 34 * 8);
            ecran.Print( ":" + _ss, 3, ILI9341.COLOR_BLUE_WINDOWS);

            ecran.PlaceCursor(23 * 6, 34 * 8);
            ecran.Print(_ss, 3, ILI9341.COLOR_BLUE_WINDOWS);

            // Affiche Dow dd/mm/yyyy

            ecran.PlaceCursor(0, 38 * 8);
            ecran.Print( _dow + " " + _day + "/" + _month + "/" + _year, 2, ILI9341.COLOR_BLUE_WINDOWS);

        }

        private async void InitSpiDisplay()
        {

            // Initialisation de l'affichage
            await ecran.PowerOnSequence();

            // Effacer l'écran
            ecran.ClearScreen();

            // Dessiner l'image de fond
            ecran.DrawPicture(rgb_win10iot, 0, 0, 240, 256);
            rgb = "win10iot";

            // Dessiner l'image du bas
            ecran.DrawPicture(rgb_black, 0, 256, 240, 64);

            // Initialiser l'affichage de l'heure et de la date
            string hh = "09";
            string mm = "30";
            string ss = "25";
            string dow = "Ven";
            string day = "08";
            string month = "10";
            string year = "2021";

            afficherHorloge(hh, mm, ss, dow, day, month, year);

        }

        private void TravauxTimer()
        {

            // Configuration du Timer Horloge
            DispatcherTimer dispatcherTimerHorloge = new DispatcherTimer();
            dispatcherTimerHorloge.Tick += DispatcherTimerHorloge_Tick;
            dispatcherTimerHorloge.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimerHorloge.Start();

            // Configuration du Timer Image
            DispatcherTimer dispatcherTimerImage = new DispatcherTimer();
            dispatcherTimerImage.Tick += DispatcherTimerImage_Tick;
            dispatcherTimerImage.Interval = new TimeSpan(0, 0, 0, 0, 2000);
            dispatcherTimerImage.Start();

        }

        private void DispatcherTimerHorloge_Tick(object sender, object e)
        {

            DateTime _DateTime = DateTime.Now;

            string _hh = _DateTime.Hour.ToString();
            string _mm = _DateTime.Minute.ToString();
            string _ss = _DateTime.Second.ToString();

            string _dow = _DateTime.DayOfWeek.ToString();
            string _day = _DateTime.Day.ToString();
            string _month = _DateTime.Month.ToString();
            string _year = _DateTime.Year.ToString();

            if (_hh.Length < 2) { _hh = "0" + _hh; }
            if (_mm.Length < 2) { _mm = "0" + _mm; }
            if (_ss.Length < 2) { _ss = "0" + _ss; }

            if (_dow.Equals("Sunday")) { _dow = "Dim"; }
            if (_dow.Equals("Monday")) { _dow = "Lun"; }
            if (_dow.Equals("Tuesday")) { _dow = "Mar"; }
            if (_dow.Equals("Wednesday")) { _dow = "Mer"; }
            if (_dow.Equals("Thursday")) { _dow  = "Jeu"; }
            if (_dow.Equals("Friday")) { _dow = "Ven"; }
            if (_dow.Equals("Saturday")) { _dow  = "Sam"; }

            if (_day.Length < 2) { _day = "0" + _day; }
            if (_month.Length < 2) { _month = "0" + _month; }

            afficherHorloge(_hh, _mm, _ss, _dow, _day, _month, _year);

        }

        private void DispatcherTimerImage_Tick(object sender, object e)
        {

            if( "win10iot".Equals( rgb ) )
            {

                ecran.DrawPicture(rgb_csa, 0, 0, 240, 256);
                rgb = "csa";

            }
            else if( "csa".Equals( rgb ) )
            {

                ecran.DrawPicture(rgb_admin, 0, 0, 240, 256);
                rgb = "admin";

            }
            else if ("admin".Equals(rgb))
            {

                ecran.DrawPicture(rgb_gamer, 0, 0, 240, 256);
                rgb = "gamer";

            }
            else if ("gamer".Equals(rgb))
            {

                ecran.DrawPicture(rgb_blast, 0, 0, 240, 256);
                rgb = "blast";

            }
            else if ("blast".Equals(rgb))
            {

                ecran.DrawPicture(rgb_csa, 0, 0, 240, 256);
                rgb = "csa";

            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }

}
