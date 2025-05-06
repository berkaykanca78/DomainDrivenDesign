using System.Security.Cryptography;
using System.Text;

namespace DomainDrivenDesign.WebApi.Helpers;
public class EncryptionHelper
{
    /* Example:
      var key = EncryptionHelper.GenerateKey();
      string original = "merhaba dünya";
      string encrypted = EncryptionHelper.Encrypt(original, key);
      string decrypted = EncryptionHelper.Decrypt(encrypted, key);
    */

    public static string GenerateKey()
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }

    public static string Encrypt(string plainText, string keyBase64)
    {
        byte[] key = Convert.FromBase64String(keyBase64);
        
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV(); // Her şifreleme için yeni bir IV oluştur
            
            using (var encryptor = aes.CreateEncryptor())
            using (var memoryStream = new MemoryStream())
            {
                // IV'yi şifreli metne ekle
                memoryStream.Write(aes.IV, 0, aes.IV.Length);
                
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(plainText);
                }
                
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public static string Decrypt(string cipherTextBase64, string keyBase64)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);
        byte[] key = Convert.FromBase64String(keyBase64);
        
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            
            // IV'yi şifreli metinden al (AES için ilk 16 byte)
            byte[] iv = new byte[aes.BlockSize / 8];
            Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;
            
            using (var decryptor = aes.CreateDecryptor())
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                {
                    // IV sonrası verileri şifresi çöz
                    cryptoStream.Write(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
                }
                
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
