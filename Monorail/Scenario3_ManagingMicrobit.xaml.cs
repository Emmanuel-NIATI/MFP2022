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

namespace Monorail
{

    public sealed partial class Scenario3_ManagingMicrobit : Page
    {

        // Zone commune
        private MainPage rootPage;

        // Zone Microbit
        String LocalSettingName;
        String LocalSettingAddress;
        String LocalSettingColor;

        // BLOCKY TALKY
        private string SelectedServiceBTUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d6";
        private string SelectedCharacteristicTxUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d7";
        private string SelectedCharacteristicRxUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d8";

        private GattDeviceService selectedServiceBT;
        private GattCharacteristic selectedCharacteristicTx;
        private GattCharacteristic selectedCharacteristicRx;

        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df);

        public Scenario3_ManagingMicrobit()
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

                    if (gattDeviceService.Uuid.ToString().Equals(SelectedServiceBTUUID))
                    {

                        selectedServiceBT = this.rootPage.BluetoothLEDevice.GetGattService(gattDeviceService.Uuid);

                        if( selectedServiceBT != null )
                        {

                            Debug.WriteLine(">>>>>>>>>> selectedServiceBT : non Null...");

                        }

                        IReadOnlyList<GattCharacteristic> listGattCharacteristic = selectedServiceBT.GetAllCharacteristics();

                        foreach (GattCharacteristic gattCharacteristic in listGattCharacteristic)
                        {

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicTxUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicTx = selectedServiceBT.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicTx = listGattCharacteristicTx[0];

                                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicTx.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);

                                selectedCharacteristicTx.ValueChanged += SelectedTxCharacteristic_ValueChanged;

                            }

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicRxUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicRx = selectedServiceBT.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicRx = listGattCharacteristicRx[0];

                            }

                        }

                    }

                }

            }

        }

        private void SelectedTxCharacteristic_ValueChanged(GattCharacteristic characteristic, GattValueChangedEventArgs e)
        {

            Debug.WriteLine(">>>>>>>>>> Blocky Talky : donnée reçue !!!");

            var dataReader = DataReader.FromBuffer(e.CharacteristicValue);
            String information = dataReader.ReadString(e.CharacteristicValue.Length);

            Debug.WriteLine(">>>>>>>>>> Blocky Talky : information : " + information);

        }

        private async void ButtonA_Click(object sender, RoutedEventArgs e)
        {

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary("S^key^A#", BinaryStringEncoding.Utf8);

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

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary("S^key^B#", BinaryStringEncoding.Utf8);

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

        private async void ButtonAB_Click(object sender, RoutedEventArgs e)
        {

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary("S^key^AB#", BinaryStringEncoding.Utf8);

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

    }

}
