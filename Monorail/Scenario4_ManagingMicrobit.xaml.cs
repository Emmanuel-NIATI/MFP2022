﻿using System;
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
        public void ManageMicrobit()
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

                foreach (GattDeviceService gattDeviceService in listGattDeviceService)
                {

                    if (gattDeviceService.Uuid.ToString().Equals(SelectedServiceLedUUID))
                    {

                        selectedServiceLed = this.rootPage.BluetoothLEDevice.GetGattService(gattDeviceService.Uuid);

                        IReadOnlyList<GattCharacteristic> listGattCharacteristic = selectedServiceLed.GetAllCharacteristics();

                        foreach (GattCharacteristic gattCharacteristic in listGattCharacteristic)
                        {

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicLedMatrixUUID))
                            {

                                Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> selectedCharacteristicLedMatrix OK !");

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicLedMatrix = selectedServiceLed.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicLedMatrix = listGattCharacteristicLedMatrix[0];


                                GattCharacteristicProperties properties = selectedCharacteristicLedMatrix.CharacteristicProperties;

                                IAsyncOperation<GattCommunicationStatus> gattCommunicationStatus;


                                if (properties.HasFlag(GattCharacteristicProperties.Read))
                                {

                                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> selectedCharacteristicLedMatrix Read OK !");

                                }

                                if (properties.HasFlag(GattCharacteristicProperties.Indicate))
                                {

                                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> selectedCharacteristicLedMatrix Indicate OK !");

                                    gattCommunicationStatus = selectedCharacteristicLedMatrix.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);

                                }

                                if (properties.HasFlag(GattCharacteristicProperties.Notify))
                                {

                                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> selectedCharacteristicLedMatrix Notify OK !");

                                    gattCommunicationStatus = selectedCharacteristicLedMatrix.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                }

                            }

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicLedTextUUID))
                            {

                                Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> selectedCharacteristicLedText OK !");

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicLedText = selectedServiceLed.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicLedText = listGattCharacteristicLedText[0];

                            }

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicLedScrollingDelayUUID))
                            {

                                Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> selectedCharacteristicLedScrollingDelay OK !");

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicLedScrollingDelay = selectedServiceLed.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicLedScrollingDelay = listGattCharacteristicLedScrollingDelay[0];

                                GattCharacteristicProperties properties = selectedCharacteristicLedScrollingDelay.CharacteristicProperties;

                            }

                        }

                    }

                }

            }

        }


        // Octet 0
        private async void Button_o0_b4_Click(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine("Led Matrix : donnée lues !!!");

            IAsyncOperation<GattReadResult> gattReadResult = selectedCharacteristicLedMatrix.ReadValueAsync();

            IBuffer buffer = gattReadResult.GetResults().Value;

            for (int i = 0; i < buffer.Length; i++)
            {

                Debug.WriteLine("" + i);

                byte b = buffer.ToArray()[i];

                char c = Convert.ToChar(b);

                Debug.WriteLine(c);

            }

        }

        private async void Button_o0_b3_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o0_b2_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o0_b1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o0_b0_Click(object sender, RoutedEventArgs e)
        {

        }

        // Octet 1
        private async void Button_o1_b4_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o1_b3_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o1_b2_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o1_b1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o1_b0_Click(object sender, RoutedEventArgs e)
        {

        }

        // Octet 2
        private async void Button_o2_b4_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o2_b3_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o2_b2_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o2_b1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o2_b0_Click(object sender, RoutedEventArgs e)
        {

        }

        // Octet 3
        private async void Button_o3_b4_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o3_b3_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o3_b2_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o3_b1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o3_b0_Click(object sender, RoutedEventArgs e)
        {

        }

        // Octet 4
        private async void Button_o4_b4_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o4_b3_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o4_b2_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o4_b1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_o4_b0_Click(object sender, RoutedEventArgs e)
        {

        }




        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            string message = "Bonjour " + ComboBox.SelectedItem;

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary( message, BinaryStringEncoding.Utf8);

            try
            {

                // BT_Code: Writes the value from the buffer to the characteristic.
                IAsyncOperation<GattCommunicationStatus> gattCommunicationStatus = selectedCharacteristicLedText.WriteValueAsync(buffer);

                if (gattCommunicationStatus.Status.Equals(AsyncStatus.Started))
                {
                    rootPage.NotifyUser("Successfully wrote message to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write message to device failed", NotifyType.ErrorMessage);
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

        }

        private async void ButtonScrollingDelay_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                // BT_Code: Writes the value from the buffer to the characteristic.
                IAsyncOperation<GattReadResult> gattReadResult = selectedCharacteristicLedScrollingDelay.ReadValueAsync();

                IBuffer buffer = gattReadResult.GetResults().Value;

                Debug.WriteLine(buffer.Capacity);
                Debug.WriteLine(buffer.Length);

                for (int i = 0; i < buffer.Length; i++)
                {

                    Debug.WriteLine("" + i);

                    byte b = buffer.ToArray()[i];

                    Debug.WriteLine("" + b);

                }

                if (gattReadResult.Status.Equals(AsyncStatus.Completed))
                {
                    rootPage.NotifyUser("Successfully read message to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write message to device failed", NotifyType.ErrorMessage);
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

        }







    }

}
