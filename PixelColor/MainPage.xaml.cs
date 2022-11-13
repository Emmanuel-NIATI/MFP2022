using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PixelColor
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Brush SCB_Black = new SolidColorBrush(Windows.UI.Colors.Black);
        private Brush SCB_White = new SolidColorBrush(Windows.UI.Colors.White);
        private Brush SCB_Blue = new SolidColorBrush(Windows.UI.Colors.Blue);
        private Brush SCB_LightSkyBlue = new SolidColorBrush(Windows.UI.Colors.LightSkyBlue);
        private Brush SCB_Green = new SolidColorBrush(Windows.UI.Colors.Green);
        private Brush SCB_LightGreen = new SolidColorBrush(Windows.UI.Colors.LightGreen);
        private Brush SCB_Red = new SolidColorBrush(Windows.UI.Colors.Red);
        private Brush SCB_Pink = new SolidColorBrush(Windows.UI.Colors.Pink);
        private Brush SCB_Orange = new SolidColorBrush(Windows.UI.Colors.Orange);
        private Brush SCB_Yellow = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private Brush SCB_Purple = new SolidColorBrush(Windows.UI.Colors.Purple);
        private Brush SCB_Violet = new SolidColorBrush(Windows.UI.Colors.Violet);

        private Brush SCB_Gray = new SolidColorBrush(Windows.UI.Colors.Gray);

        private Brush SCB_Color = new SolidColorBrush(Windows.UI.Colors.White);

        public MainPage()
        {

            this.InitializeComponent();

        }

        private void Initialiser()
        {

            for(int y=0; y<30; y++)
            {

                StackPanel sp = new StackPanel();

                int noSP = y + 1;
                String StrNoSP = "" + noSP;

                if( StrNoSP.Length < 2 )    { StrNoSP = "0" + StrNoSP;   }

                sp.Name = "StackPanelRightLigne" + StrNoSP;

                sp.Orientation = Orientation.Horizontal;

                for (int x = 0; x < 30; x++)
                {

                    Button b = new Button();

                    int noB = x + 1;
                    String StrNoB = "" + noB;

                    if (StrNoB.Length < 2) { StrNoB = "0" + StrNoB; }

                    b.Name = "Btn" + StrNoB + StrNoSP;

                    b.Width = 20;
                    b.Height = 20;

                    b.Background = SCB_White;
                    b.BorderBrush = SCB_Gray;

                    b.BorderThickness = new Thickness(1);

                    b.IsEnabled = true;

                    b.Click += B_Click;

                    sp.Children.Add(b);

                }

                StackPanelBottomRight.Children.Add( sp );

            }

        }

        private void B_Click(object sender, RoutedEventArgs e)
        {

            String name = ( (Button) sender ).Name;

            Debug.WriteLine(name);

            Button b = (Button) StackPanelBottomRight.FindName(name);

            b.Background = SCB_Color;

        }

        private void InitialiserBtnBorderBrush()
        {

            BtnBlack.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnWhite.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnBlue.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnLightSkyBlue.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnGreen.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnLightGreen.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnRed.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnPink.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnOrange.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnYellow.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnPurple.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
            BtnViolet.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);

        }

        private void BtnBlack_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Black;

            this.InitialiserBtnBorderBrush();

            BtnBlack.BorderBrush = SCB_Black;

        }

        private void BtnWhite_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_White;

            this.InitialiserBtnBorderBrush();

            BtnWhite.BorderBrush = SCB_Black;

        }

        private void BtnBlue_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Blue;

            this.InitialiserBtnBorderBrush();

            BtnBlue.BorderBrush = SCB_Black;

        }

        private void BtnLightSkyBlue_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_LightSkyBlue;

            this.InitialiserBtnBorderBrush();

            BtnLightSkyBlue.BorderBrush = SCB_Black;

        }

        private void BtnGreen_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Green;

            this.InitialiserBtnBorderBrush();

            BtnGreen.BorderBrush = SCB_Black;

        }

        private void BtnLightGreen_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_LightGreen;

            this.InitialiserBtnBorderBrush();

            BtnLightGreen.BorderBrush = SCB_Black;

        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Red;

            this.InitialiserBtnBorderBrush();

            BtnRed.BorderBrush = SCB_Black;

        }

        private void BtnPink_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Pink;

            this.InitialiserBtnBorderBrush();

            BtnPink.BorderBrush = SCB_Black;

        }

        private void BtnOrange_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Orange;

            this.InitialiserBtnBorderBrush();

            BtnOrange.BorderBrush = SCB_Black;

        }

        private void BtnYellow_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Yellow;

            this.InitialiserBtnBorderBrush();

            BtnYellow.BorderBrush = SCB_Black;

        }

        private void BtnPurple_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Purple;

            this.InitialiserBtnBorderBrush();

            BtnPurple.BorderBrush = SCB_Black;

        }

        private void BtnViolet_Click(object sender, RoutedEventArgs e)
        {

            SCB_Color = SCB_Violet;

            this.InitialiserBtnBorderBrush();

            BtnViolet.BorderBrush = SCB_Black;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            this.Initialiser();

        }

    }

}
