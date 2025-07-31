using UnityEngine;
using System.Text.RegularExpressions;

namespace com.draconianmarshmallows.geofire.util
{
    public class Base32Utils
    {
        /* number of bits per base 32 character */
        public const int BITS_PER_BASE32_CHAR = 5;

        private const string BASE32_CHARS = "0123456789bcdefghjkmnpqrstuvwxyz";

        public static char valueToBase32Char(int value)
        {
            if (value < 0 || value >= BASE32_CHARS.Length)
            {
                throw new UnityException("Not a valid base32 value: " + value);
            }
            return BASE32_CHARS[value];
        }

        public static int base32CharToValue(char base32Char)
        {
            int value = BASE32_CHARS.IndexOf(base32Char);

            if (value == -1)
            {
                throw new UnityException("Not a valid base32 char: " + base32Char);
            }
            else {
                return value;
            }
        }

        public static bool isValidBase32String(string str)
        {
            return Regex.IsMatch(str, "^[" + BASE32_CHARS + "]*$");
        }
    }
}
