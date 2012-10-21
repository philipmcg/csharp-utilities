using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Text.RegularExpressions;


namespace Utilities
{

    /// <summary>
    /// Represents a repository of Colors which are fetched by their hex string representation.
    /// </summary>
    public class HexColorManager : Dictionary<string, Color>
    {
        public new Color this[string key]
        {
            get
            {
                Color ret;
                if (!base.TryGetValue(key, out ret))
                    base.Add(key, ColorTools.ColorFromHexString(key));
                return base[key];
            }
        }
    }

    public sealed class ColorTools
    {
        public static Color ColorFromArgbString(string value, char delim)
        {
            Color color;
            string[] srgb = value.Split(delim);
            int[] rgb = new int[srgb.Length];
            for (int k = 0; k < srgb.Length; k++)
            {
                rgb[k] = Convert.ToInt32(srgb[k]);
            }
            if (srgb.Length == 3)
                color = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            else if (srgb.Length == 4)
                color = Color.FromArgb(rgb[0], rgb[1], rgb[2], rgb[3]);
            else
                color = Color.Pink;
            return color;
        }
        public static Color RandomColor()
        {
            return Color.FromArgb(255, Rand.Int(256), Rand.Int(256), Rand.Int(256));
        }
        public static Color ColorFromHexString(string hexColor)
        {
            string hc = ExtractHexDigits(hexColor);
            if (hc.Length != 6 && hc.Length != 8)
            {
                return Color.Empty;
            }

            string a = "ff";
            string r;
            string g;
            string b;

            if (hc.Length == 8)
            {
                a = hc.Substring(0, 2);
                r = hc.Substring(2, 2);
                g = hc.Substring(4, 2);
                b = hc.Substring(6, 2);
            }
            else
            {
                r = hc.Substring(0, 2);
                g = hc.Substring(2, 2);
                b = hc.Substring(4, 2);
            }
            Color color = Color.Empty;
            
            try
            {
                int ai
                   = Int32.Parse(a, System.Globalization.NumberStyles.HexNumber);
                int ri
                   = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi
                   = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi
                   = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                color = Color.FromArgb(ai, ri, gi, bi);
            }
            catch
            {
                return Color.Empty;
            }
            return color;
        }


        public static string ExtractHexDigits(string input)
        {
            Regex isHexDigit
               = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            string newnum = "";
            foreach (char c in input)
            {
                if (isHexDigit.IsMatch(c.ToString()))
                    newnum += c.ToString();
            }
            return newnum;
        }


        static char[] hexDigits = {
     '0', '1', '2', '3', '4', '5', '6', '7',
     '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static string ColorToHexString(Color color)
        {
            byte[] bytes = new byte[3];
            bytes[0] = color.R;
            bytes[1] = color.G;
            bytes[2] = color.B;
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }
    }
}
