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

namespace Bluetooth
{

    public sealed partial class Scenario2_ManagingMicrobit : Page
    {

        // Zone commune
        private MainPage rootPage;

        // Zone Microbit
        String LocalSettingName;
        String LocalSettingAddress;
        String LocalSettingColor;

        // UART
        private string SelectedServiceUARTUUID = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
        private string SelectedCharacteristicTxUUID = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
        private string SelectedCharacteristicRxUUID = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";

        private GattDeviceService selectedServiceUART;
        private GattCharacteristic selectedCharacteristicTx;
        private GattCharacteristic selectedCharacteristicRx;

        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df);

        public Scenario2_ManagingMicrobit()
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

                foreach (GattDeviceService gattDeviceService in listGattDeviceService)
                {

                    if (gattDeviceService.Uuid.ToString().Equals(SelectedServiceUARTUUID))
                    {

                        selectedServiceUART = this.rootPage.BluetoothLEDevice.GetGattService(gattDeviceService.Uuid);

                        IReadOnlyList<GattCharacteristic> listGattCharacteristic = selectedServiceUART.GetAllCharacteristics();

                        foreach (GattCharacteristic gattCharacteristic in listGattCharacteristic)
                        {

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicTxUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicTx = selectedServiceUART.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicTx = listGattCharacteristicTx[0];
                                
                                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicTx.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);

                                if( gattCommunicationStatus.Equals(GattCommunicationStatus.Success)  )
                                {

                                    selectedCharacteristicTx.ValueChanged += SelectedCharacteristicTx_ValueChanged;

                                }

                            }

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicRxUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicRx = selectedServiceUART.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicRx = listGattCharacteristicRx[0];

                            }

                        }

                    }

                }

            }

        }

        private void SelectedCharacteristicTx_ValueChanged(GattCharacteristic characteristic, GattValueChangedEventArgs e)
        {

            Debug.WriteLine(">>>>>>>>>> UART : donnée reçue !!!");

            var dataReader = DataReader.FromBuffer(e.CharacteristicValue);
            String information = dataReader.ReadString(e.CharacteristicValue.Length);

            Debug.WriteLine(">>>>>>>>>> UART : information : " + information);

        }

        private async void ButtonA_Click(object sender, RoutedEventArgs e)
        {

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary("A#", BinaryStringEncoding.Utf8);

            try
            {

                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicRx.WriteValueAsync(buffer);

                if (gattCommunicationStatus.Equals(GattCommunicationStatus.Success)) 
                {
                    rootPage.NotifyUser("Successfully wrote value A to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write value A to device failed", NotifyType.ErrorMessage);
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

        private async void ButtonB_Click(object sender, RoutedEventArgs e)
        {

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary("B#", BinaryStringEncoding.Utf8);

            try
            {
                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicRx.WriteValueAsync(buffer);

                if (gattCommunicationStatus.Equals(GattCommunicationStatus.Success))
                {
                    rootPage.NotifyUser("Successfully wrote value B to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write value B to device failed", NotifyType.ErrorMessage);
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

        private async void ButtonAB_Click(object sender, RoutedEventArgs e)
        {

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary("AB#", BinaryStringEncoding.Utf8);

            try
            {
                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicRx.WriteValueAsync(buffer);

                if (gattCommunicationStatus.Equals(GattCommunicationStatus.Success))
                {
                    rootPage.NotifyUser("Successfully wrote value A + B to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write value A + B to device failed", NotifyType.ErrorMessage);
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
