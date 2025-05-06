using System.Text;
using Sodium;

namespace DomainDrivenDesign.WebApi.Helpers;
public class EncryptionHelper
{
    /* Example:
      var key1 = EncryptionHelper.GenerateKey();
      string original = "merhaba dünya";
      string encrypted = EncryptionHelper.Encrypt(original, key1);
      string decrypted = EncryptionHelper.Decrypt(encrypted, key1);
    */

    public static byte[] GenerateKey()
    {
        return SecretBox.GenerateKey();
    }

    public static string Encrypt(string plaintext, byte[] key)
    {
        byte[] nonce = SecretBox.GenerateNonce(); // 24-byte nonce
        byte[] messageBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] cipher = SecretBox.Create(messageBytes, nonce, key);

        byte[] combined = new byte[nonce.Length + cipher.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(cipher, 0, combined, nonce.Length, cipher.Length);

        return Convert.ToBase64String(combined);
    }

    public static string Decrypt(string encryptedBase64, byte[] key)
    {
        byte[] combined = Convert.FromBase64String(encryptedBase64);
        byte[] nonce = new byte[24];
        byte[] cipher = new byte[combined.Length - nonce.Length];

        Buffer.BlockCopy(combined, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(combined, nonce.Length, cipher, 0, cipher.Length);

        byte[] decrypted = SecretBox.Open(cipher, nonce, key);
        return Encoding.UTF8.GetString(decrypted);
    }
}
