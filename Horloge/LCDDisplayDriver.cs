using DisplayFont;
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
using Windows.Storage.FileProperties;
using System.Diagnostics;

namespace LCDDisplayDriver
{

    public class ILI9341
    {

        // Plan de cablâge

        // LCD ILI9341		RPI0W	-	RPI2b	-	RPI3b
        //					pi4j					python / C#

        // 1) +Vcc			 1) +3.3V				 1) +3.3V 					Fil rouge
        // 2) GND			20) GND					20) GND						Fil noir
        // 3) CS			24) CE0 (SPI) GPIO 10	24) CE0 (SPI) GPIO 8		Fil jaune
        // 4) RESET			16) GPIO 4				16) GPIO 23					Fil violet
        // 5) DC/RS			15) GPIO 3				15) GPIO 22					Fil vert
        // 6) MOSI			19) MOSI (SPI) GPIO 12	19) MOSI (SPI) GPIO 10		Fil bleu
        // 7) SCK			23) SCLK (SPI) GPIO 14	23) SCLK (SPI) GPIO 11		Fil blanc
        // 8) LED			17) +3.3V				17) +3.3V					Fil rouge
        // 9) MISO			21) MISO (SPI) GPIO 13	21) MISO (SPI) GPIO 9		Fil orange


        // Mémo :

        // sbyte	Signed 1 byte       Stores whole numbers from -128 to 127
        // byte     Unsigned 1 byte     Stores whole numbers from  0 to 255

        // short	Signed 2 bytes      Stores whole numbers from -32,768 to 32,767
        // ushort	Unsigned 2 bytes    Stores whole numbers from 0 to 65,535

        // int      Signed 4 bytes      Stores whole numbers from -2,147,483,648 to 2,147,483,647
        // uint     Unsigned 4 bytes    Stores whole numbers from 0 to 4,294,967,295

        // long     Signed 8 bytes      Stores whole numbers from -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807
        // ulong    Signed 8 bytes      Stores whole numbers from 0 to 18,446,744,073,709,551,615


        // Liste des variables

        public const int LCD_W = 240;
        public const int LCD_H = 320;

        private const byte DATA_COMMAND_PIN = 22;
        private const byte RESET_PIN = 23;

        private const string SPI_CONTROLLER_NAME = "SPI0";
        private const byte SPI_CHIP_SELECT_LINE = 0;

        private GpioPin DataCommand;
        private GpioPin Reset;

        private SpiDevice SpiDeviceCS0;

        private int resolution;

        private int cursorX;
        private int cursorY;


        // Liste des résolutions

        private static readonly byte HEIGHT_04 = 4;
        private static readonly byte HEIGHT_08 = 8;
        private static readonly byte HEIGHT_10 = 10;
        private static readonly byte HEIGHT_16 = 16;
        private static readonly byte HEIGHT_32 = 32;


        // Liste des commandes

        private static readonly byte[] CMD_NO_OPERATION = { 0x00 };
        private static readonly byte[] CMD_SOFTWARE_RESET = { 0x01 };
        private static readonly byte[] CMD_READ_DISPLAY_IDENTIFICATION_INFORMATION = { 0x04 };
        private static readonly byte[] CMD_READ_DISPLAY_STATUS = { 0x09 };
        private static readonly byte[] CMD_READ_DISPLAY_POWER_MODE = { 0x0A };
        private static readonly byte[] CMD_READ_DISPLAY_MADCTL = { 0x0B };
        private static readonly byte[] CMD_READ_DISPLAY_PIXEL_FORMAT = { 0x0C };
        private static readonly byte[] CMD_READ_DISPLAY_IMAGE_FORMAT = { 0x0D };
        private static readonly byte[] CMD_READ_DISPLAY_SIGNAL_MODE = { 0x0E };
        private static readonly byte[] CMD_READ_DISPLAY_SELF_DIAGNOSTIC_RESULT = { 0x0F };
        private static readonly byte[] CMD_ENTER_SLEEP_MODE = { 0x10 };
        private static readonly byte[] CMD_SLEEP_OUT = { 0x11 };
        private static readonly byte[] CMD_PARTIAL_MODE_ON = { 0x12 };
        private static readonly byte[] CMD_NORMAL_DISPLAY_MODE = { 0x13 };
        private static readonly byte[] CMD_DISPLAY_INVERSION_OFF = { 0x20 };
        private static readonly byte[] CMD_DISPLAY_INVERSION_ON = { 0x21 };
        private static readonly byte[] CMD_GAMMA_SET = { 0x26 };
        private static readonly byte[] CMD_DISPLAY_OFF = { 0x28 };
        private static readonly byte[] CMD_DISPLAY_ON = { 0x29 };
        private static readonly byte[] CMD_COLUMN_ADDRESS_SET = { 0x2A };
        private static readonly byte[] CMD_PAGE_ADDRESS_SET = { 0x2B };
        private static readonly byte[] CMD_MEMORY_WRITE = { 0x2C };
        private static readonly byte[] CMD_COLOR_SET = { 0x2D };
        private static readonly byte[] CMD_MEMORY_READ = { 0x2E };
        private static readonly byte[] CMD_PARTIAL_AREA = { 0x30 };
        private static readonly byte[] CMD_VERTICAL_SCROLLING_DEFINITION = { 0x33 };
        private static readonly byte[] CMD_TEARING_EFFECT_LINE_OFF = { 0x34 };
        private static readonly byte[] CMD_TEARING_EFFECT_LINE_ON = { 0x35 };
        private static readonly byte[] CMD_MEMORY_ACCESS_CONTROL = { 0x36 };
        private static readonly byte[] CMD_VERTICAL_SCROLLING_START_ADDRESS = { 0x37 };
        private static readonly byte[] CMD_IDLE_MODE_OFF = { 0x38 };
        private static readonly byte[] CMD_IDLE_MODE_ON = { 0x39 };
        private static readonly byte[] CMD_PIXEL_FORMAT_SET = { 0x3A };
        private static readonly byte[] CMD_WRITE_MEMORY_CONTINUE = { 0x3C };
        private static readonly byte[] CMD_READ_MEMORY_CONTINUE = { 0x3E };
        private static readonly byte[] CMD_SET_TEAR_SCANLINE = { 0x44 };
        private static readonly byte[] CMD_GET_SCANLINE = { 0x45 };
        private static readonly byte[] CMD_WRITE_DISPLAY_BRIGHTNESS = { 0x51 };
        private static readonly byte[] CMD_READ_DISPLAY_BRIGHTNESS = { 0x52 };
        private static readonly byte[] CMD_WRITE_CTRL_DISPLAY = { 0x53 };
        private static readonly byte[] CMD_READ_CTRL_DISPLAY = { 0x54 };
        private static readonly byte[] CMD_WRITE_CONTENT_ADAPTIVE_BRIGHTNESS_CONTROL = { 0x55 };
        private static readonly byte[] CMD_READ_CONTENT_ADAPTIVE_BRIGHTNESS_CONTROL = { 0x56 };
        private static readonly byte[] CMD_WRITE_CABC_MINIMUM_BRIGHTNESS = { 0x5E };
        private static readonly byte[] CMD_READ_CABC_MINIMUM_BRIGHTNESS = { 0x5F };
        private static readonly byte[] CMD_RGB_INTERFACE_SIGNAL_CONTROL = { 0xB0 };
        private static readonly byte[] CMD_FRAME_CONTROL_NORMAL_MODE = { 0xB1 };
        private static readonly byte[] CMD_FRAME_CONTROL_IDLE_MODE = { 0xB2 };
        private static readonly byte[] CMD_FRAME_CONTROL_PARTIAL_MODE = { 0xB3 };
        private static readonly byte[] CMD_DISPLAY_INVERSION_CONTROL = { 0xB4 };
        private static readonly byte[] CMD_BLANKING_PORCH_CONTROL = { 0xB5 };
        private static readonly byte[] CMD_DISPLAY_FUNCTION_CONTROL = { 0xB6 };
        private static readonly byte[] CMD_ENTRY_MODE_SET = { 0xB7 };
        private static readonly byte[] CMD_BACKLIGHT_CONTROL_1 = { 0xB8 };
        private static readonly byte[] CMD_BACKLIGHT_CONTROL_2 = { 0xB9 };
        private static readonly byte[] CMD_BACKLIGHT_CONTROL_3 = { 0xBA };
        private static readonly byte[] CMD_BACKLIGHT_CONTROL_4 = { 0xBB };
        private static readonly byte[] CMD_BACKLIGHT_CONTROL_5 = { 0xBc };
        private static readonly byte[] CMD_BACKLIGHT_CONTROL_7 = { 0xBe };
        private static readonly byte[] CMD_BACKLIGHT_CONTROL_8 = { 0xBf };
        private static readonly byte[] CMD_POWER_CONTROL_1 = { 0xC0 };
        private static readonly byte[] CMD_POWER_CONTROL_2 = { 0xC1 };
        private static readonly byte[] CMD_VCOM_CONTROL_1 = { 0xC5 };
        private static readonly byte[] CMD_VCOM_CONTROL_2 = { 0xC7 };
        private static readonly byte[] CMD_POWER_CONTROL_A = { 0xCB };
        private static readonly byte[] CMD_POWER_CONTROL_B = { 0xCF };
        private static readonly byte[] CMD_NV_MEMORY_WRITE = { 0xD0 };
        private static readonly byte[] CMD_NV_MEMORY_PROTECTION_KEY = { 0xD1 };
        private static readonly byte[] CMD_NV_MEMORY_STATUS_READ = { 0xD2 };
        private static readonly byte[] CMD_READ_ID4 = { 0xD3 };
        private static readonly byte[] CMD_READ_ID1 = { 0xDA };
        private static readonly byte[] CMD_READ_ID2 = { 0xDB };
        private static readonly byte[] CMD_READ_ID3 = { 0xDC };
        private static readonly byte[] CMD_POSITIVE_GAMMA_CORRECTION = { 0xE0 };
        private static readonly byte[] CMD_NEGATIVE_GAMMA_CORRECTION = { 0xE1 };
        private static readonly byte[] CMD_DIGITAL_GAMMA_CONTROL_1 = { 0xE2 };
        private static readonly byte[] CMD_DIGITAL_GAMMA_CONTROL_2 = { 0xE3 };
        private static readonly byte[] CMD_DRIVER_TIMING_CONTROL_A = { 0xE8 };
        private static readonly byte[] CMD_DRIVER_TIMING_CONTROL_B = { 0xEA };
        private static readonly byte[] CMD_POWER_ON_SEQUENCE_CONTROL = { 0xED };
        private static readonly byte[] CMD_ENABLE_3G = { 0xF2 };
        private static readonly byte[] CMD_INTERFACE_CONTROL = { 0xF6 };
        private static readonly byte[] CMD_PUMP_RATIO_CONTROL = { 0xF7 };

        // Liste des couleurs

        public static readonly uint COLOR_BLACK = 0x0000;         //   0,   0,   0
        public static readonly uint COLOR_NAVY = 0x000F;          //   0,   0, 123
        public static readonly uint COLOR_DARKGREEN = 0x03E0;     //   0, 125,   0
        public static readonly uint COLOR_DARKCYAN = 0x03EF;      //   0, 125, 123
        public static readonly uint COLOR_MAROON = 0x7800;        // 123,   0,   0
        public static readonly uint COLOR_PURPLE = 0x780F;        // 123,   0, 123
        public static readonly uint COLOR_OLIVE = 0x7BE0;         // 123, 125,   0
        public static readonly uint COLOR_LIGHTGREY = 0xC618;     // 198, 195, 198
        public static readonly uint COLOR_DARKGREY = 0x7BEF;      // 123, 125, 123
        public static readonly uint COLOR_BLUE = 0x001F;          //   0,   0, 255
        public static readonly uint COLOR_GREEN = 0x07E0;         //   0, 255,   0
        public static readonly uint COLOR_CYAN = 0x07FF;          //   0, 255, 255
        public static readonly uint COLOR_RED = 0xF800;           // 255,   0,   0
        public static readonly uint COLOR_MAGENTA = 0xF81F;       // 255,   0, 255
        public static readonly uint COLOR_YELLOW = 0xFFE0;        // 255, 255,   0
        public static readonly uint COLOR_WHITE = 0xFFFF;         // 255, 255, 255
        public static readonly uint COLOR_ORANGE = 0xFD20;        // 255, 165,   0
        public static readonly uint COLOR_GREENYELLOW = 0xAFE5;   // 173, 255,  41
        public static readonly uint COLOR_PINK = 0xFC18;          // 255, 130, 198

        public static readonly uint COLOR_BLUE_WINDOWS = 0x0B98;  //   8, 113, 196

        public static readonly uint COLOR_PINK_PAULINE = 0xFD38;  // 255, 165, 198

        public ILI9341()
        {

            this.resolution = HEIGHT_04;
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

            SendCommand(CMD_ENTER_SLEEP_MODE);

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

            SendCommand(CMD_PIXEL_FORMAT_SET);
            SendData(new byte[] { 0x55 });

            SendCommand(CMD_FRAME_CONTROL_NORMAL_MODE);
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

            SendCommand(CMD_MEMORY_WRITE);

        }


        // Zone Couleur

        public uint RGB888ToRGB565(byte r8, byte g8, byte b8)
        {

            ushort r5 = (ushort)((r8 * 249 + 1014) >> 11);
            ushort g6 = (ushort)((g8 * 253 + 505) >> 10);
            ushort b5 = (ushort)((b8 * 249 + 1014) >> 11);

            return (uint)(r5 << 11 | g6 << 5 | b5);

        }


        // Zone Ecran

        private void FillRectangle(int x0, int y0, int width, int height, uint color)
        {

            int nbrBande = height / resolution;

            byte[] buffer = new byte[width * resolution * 2];

            byte VH = (byte)((color >> 8) & 0xFF);
            byte VL = (byte)(color & 0xFF);

            for (int index = 0; index < buffer.Length; index += 2)
            {

                buffer[index] = VH;
                buffer[index + 1] = VL;

            }

            for( int b = 0; b < nbrBande; b++ )
            {

                SetAddress(x0, y0 + b * resolution, width, y0 + (b + 1) * resolution);
                SendData(buffer);

            }

        }

        public void ColorScreen(uint color)
        {

            this.FillRectangle(0, 0, LCD_W, LCD_H, color);
        }

        public void ClearScreen()
        {

            this.ColorScreen( COLOR_BLACK );
        }


        // Zone Image

        public async void LoadImage(uint[] _picture, string name )
        {

            StorageFile sourcefile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(name));

            ImageProperties imageProperties = await sourcefile.Properties.GetImagePropertiesAsync();

            uint width = imageProperties.Width;
            uint height = imageProperties.Height;
            
            using (IRandomAccessStream fileStream = await sourcefile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                BitmapTransform transform = new BitmapTransform()
                {

                    ScaledWidth = System.Convert.ToUInt32(width),
                    ScaledHeight = System.Convert.ToUInt32(height)
                };

                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);

                byte[] sourcePixels = pixelData.DetachPixelData();

                if (sourcePixels.Length == width * height * 4)
                {

                    int pi = 0;
                    int i = 0;
                    byte red = 0, green = 0, blue = 0;

                    foreach (byte b in sourcePixels)
                    {

                        switch (i)
                        {

                            case 0:
                                blue = b;
                                break;
                            case 1:
                                green = b;
                                break;
                            case 2:
                                red = b;
                                break;
                            case 3:
                                _picture[pi] = RGB888ToRGB565(red, green, blue);
                                pi++;
                                break;
                        }

                        i = (i + 1) % 4;
                    }

                }
                else
                {

                    return;
                }

            }

        }

        public void DrawPicture(uint[] _picture, int x0, int y0, int width, int height)
        {

            int block_size = width * resolution;

            byte[] buffer = new byte[block_size * 2];

            int i = 0;
            int line = 0;

            foreach (uint s in _picture)
            {

                buffer[i * 2] = (byte)((s >> 8) & 0xFF);
                buffer[i * 2 + 1] = (byte)(s & 0xFF);
                i++;

                if (i >= block_size)
                {

                    i = 0;
                    SetAddress(x0, y0 + line * resolution, x0 + width - 1, y0 + (line + 1) * resolution - 1);
                    SendData(buffer);
                    line++;

                }

            }

        }


        // Zone Texte

        private void MakeChar(char c, int x0, int y0, int size, uint color)
        {

            int widthCharOriginal = 6;
            int heightCharOriginal = 8;

            int widthCharSized = widthCharOriginal * size;
            int heightCharSized = heightCharOriginal * size;

            // bounds checks
            if ((x0 >= LCD_W) || (y0 >= LCD_H) || ((x0 + widthCharSized - 1) < 0) || ((y0 + heightCharSized - 1) < 0))
            {

                return;
            }
        
            byte[] car = DisplayFontTable.GetFontCharacterDescriptorFromFontTableStandart(c).Data;

            String c0 = car[0].ToString("X");
            String c1 = car[1].ToString("X");
            String c2 = car[2].ToString("X");
            String c3 = car[3].ToString("X");
            String c4 = car[4].ToString("X");

            if (c0.Length < 2) { c0 = "0" + c0; }
            if (c1.Length < 2) { c1 = "0" + c1; }
            if (c2.Length < 2) { c2 = "0" + c2; }
            if (c3.Length < 2) { c3 = "0" + c3; }
            if (c4.Length < 2) { c4 = "0" + c4; }

            String c01 = c0.Substring(0, 1);
            String c00 = c0.Substring(1, 1);

            String c11 = c1.Substring(0, 1);
            String c10 = c1.Substring(1, 1);

            String c21 = c2.Substring(0, 1);
            String c20 = c2.Substring(1, 1);

            String c31 = c3.Substring(0, 1);
            String c30 = c3.Substring(1, 1);

            String c41 = c4.Substring(0, 1);
            String c40 = c4.Substring(1, 1);

            bool[] b01 = Convertissor.ConvertHexToBin(c01);
            bool[] b00 = Convertissor.ConvertHexToBin(c00);

            bool[] b11 = Convertissor.ConvertHexToBin(c11);
            bool[] b10 = Convertissor.ConvertHexToBin(c10);

            bool[] b21 = Convertissor.ConvertHexToBin(c21);
            bool[] b20 = Convertissor.ConvertHexToBin(c20);

            bool[] b31 = Convertissor.ConvertHexToBin(c31);
            bool[] b30 = Convertissor.ConvertHexToBin(c30);

            bool[] b41 = Convertissor.ConvertHexToBin(c41);
            bool[] b40 = Convertissor.ConvertHexToBin(c40);

            uint[] _charOriginal = new uint[widthCharOriginal * heightCharOriginal];

            if (b00[0]) { _charOriginal[0] = color; } else { _charOriginal[0] = 0x0000; }
            if (b10[0]) { _charOriginal[1] = color; } else { _charOriginal[1] = 0x0000; }
            if (b20[0]) { _charOriginal[2] = color; } else { _charOriginal[2] = 0x0000; }
            if (b30[0]) { _charOriginal[3] = color; } else { _charOriginal[3] = 0x0000; }
            if (b40[0]) { _charOriginal[4] = color; } else { _charOriginal[4] = 0x0000; }
            _charOriginal[5] = 0x0000;

            if (b00[1]) { _charOriginal[6] = color; } else { _charOriginal[6] = 0x0000; }
            if (b10[1]) { _charOriginal[7] = color; } else { _charOriginal[7] = 0x0000; }
            if (b20[1]) { _charOriginal[8] = color; } else { _charOriginal[8] = 0x0000; }
            if (b30[1]) { _charOriginal[9] = color; } else { _charOriginal[9] = 0x0000; }
            if (b40[1]) { _charOriginal[10] = color; } else { _charOriginal[10] = 0x0000; }
            _charOriginal[11] = 0x0000;

            if (b00[2]) { _charOriginal[12] = color; } else { _charOriginal[12] = 0x0000; }
            if (b10[2]) { _charOriginal[13] = color; } else { _charOriginal[13] = 0x0000; }
            if (b20[2]) { _charOriginal[14] = color; } else { _charOriginal[14] = 0x0000; }
            if (b30[2]) { _charOriginal[15] = color; } else { _charOriginal[15] = 0x0000; }
            if (b40[2]) { _charOriginal[16] = color; } else { _charOriginal[16] = 0x0000; }
            _charOriginal[17] = 0x0000;

            if (b00[3]) { _charOriginal[18] = color; } else { _charOriginal[18] = 0x0000; }
            if (b10[3]) { _charOriginal[19] = color; } else { _charOriginal[19] = 0x0000; }
            if (b20[3]) { _charOriginal[20] = color; } else { _charOriginal[20] = 0x0000; }
            if (b30[3]) { _charOriginal[21] = color; } else { _charOriginal[21] = 0x0000; }
            if (b40[3]) { _charOriginal[22] = color; } else { _charOriginal[22] = 0x0000; }
            _charOriginal[23] = 0x0000;

            if (b01[0]) { _charOriginal[24] = color; } else { _charOriginal[24] = 0x0000; }
            if (b11[0]) { _charOriginal[25] = color; } else { _charOriginal[25] = 0x0000; }
            if (b21[0]) { _charOriginal[26] = color; } else { _charOriginal[26] = 0x0000; }
            if (b31[0]) { _charOriginal[27] = color; } else { _charOriginal[27] = 0x0000; }
            if (b41[0]) { _charOriginal[28] = color; } else { _charOriginal[28] = 0x0000; }
            _charOriginal[29] = 0x0000;

            if (b01[1]) { _charOriginal[30] = color; } else { _charOriginal[30] = 0x0000; }
            if (b11[1]) { _charOriginal[31] = color; } else { _charOriginal[31] = 0x0000; }
            if (b21[1]) { _charOriginal[32] = color; } else { _charOriginal[32] = 0x0000; }
            if (b31[1]) { _charOriginal[33] = color; } else { _charOriginal[33] = 0x0000; }
            if (b41[1]) { _charOriginal[34] = color; } else { _charOriginal[34] = 0x0000; }
            _charOriginal[35] = 0x0000;

            if (b01[2]) { _charOriginal[36] = color; } else { _charOriginal[36] = 0x0000; }
            if (b11[2]) { _charOriginal[37] = color; } else { _charOriginal[37] = 0x0000; }
            if (b21[2]) { _charOriginal[38] = color; } else { _charOriginal[38] = 0x0000; }
            if (b31[2]) { _charOriginal[39] = color; } else { _charOriginal[39] = 0x0000; }
            if (b41[2]) { _charOriginal[40] = color; } else { _charOriginal[40] = 0x0000; }
            _charOriginal[41] = 0x0000;

            _charOriginal[42] = 0x0000;
            _charOriginal[43] = 0x0000;
            _charOriginal[44] = 0x0000;
            _charOriginal[45] = 0x0000;
            _charOriginal[46] = 0x0000;
            _charOriginal[47] = 0x0000;

            uint[] _charSized = new uint[widthCharSized * heightCharSized];

            int cs = 0;

            for (int hco = 0; hco < heightCharOriginal; hco++)
            {

                for (int hts = 0; hts < size; hts++)
                {

                    for (int wco = 0; wco < widthCharOriginal; wco++)
                    {

                        for (int wts = 0; wts < size; wts++)
                        {

                            _charSized[cs] = _charOriginal[hco * widthCharOriginal + wco];

                            cs++;

                        }

                    }

                }

            }

            DrawPicture(_charSized, x0, y0, widthCharSized, heightCharSized);
            	
        }

        private void Print(char c, int size, uint color)
        {

    	    if ( c == '\n' )         // Nouvelle ligne
            {

                // Next line based on font size
                this.cursorY += size * 8;
                // Back to  character 0
                this.cursorX = 0;

            }
            else if ( c == '\r' )		// Retour à la ligne
            {

                // Back to  character 0
                this.cursorX = 0;

            }
            else if ( c == '\t' )		// Tabulation (4 caractères)
            {

                // 4 caractères plus à droite
                this.cursorX += size * 6 * 4;

            }
            else
            {

                this.MakeChar(c, this.cursorX, this.cursorY, size, color);

                this.cursorX += size * 6;

                if (this.cursorX > (LCD_W - size * 6))
                {

                    // next line based on font size
                    this.cursorY += (byte)(size * 8);
                    // back to  character 0
                    this.cursorX = 0;
                }

            }

        }

        public void PlaceCursor(int x, int y)
        {   

            this.cursorX = x;
            this.cursorY = y;

        }

        public void Print(String s, int size, uint color)
        {

            for (int i = 0; i < s.Length; i++)
            {

                this.Print(s.ElementAt<char>(i), size, color);
            }

        }

        public void Println(String s, int size, uint color)
        {

            for (int i = 0; i<s.Length; i++)
            {

                this.Print(s.ElementAt<char>(i), size, color);
            }

            this.Print("\n", size, color);

        }


        // Zone Graphique

        public void LCDPixel(int x0, int y0, int color)
        {

            if ((x0< 0) || (x0 >= LCD_W) || (y0< 0) || (y0 >= LCD_H))
            {
                return;
            }

            int x1 = x0 + 1;
            int y1 = y0 + 1;

            byte VH = (byte)(color >> 8);
            byte VL = (byte)(color & 0xFF);

            byte[] buffer = new byte[2];

            for (int index = 0; index<buffer.Length; index += 2)
            {
                buffer[index] = VH;
                buffer[index + 1] = VL;

            }

            SetAddress(x0, y0, x1, y1);
            SendData(buffer);

        }

        public void DrawLine(int x1, int y1, int x2, int y2, int color)
        {

            if ( x1 >= LCD_W || y1 >= LCD_H || x2 >= LCD_W || y2 >= LCD_H )
            {
                return;
            }

            if (x1 == x2)
            {

                for (int n = y1; n < y2; n++)
                {

                    int xn = x1;
                    int yn = n;

                    LCDPixel(xn, yn, color);

                }

            }
            else if (y1 == y2)
            {

                for (int n = x1; n < x2; n++)
                {

                    int xn = n;
                    int yn = y1;

                    LCDPixel(xn, yn, color);

                }

            }
            else if (x1 == x2 && y1 == y2)
            {

                LCDPixel(x1, y1, color);
            }
            else if( x2 > x1 )
            {

                double dx = x2 - x1;
                double dy = y2 - y1;

                double a = dy / dx;

                for (int n = 0; n < (x2 - x1); n++)
                {

                    int xn = n + x1;

                    int yn = (int)(a * n + y1);

                    LCDPixel(xn, yn, color);

                }

            }
            else if (x2 < x1)
            {

                int X1 = x2;
                int Y1 = y2;

                int X2 = x1;
                int Y2 = y1;

                double dX = X2 - X1;
                double dY = Y2 - Y1;

                double a = dY / dX;

                for (int n = 0; n < (X2 - X1); n++)
                {

                    int xn = (int)(n + X1);

                    int yn = (int)(a * n + Y1);

                    LCDPixel(xn, yn, color);

                }

            }

        }

        public void DrawArc(int x0, int y0, int Radius, int startAngle, int endAngle, int color)
        {

            double increment = 0.1;

            double startA = startAngle;

            double endA = endAngle;

            if (startA > 360)
            {
                startA = 360;
            }

            if (endA > 360)
            {

                endA = 360;
            }

            for (double i = startA; i < endA; i += increment)
            {

                double angle = i * Math.PI / 180;

                int x = (int)(x0 + Radius * Math.Sin(angle));
                int y = (int)(y0 - Radius * Math.Cos(angle));

                LCDPixel(x, y, color);

            }

        }

        public void DrawCircle(int x0, int y0, int radius, int color)
        {

            DrawArc(x0, y0, radius, 0, 360, color);
        }

        public void DrawRectangle(int x0, int y0, int width, int height, int color)
        {

            // bounds check
            if (x0 >= LCD_W || y0 >= LCD_H || (x0 + width - 1) >= LCD_W || (y0 + height - 1) >= LCD_H)
            {

                return;
            }

            int xa = x0;
            int ya = y0;

            int xb = x0 + width - 1;
            int yb = y0;

            int xc = x0 + width - 1;
            int yc = y0 + height - 1;

            int xd = x0;
            int yd = y0 + height - 1;

            DrawLine(xa, ya, xb, yb, color);
            DrawLine(xd, yd, xc, yc, color);
            DrawLine(xa, ya, xd, yd, color);
            DrawLine(xb, yb, xc, yc, color);

        }
                                   
    }

}
