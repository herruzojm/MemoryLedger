using System.Security.Cryptography;
using System.Text;

namespace MemoryLedgerApp.Utilities;

public static class EncryptionService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public static byte[] Encrypt(string plainText, string password)
    {
        using var aes = Aes.Create();
        aes.KeySize = KeySize * 8;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        aes.Key = DeriveKey(password, salt);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Combine(salt, aes.IV, cipherBytes);
    }

    public static string Decrypt(byte[] encryptedData, string password)
    {
        var salt = encryptedData[..SaltSize];
        var iv = encryptedData[SaltSize..(SaltSize + 16)];
        var cipherText = encryptedData[(SaltSize + 16)..];

        using var aes = Aes.Create();
        aes.KeySize = KeySize * 8;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = DeriveKey(password, salt);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(KeySize);
    }

    private static byte[] Combine(params byte[][] arrays)
    {
        var totalLength = arrays.Sum(a => a.Length);
        var result = new byte[totalLength];
        var offset = 0;
        foreach (var array in arrays)
        {
            Buffer.BlockCopy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }
        return result;
    }
}
