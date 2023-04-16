using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib
{
    public class CryptoKey
    {
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }


        public CryptoKey(string key)
        {
            string[] rawKey = key.Split("|");
            if(rawKey.Length != 2)
            {
                throw new FormatException("format for Key <IV>|<Key> is invalid");
            }

            IV = Convert.FromBase64String(rawKey[0]);
            Key = Convert.FromBase64String(rawKey[1]);
        }

        public CryptoKey(byte[] key, byte[] iv)
        {
            if(key.IsNullOrEmpty())
            {
                throw new ArgumentNullException("key is null/empty");
            }
            if (iv.IsNullOrEmpty())
            {
                throw new ArgumentNullException("IV is null/empty");
            }

            this.Key = key;
            this.IV = iv;
        }

        /// <summary>
        /// Combines a second key via an XOR operation of the Key and IV values
        /// </summary>
        /// <param name="secondKey"></param>
        /// <exception cref="ArgumentNullException">if the second key is null</exception>
        public void CombineKey(CryptoKey secondKey)
        {
            if (secondKey == null)
            {
                throw new ArgumentNullException("second key is null");
            }

            BitArray key1 = new BitArray(Key);
            BitArray key2 = new BitArray(secondKey.Key);

            Key = key1.Xor(key2).ToByteArray();

            BitArray iv1 = new BitArray(IV);
            BitArray iv2 = new BitArray(secondKey.IV);

            IV = iv1.Xor(iv2).ToByteArray();
        }

        /// <summary>
        /// Combines a set of keys (via XOR combination of the key/IV values)
        /// </summary>
        /// <param name="keys">the list of keys to combine</param>
        /// <exception cref="ArgumentNullException">if the set of keys is null</exception>
        public void CombineKey(IEnumerable<CryptoKey> keys)
        {
            if(keys == null)
            {
                throw new ArgumentNullException("key array is null");
            }
            foreach(CryptoKey key in keys)
            {
                CombineKey(key);
            }
        }

        public override string ToString()
        {
            return $"{Convert.ToBase64String(IV)}|{Convert.ToBase64String(Key)}";
        }
    }
}
