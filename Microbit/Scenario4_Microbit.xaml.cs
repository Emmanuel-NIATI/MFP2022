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

namespace Microbit
{

    public sealed partial class Scenario4_Microbit : Page
    {

        /*

            BBC micro:bit [vuzov]
            243311091039748
            
            BBC micro:bit [pogat]
            235861686816099

            UART_SERVICE = "6e400001-b5a3-f393-e0a9-e50e24dcca9e"
            TX_CHARACTERISTIC = "6e400002-b5a3-f393-e0a9-e50e24dcca9e"
            RX_CHARACTERISTIC = "6e400003-b5a3-f393-e0a9-e50e24dcca9e"

            BLOCKY_TALKY_SERVICE = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d6"
            TX_CHARACTERISTIC = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d7"
            RX_CHARACTERISTIC = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d8"

            LED_SERVICE = "E95DD91D251D470AA062FA1922DFA9A8";
            LED_MATRIX = "E95D7B77251D470AA062FA1922DFA9A8";
            LED_TEXT = "E95D93EE251D470AA062FA1922DFA9A8";

        */

        private MainPage rootPage;

        private ulong SelectedBleDeviceAddr;
        private string SelectedBleDeviceName = "No device selected";

        private string SelectedServiceUUID;
        private string SelectedTxCharacteristicUUID;
        private string SelectedRxCharacteristicUUID;

        private IReadOnlyList<GattDeviceService> listGattDeviceService;
        private IReadOnlyList<GattCharacteristic> listGattCharacteristic;


        private BluetoothLEDevice bluetoothLeDevice = null;

        private GattDeviceService selectedService;
        private GattCharacteristic selectedTxCharacteristic;
        private GattCharacteristic selectedRxCharacteristic;

        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df);

        public Scenario4_Microbit()
        {
            this.InitializeComponent();
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

            rootPage.NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

            rootPage.NotifyUser("Navigating away. Watcher stopped.", NotifyType.StatusMessage);

        }


        protected void App_Suspending(object sender, object e)
        {

            bluetoothLeDevice = null;
            selectedService = null;
            selectedRxCharacteristic = null;
            selectedTxCharacteristic = null;

            rootPage.NotifyUser("App suspending. Watcher stopped.", NotifyType.StatusMessage);

        }

        private void App_Resuming(object sender, object e)
        {

            rootPage.NotifyUser("Press Run to start watcher.", NotifyType.StatusMessage);

        }

        private async void ConnectMicrobit_Click(object sender, RoutedEventArgs e)
        {

            if (MyComboBox.SelectedItem.Equals("Domicile"))
            {

                SelectedBleDeviceAddr = 243311091039748;
                SelectedBleDeviceName = "BBC micro:bit[vuzov]";

                // UART
                SelectedServiceUUID = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
                SelectedTxCharacteristicUUID = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
                SelectedRxCharacteristicUUID = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";

            }
            else if (MyComboBox.SelectedItem.Equals("Local FO"))
            {

                SelectedBleDeviceAddr = 235861686816099;
                SelectedBleDeviceName = "BBC micro:bit[pogat]";

                // UART
                SelectedServiceUUID = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
                SelectedTxCharacteristicUUID = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
                SelectedRxCharacteristicUUID = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";

            }

            try
            {

                bluetoothLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(SelectedBleDeviceAddr);

                if (bluetoothLeDevice == null)
                {

                    rootPage.NotifyUser("Failed to connect to device.", NotifyType.ErrorMessage);

                }

            }
            catch (Exception ex) when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
            {

                rootPage.NotifyUser("Bluetooth radio is not on.", NotifyType.ErrorMessage);

            }

            if (bluetoothLeDevice != null)
            {

                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                if (result.Status == GattCommunicationStatus.Success)
                {

                    var services = result.Services;

                    rootPage.NotifyUser(String.Format("Found {0} services", services.Count), NotifyType.StatusMessage);

                    if (services.Count > 0)
                    {

                        foreach (var service in services)
                        {

                            if (service.Uuid.ToString().Equals(SelectedServiceUUID))
                            {

                                Debug.WriteLine("Service UART : " + service.Uuid.ToString());

                                var gattServiceResult = await bluetoothLeDevice.GetGattServicesForUuidAsync(service.Uuid);

                                selectedService = gattServiceResult.Services[0];

                                IReadOnlyList<GattCharacteristic> characteristics = null;

                                try
                                {
                                    // Ensure we have access to the device.
                                    var accessStatus = await selectedService.RequestAccessAsync();
                                    if (accessStatus == DeviceAccessStatus.Allowed)
                                    {
                                        // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                                        // and the new Async functions to get the characteristics of unpaired devices as well. 
                                        var resultCharacteristic = await selectedService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                                        if (resultCharacteristic.Status == GattCommunicationStatus.Success)
                                        {
                                            characteristics = resultCharacteristic.Characteristics;
                                        }
                                        else
                                        {
                                            rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                                            // On error, act as if there are no characteristics.
                                            characteristics = new List<GattCharacteristic>();
                                        }
                                    }
                                    else
                                    {
                                        // Not granted access
                                        rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                                        // On error, act as if there are no characteristics.
                                        characteristics = new List<GattCharacteristic>();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    rootPage.NotifyUser("Restricted service. Can't read characteristics: " + ex.Message, NotifyType.ErrorMessage);
                                    // On error, act as if there are no characteristics.
                                    characteristics = new List<GattCharacteristic>();
                                }

                                foreach (GattCharacteristic characteristic in characteristics)
                                {

                                    if (characteristic.Uuid.ToString().Equals(SelectedTxCharacteristicUUID))
                                    {

                                        Debug.WriteLine("Tx UART : " + characteristic.Uuid.ToString());

                                        var gattCharacteristicsResult = await selectedService.GetCharacteristicsForUuidAsync(characteristic.Uuid);
                                        selectedTxCharacteristic = gattCharacteristicsResult.Characteristics[0];

                                    }

                                    if (characteristic.Uuid.ToString().Equals(SelectedRxCharacteristicUUID))
                                    {

                                        Debug.WriteLine("Rx UART : " + characteristic.Uuid.ToString());

                                        var gattCharacteristicsResult = await selectedService.GetCharacteristicsForUuidAsync(characteristic.Uuid);
                                        selectedRxCharacteristic = gattCharacteristicsResult.Characteristics[0];

                                        selectedRxCharacteristic.ValueChanged += RxCharacteristic_ValueChanged;

                                    }

                                }

                            }

                        }

                    }

                }
                else
                {
                    rootPage.NotifyUser("Device unreachable", NotifyType.ErrorMessage);
                }

            }

        }

        void RxCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {

            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] input = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(input);

            for (int i = 0; i < input.Length; i++)
            {
                Debug.WriteLine(input[i]);
            }


        }

        private async void AButton_Click(object sender, RoutedEventArgs e)
        {

            var buffer = CryptographicBuffer.ConvertStringToBinary("S^ordre^A#", BinaryStringEncoding.Utf8);

            try
            {

                // BT_Code: Writes the value from the buffer to the characteristic.
                var result = await selectedTxCharacteristic.WriteValueWithResultAsync(buffer);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    rootPage.NotifyUser("Successfully wrote key=ordre et value=A to device", NotifyType.StatusMessage);

                }
                else
                {
                    rootPage.NotifyUser($"Write failed: {result.Status} {result.ProtocolError}", NotifyType.ErrorMessage);
                }

            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED || ex.HResult == E_ACCESSDENIED)
            {
                // This usually happens when a device reports that it support writing, but it actually doesn't.
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }

        }

        private async void BButton_Click(object sender, RoutedEventArgs e)
        {

            var buffer = CryptographicBuffer.ConvertStringToBinary("B", BinaryStringEncoding.Utf8);

            try
            {
                // BT_Code: Writes the value from the buffer to the characteristic.
                var result = await selectedTxCharacteristic.WriteValueWithResultAsync(buffer);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    rootPage.NotifyUser("Successfully wrote value B to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser($"Write failed: {result.Status}", NotifyType.ErrorMessage);
                }
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED || ex.HResult == E_ACCESSDENIED)
            {
                // This usually happens when a device reports that it support writing, but it actually doesn't.
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }

        }

        private async void ABButton_Click(object sender, RoutedEventArgs e)
        {

            var buffer = CryptographicBuffer.ConvertStringToBinary("AB", BinaryStringEncoding.Utf8);

            try
            {
                // BT_Code: Writes the value from the buffer to the characteristic.
                var result = await selectedTxCharacteristic.WriteValueWithResultAsync(buffer);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    rootPage.NotifyUser("Successfully wrote value A+B to device", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser($"Write failed: {result.Status}", NotifyType.ErrorMessage);
                }
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED || ex.HResult == E_ACCESSDENIED)
            {
                // This usually happens when a device reports that it support writing, but it actually doesn't.
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }

        }

    }

}
