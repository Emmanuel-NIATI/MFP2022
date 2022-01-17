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




// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Microbit
{

    public sealed partial class Scenario3_Microbit : Page
    {

        /*

            BBC micro:bit [vuzov]
            243311091039748
            
            BBC micro:bit [pogat]
            235861686816099

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

        private ObservableCollection<BluetoothLEAttributeDisplay> ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
        private ObservableCollection<BluetoothLEAttributeDisplay> CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();

        private BluetoothLEDevice bluetoothLeDevice = null;
        private GattDeviceService selectedService;
        private GattCharacteristic selectedTxCharacteristic;
        private GattCharacteristic selectedRxCharacteristic;
        
        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df);

        public Scenario3_Microbit()
        {

            this.InitializeComponent();

            rootPage = MainPage.Current;

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

            if(MyComboBox.SelectedItem.Equals("Domicile"))
            {

                SelectedBleDeviceAddr = 243311091039748;
                SelectedBleDeviceName = "BBC micro:bit[vuzov]";

                // BLOCKY_TALKY
                SelectedServiceUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d6";
                SelectedTxCharacteristicUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d7";
                SelectedRxCharacteristicUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d8";

            }
            else if (MyComboBox.SelectedItem.Equals("Local FO"))
            {

                SelectedBleDeviceAddr = 235861686816099;
                SelectedBleDeviceName = "BBC micro:bit[pogat]";

                // BLOCKY_TALKY
                SelectedServiceUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d6";
                SelectedTxCharacteristicUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d7";
                SelectedRxCharacteristicUUID = "0b78ac2d-fe36-43ac-32d0-a29d8fbe05d8";

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

                        ServiceList.Visibility = Visibility.Visible;

                        foreach (var service in services)
                        {
                            ServiceCollection.Add(new BluetoothLEAttributeDisplay(service));

                            if( service.Uuid.ToString().Equals( SelectedServiceUUID ) )
                            {

                                Debug.WriteLine("Service BLOCKY_TALKY : " + service.Uuid.ToString());

                                selectedService = service;
                                
                                IReadOnlyList<GattCharacteristic> characteristics = null;
                                                                                                                                                                                                                                                                                   
                                try
                                {
                                    // Ensure we have access to the device.
                                    var accessStatus = await service.RequestAccessAsync();
                                    if (accessStatus == DeviceAccessStatus.Allowed)
                                    {
                                        // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                                        // and the new Async functions to get the characteristics of unpaired devices as well. 
                                        var resultCharacteristic = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
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

                                foreach (GattCharacteristic c in characteristics)
                                {
                                    CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));

                                    if (c.Uuid.ToString().Equals(SelectedTxCharacteristicUUID))
                                    {

                                        Debug.WriteLine("Tx Characteristic BLOCKY_TALKY : " + c.Uuid.ToString());

                                        selectedTxCharacteristic = c;

                                        GattDescriptorsResult gattDescriptorsResult = await c.GetDescriptorsAsync();

                                        IReadOnlyList<GattDescriptor> descriptors = gattDescriptorsResult.Descriptors;

                                        foreach(GattDescriptor gattDescriptor in descriptors)
                                        {
                                            Debug.WriteLine("Description : " + gattDescriptor.ToString() );
                                        }


                                        IReadOnlyList<GattPresentationFormat> PresentationFormats = c.PresentationFormats;

                                        foreach (GattPresentationFormat g in PresentationFormats)
                                        {

                                            Debug.WriteLine("Description : " + g.Description);

                                        }

                                    }

                                    if (c.Uuid.ToString().Equals(SelectedRxCharacteristicUUID))
                                    {

                                        Debug.WriteLine("Rx Characteristic BLOCKY_TALKY : " + c.Uuid.ToString());

                                        selectedRxCharacteristic = c;

                                        IReadOnlyList<GattPresentationFormat> PresentationFormats = c.PresentationFormats;

                                        foreach (GattPresentationFormat g in PresentationFormats)
                                        {

                                            Debug.WriteLine("Description : " + g.Description);

                                        }

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

        private async void ServiceList_SelectionChanged()
        {

            var attributeInfoDisp = (BluetoothLEAttributeDisplay)ServiceList.SelectedItem;

            CharacteristicCollection.Clear();

            IReadOnlyList<GattCharacteristic> characteristics = null;

            try
            {
                // Ensure we have access to the device.
                var accessStatus = await attributeInfoDisp.service.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {
                    // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                    // and the new Async functions to get the characteristics of unpaired devices as well. 
                    var result = await attributeInfoDisp.service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        characteristics = result.Characteristics;
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
                rootPage.NotifyUser("Restricted service. Can't read characteristics: " + ex.Message,
                    NotifyType.ErrorMessage);
                // On error, act as if there are no characteristics.
                characteristics = new List<GattCharacteristic>();
            }

            foreach (GattCharacteristic c in characteristics)
            {
                CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));

            }
            CharacteristicList.Visibility = Visibility.Visible;
        }

        private async void CharacteristicList_SelectionChanged()
        {

            var attributeInfoDisp = (BluetoothLEAttributeDisplay)CharacteristicList.SelectedItem;

        }

        private async void AButton_Click(object sender, RoutedEventArgs e)
        {

            var buffer = CryptographicBuffer.ConvertStringToBinary("ordre 2 A " , BinaryStringEncoding.Utf8);

            try
            {

                await selectedTxCharacteristic.WriteValueAsync(buffer);

                /*
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
                */

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

        public class BluetoothLEAttributeDisplay
        {
            public GattCharacteristic characteristic;
            public GattDescriptor descriptor;

            public GattDeviceService service;

            public BluetoothLEAttributeDisplay(GattDeviceService service)
            {
                this.service = service;
                AttributeDisplayType = AttributeType.Service;
            }

            public BluetoothLEAttributeDisplay(GattCharacteristic characteristic)
            {
                this.characteristic = characteristic;
                AttributeDisplayType = AttributeType.Characteristic;
            }

            public string Name
            {
                get
                {
                    switch (AttributeDisplayType)
                    {
                        case AttributeType.Service:
                            if (IsSigDefinedUuid(service.Uuid))
                            {
                                GattNativeServiceUuid serviceName;
                                if (Enum.TryParse(Utilities.ConvertUuidToShortId(service.Uuid).ToString(), out serviceName))
                                {
                                    return serviceName.ToString();
                                }
                            }
                            else
                            {
                                return "Custom Service: " + service.Uuid;
                            }
                            break;
                        case AttributeType.Characteristic:
                            if (IsSigDefinedUuid(characteristic.Uuid))
                            {
                                GattNativeCharacteristicUuid characteristicName;
                                if (Enum.TryParse(Utilities.ConvertUuidToShortId(characteristic.Uuid).ToString(),
                                    out characteristicName))
                                {
                                    return characteristicName.ToString();
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(characteristic.UserDescription))
                                {
                                    return characteristic.UserDescription;
                                }

                                else
                                {
                                    return "Custom Characteristic: " + characteristic.Uuid;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    return "Invalid";
                }
            }

            public AttributeType AttributeDisplayType { get; }

            /// <summary>
            ///     The SIG has a standard base value for Assigned UUIDs. In order to determine if a UUID is SIG defined,
            ///     zero out the unique section and compare the base sections.
            /// </summary>
            /// <param name="uuid">The UUID to determine if SIG assigned</param>
            /// <returns></returns>
            private static bool IsSigDefinedUuid(Guid uuid)
            {
                var bluetoothBaseUuid = new Guid("00000000-0000-1000-8000-00805F9B34FB");

                var bytes = uuid.ToByteArray();
                // Zero out the first and second bytes
                // Note how each byte gets flipped in a section - 1234 becomes 34 12
                // Example Guid: 35918bc9-1234-40ea-9779-889d79b753f0
                //                   ^^^^
                // bytes output = C9 8B 91 35 34 12 EA 40 97 79 88 9D 79 B7 53 F0
                //                ^^ ^^
                bytes[0] = 0;
                bytes[1] = 0;
                var baseUuid = new Guid(bytes);
                return baseUuid == bluetoothBaseUuid;
            }
        }

        public enum AttributeType
        {
            Service = 0,
            Characteristic = 1,
            Descriptor = 2
        }

        /// <summary>
        ///     This enum assists in finding a string representation of a BT SIG assigned value for Service UUIDS
        ///     Reference: https://developer.bluetooth.org/gatt/services/Pages/ServicesHome.aspx
        /// </summary>
        public enum GattNativeServiceUuid : ushort
        {
            None = 0,
            AlertNotification = 0x1811,
            Battery = 0x180F,
            BloodPressure = 0x1810,
            CurrentTimeService = 0x1805,
            CyclingSpeedandCadence = 0x1816,
            DeviceInformation = 0x180A,
            GenericAccess = 0x1800,
            GenericAttribute = 0x1801,
            Glucose = 0x1808,
            HealthThermometer = 0x1809,
            HeartRate = 0x180D,
            HumanInterfaceDevice = 0x1812,
            ImmediateAlert = 0x1802,
            LinkLoss = 0x1803,
            NextDSTChange = 0x1807,
            PhoneAlertStatus = 0x180E,
            ReferenceTimeUpdateService = 0x1806,
            RunningSpeedandCadence = 0x1814,
            ScanParameters = 0x1813,
            TxPower = 0x1804,
            SimpleKeyService = 0xFFE0
        }

        /// <summary>
        ///     This enum is nice for finding a string representation of a BT SIG assigned value for Characteristic UUIDs
        ///     Reference: https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicsHome.aspx
        /// </summary>
        public enum GattNativeCharacteristicUuid : ushort
        {
            None = 0,
            AlertCategoryID = 0x2A43,
            AlertCategoryIDBitMask = 0x2A42,
            AlertLevel = 0x2A06,
            AlertNotificationControlPoint = 0x2A44,
            AlertStatus = 0x2A3F,
            Appearance = 0x2A01,
            BatteryLevel = 0x2A19,
            BloodPressureFeature = 0x2A49,
            BloodPressureMeasurement = 0x2A35,
            BodySensorLocation = 0x2A38,
            BootKeyboardInputReport = 0x2A22,
            BootKeyboardOutputReport = 0x2A32,
            BootMouseInputReport = 0x2A33,
            CSCFeature = 0x2A5C,
            CSCMeasurement = 0x2A5B,
            CurrentTime = 0x2A2B,
            DateTime = 0x2A08,
            DayDateTime = 0x2A0A,
            DayofWeek = 0x2A09,
            DeviceName = 0x2A00,
            DSTOffset = 0x2A0D,
            ExactTime256 = 0x2A0C,
            FirmwareRevisionString = 0x2A26,
            GlucoseFeature = 0x2A51,
            GlucoseMeasurement = 0x2A18,
            GlucoseMeasurementContext = 0x2A34,
            HardwareRevisionString = 0x2A27,
            HeartRateControlPoint = 0x2A39,
            HeartRateMeasurement = 0x2A37,
            HIDControlPoint = 0x2A4C,
            HIDInformation = 0x2A4A,
            IEEE11073_20601RegulatoryCertificationDataList = 0x2A2A,
            IntermediateCuffPressure = 0x2A36,
            IntermediateTemperature = 0x2A1E,
            LocalTimeInformation = 0x2A0F,
            ManufacturerNameString = 0x2A29,
            MeasurementInterval = 0x2A21,
            ModelNumberString = 0x2A24,
            NewAlert = 0x2A46,
            PeripheralPreferredConnectionParameters = 0x2A04,
            PeripheralPrivacyFlag = 0x2A02,
            PnPID = 0x2A50,
            ProtocolMode = 0x2A4E,
            ReconnectionAddress = 0x2A03,
            RecordAccessControlPoint = 0x2A52,
            ReferenceTimeInformation = 0x2A14,
            Report = 0x2A4D,
            ReportMap = 0x2A4B,
            RingerControlPoint = 0x2A40,
            RingerSetting = 0x2A41,
            RSCFeature = 0x2A54,
            RSCMeasurement = 0x2A53,
            SCControlPoint = 0x2A55,
            ScanIntervalWindow = 0x2A4F,
            ScanRefresh = 0x2A31,
            SensorLocation = 0x2A5D,
            SerialNumberString = 0x2A25,
            ServiceChanged = 0x2A05,
            SoftwareRevisionString = 0x2A28,
            SupportedNewAlertCategory = 0x2A47,
            SupportedUnreadAlertCategory = 0x2A48,
            SystemID = 0x2A23,
            TemperatureMeasurement = 0x2A1C,
            TemperatureType = 0x2A1D,
            TimeAccuracy = 0x2A12,
            TimeSource = 0x2A13,
            TimeUpdateControlPoint = 0x2A16,
            TimeUpdateState = 0x2A17,
            TimewithDST = 0x2A11,
            TimeZone = 0x2A0E,
            TxPowerLevel = 0x2A07,
            UnreadAlertStatus = 0x2A45,
            AggregateInput = 0x2A5A,
            AnalogInput = 0x2A58,
            AnalogOutput = 0x2A59,
            CyclingPowerControlPoint = 0x2A66,
            CyclingPowerFeature = 0x2A65,
            CyclingPowerMeasurement = 0x2A63,
            CyclingPowerVector = 0x2A64,
            DigitalInput = 0x2A56,
            DigitalOutput = 0x2A57,
            ExactTime100 = 0x2A0B,
            LNControlPoint = 0x2A6B,
            LNFeature = 0x2A6A,
            LocationandSpeed = 0x2A67,
            Navigation = 0x2A68,
            NetworkAvailability = 0x2A3E,
            PositionQuality = 0x2A69,
            ScientificTemperatureinCelsius = 0x2A3C,
            SecondaryTimeZone = 0x2A10,
            String = 0x2A3D,
            TemperatureinCelsius = 0x2A1F,
            TemperatureinFahrenheit = 0x2A20,
            TimeBroadcast = 0x2A15,
            BatteryLevelState = 0x2A1B,
            BatteryPowerState = 0x2A1A,
            PulseOximetryContinuousMeasurement = 0x2A5F,
            PulseOximetryControlPoint = 0x2A62,
            PulseOximetryFeatures = 0x2A61,
            PulseOximetryPulsatileEvent = 0x2A60,
            SimpleKeyState = 0xFFE1
        }

        /// <summary>
        ///     This enum assists in finding a string representation of a BT SIG assigned value for Descriptor UUIDs
        ///     Reference: https://developer.bluetooth.org/gatt/descriptors/Pages/DescriptorsHomePage.aspx
        /// </summary>
        public enum GattNativeDescriptorUuid : ushort
        {
            CharacteristicExtendedProperties = 0x2900,
            CharacteristicUserDescription = 0x2901,
            ClientCharacteristicConfiguration = 0x2902,
            ServerCharacteristicConfiguration = 0x2903,
            CharacteristicPresentationFormat = 0x2904,
            CharacteristicAggregateFormat = 0x2905,
            ValidRange = 0x2906,
            ExternalReportReference = 0x2907,
            ReportReference = 0x2908
        }

        public static class Utilities
        {
            /// <summary>
            ///     Converts from standard 128bit UUID to the assigned 32bit UUIDs. Makes it easy to compare services
            ///     that devices expose to the standard list.
            /// </summary>
            /// <param name="uuid">UUID to convert to 32 bit</param>
            /// <returns></returns>
            public static ushort ConvertUuidToShortId(Guid uuid)
            {
                // Get the short Uuid
                var bytes = uuid.ToByteArray();
                var shortUuid = (ushort)(bytes[0] | (bytes[1] << 8));
                return shortUuid;
            }

            /// <summary>
            ///     Converts from a buffer to a properly sized byte array
            /// </summary>
            /// <param name="buffer"></param>
            /// <returns></returns>
            public static byte[] ReadBufferToBytes(IBuffer buffer)
            {
                var dataLength = buffer.Length;
                var data = new byte[dataLength];
                using (var reader = DataReader.FromBuffer(buffer))
                {
                    reader.ReadBytes(data);
                }
                return data;
            }
        }

    }

}
