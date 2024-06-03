using System.Security.Cryptography;
using System.Text;

namespace Mcsg.Lib.Common.Helpers
{
    public static class CryptoHelper
    {
        private static byte[] salt = new byte[] { 0x23, 0x34, 0x64, 0xee, 0xe2, 0x4d, 0xF5, 0x64, 0x76, 0x15, 0x54, 0x65, 0x76 };
        public static string Encrypt(string payload, string encryptionKey)
        {
            string result = String.Empty;
            try
            {
                byte[] clearBytes = Encoding.UTF8.GetBytes(payload);
                using (RijndaelManaged encryptor = new RijndaelManaged())
                {
                    encryptor.KeySize = 256;
                    encryptor.BlockSize = 128;

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(encryptionKey, salt);
                    encryptor.Key = key.GetBytes(encryptor.KeySize / 8);
                    encryptor.IV = key.GetBytes(encryptor.BlockSize / 8);
                    encryptor.Mode = CipherMode.CBC;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        result = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static string Decrypt(string cipherTxt, string encryptionKey)
        {
            string result = String.Empty;
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherTxt);

                using (RijndaelManaged encryptor = new RijndaelManaged())
                {
                    encryptor.KeySize = 256;
                    encryptor.BlockSize = 128;

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(encryptionKey, salt);
                    encryptor.Key = key.GetBytes(encryptor.KeySize / 8);
                    encryptor.IV = key.GetBytes(encryptor.BlockSize / 8);
                    encryptor.Mode = CipherMode.CBC;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        result = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                var c = ex;
            }
            return result;
        }
    }
}
