using System.Security.Cryptography;

namespace AesBasicExample
{
    public static class AesExample
    {
        /// <summary>
        /// ランダムなキーを生成する。
        /// </summary>
        /// <returns>キー</returns>
        public static byte[] CreateKey()
        {
            // 既定では256ビット(32バイト)
            using var aes = CreateInstance();
            aes.GenerateKey();
            return aes.Key;
        }

        /// <summary>
        /// 初期ベクトルを生成する。
        /// </summary>
        /// <returns>初期ベクトル</returns>
        public static byte[] CreateIv()
        {
            // AES(CBC)ではブロック長と同じ128ビット(64バイト)
            // ※ブロック長はAESの仕様として128ビット固定
            using var aes = CreateInstance();
            aes.GenerateIV();
            return aes.IV;
        }

        public static void Encrypt(Stream output, byte[] cleartext, byte[] key, byte[] iv)
        {
            using var aes = CreateInstance();
            aes.Key = key;
            aes.IV = iv;
            using var encryptor = aes.CreateEncryptor();
            using var cryptoStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(cleartext, 0, cleartext.Length);
            cryptoStream.Flush();
        }

        public static byte[] Decrypt(Stream input, byte[] key, byte[] iv)
        {
            using var aes = CreateInstance();
            aes.Key = key;
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            using var cryptoStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read);
            using var output = new MemoryStream();
            cryptoStream.CopyTo(output);
            return output.ToArray();
        }

        public static byte[] Encrypt(byte[] cleartext, byte[] key, byte[] iv)
        {
            using var aes = CreateInstance();
            aes.Key = key;
            aes.IV = iv;
            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(cleartext, 0, cleartext.Length);
        }

        public static byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
        {
            using var aes = CreateInstance();
            aes.Key = key;
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        }


        // その他

        private static Aes CreateInstance()
        {
            var aes = Aes.Create();
            aes.BlockSize = 128; // 既定値(AESの有効なブロックサイズは128bit固定)
            aes.KeySize = 256; // 既定値は256(128, 192, 256bitを使用可)
            aes.Mode = CipherMode.CBC; // 既定値
            aes.Padding = PaddingMode.PKCS7; // 既定値
            return aes;
        }
    }
}
