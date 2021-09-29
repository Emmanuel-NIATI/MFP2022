using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.Devices.Gpio;
using Windows.Graphics.Imaging;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LCDDisplayDriver
{

    public class ILI9341
    {

        // Plan de cablâge


        // Liste des variables

        public const int LCD_W = 240;
        public const int LCD_H = 320;

        private const Int32 DATA_COMMAND_PIN = 22;
        private const Int32 RESET_PIN = 23;

        private const string SPI_CONTROLLER_NAME = "SPI0";
        private const Int32 SPI_CHIP_SELECT_LINE = 0;

        private GpioPin DataCommand;
        private GpioPin Reset;

        private SpiDevice SpiDeviceCS0;

        private int resolution;

        // Liste des commandes

        private static readonly byte[] CMD_ENTER_SLEEP = { 0x10 };
        private static readonly byte[] CMD_SLEEP_OUT = { 0x11 };

        private static readonly byte[] CMD_GAMMA_SET = { 0x26 };
        private static readonly byte[] CMD_DISPLAY_OFF = { 0x28 };
        private static readonly byte[] CMD_DISPLAY_ON = { 0x29 };
        private static readonly byte[] CMD_COLUMN_ADDRESS_SET = { 0x2a };
        private static readonly byte[] CMD_PAGE_ADDRESS_SET = { 0x2b };
        private static readonly byte[] CMD_MEMORY_WRITE_MODE = { 0x2C };

        private static readonly byte[] CMD_MEMORY_ACCESS_CONTROL = { 0x36 };
        private static readonly byte[] CMD_PIXEL_FORMAT = { 0x3a };

        private static readonly byte[] CMD_FRAME_RATE_CONTROL = { 0xb1 };
        private static readonly byte[] CMD_DISPLAY_FUNCTION_CONTROL = { 0xb6 };

        private static readonly byte[] CMD_POWER_CONTROL_1 = { 0xc0 };
        private static readonly byte[] CMD_POWER_CONTROL_2 = { 0xc1 };
        private static readonly byte[] CMD_VCOM_CONTROL_1 = { 0xc5 };
        private static readonly byte[] CMD_VCOM_CONTROL_2 = { 0xc7 };
        private static readonly byte[] CMD_POWER_CONTROL_A = { 0xcb };
        private static readonly byte[] CMD_POWER_CONTROL_B = { 0xcf };

        private static readonly byte[] CMD_POSITIVE_GAMMA_CORRECTION = { 0xe0 };
        private static readonly byte[] CMD_NEGATIVE_GAMMA_CORRECTION = { 0xe1 };
        private static readonly byte[] CMD_DRIVER_TIMING_CONTROL_A = { 0xe8 };
        private static readonly byte[] CMD_DRIVER_TIMING_CONTROL_B = { 0xea };
        private static readonly byte[] CMD_POWER_ON_SEQUENCE_CONTROL = { 0xed };

        private static readonly byte[] CMD_ENABLE_3G = { 0xf2 };
        private static readonly byte[] CMD_PUMP_RATIO_CONTROL = { 0xf7 };

        // Liste des couleurs

        public static readonly ushort COLOR_BLACK = 0x0000;         //   0,   0,   0
        public static readonly ushort COLOR_NAVY = 0x000F;          //   0,   0, 123
        public static readonly ushort COLOR_DARKGREEN = 0x03E0;     //   0, 125,   0
        public static readonly ushort COLOR_DARKCYAN = 0x03EF;      //   0, 125, 123
        public static readonly ushort COLOR_MAROON = 0x7800;        // 123,   0,   0
        public static readonly ushort COLOR_PURPLE = 0x780F;        // 123,   0, 123
        public static readonly ushort COLOR_OLIVE = 0x7BE0;         // 123, 125,   0
        public static readonly ushort COLOR_LIGHTGREY = 0xC618;     // 198, 195, 198
        public static readonly ushort COLOR_DARKGREY = 0x7BEF;      // 123, 125, 123
        public static readonly ushort COLOR_BLUE = 0x001F;          //   0,   0, 255
        public static readonly ushort COLOR_GREEN = 0x07E0;         //   0, 255,   0
        public static readonly ushort COLOR_CYAN = 0x07FF;          //   0, 255, 255
        public static readonly ushort COLOR_RED = 0xF800;           // 255,   0,   0
        public static readonly ushort COLOR_MAGENTA = 0xF81F;       // 255,   0, 255
        public static readonly ushort COLOR_YELLOW = 0xFFE0;        // 255, 255,   0
        public static readonly ushort COLOR_WHITE = 0xFFFF;         // 255, 255, 255
        public static readonly ushort COLOR_ORANGE = 0xFD20;        // 255, 165,   0
        public static readonly ushort COLOR_GREENYELLOW = 0xAFE5;   // 173, 255,  41
        public static readonly ushort COLOR_PINK = 0xFC18;          // 255, 130, 198

        public static readonly ushort COLOR_PINK_PAULINE = 0xFD38;  // 255, 165, 198

        public ILI9341()
        {

        }

        private async Task Configuration()
        {

            DataCommand = GpioController.GetDefault().OpenPin(DATA_COMMAND_PIN);
            DataCommand.SetDriveMode(GpioPinDriveMode.Output);
            DataCommand.Write(GpioPinValue.High);

            Reset = GpioController.GetDefault().OpenPin(RESET_PIN);
            Reset.SetDriveMode(GpioPinDriveMode.Output);
            Reset.Write(GpioPinValue.High);

            var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
            settings.ClockFrequency = 10000000;
            settings.Mode = SpiMode.Mode3;
            string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);

            SpiDeviceCS0 = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);

        }

        private void SendCommand(byte[] Command)
        {

            DataCommand.Write(GpioPinValue.Low);
            SpiDeviceCS0.Write(Command);

        }

        private void SendData(byte[] Data)
        {

            DataCommand.Write(GpioPinValue.High);
            SpiDeviceCS0.Write(Data);

        }

        private async Task SendReset()
        {

            Reset.Write(GpioPinValue.High);
            await Task.Delay(5);
            Reset.Write(GpioPinValue.Low);
            await Task.Delay(5);
            Reset.Write(GpioPinValue.High);
            await Task.Delay(20);

        }

        public async Task SleepIn()
        {

            SendCommand(CMD_DISPLAY_OFF);

            await Task.Delay(120);

            SendCommand(CMD_ENTER_SLEEP);

        }

        public async Task SleepOut()
        {

            SendCommand(CMD_SLEEP_OUT);

            await Task.Delay(120);

            SendCommand(CMD_DISPLAY_ON);

        }

        public async Task PowerOnSequence()
        {

            await Configuration();

            await SendReset();

            SendCommand(CMD_GAMMA_SET);
            SendData(new byte[] { 0x01 });

            SendCommand(CMD_MEMORY_ACCESS_CONTROL);
            SendData(new byte[] { 0x48 });

            SendCommand(CMD_PIXEL_FORMAT);
            SendData(new byte[] { 0x55 });

            SendCommand(CMD_FRAME_RATE_CONTROL);
            SendData(new byte[] { 0x00, 0x18 });

            SendCommand(CMD_DISPLAY_FUNCTION_CONTROL);
            SendData(new byte[] { 0x08, 0x82, 0x27 });

            SendCommand(CMD_POWER_CONTROL_1);
            SendData(new byte[] { 0x23 });

            SendCommand(CMD_POWER_CONTROL_2);
            SendData(new byte[] { 0x10 });

            SendCommand(CMD_VCOM_CONTROL_1);
            SendData(new byte[] { 0x3e, 0x28 });

            SendCommand(CMD_VCOM_CONTROL_2);
            SendData(new byte[] { 0x86 });

            SendCommand(CMD_POWER_CONTROL_A);
            SendData(new byte[] { 0x39, 0x2C, 0x00, 0x34, 0x02 });

            SendCommand(CMD_POWER_CONTROL_B);
            SendData(new byte[] { 0x00, 0xC1, 0x30 });

            SendCommand(CMD_POSITIVE_GAMMA_CORRECTION);
            SendData(new byte[] { 0x0F, 0x31, 0x2B, 0x0C, 0x0E, 0x08, 0x4E, 0xF1, 0x37, 0x07, 0x10, 0x03, 0x0E, 0x09, 0x00 });

            SendCommand(CMD_NEGATIVE_GAMMA_CORRECTION);
            SendData(new byte[] { 0x00, 0x0E, 0x14, 0x03, 0x11, 0x07, 0x31, 0xC1, 0x48, 0x08, 0x0F, 0x0C, 0x31, 0x36, 0x0F });

            SendCommand(CMD_DRIVER_TIMING_CONTROL_A);
            SendData(new byte[] { 0x85, 0x00, 0x78 });

            SendCommand(CMD_DRIVER_TIMING_CONTROL_B);
            SendData(new byte[] { 0x00, 0x00 });

            SendCommand(CMD_POWER_ON_SEQUENCE_CONTROL);
            SendData(new byte[] { 0x64, 0x03, 0x12, 0x81 });

            SendCommand(CMD_ENABLE_3G);
            SendData(new byte[] { 0x00 });

            SendCommand(CMD_PUMP_RATIO_CONTROL);
            SendData(new byte[] { 0x20 });

            await SleepOut();

        }

        private void SetAddress(int x0, int y0, int x1, int y1)
        {

            SendCommand(CMD_COLUMN_ADDRESS_SET);
            SendData(new byte[] { (byte)(x0 >> 8), (byte)(x0), (byte)(x1 >> 8), (byte)(x1) });

            SendCommand(CMD_PAGE_ADDRESS_SET);
            SendData(new byte[] { (byte)(y0 >> 8), (byte)(y0), (byte)(y1 >> 8), (byte)(y1) });

            SendCommand(CMD_MEMORY_WRITE_MODE);

        }



        // Zone Couleur

        public ushort RGB888ToRGB565(byte r8, byte g8, byte b8)
        {

            ushort r5 = (ushort)((r8 * 249 + 1014) >> 11);
            ushort g6 = (ushort)((g8 * 253 + 505) >> 10);
            ushort b5 = (ushort)((b8 * 249 + 1014) >> 11);

            return (ushort)(r5 << 11 | g6 << 5 | b5);

        }

        // Zone Ecran

        private void FillRectangle(UInt16 x0, UInt16 y0, UInt16 width, UInt16 height, int color)
        {

            if ((x0 + width - 1) >= LCD_W)
            {
                width = (UInt16)(LCD_W - x0);
            }

            UInt16 x1 = (UInt16)(x0 + width - 1);

            if ((y0 + height - 1) >= LCD_H)
            {
                height = (UInt16)(LCD_H - y0);
            }

            UInt16 y1 = (UInt16)(y0 + height - 1);

            byte VH = (byte)((color >> 8) & 0xFF);
            byte VL = (byte)(color & 0xFF);

            byte[] buffer = new byte[width * height * 2];

            for (int index = 0; index < buffer.Length; index += 2)
            {

                buffer[index] = VH;
                buffer[index + 1] = VL;

            }

            SetAddress(x0, y0, x1, y1);
            SendData(buffer);

        }

        public void ColorScreen(int color)
        {

            this.FillRectangle(0, 0, (UInt16)LCD_W, (UInt16)LCD_H, color);
        }

        public void ClearScreen()
        {

            this.ColorScreen( COLOR_BLACK );
        }

    }

}
