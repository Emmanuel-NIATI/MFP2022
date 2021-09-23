using System;

namespace DisplayFont
{

    public class Convertissor
    {

        public static bool[] ConvertHexToBin(String _hex)
        {

            bool[] res = new bool[4];

            if ("0".Equals(_hex)) { res[0] = false; res[1] = false; res[2] = false; res[3] = false; }

            if ("1".Equals(_hex)) { res[0] = true; res[1] = false; res[2] = false; res[3] = false; }

            if ("2".Equals(_hex)) { res[0] = false; res[1] = true; res[2] = false; res[3] = false; }

            if ("3".Equals(_hex)) { res[0] = true; res[1] = true; res[2] = false; res[3] = false; }

            if ("4".Equals(_hex)) { res[0] = false; res[1] = false; res[2] = true; res[3] = false; }

            if ("5".Equals(_hex)) { res[0] = true; res[1] = false; res[2] = true; res[3] = false; }

            if ("6".Equals(_hex)) { res[0] = false; res[1] = true; res[2] = true; res[3] = false; }

            if ("7".Equals(_hex)) { res[0] = true; res[1] = true; res[2] = true; res[3] = false; }

            if ("8".Equals(_hex)) { res[0] = false; res[1] = false; res[2] = false; res[3] = true; }

            if ("9".Equals(_hex)) { res[0] = true; res[1] = false; res[2] = false; res[3] = true; }

            if ("A".Equals(_hex) || "a".Equals(_hex)) { res[0] = false; res[1] = true; res[2] = false; res[3] = true; }

            if ("B".Equals(_hex) || "b".Equals(_hex)) { res[0] = true; res[1] = true; res[2] = false; res[3] = true; }

            if ("C".Equals(_hex) || "c".Equals(_hex)) { res[0] = false; res[1] = false; res[2] = true; res[3] = true; }

            if ("D".Equals(_hex) || "d".Equals(_hex)) { res[0] = true; res[1] = false; res[2] = true; res[3] = true; }

            if ("E".Equals(_hex) || "E".Equals(_hex)) { res[0] = false; res[1] = true; res[2] = true; res[3] = true; }

            if ("F".Equals(_hex) || "f".Equals(_hex)) { res[0] = true; res[1] = true; res[2] = true; res[3] = true; }

            return res;

        }

        public static String ConvertBinToHex(bool _b3, bool _b2, bool _b1, bool _b0)
        {

            String res = "0";

            if (!_b3 && !_b2 && !_b1 && !_b0) { res = "0"; }
            if (!_b3 && !_b2 && !_b1 && _b0) { res = "1"; }
            if (!_b3 && !_b2 && _b1 && !_b0) { res = "2"; }
            if (!_b3 && !_b2 && _b1 && _b0) { res = "3"; }
            if (!_b3 && _b2 && !_b1 && !_b0) { res = "4"; }
            if (!_b3 && _b2 && !_b1 && _b0) { res = "5"; }
            if (!_b3 && _b2 && _b1 && !_b0) { res = "6"; }
            if (!_b3 && _b2 && _b1 && _b0) { res = "7"; }
            if (_b3 && !_b2 && !_b1 && !_b0) { res = "8"; }
            if (_b3 && !_b2 && !_b1 && _b0) { res = "9"; }
            if (_b3 && !_b2 && _b1 && !_b0) { res = "A"; }
            if (_b3 && !_b2 && _b1 && _b0) { res = "B"; }
            if (_b3 && _b2 && !_b1 && !_b0) { res = "C"; }
            if (_b3 && _b2 && !_b1 && _b0) { res = "D"; }
            if (_b3 && _b2 && _b1 && !_b0) { res = "E"; }
            if (_b3 && _b2 && _b1 && _b0) { res = "F"; }

            return res;

        }

    }

    public class FontCharacterDescriptor
    {

        public readonly char Character;
        public readonly byte[] Data;
        public readonly String Description;

        public FontCharacterDescriptor( Char _Character, byte[] _Data, String _Description)
        {

            Character = _Character;
            Data = _Data;
            Description = _Description;

        }

    }

    public static class DisplayFontTable
    {

        public static FontCharacterDescriptor GetFontCharacterDescriptorFromFontTableStandart(UInt32 _Order)
        {

            return FontTableStandart[_Order];
        }

        public static UInt32 GetFontTableStandartSize()
        {

            return (UInt32) FontTableStandart.Length;
        }

        private static readonly FontCharacterDescriptor[] FontTableStandart =
        {

            new FontCharacterDescriptor('☺', new byte[]{ 0x41, 0x14, 0x30, 0x14, 0x41}, "Caractère ☺"),
            new FontCharacterDescriptor('☻', new byte[]{ 0x3E, 0x6B, 0x4F, 0x6B, 0x3E}, "Caractère ☻"),
            new FontCharacterDescriptor('♥', new byte[]{ 0x1C, 0x3E, 0x7C, 0x3E, 0x1C}, "Caractère ♥"),
            new FontCharacterDescriptor('♦', new byte[]{ 0x18, 0x3C, 0x7E, 0x3C, 0x18}, "Caractère ♦"),
            new FontCharacterDescriptor('♣', new byte[]{ 0x1C, 0x57, 0x7D, 0x57, 0x1C}, "Caractère ♣"),
            new FontCharacterDescriptor('♠', new byte[]{ 0x1C, 0x5E, 0x7F, 0x5E, 0x1C}, "Caractère ♠"),
            new FontCharacterDescriptor('•', new byte[]{ 0x00, 0x1C, 0x1C, 0x1C, 0x00}, "Caractère •"),
            new FontCharacterDescriptor('◘', new byte[]{ 0x7F, 0x63, 0x63, 0x63, 0x7F}, "Caractère ◘"),
            new FontCharacterDescriptor('○', new byte[]{ 0x00, 0x1C, 0x14, 0x1C, 0x00}, "Caractère ○"),
            new FontCharacterDescriptor('◙', new byte[]{ 0x7F, 0x63, 0x6B, 0x63, 0x7F}, "Caractère ◙"),
            new FontCharacterDescriptor('♂', new byte[]{ 0x30, 0x48, 0x3A, 0x06, 0x0E}, "Caractère ♂"),
            new FontCharacterDescriptor('♀', new byte[]{ 0x26, 0x29, 0x79, 0x29, 0x26}, "Caractère ♀"),
            new FontCharacterDescriptor('♪', new byte[]{ 0x40, 0x7F, 0x01, 0x06, 0x00}, "Caractère ♪"),
            new FontCharacterDescriptor('♫', new byte[]{ 0x40, 0x7F, 0x01, 0x21, 0x3F}, "Caractère ♫"),
            new FontCharacterDescriptor('☼', new byte[]{ 0x2A, 0x1C, 0x36, 0x1C, 0x2A}, "Caractère ☼"),
            new FontCharacterDescriptor('►', new byte[]{ 0x00, 0x3E, 0x1C, 0x08, 0x00}, "Caractère ►"),
            new FontCharacterDescriptor('◄', new byte[]{ 0x00, 0x08, 0x1C, 0x3E, 0x00}, "Caractère ◄"),
            new FontCharacterDescriptor('↕', new byte[]{ 0x14, 0x22, 0x7F, 0x22, 0x14}, "Caractère ↕"),
            new FontCharacterDescriptor('‼', new byte[]{ 0x00, 0x5F, 0x00, 0x5F, 0x00}, "Caractère ‼"),
            new FontCharacterDescriptor('¶', new byte[]{ 0x06, 0x09, 0x7F, 0x01, 0x7F}, "Caractère ¶"),
            new FontCharacterDescriptor('§', new byte[]{ 0x00, 0x5F, 0x55, 0x7D, 0x00}, "Caractère §"),
            new FontCharacterDescriptor('▬', new byte[]{ 0x00, 0x08, 0x08, 0x08, 0x00}, "Caractère ▬"),
            new FontCharacterDescriptor('↨', new byte[]{ 0x94, 0xA2, 0xFF, 0xA2, 0x94}, "Caractère ↨"),
            new FontCharacterDescriptor('↑', new byte[]{ 0x08, 0x04, 0x7E, 0x04, 0x08}, "Caractère ↑"),
            new FontCharacterDescriptor('↓', new byte[]{ 0x10, 0x20, 0x7E, 0x20, 0x10}, "Caractère ↓"),
            new FontCharacterDescriptor('→', new byte[]{ 0x08, 0x08, 0x2A, 0x1C, 0x08}, "Caractère →"),
            new FontCharacterDescriptor('←', new byte[]{ 0x08, 0x1C, 0x2A, 0x08, 0x08}, "Caractère ←"),
            new FontCharacterDescriptor('∟', new byte[]{ 0x1E, 0x10, 0x10, 0x10, 0x10}, "Caractère ∟"),
            new FontCharacterDescriptor('↔', new byte[]{ 0x08, 0x1C, 0x08, 0x1C, 0x08}, "Caractère ↔"),
            new FontCharacterDescriptor('▲', new byte[]{ 0x10, 0x18, 0x1C, 0x18, 0x10}, "Caractère ▲"),
            new FontCharacterDescriptor('▼', new byte[]{ 0x04, 0x0C, 0x1C, 0x0C, 0x04}, "Caractère ▼"),
            new FontCharacterDescriptor(' ', new byte[]{ 0x00, 0x00, 0x00, 0x00, 0x00}, "Caractère  "),
            new FontCharacterDescriptor('!', new byte[]{ 0x00, 0x00, 0x5F, 0x00, 0x00}, "Caractère !"),
            new FontCharacterDescriptor('"', new byte[]{ 0x04, 0x03, 0x04, 0x03, 0x00}, "Caractère \""),
            new FontCharacterDescriptor('#', new byte[]{ 0x14, 0x7F, 0x14, 0x7F, 0x14}, "Caractère #"),
            new FontCharacterDescriptor('$', new byte[]{ 0x24, 0x2A, 0x7F, 0x2A, 0x12}, "Caractère $"),
            new FontCharacterDescriptor('%', new byte[]{ 0x23, 0x13, 0x08, 0x64, 0x62}, "Caractère %"),
            new FontCharacterDescriptor('&', new byte[]{ 0x36, 0x49, 0x56, 0x20, 0x50}, "Caractère &"),
            new FontCharacterDescriptor('\'', new byte[]{ 0x00, 0x00, 0x04, 0x03, 0x00}, "Caractère \'"),
            new FontCharacterDescriptor('(', new byte[]{ 0x00, 0x1C, 0x22, 0x41, 0x00}, "Caractère ("),
            new FontCharacterDescriptor(')', new byte[]{ 0x00, 0x41, 0x22, 0x1C, 0x00}, "Caractère )"),
            new FontCharacterDescriptor('*', new byte[]{ 0x2A, 0x1C, 0x3E, 0x1C, 0x2A}, "Caractère *"),
            new FontCharacterDescriptor('+', new byte[]{ 0x08, 0x08, 0x3E, 0x08, 0x08}, "Caractère +"),
            new FontCharacterDescriptor(',', new byte[]{ 0x00, 0x00, 0x40, 0x30, 0x00}, "Caractère ,"),
            new FontCharacterDescriptor('-', new byte[]{ 0x08, 0x08, 0x08, 0x08, 0x08}, "Caractère -"),
            new FontCharacterDescriptor('.', new byte[]{ 0x00, 0x00, 0x60, 0x60, 0x00}, "Caractère ."),
            new FontCharacterDescriptor('/', new byte[]{ 0x20, 0x10, 0x08, 0x04, 0x02}, "Caractère /"),
            new FontCharacterDescriptor('0', new byte[]{ 0x3E, 0x51, 0x49, 0x45, 0x3E}, "Chiffre 0"),
            new FontCharacterDescriptor('1', new byte[]{ 0x00, 0x42, 0x7F, 0x40, 0x00}, "Chiffre 1"),
            new FontCharacterDescriptor('2', new byte[]{ 0x42, 0x61, 0x51, 0x49, 0x46}, "Chiffre 2"),
            new FontCharacterDescriptor('3', new byte[]{ 0x21, 0x41, 0x45, 0x4B, 0x31}, "Chiffre 3"),
            new FontCharacterDescriptor('4', new byte[]{ 0x18, 0x14, 0x12, 0x7F, 0x10}, "Chiffre 4"),
            new FontCharacterDescriptor('5', new byte[]{ 0x27, 0x45, 0x45, 0x45, 0x39}, "Chiffre 5"),
            new FontCharacterDescriptor('6', new byte[]{ 0x3C, 0x4A, 0x49, 0x49, 0x30}, "Chiffre 6"),
            new FontCharacterDescriptor('7', new byte[]{ 0x01, 0x71, 0x09, 0x05, 0x03}, "Chiffre 7"),
            new FontCharacterDescriptor('8', new byte[]{ 0x36, 0x49, 0x49, 0x49, 0x36}, "Chiffre 8"),
            new FontCharacterDescriptor('9', new byte[]{ 0x06, 0x49, 0x49, 0x29, 0x1E}, "Chiffre 9"),
            new FontCharacterDescriptor(':', new byte[]{ 0x00, 0x00, 0x14, 0x00, 0x00}, "Caractère :"),
            new FontCharacterDescriptor(';', new byte[]{ 0x00, 0x00, 0x40, 0x34, 0x00}, "Caractère ;"),
            new FontCharacterDescriptor('<', new byte[]{ 0x00, 0x08, 0x14, 0x22, 0x41}, "Caractère <"),
            new FontCharacterDescriptor('=', new byte[]{ 0x14, 0x14, 0x14, 0x14, 0x14}, "Caractère ="),
            new FontCharacterDescriptor('>', new byte[]{ 0x41, 0x22, 0x14, 0x08, 0x00}, "Caractère >"),
            new FontCharacterDescriptor('?', new byte[]{ 0x02, 0x01, 0x59, 0x09, 0x06}, "Caractère ?"),
            new FontCharacterDescriptor('@', new byte[]{ 0x32, 0x49, 0x79, 0x41, 0x3E}, "Caractère @"),
            new FontCharacterDescriptor('A', new byte[]{ 0x7E, 0x11, 0x11, 0x11, 0x7E}, "Lettre A majuscule"),
            new FontCharacterDescriptor('B', new byte[]{ 0x7F, 0x49, 0x49, 0x49, 0x36}, "Lettre B majuscule"),
            new FontCharacterDescriptor('C', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x22}, "Lettre C majuscule"),
            new FontCharacterDescriptor('D', new byte[]{ 0x7F, 0x41, 0x41, 0x22, 0x1C}, "Lettre D majuscule"),
            new FontCharacterDescriptor('E', new byte[]{ 0x7F, 0x49, 0x49, 0x49, 0x41}, "Lettre E majuscule"),
            new FontCharacterDescriptor('F', new byte[]{ 0x7F, 0x09, 0x09, 0x09, 0x01}, "Lettre F majuscule"),
            new FontCharacterDescriptor('G', new byte[]{ 0x3E, 0x41, 0x49, 0x49, 0x7A}, "Lettre G majuscule"),
            new FontCharacterDescriptor('H', new byte[]{ 0x7F, 0x08, 0x08, 0x08, 0x7F}, "Lettre H majuscule"),
            new FontCharacterDescriptor('I', new byte[]{ 0x00, 0x41, 0x7F, 0x41, 0x00}, "Lettre I majuscule"),
            new FontCharacterDescriptor('J', new byte[]{ 0x20, 0x40, 0x41, 0x3F, 0x01}, "Lettre J majuscule"),
            new FontCharacterDescriptor('K', new byte[]{ 0x7F, 0x08, 0x14, 0x22, 0x41}, "Lettre K majuscule"),
            new FontCharacterDescriptor('L', new byte[]{ 0x7F, 0x40, 0x40, 0x40, 0x40}, "Lettre L majuscule"),
            new FontCharacterDescriptor('M', new byte[]{ 0x7F, 0x02, 0x0C, 0x02, 0x7F}, "Lettre M majuscule"),
            new FontCharacterDescriptor('N', new byte[]{ 0x7F, 0x04, 0x08, 0x10, 0x7F}, "Lettre N majuscule"),
            new FontCharacterDescriptor('O', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x3E}, "Lettre O majuscule"),
            new FontCharacterDescriptor('P', new byte[]{ 0x7F, 0x09, 0x09, 0x09, 0x06}, "Lettre P majuscule"),
            new FontCharacterDescriptor('Q', new byte[]{ 0x3E, 0x41, 0x51, 0x21, 0x5E}, "Lettre Q majuscule"),
            new FontCharacterDescriptor('R', new byte[]{ 0x7F, 0x09, 0x19, 0x29, 0x46}, "Lettre R majuscule"),
            new FontCharacterDescriptor('S', new byte[]{ 0x26, 0x49, 0x49, 0x49, 0x32}, "Lettre S majuscule"),
            new FontCharacterDescriptor('T', new byte[]{ 0x01, 0x01, 0x7F, 0x01, 0x01}, "Lettre T majuscule"),
            new FontCharacterDescriptor('U', new byte[]{ 0x3F, 0x40, 0x40, 0x40, 0x3F}, "Lettre U majuscule"),
            new FontCharacterDescriptor('V', new byte[]{ 0x1F, 0x20, 0x40, 0x20, 0x1F}, "Lettre V majuscule"),
            new FontCharacterDescriptor('W', new byte[]{ 0x3F, 0x40, 0x38, 0x40, 0x3F}, "Lettre W majuscule"),
            new FontCharacterDescriptor('X', new byte[]{ 0x63, 0x14, 0x08, 0x14, 0x63}, "Lettre X majuscule"),
            new FontCharacterDescriptor('Y', new byte[]{ 0x07, 0x08, 0x70, 0x08, 0x07}, "Lettre Y majuscule"),
            new FontCharacterDescriptor('Z', new byte[]{ 0x61, 0x51, 0x49, 0x45, 0x43}, "Lettre Z majuscule"),
            new FontCharacterDescriptor('[', new byte[]{ 0x00, 0x7F, 0x41, 0x41, 0x00}, "Caractère ["),
            new FontCharacterDescriptor('\\', new byte[]{ 0x02, 0x04, 0x08, 0x10, 0x20}, "Caractère \\"),
            new FontCharacterDescriptor(']', new byte[]{ 0x00, 0x41, 0x41, 0x7F, 0x41}, "Caractère ]"),
            new FontCharacterDescriptor('^', new byte[]{ 0x00, 0x02, 0x01, 0x02, 0x00}, "Caractère ^"),
            new FontCharacterDescriptor('_', new byte[]{ 0x40, 0x40, 0x40, 0x40, 0x40}, "Caractère _"),
            new FontCharacterDescriptor('`', new byte[]{ 0x00, 0x00, 0x03, 0x04, 0x00}, "Caractère `"),
            new FontCharacterDescriptor('a', new byte[]{ 0x20, 0x54, 0x54, 0x54, 0x78}, "Lettre a minuscule"),
            new FontCharacterDescriptor('b', new byte[]{ 0x7F, 0x28, 0x44, 0x44, 0x38}, "Lettre b minuscule"),
            new FontCharacterDescriptor('c', new byte[]{ 0x38, 0x44, 0x44, 0x44, 0x28}, "Lettre c minuscule"),
            new FontCharacterDescriptor('d', new byte[]{ 0x38, 0x44, 0x44, 0x28, 0x7F}, "Lettre d minuscule"),
            new FontCharacterDescriptor('e', new byte[]{ 0x38, 0x54, 0x54, 0x54, 0x18}, "Lettre e minuscule"),
            new FontCharacterDescriptor('f', new byte[]{ 0x00, 0x08, 0x7E, 0x09, 0x02}, "Lettre f minuscule"),
            new FontCharacterDescriptor('g', new byte[]{ 0x0C, 0x52, 0x52, 0x52, 0x3E}, "Lettre g minuscule"),
            new FontCharacterDescriptor('h', new byte[]{ 0x7F, 0x08, 0x04, 0x04, 0x78}, "Lettre h minuscule"),
            new FontCharacterDescriptor('i', new byte[]{ 0x00, 0x44, 0x7D, 0x40, 0x00}, "Lettre i minuscule"),
            new FontCharacterDescriptor('j', new byte[]{ 0x20, 0x40, 0x44, 0x3D, 0x00}, "Lettre j minuscule"),
            new FontCharacterDescriptor('k', new byte[]{ 0x7F, 0x10, 0x28, 0x44, 0x00}, "Lettre k minuscule"),
            new FontCharacterDescriptor('l', new byte[]{ 0x00, 0x41, 0x7F, 0x40, 0x00}, "Lettre l minuscule"),
            new FontCharacterDescriptor('m', new byte[]{ 0x7C, 0x04, 0x18, 0x04, 0x78}, "Lettre m minuscule"),
            new FontCharacterDescriptor('n', new byte[]{ 0x7C, 0x08, 0x04, 0x04, 0x78}, "Lettre n minuscule"),
            new FontCharacterDescriptor('o', new byte[]{ 0x38, 0x44, 0x44, 0x44, 0x38}, "Lettre o minuscule"),
            new FontCharacterDescriptor('p', new byte[]{ 0x7C, 0x08, 0x14, 0x14, 0x08}, "Lettre p minuscule"),
            new FontCharacterDescriptor('q', new byte[]{ 0x08, 0x14, 0x14, 0x08, 0x7C}, "Lettre q minuscule"),
            new FontCharacterDescriptor('r', new byte[]{ 0x7C, 0x08, 0x04, 0x04, 0x08}, "Lettre r minuscule"),
            new FontCharacterDescriptor('s', new byte[]{ 0x48, 0x54, 0x54, 0x54, 0x24}, "Lettre s minuscule"),
            new FontCharacterDescriptor('t', new byte[]{ 0x04, 0x3F, 0x44, 0x40, 0x20}, "Lettre t minuscule"),
            new FontCharacterDescriptor('u', new byte[]{ 0x3C, 0x40, 0x40, 0x20, 0x7C}, "Lettre u minuscule"),
            new FontCharacterDescriptor('v', new byte[]{ 0x1C, 0x20, 0x40, 0x20, 0x1C}, "Lettre v minuscule"),
            new FontCharacterDescriptor('w', new byte[]{ 0x3C, 0x40, 0x30, 0x40, 0x3C}, "Lettre w minuscule"),
            new FontCharacterDescriptor('x', new byte[]{ 0x44, 0x28, 0x10, 0x28, 0x44}, "Lettre x minuscule"),
            new FontCharacterDescriptor('y', new byte[]{ 0x0C, 0x50, 0x50, 0x50, 0x3C}, "Lettre y minuscule"),
            new FontCharacterDescriptor('z', new byte[]{ 0x44, 0x64, 0x54, 0x4C, 0x44}, "Lettre z minuscule"),
            new FontCharacterDescriptor('{', new byte[]{ 0x00, 0x08, 0x36, 0x41, 0x00}, "Caractère {"),
            new FontCharacterDescriptor('|', new byte[]{ 0x00, 0x00, 0x77, 0x00, 0x00}, "Caractère |"),
            new FontCharacterDescriptor('}', new byte[]{ 0x00, 0x41, 0x36, 0x08, 0x00}, "Caractère }"),
            new FontCharacterDescriptor('~', new byte[]{ 0x02, 0x01, 0x02, 0x04, 0x02}, "Caractère ~"),
            new FontCharacterDescriptor('⌂', new byte[]{ 0x3C, 0x26, 0x23, 0x26, 0x3C}, "Caractère ⌂"),
            new FontCharacterDescriptor('Ç', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x22}, "Lettre Ç majuscule"),
            new FontCharacterDescriptor('ü', new byte[]{ 0x3C, 0x41, 0x40, 0x21, 0x7C}, "Lettre ü minuscule"),
            new FontCharacterDescriptor('é', new byte[]{ 0x38, 0x54, 0x56, 0x55, 0x18}, "Lettre é minuscule"),
            new FontCharacterDescriptor('â', new byte[]{ 0x20, 0x56, 0x55, 0x56, 0x78}, "Lettre â minuscule"),
            new FontCharacterDescriptor('ä', new byte[]{ 0x20, 0x55, 0x54, 0x55, 0x78}, "Lettre ä minuscule"),
            new FontCharacterDescriptor('à', new byte[]{ 0x20, 0x55, 0x56, 0x54, 0x78}, "Lettre à minuscule"),
            new FontCharacterDescriptor('å', new byte[]{ 0x20, 0x54, 0x55, 0x54, 0x78}, "Lettre å minuscule"),
            new FontCharacterDescriptor('ç', new byte[]{ 0x0E, 0x51, 0x39, 0x11, 0x0A}, "Lettre ç minuscule"),
            new FontCharacterDescriptor('ê', new byte[]{ 0x38, 0x56, 0x55, 0x56, 0x18}, "Lettre ê minuscule"),
            new FontCharacterDescriptor('ë', new byte[]{ 0x38, 0x55, 0x54, 0x55, 0x18}, "Lettre ë minuscule"),
            new FontCharacterDescriptor('è', new byte[]{ 0x38, 0x55, 0x56, 0x54, 0x18}, "Lettre è minuscule"),
            new FontCharacterDescriptor('ï', new byte[]{ 0x00, 0x45, 0x7C, 0x41, 0x00}, "Lettre ï minuscule"),
            new FontCharacterDescriptor('î', new byte[]{ 0x00, 0x46, 0x7D, 0x42, 0x00}, "Lettre î minuscule"),
            new FontCharacterDescriptor('ì', new byte[]{ 0x00, 0x45, 0x7E, 0x40, 0x00}, "Lettre ì minuscule"),
            new FontCharacterDescriptor('Ä', new byte[]{ 0x7E, 0x11, 0x11, 0x11, 0x7E}, "Lettre Ä majuscule"),
            new FontCharacterDescriptor('Å', new byte[]{ 0x7E, 0x11, 0x11, 0x11, 0x7E}, "Lettre Å majuscule"),
            new FontCharacterDescriptor('É', new byte[]{ 0x7F, 0x49, 0x49, 0x49, 0x41}, "Lettre É majuscule"),
            new FontCharacterDescriptor('æ', new byte[]{ 0x20, 0x54, 0x78, 0x54, 0x58}, "Lettre æ minuscule"),
            new FontCharacterDescriptor('Æ', new byte[]{ 0x7C, 0x12, 0x7F, 0x49, 0x41}, "Lettre Æ majuscule"),
            new FontCharacterDescriptor('ô', new byte[]{ 0x38, 0x46, 0x45, 0x46, 0x38}, "Lettre ô minuscule"),
            new FontCharacterDescriptor('ö', new byte[]{ 0x38, 0x45, 0x44, 0x45, 0x38}, "Lettre ö minuscule"),
            new FontCharacterDescriptor('ò', new byte[]{ 0x38, 0x45, 0x46, 0x44, 0x38}, "Lettre ò minuscule"),
            new FontCharacterDescriptor('û', new byte[]{ 0x3C, 0x42, 0x41, 0x22, 0x7C}, "Lettre û minuscule"),
            new FontCharacterDescriptor('ù', new byte[]{ 0x3C, 0x41, 0x42, 0x30, 0x7C}, "Lettre ù minuscule"),
            new FontCharacterDescriptor('ÿ', new byte[]{ 0x0C, 0x51, 0x50, 0x51, 0x3C}, "Lettre ÿ minuscule"),
            new FontCharacterDescriptor('Ö', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x3E}, "Lettre Ö majuscule"),
            new FontCharacterDescriptor('Ü', new byte[]{ 0x3E, 0x40, 0x40, 0x40, 0x3E}, "Lettre Ü majuscule"),
            new FontCharacterDescriptor('ø', new byte[]{ 0x1C, 0x32, 0x2A, 0x26, 0x1C}, "Caractère ø"),
            new FontCharacterDescriptor('£', new byte[]{ 0x00, 0x48, 0x7F, 0x49, 0x42}, "Caractère £"),
            new FontCharacterDescriptor('Ø', new byte[]{ 0x3E, 0x51, 0x49, 0x45, 0x3E}, "Caractère Ø"),
            new FontCharacterDescriptor('×', new byte[]{ 0x44, 0x28, 0x10, 0x28, 0x44}, "Caractère ×"),
            new FontCharacterDescriptor('ƒ', new byte[]{ 0x00, 0x44, 0x7F, 0x05, 0x00}, "Caractère ƒ"),
            new FontCharacterDescriptor('á', new byte[]{ 0x20, 0x54, 0x56, 0x55, 0x78}, "Lettre á minuscule"),
            new FontCharacterDescriptor('í', new byte[]{ 0x00, 0x04, 0x7E, 0x01, 0x00}, "Lettre í minuscule"),
            new FontCharacterDescriptor('ó', new byte[]{ 0x38, 0x44, 0x46, 0x45, 0x38}, "Lettre ó minuscule"),
            new FontCharacterDescriptor('ú', new byte[]{ 0x3C, 0x40, 0x42, 0x21, 0x7C}, "Lettre ú minuscule"),
            new FontCharacterDescriptor('ñ', new byte[]{ 0x7A, 0x11, 0x0A, 0x0C, 0x72}, "Lettre ñ minuscule"),
            new FontCharacterDescriptor('Ñ', new byte[]{ 0x7F, 0x04, 0x08, 0x10, 0x7F}, "Lettre Ñ majuscule"),
            new FontCharacterDescriptor('ª', new byte[]{ 0x48, 0x55, 0x55, 0x55, 0x5E}, "Caractère ª"),
            new FontCharacterDescriptor('º', new byte[]{ 0x4E, 0x51, 0x51, 0x51, 0x4E}, "Caractère º"),
            new FontCharacterDescriptor('¿', new byte[]{ 0x30, 0x48, 0x4D, 0x40, 0x20}, "Caractère ¿"),
            new FontCharacterDescriptor('®', new byte[]{ 0x41, 0x5D, 0x49, 0x45, 0x41}, "Caractère ®"),
            new FontCharacterDescriptor('¬', new byte[]{ 0x08, 0x08, 0x08, 0x08, 0x18}, "Caractère ¬"),
            new FontCharacterDescriptor('½', new byte[]{ 0x00, 0x00, 0x00, 0x00, 0x00}, "Caractère ½"),
            new FontCharacterDescriptor('¼', new byte[]{ 0x00, 0x00, 0x00, 0x00, 0x00}, "Caractère ¼"),
            new FontCharacterDescriptor('¡', new byte[]{ 0x00, 0x44, 0x7D, 0x40, 0x00}, "Caractère ¡"),
            new FontCharacterDescriptor('«', new byte[]{ 0x08, 0x36, 0x00, 0x08, 0x36}, "Caractère «"),
            new FontCharacterDescriptor('»', new byte[]{ 0x36, 0x08, 0x00, 0x36, 0x08}, "Caractère »"),
            new FontCharacterDescriptor('░', new byte[]{ 0x49, 0x00, 0x00, 0x49, 0x00}, "Caractère ░"),
            new FontCharacterDescriptor('▒', new byte[]{ 0x55, 0x2A, 0x55, 0x2A, 0x55}, "Caractère ▒"),
            new FontCharacterDescriptor('▓', new byte[]{ 0x7F, 0x7F, 0x7F, 0x7F, 0x7F}, "Caractère ▓"),
            new FontCharacterDescriptor('│', new byte[]{ 0x00, 0x00, 0x7F, 0x00, 0x00}, "Caractère │"),
            new FontCharacterDescriptor('┤', new byte[]{ 0x08, 0x08, 0x7F, 0x00, 0x00}, "Caractère ┤"),
            new FontCharacterDescriptor('Á', new byte[]{ 0x7E, 0x11, 0x11, 0x11, 0x7E}, "Lettre Á majuscule"),
            new FontCharacterDescriptor('Â', new byte[]{ 0x7E, 0x11, 0x11, 0x11, 0x7E}, "Lettre Â majuscule"),
            new FontCharacterDescriptor('À', new byte[]{ 0x7E, 0x11, 0x11, 0x11, 0x7E}, "Lettre À majuscule"),
            new FontCharacterDescriptor('©', new byte[]{ 0x41, 0x5D, 0x55, 0x55, 0x41}, "Caractère ©"),
            new FontCharacterDescriptor('╣', new byte[]{ 0x14, 0x77, 0x00, 0x7F, 0x00}, "Caractère ╣"),
            new FontCharacterDescriptor('║', new byte[]{ 0x00, 0x7F, 0x00, 0x7F, 0x00}, "Caractère ║"),
            new FontCharacterDescriptor('╗', new byte[]{ 0x14, 0x74, 0x04, 0x7C, 0x00}, "Caractère ╗"),
            new FontCharacterDescriptor('╝', new byte[]{ 0x14, 0x17, 0x10, 0x1F, 0x00}, "Caractère ╝"),
            new FontCharacterDescriptor('¢', new byte[]{ 0x00, 0x1C, 0x36, 0x14, 0x00}, "Caractère ¢"),
            new FontCharacterDescriptor('¥', new byte[]{ 0x01, 0x2A, 0x7C, 0x2A, 0x01}, "Caractère ¥"),
            new FontCharacterDescriptor('┐', new byte[]{ 0x08, 0x08, 0x78, 0x00, 0x00}, "Caractère ┐"),
            new FontCharacterDescriptor('└', new byte[]{ 0x00, 0x00, 0x0F, 0x08, 0x08}, "Caractère └"),
            new FontCharacterDescriptor('┴', new byte[]{ 0x08, 0x08, 0x0F, 0x08, 0x08}, "Caractère ┴"),
            new FontCharacterDescriptor('┬', new byte[]{ 0x08, 0x08, 0x7F, 0x08, 0x08}, "Caractère ┬"),
            new FontCharacterDescriptor('├', new byte[]{ 0x00, 0x00, 0x7F, 0x08, 0x08}, "Caractère ├"),
            new FontCharacterDescriptor('─', new byte[]{ 0x00, 0x08, 0x08, 0x08, 0x00}, "Caractère ─"),
            new FontCharacterDescriptor('┼', new byte[]{ 0x08, 0x08, 0x7F, 0x08, 0x08}, "Caractère ┼"),
            new FontCharacterDescriptor('ã', new byte[]{ 0x22, 0x55, 0x56, 0x54, 0x7A}, "Lettre ã minuscule"),
            new FontCharacterDescriptor('Ã', new byte[]{ 0x7E, 0x11, 0x11, 0x11, 0x7E}, "Lettre Ã majuscule"),
            new FontCharacterDescriptor('╚', new byte[]{ 0x00, 0x1F, 0x10, 0x17, 0x14}, "Caractère ╚"),
            new FontCharacterDescriptor('╔', new byte[]{ 0x00, 0x7C, 0x04, 0x74, 0x14}, "Caractère ╔"),
            new FontCharacterDescriptor('╩', new byte[]{ 0x14, 0x17, 0x10, 0x17, 0x14}, "Caractère ╩"),
            new FontCharacterDescriptor('╦', new byte[]{ 0x14, 0x74, 0x04, 0x74, 0x14}, "Caractère ╦"),
            new FontCharacterDescriptor('╠', new byte[]{ 0x00, 0x7F, 0x00, 0x77, 0x14}, "Caractère ╠"),
            new FontCharacterDescriptor('═', new byte[]{ 0x14, 0x14, 0x14, 0x14, 0x14}, "Caractère ═"),
            new FontCharacterDescriptor('╬', new byte[]{ 0x14, 0x77, 0x00, 0x77, 0x14}, "Caractère ╬"),
            new FontCharacterDescriptor('¤', new byte[]{ 0x22, 0x1C, 0x14, 0x1C, 0x22}, "Caractère ¤"),
            new FontCharacterDescriptor('ð', new byte[]{ 0x3C, 0x45, 0x49, 0x49, 0x3E}, "Caractère ð"),
            new FontCharacterDescriptor('Ð', new byte[]{ 0x08, 0x7F, 0x49, 0x41, 0x3E}, "Caractère Ð"),
            new FontCharacterDescriptor('Ê', new byte[]{ 0x7F, 0x49, 0x49, 0x49, 0x41}, "Lettre Ê majuscule"),
            new FontCharacterDescriptor('Ë', new byte[]{ 0x7F, 0x49, 0x49, 0x49, 0x41}, "Lettre Ë majuscule"),
            new FontCharacterDescriptor('È', new byte[]{ 0x7F, 0x49, 0x49, 0x49, 0x41}, "Lettre È majuscule"),
            new FontCharacterDescriptor('ı', new byte[]{ 0x00, 0x00, 0x1C, 0x00, 0x00}, "Caractère ı"),
            new FontCharacterDescriptor('Í', new byte[]{ 0x00, 0x41, 0x7F, 0x41, 0x00}, "Lettre Í majuscule"),
            new FontCharacterDescriptor('Î', new byte[]{ 0x00, 0x41, 0x7F, 0x41, 0x00}, "Lettre Î majuscule"),
            new FontCharacterDescriptor('Ï', new byte[]{ 0x00, 0x41, 0x7F, 0x41, 0x00}, "Lettre Ï majuscule"),
            new FontCharacterDescriptor('┘', new byte[]{ 0x08, 0x08, 0x0F, 0x00, 0x00}, "Caractère ┘"),
            new FontCharacterDescriptor('┌', new byte[]{ 0x00, 0x00, 0x78, 0x08, 0x08}, "Caractère ┌"),
            new FontCharacterDescriptor('█', new byte[]{ 0x7F, 0x7F, 0x7F, 0x7F, 0x7F}, "Caractère █"),
            new FontCharacterDescriptor('▄', new byte[]{ 0x70, 0x70, 0x70, 0x70, 0x70}, "Caractère ▄"),
            new FontCharacterDescriptor('¦', new byte[]{ 0x00, 0x00, 0x36, 0x00, 0x00}, "Caractère ¦"),
            new FontCharacterDescriptor('Ì', new byte[]{ 0x00, 0x41, 0x7F, 0x41, 0x00}, "Caractère Ì"),
            new FontCharacterDescriptor('▀', new byte[]{ 0x07, 0x07, 0x07, 0x07, 0x07}, "Caractère ▀"),
            new FontCharacterDescriptor('Ó', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x3E}, "Lettre Ó majuscule"),
            new FontCharacterDescriptor('ß', new byte[]{ 0x7E, 0x01, 0x49, 0x56, 0x20}, "Lettre ß majuscule"),
            new FontCharacterDescriptor('Ô', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x3E}, "Lettre Ô majuscule"),
            new FontCharacterDescriptor('Ò', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x3E}, "Lettre Ò majuscule"),
            new FontCharacterDescriptor('õ', new byte[]{ 0x3A, 0x45, 0x46, 0x44, 0x3A}, "Lettre õ"),
            new FontCharacterDescriptor('Õ', new byte[]{ 0x3E, 0x41, 0x41, 0x41, 0x3E}, "Lettre Õ majuscule"),
            new FontCharacterDescriptor('µ', new byte[]{ 0x7E, 0x08, 0x08, 0x08, 0x0F}, "Caractère µ"),
            new FontCharacterDescriptor('þ', new byte[]{ 0x00, 0x7F, 0x14, 0x1C, 0x00}, "Caractère þ"),
            new FontCharacterDescriptor('Þ', new byte[]{ 0x00, 0x3E, 0x14, 0x1C, 0x00}, "Caractère Þ"),
            new FontCharacterDescriptor('Ú', new byte[]{ 0x3F, 0x40, 0x40, 0x40, 0x3F}, "Lettre Ú majuscule"),
            new FontCharacterDescriptor('Û', new byte[]{ 0x3F, 0x40, 0x40, 0x40, 0x3F}, "Lettre Û majuscule"),
            new FontCharacterDescriptor('Ù', new byte[]{ 0x3F, 0x40, 0x40, 0x40, 0x3F}, "Lettre Ù majuscule"),
            new FontCharacterDescriptor('ý', new byte[]{ 0x0C, 0x50, 0x52, 0x51, 0x3C}, "Lettre ý minuscule"),
            new FontCharacterDescriptor('Ý', new byte[]{ 0x07, 0x08, 0x70, 0x08, 0x07}, "Lettre Ý majuscule"),
            new FontCharacterDescriptor('¯', new byte[]{ 0x00, 0x02, 0x02, 0x02, 0x00}, "Caractère ¯"),
            new FontCharacterDescriptor('´', new byte[]{ 0x00, 0x00, 0x02, 0x01, 0x00}, "Caractère ´"),
            new FontCharacterDescriptor('±', new byte[]{ 0x00, 0x48, 0x5C, 0x48, 0x00}, "Caractère ±"),
            new FontCharacterDescriptor('‗', new byte[]{ 0x50, 0x50, 0x50, 0x50, 0x50}, "Caractère ‗"),
            new FontCharacterDescriptor('¾', new byte[]{ 0x00, 0x00, 0x00, 0x00, 0x00}, "Caractère ¾"),
            new FontCharacterDescriptor('¶', new byte[]{ 0x06, 0x09, 0x7F, 0x01, 0x7F}, "Caractère ¶"),
            new FontCharacterDescriptor('§', new byte[]{ 0x00, 0x1F, 0x55, 0x7C, 0x00}, "Caractère §"),
            new FontCharacterDescriptor('÷', new byte[]{ 0x00, 0x08, 0x2A, 0x08, 0x00}, "Caractère ÷"),
            new FontCharacterDescriptor('¸', new byte[]{ 0x00, 0x00, 0x40, 0x20, 0x00}, "Caractère ¸"),
            new FontCharacterDescriptor('°', new byte[]{ 0x00, 0x00, 0x01, 0x00, 0x00}, "Caractère °"),
            new FontCharacterDescriptor('¨', new byte[]{ 0x00, 0x01, 0x00, 0x01, 0x00}, "Caractère ¨"),
            new FontCharacterDescriptor('·', new byte[]{ 0x00, 0x00, 0x08, 0x00, 0x00}, "Caractère ·"),
            new FontCharacterDescriptor('¹', new byte[]{ 0x00, 0x12, 0x1F, 0x10, 0x00}, "Caractère ¹"),
            new FontCharacterDescriptor('³', new byte[]{ 0x00, 0x15, 0x15, 0x1F, 0x00}, "Caractère ³"),
            new FontCharacterDescriptor('²', new byte[]{ 0x00, 0x1D, 0x15, 0x17, 0x00}, "Caractère ²"),
            new FontCharacterDescriptor('■', new byte[]{ 0x00, 0x1C, 0x1C, 0x1C, 0x00}, "Caractère ■"),
            new FontCharacterDescriptor('€', new byte[]{ 0x14, 0x3E, 0x55, 0x55, 0x55}, "Caractère €")

        };

    }

}
