using System.Collections;

namespace CryptoLib
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Helper method to convert a BitArray back to a byte[] 
        /// </summary>
        /// <param name="bits">the bitarray object to convert</param>
        /// <returns>a byte[] from thge BitArray data</returns>
        /// <exception cref="ArgumentNullException">if the BitArray is null</exception>
        public static byte[] ToByteArray(this BitArray bits)
        {
            if(bits == null)
            {
                throw new ArgumentNullException("bit array is null");
            }

            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        /// <summary>
        /// Dumb helper method to check if an array is null/empty
        /// </summary>
        /// <typeparam name="T">array type</typeparam>
        /// <param name="array">array to check</param>
        /// <returns>true if the array is null or has a zero length, false otherwise</returns>
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }
    }
}
