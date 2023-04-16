using System.Security.Cryptography;

namespace CryptoLib;

public class CryptoHelper
{
    public static string SymmetricAlgorithmName { get; set; }

    static CryptoHelper()
    {
        SymmetricAlgorithmName = "AES";
    }


    private static SymmetricAlgorithm GetAlgorithm()
    {
        SymmetricAlgorithm? symAlg = SymmetricAlgorithm.Create(SymmetricAlgorithmName);

        if(symAlg == null)
        {            
            throw new CryptographicException(string.Format("Error Creating Algorithem '{0}'", SymmetricAlgorithmName));
        }

        return symAlg;
    }


    /// <summary>
    /// Creates a new random crypto key based off the algorithm
    /// </summary>
    /// <returns>a crypto key object</returns>
    public static CryptoKey GenerateCryptoKey()
    {
        byte[] key = null;
        byte[] iv = null;

        using (SymmetricAlgorithm alg = GetAlgorithm())
        {
            key = alg.Key;
            iv = alg.IV;
        }

        return new CryptoKey(key, iv);
    }

    /// <summary>
    /// Helper method so we know the default Keysize of the algorithm
    /// </summary>
    public static int KeySize
    {
        get
        {
            int keySize = 0;
            using (SymmetricAlgorithm alg = GetAlgorithm())
            {
                keySize = alg.KeySize;
            }
            return keySize;
        }
    }

    /// <summary>
    /// Converts a string into a base 64 encoded string
    /// </summary>
    /// <param name="plainText">the plain text striing to encrypt</param>
    /// <param name="cryptoKey">the key to use with the encryption</param>
    /// <returns>a base 64 encoded string</returns>
    public static string EncryptStringToString(string plainText, CryptoKey cryptoKey)
    {
        byte[] cipherText = EncryptStringToBytes(plainText, cryptoKey);
        return Convert.ToBase64String(cipherText);
    }

    /// <summary>
    /// Converts a base64 encoded string back to a clear text string
    /// </summary>
    /// <param name="cipherText">the base64 encoded cipherdata </param>
    /// <param name="cryptoKey">the key to decypt the string</param>
    /// <returns>a cleartext string from the ciphertext</returns>
    public static string DecryptoStringToString(string cipherText,CryptoKey cryptoKey){
        byte[] cipherData = Convert.FromBase64String(cipherText);
        return DecryptStringFromBytes(cipherData, cryptoKey);
    }

    /// <summary>
    /// Convert a string into encrypted data
    /// </summary>
    /// <param name="plainText">the string to encrypt</param>
    /// <param name="cryptoKey">the key to used in the encryption</param>
    /// <returns>cipherdata of the string</returns>
    /// <exception cref="ArgumentNullException">if the plaintext string is null/empty</exception>
    public static byte[] EncryptStringToBytes(string plainText, CryptoKey cryptoKey)
    {
        if (String.IsNullOrEmpty(plainText))
        {
            throw new ArgumentNullException("plaintext striing is null/empty");
        }

        byte[] encrypted = null;
        using (SymmetricAlgorithm alg = GetAlgorithm())
        {
            alg.Key = cryptoKey.Key;
            alg.IV = cryptoKey.IV;

            ICryptoTransform encryptor = alg.CreateEncryptor(alg.Key, alg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

    /// <summary>
    /// Converts cipher data back to a cleartext string
    /// </summary>
    /// <param name="cipherData">the ciphedata(text) to decrypt</param>
    /// <param name="cryptoKey">the key needed to decrypt the data</param>
    /// <returns>the original string that was encrypted</returns>
    /// <exception cref="ArgumentNullException">if the cipherData is null/empty</exception>
    private static string DecryptStringFromBytes(byte[] cipherData, CryptoKey cryptoKey)
    {
        if (cipherData.IsNullOrEmpty())
        {
            throw new ArgumentNullException("cipherText is null/empty");
        }

        string plaintext = null;

        using (SymmetricAlgorithm alg = GetAlgorithm())
        {
            alg.Key = cryptoKey.Key;
            alg.IV = cryptoKey.IV;

            ICryptoTransform decryptor = alg.CreateDecryptor(alg.Key, alg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }
}
