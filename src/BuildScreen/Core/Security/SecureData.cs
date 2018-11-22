using System;
using System.Security.Cryptography;
using System.Text;

namespace BuildScreen.Core.Security
{
    public static class SecureData
    {
        static readonly byte[] Entropy = Encoding.Unicode.GetBytes("OP¥KÚàDVÀªÖùÜ6Í,¾êEÄ5pèAÄ+f#ék¡úÛvÆÊo¨°N0ZUÝð");

        public static string Encrypt(string decryptedData)
        {
            byte[] encryptedData = ProtectedData.Protect(Encoding.Unicode.GetBytes(decryptedData), Entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return encryptedData;

            byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), Entropy, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(decryptedData);
        }
    }
}
