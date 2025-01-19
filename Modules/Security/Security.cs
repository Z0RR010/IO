using IO.Modules.ResourceManager;
using System.Security.Cryptography;
using System.Text;

namespace IO.Modules.Security
{
    public static class Security
    {

        private static byte[] customIV = { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };

        public static string hashData(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashedBytes = sha256.ComputeHash(inputBytes);
                string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hashedString;
            }
        }

        private static byte[] stringKeyToBytes(string key)
        {
			byte[] keyBytes = key
               .Split('-')
               .Select(hex => Convert.ToByte(hex, 16))
               .ToArray();
            return keyBytes;
        }

        public static string encryptPESEL(string input, string key)
        {
            using (Aes myAes = Aes.Create())
            {
				UserExecuter userExecuter = new UserExecuter();
                myAes.KeySize = 256;
                myAes.IV = customIV;
                myAes.Key = stringKeyToBytes(key);
				byte[] encrypted = EncryptStringToBytes_Aes(input, myAes.Key, myAes.IV);
				return BitConverter.ToString(encrypted);
            }
        }

        public static string decryptPESEL(string input, User user)
        {
            using (Aes myAes = Aes.Create())
            {
                UserExecuter userExecuter = new UserExecuter();
                myAes.KeySize = 256;
                myAes.IV = customIV;
                myAes.Key = stringKeyToBytes(userExecuter.GetEncryptionKey(user.Email));
                byte[] inputBytes = stringKeyToBytes(input);
                return DecryptStringFromBytes_Aes(inputBytes, myAes.Key, myAes.IV);
            }
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
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
}