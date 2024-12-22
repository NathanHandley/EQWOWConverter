using System.Text;

namespace LanternExtractor.EQ.Wld
{
    /// <summary>
    /// Decodes WLD strings using the hash key
    /// </summary>
    public static class WldStringDecoder
    {
        /// <summary>
        /// The key used to decode the string
        /// </summary>
        private static readonly byte[] HashKey = {0x95, 0x3A, 0xC5, 0x2A, 0x95, 0x7A, 0x95, 0x6A};

        /// <summary>
        /// Returns a decoded string
        /// </summary>
        /// <param name="encodedString">The encoded string to be decoded</param>
        /// <returns>The decoded string</returns>
        public static string DecodeString(byte[] encodedString)
        {
            for (int i = 0; i < encodedString.Length; ++i)
            {
                encodedString[i] ^= HashKey[i % 8];
            }

            return Encoding.UTF8.GetString(encodedString, 0, encodedString.Length);
        }
    }
}