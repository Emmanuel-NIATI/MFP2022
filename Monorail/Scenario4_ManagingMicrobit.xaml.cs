using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace Monorail
{

    public sealed partial class Scenario4_ManagingMicrobit : Page
    {

        // Zone commune
        private MainPage rootPage;

        // Zone Microbit
        String LocalSettingName;
        String LocalSettingAddress;
        String LocalSettingColor;

        // LED
        private string SelectedServiceLedUUID = "e95dd91d-251d-470a-a062-fa1922dfa9a8";
        private string SelectedCharacteristicLedMatrixUUID = "e95d7b77-251d-470a-a062-fa1922dfa9a8";
        private string SelectedCharacteristicLedTextUUID = "e95d93ee-251d-470a-a062-fa1922dfa9a8";
        private string SelectedCharacteristicLedScrollingDelayUUID = "e95d0d2d-251d-470a-a062-fa1922dfa9a8";

        private GattDeviceService selectedServiceLed;
        private GattCharacteristic selectedCharacteristicLedMatrix;
        private GattCharacteristic selectedCharacteristicLedText;
        private GattCharacteristic selectedCharacteristicLedScrollingDelay;

        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df);

        // Octet 0
        bool o0_b4 = false;
        bool o0_b3 = false;
        bool o0_b2 = false;
        bool o0_b1 = false;
        bool o0_b0 = false;

        // Octet 1
        bool o1_b4 = false;
        bool o1_b3 = false;
        bool o1_b2 = false;
        bool o1_b1 = false;
        bool o1_b0 = false;

        // Octet 2
        bool o2_b4 = false;
        bool o2_b3 = false;
        bool o2_b2 = false;
        bool o2_b1 = false;
        bool o2_b0 = false;

        // Octet 3
        bool o3_b4 = false;
        bool o3_b3 = false;
        bool o3_b2 = false;
        bool o3_b1 = false;
        bool o3_b0 = false;

        // Octet 4
        bool o4_b4 = false;
        bool o4_b3 = false;
        bool o4_b2 = false;
        bool o4_b1 = false;
        bool o4_b0 = false;

        public Scenario4_ManagingMicrobit()
        {

            this.InitializeComponent();

            // Zone commune
            this.rootPage = MainPage.Current;

            // Zone Microbit
            this.ManageMicrobit();

        }

        // Zone commune
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Zone commune
            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

            // Zone Microbit


            // Zone notification
            rootPage.NotifyUser("Gestion de la carte micro:bit.", NotifyType.StatusMessage);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            // Zone commune
            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

            // Zone Microbit


            // Zone notification
            rootPage.NotifyUser("A bientôt !", NotifyType.StatusMessage);

        }

        protected void App_Suspending(object sender, object e)
        {


            // Zone Microbit


            // Zone notification
            rootPage.NotifyUser("A bientôt !", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {


            // Zone Microbit


            // Zone notification
            rootPage.NotifyUser("Gestion de la carte micro:bit.", NotifyType.StatusMessage);

        }


        // Zone Microbit
        public async void ManageMicrobit()
        {

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            LocalSettingName = localSettings.Values["Name"] as string;
            LocalSettingAddress = localSettings.Values["Address"] as string;
            LocalSettingColor = localSettings.Values["Color"] as string;

            if (LocalSettingName == null)
            {
                localSettingName.Text = "";
            }

            if (LocalSettingAddress == null)
            {
                localSettingAddress.Text = "";
            }

            if (LocalSettingName != null && LocalSettingAddress != null && LocalSettingColor != null)
            {

                if (!LocalSettingName.Equals("") && !LocalSettingAddress.Equals("") && !LocalSettingColor.Equals(""))
                {

                    localSettingName.Text = LocalSettingName;

                    localSettingAddress.Text = LocalSettingAddress;

                    if (LocalSettingColor.Equals("bleu")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_bleu.png")); }
                    if (LocalSettingColor.Equals("jaune")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_jaune.png")); }
                    if (LocalSettingColor.Equals("rouge")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_rouge.png")); }
                    if (LocalSettingColor.Equals("vert")) { ImageMicrobit.Source = new BitmapImage(new Uri("ms-appx:///Assets/microbit_vert.png")); }

                }

            }
            else
            {

                localSettings.Values["Name"] = null;
                localSettings.Values["Address"] = null;
                localSettings.Values["Color"] = null;

            }

            if (this.rootPage.BluetoothLEDevice != null)
            {

                IReadOnlyList<GattDeviceService> listGattDeviceService = this.rootPage.BluetoothLEDevice.GattServices;

                Debug.WriteLine(">>>>>>>>>> listGattDeviceService : " + listGattDeviceService.Count);

                foreach (GattDeviceService gattDeviceService in listGattDeviceService)
                {

                    Debug.WriteLine(">>>>>>>>>> gattDeviceService : " + gattDeviceService.Uuid);

                    if (gattDeviceService.Uuid.ToString().Equals(SelectedServiceLedUUID))
                    {

                        selectedServiceLed = this.rootPage.BluetoothLEDevice.GetGattService(gattDeviceService.Uuid);

                        if (selectedServiceLed != null)
                        {

                            Debug.WriteLine(">>>>>>>>>> selectedServiceLed :  Non null...");

                        }

                        IReadOnlyList<GattCharacteristic> listGattCharacteristic = selectedServiceLed.GetAllCharacteristics();

                        foreach (GattCharacteristic gattCharacteristic in listGattCharacteristic)
                        {

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicLedMatrixUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicLedMatrix = selectedServiceLed.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicLedMatrix = listGattCharacteristicLedMatrix[0];

                                


                                if (selectedCharacteristicLedMatrix != null)
                                {

                                    Debug.WriteLine(">>>>>>>>>> selectedCharacteristicLedMatrix :  Non null...");

                                }

                            }

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicLedTextUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicLedText = selectedServiceLed.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicLedText = listGattCharacteristicLedText[0];

                                if (selectedCharacteristicLedText != null)
                                {

                                    Debug.WriteLine(">>>>>>>>>> selectedCharacteristicLedText : non Null...");

                                }

                            }

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicLedScrollingDelayUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicLedScrollingDelay = selectedServiceLed.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicLedScrollingDelay = listGattCharacteristicLedScrollingDelay[0];
                                
                                if (selectedCharacteristicLedScrollingDelay != null)
                                {

                                    Debug.WriteLine(">>>>>>>>>> selectedCharacteristicLedScrollingDelay : non Null...");

                                }

                            }

                        }

                    }

                }

            }

        }

        // Conversion bool[] to int
        private int ConvertBoolTabToInt(bool[] boolTab)
        {
            int resultat = 0;

            for(int i=0; i < boolTab.Length; i++)
            {

                if( boolTab[i] )
                {

                    if (i == 0) { resultat = resultat + 1; }
                    if (i == 1) { resultat = resultat + 2; }
                    if (i == 2) { resultat = resultat + 4; }
                    if (i == 3) { resultat = resultat + 8; }
                    if (i == 4) { resultat = resultat + 16; }
                }

            }

            return resultat;
        }

        // Mise à jour de la matrice de Led
        private async void UpdateLedMatrix()
        {

            byte[] o = new byte[5];

            bool[] o0Tab = new bool[] { o0_b0, o0_b1, o0_b2, o0_b3, o0_b4, false, false, false };
            bool[] o1Tab = new bool[] { o1_b0, o1_b1, o1_b2, o1_b3, o1_b4, false, false, false };
            bool[] o2Tab = new bool[] { o2_b0, o2_b1, o2_b2, o2_b3, o2_b4, false, false, false };
            bool[] o3Tab = new bool[] { o3_b0, o3_b1, o3_b2, o3_b3, o3_b4, false, false, false };
            bool[] o4Tab = new bool[] { o4_b0, o4_b1, o4_b2, o4_b3, o4_b4, false, false, false };

            int i0 = ConvertBoolTabToInt(o0Tab);
            int i1 = ConvertBoolTabToInt(o1Tab);
            int i2 = ConvertBoolTabToInt(o2Tab);
            int i3 = ConvertBoolTabToInt(o3Tab);
            int i4 = ConvertBoolTabToInt(o4Tab);

            o[0] = (byte) i0;
            o[1] = (byte) i1;
            o[2] = (byte) i2;
            o[3] = (byte) i3;
            o[4] = (byte) i4;

            try
            {

                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicLedMatrix.WriteValueAsync(o.AsBuffer());

                if (gattCommunicationStatus.Equals(GattCommunicationStatus.Success))
                {
                    rootPage.NotifyUser("Successfully wrote Led Matrix to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write Led Matrix to device failed", NotifyType.ErrorMessage);
                }

            }
            catch (Exception exception) when (exception.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception) when (exception.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception) when (exception.HResult == E_ACCESSDENIED)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception) when (exception.HResult == E_DEVICE_NOT_AVAILABLE)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }

        }


        // Octet 0
        private async void Button_o0_b4_Click(object sender, RoutedEventArgs e)
        {

            if (o0_b4)
            {
                o0_b4 = false;
                Button_o0_b4.Background =  new SolidColorBrush( Colors.Gray );
            }
            else
            {
                o0_b4 = true;
                Button_o0_b4.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o0_b3_Click(object sender, RoutedEventArgs e)
        {

            if (o0_b3)
            {
                o0_b3 = false;
                Button_o0_b3.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o0_b3 = true;
                Button_o0_b3.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o0_b2_Click(object sender, RoutedEventArgs e)
        {

            if (o0_b2)
            {
                o0_b2 = false;
                Button_o0_b2.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o0_b2 = true;
                Button_o0_b2.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o0_b1_Click(object sender, RoutedEventArgs e)
        {

            if (o0_b1)
            {
                o0_b1 = false;
                Button_o0_b1.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o0_b1 = true;
                Button_o0_b1.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o0_b0_Click(object sender, RoutedEventArgs e)
        {

            if (o0_b0)
            {
                o0_b0 = false;
                Button_o0_b0.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o0_b0 = true;
                Button_o0_b0.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        // Octet 1
        private async void Button_o1_b4_Click(object sender, RoutedEventArgs e)
        {

            if (o1_b4)
            {
                o1_b4 = false;
                Button_o1_b4.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o1_b4 = true;
                Button_o1_b4.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o1_b3_Click(object sender, RoutedEventArgs e)
        {

            if (o1_b3)
            {
                o1_b3 = false;
                Button_o1_b3.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o1_b3 = true;
                Button_o1_b3.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o1_b2_Click(object sender, RoutedEventArgs e)
        {

            if (o1_b2)
            {
                o1_b2 = false;
                Button_o1_b2.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o1_b2 = true;
                Button_o1_b2.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o1_b1_Click(object sender, RoutedEventArgs e)
        {

            if (o1_b1)
            {
                o1_b1 = false;
                Button_o1_b1.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o1_b1 = true;
                Button_o1_b1.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o1_b0_Click(object sender, RoutedEventArgs e)
        {

            if (o1_b0)
            {
                o1_b0 = false;
                Button_o1_b0.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o1_b0 = true;
                Button_o1_b0.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        // Octet 2
        private async void Button_o2_b4_Click(object sender, RoutedEventArgs e)
        {

            if (o2_b4)
            {
                o2_b4 = false;
                Button_o2_b4.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o2_b4 = true;
                Button_o2_b4.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o2_b3_Click(object sender, RoutedEventArgs e)
        {

            if (o2_b3)
            {
                o2_b3 = false;
                Button_o2_b3.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o2_b3 = true;
                Button_o2_b3.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o2_b2_Click(object sender, RoutedEventArgs e)
        {

            if (o2_b2)
            {
                o2_b2 = false;
                Button_o2_b2.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o2_b2 = true;
                Button_o2_b2.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o2_b1_Click(object sender, RoutedEventArgs e)
        {

            if (o2_b1)
            {
                o2_b1 = false;
                Button_o2_b1.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o2_b1 = true;
                Button_o2_b1.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o2_b0_Click(object sender, RoutedEventArgs e)
        {

            if (o2_b0)
            {
                o2_b0 = false;
                Button_o2_b0.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o2_b0 = true;
                Button_o2_b0.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        // Octet 3
        private async void Button_o3_b4_Click(object sender, RoutedEventArgs e)
        {

            if (o3_b4)
            {
                o3_b4 = false;
                Button_o3_b4.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o3_b4 = true;
                Button_o3_b4.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o3_b3_Click(object sender, RoutedEventArgs e)
        {

            if (o3_b3)
            {
                o3_b3 = false;
                Button_o3_b3.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o3_b3 = true;
                Button_o3_b3.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o3_b2_Click(object sender, RoutedEventArgs e)
        {

            if (o3_b2)
            {
                o3_b2 = false;
                Button_o3_b2.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o3_b2 = true;
                Button_o3_b2.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o3_b1_Click(object sender, RoutedEventArgs e)
        {

            if (o3_b1)
            {
                o3_b1 = false;
                Button_o3_b1.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o3_b1 = true;
                Button_o3_b1.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o3_b0_Click(object sender, RoutedEventArgs e)
        {

            if (o3_b0)
            {
                o3_b0 = false;
                Button_o3_b0.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o3_b0 = true;
                Button_o3_b0.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        // Octet 4
        private async void Button_o4_b4_Click(object sender, RoutedEventArgs e)
        {

            if (o4_b4)
            {
                o4_b4 = false;
                Button_o4_b4.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o4_b4 = true;
                Button_o4_b4.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o4_b3_Click(object sender, RoutedEventArgs e)
        {

            if (o4_b3)
            {
                o4_b3 = false;
                Button_o4_b3.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o4_b3 = true;
                Button_o4_b3.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o4_b2_Click(object sender, RoutedEventArgs e)
        {

            if (o4_b2)
            {
                o4_b2 = false;
                Button_o4_b2.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o4_b2 = true;
                Button_o4_b2.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o4_b1_Click(object sender, RoutedEventArgs e)
        {

            if (o4_b1)
            {
                o4_b1 = false;
                Button_o4_b1.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o4_b1 = true;
                Button_o4_b1.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }

        private async void Button_o4_b0_Click(object sender, RoutedEventArgs e)
        {

            if (o4_b0)
            {
                o4_b0 = false;
                Button_o4_b0.Background = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                o4_b0 = true;
                Button_o4_b0.Background = new SolidColorBrush(Colors.Red);
            }

            this.UpdateLedMatrix();

        }


        private async void ButtonLedText_Click(object sender, RoutedEventArgs e)
        {

            string LedText = "Bonjour " + ComboBoxLedText.SelectedItem;

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(LedText, BinaryStringEncoding.Utf8);

            try
            {

                // BT_Code: Writes the value from the buffer to the characteristic.
                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicLedText.WriteValueAsync(buffer);

                if (gattCommunicationStatus.Equals(GattCommunicationStatus.Success))
                {
                    rootPage.NotifyUser("Successfully wrote Led Text to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write Led Text to device failed", NotifyType.ErrorMessage);
                }

            }
            catch (Exception exception) when (exception.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception) when (exception.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception) when (exception.HResult == E_ACCESSDENIED)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception) when (exception.HResult == E_DEVICE_NOT_AVAILABLE)
            {

                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }

        }

    }

}
