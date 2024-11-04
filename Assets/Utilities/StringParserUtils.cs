namespace Game.Core.Utilities
{
    public static class StringParserUtils
    {
        public static byte AsByte(this string s, byte defValue = 0)
        {
            if (byte.TryParse(s, out byte res))
            {
                return res;
            }
            return defValue;
        }

        public static short AsShort(this string s, short defValue = 0)
        {
            if (short.TryParse(s, out short res))
            {
                return res;
            }
            return defValue;
        }

        public static int AsInt(this string s, int defValue = 0)
        {
            if (int.TryParse(s, out int res))
            {
                return res;
            }
            return defValue;
        }

        public static ulong AsUlong(this string s, ulong defValue = 0)
        {
            if (ulong.TryParse(s, out ulong res))
            {
                return res;
            }
            return defValue;
        }

        public static float AsFloat(this string s, float defValue = 0.0f)
        {
            if (float.TryParse(s, out float res))
            {
                return res;
            }
            return defValue;
        }

        public static double AsDouble(this string s, double defValue = 0.0)
        {
            if (double.TryParse(s, out double res))
            {
                return res;
            }
            return defValue;
        }

        public static decimal AsDecimal(this string s, decimal defValue = decimal.Zero)
        {
            if (decimal.TryParse(s, out decimal res))
            {
                return res;
            }
            return defValue;
        }
    }
}
