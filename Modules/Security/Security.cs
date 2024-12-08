using System.Security.Cryptography;
using System.Text;

namespace IO.Modules.Security
{
    public static class Security
    {
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

        public static string encryptPESEL(string input, User user)
        {
			using (Aes myAes = Aes.Create())
			{
				myAes.KeySize = 256;
				// myAes.Key = to do
				byte[] encrypted = EncryptStringToBytes_Aes(input, myAes.Key, myAes.IV);
				return BitConverter.ToString(encrypted);
			}
        }

		public static string decryptPESEL(string input, User user)
		{
			using (Aes myAes = Aes.Create())
			{
				myAes.KeySize = 256;
                // myAes.Key = to do
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
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