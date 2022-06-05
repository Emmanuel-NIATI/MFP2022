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
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Gigabyte
{

    public sealed partial class Scenario2_ManagingDevice : Page
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

        public Scenario2_ManagingDevice()
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

                Debug.WriteLine(">>>>>>>>>> Scenario2_ManagingMicrobit : BluetoothLEDevice not null");

                BluetoothConnectionStatus bluetoothConnectionStatus = this.rootPage.BluetoothLEDevice.ConnectionStatus;

                if (bluetoothConnectionStatus.Equals(BluetoothConnectionStatus.Connected))
                {

                    MicrobitDeviceConnected.Text = "Connected";

                }
                else if (bluetoothConnectionStatus.Equals(BluetoothConnectionStatus.Disconnected))
                {

                    MicrobitDeviceConnected.Text = "Disconnected";
                }

                IReadOnlyList<GattDeviceService> listGattDeviceService = this.rootPage.BluetoothLEDevice.GattServices;

                Debug.WriteLine(">>>>>>>>>> listGattDeviceService : " + listGattDeviceService.Count);

                foreach (GattDeviceService gattDeviceService in listGattDeviceService)
                {

                    if (gattDeviceService.Uuid.ToString().Equals(SelectedServiceBTUUID))
                    {

                        selectedServiceBT = this.rootPage.BluetoothLEDevice.GetGattService(gattDeviceService.Uuid);

                        if (selectedServiceBT != null)
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

                                if (selectedCharacteristicTx != null)
                                {

                                    Debug.WriteLine(">>>>>>>>>> selectedCharacteristicTx : non Null...");

                                }


                                GattCharacteristicProperties gattCharacteristicProperties = selectedCharacteristicTx.CharacteristicProperties;

                                GattCommunicationStatus gattCommunicationStatus;

                                if (gattCharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify  )  )
                                {

                                    Debug.WriteLine(">>>>>>>>>> GattCharacteristicPropertie : Notify");

                                    gattCommunicationStatus = await selectedCharacteristicTx.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                                }

                                if (gattCharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                                {

                                    Debug.WriteLine(">>>>>>>>>> GattCharacteristicPropertie : Indicate");

                                    gattCommunicationStatus = await selectedCharacteristicTx.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                                }

                                selectedCharacteristicTx.ValueChanged += SelectedTxCharacteristic_ValueChanged;

                            }

                            if (gattCharacteristic.Uuid.ToString().Equals(SelectedCharacteristicRxUUID))
                            {

                                IReadOnlyList<GattCharacteristic> listGattCharacteristicRx = selectedServiceBT.GetCharacteristics(gattCharacteristic.Uuid);

                                selectedCharacteristicRx = listGattCharacteristicRx[0];

                                if (selectedCharacteristicRx != null)
                                {

                                    Debug.WriteLine(">>>>>>>>>> selectedCharacteristicRx : non Null...");

                                }

                            }

                        }

                    }

                }

            }

        }

        private void SelectedTxCharacteristic_ValueChanged(GattCharacteristic characteristic, GattValueChangedEventArgs e)
        {

            var dataReader = DataReader.FromBuffer(e.CharacteristicValue);
            byte[] input = new byte[dataReader.UnconsumedBufferLength];
            dataReader.ReadBytes(input);

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (input[8] == 1)
                {
                    ImageEtat.Source = new BitmapImage(new Uri("ms-appx:///Assets/SIG_V.png"));
                }
                else
                {
                    ImageEtat.Source = new BitmapImage(new Uri("ms-appx:///Assets/SIG_R.png"));
                }

            });

        }

        private async void ButtonA_Click(object sender, RoutedEventArgs e)
        {

            byte[] output = new byte[20];

            output[0] = 111;
            output[1] = 114;
            output[2] = 100;
            output[3] = 114;
            output[4] = 101;
            output[5] = 0;
            output[6] = 0;
            output[7] = 3;
            output[8] = 109;
            output[9] = 97;
            output[10] = 114;
            output[11] = 99;
            output[12] = 104;
            output[13] = 101;
            output[14] = 0;
            output[15] = 0;
            output[16] = 0;
            output[17] = 0;
            output[18] = 0;
            output[19] = 0;

            var dataWriter = new DataWriter();
            dataWriter.WriteBytes(output);

            IBuffer buffer = dataWriter.DetachBuffer();

            try
            {

                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicRx.WriteValueAsync(buffer);

                if (gattCommunicationStatus.Equals(GattCommunicationStatus.Success))
                {
                    rootPage.NotifyUser("Successfully wrote key=ordre and value=marche to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write key=ordre and value=marche to device failed", NotifyType.ErrorMessage);
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

            byte[] output = new byte[20];

            output[0] = 111;
            output[1] = 114;
            output[2] = 100;
            output[3] = 114;
            output[4] = 101;
            output[5] = 0;
            output[6] = 0;
            output[7] = 3;
            output[8] = 97;
            output[9] = 114;
            output[10] = 114;
            output[11] = 101;
            output[12] = 116;
            output[13] = 0;
            output[14] = 0;
            output[15] = 0;
            output[16] = 0;
            output[17] = 0;
            output[18] = 0;
            output[19] = 0;

            var dataWriter = new DataWriter();
            dataWriter.WriteBytes(output);

            IBuffer buffer = dataWriter.DetachBuffer();

            try
            {

                GattCommunicationStatus gattCommunicationStatus = await selectedCharacteristicRx.WriteValueAsync(buffer);

                if (gattCommunicationStatus.Equals(GattCommunicationStatus.Success))
                {
                    rootPage.NotifyUser("Successfully wrote key=ordre and value=arret to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Write key=ordre and value=arret to device failed", NotifyType.ErrorMessage);
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
