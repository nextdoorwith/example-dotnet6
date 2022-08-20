using System.Security.Cryptography;

namespace AesBasicExample
{
    public static class AesUtils
    {
        /// <summary>
        /// AESキーサイズ(ビット数)
        /// </summary>
        private const int AesKeySize = 256;

        /// <summary>
        /// AESキーサイズ(バイト数)
        /// </summary>
        private const int AesKeyBytes = AesKeySize / 8;

        /// <summary>
        /// PBKDF2用ソルト長
        /// </summary>
        // NIST(800-132)では、128ビットを推奨
        private const int Pbkdf2SaltBytes = 128 / 8;

        /// <summary>
        /// PBKDF2用イテレーション回数
        /// </summary>
        // NIST(800-132)では、最小1,000回、できれば10,000,000回を推奨
        // OWASPではSHA256の場合、310,000回を推奨
        private const int Pbkdf2Iteration = 1000; // 既定は1,000回

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

        /// <summary>
        /// パスワード用のソルトを生成する。
        /// </summary>
        /// <returns>ソルト</returns>
        public static byte[] CreatePasswordSalt()
        {
            // 予測が難しい暗号用乱数ジェネレータを使用する必要あり。
            // (RNGCryptoServiceProviderは非推奨になった)
            return RandomNumberGenerator.GetBytes(Pbkdf2SaltBytes);
        }

        /// <summary>
        /// パスワードからキーを生成する。
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <param name="passwordSalt">パスワード用ソルト</param>
        /// <returns>キー</returns>
        public static byte[] CreateKeyFromPassword(string password, byte[] passwordSalt)
        {
            // 内部で使用するハッシュ関数として、NIST推奨で広く使われるSHA-256を使用
            // (既定はSHA-1だが安全性が低いため。CA5379も参考のこと。)
            var key = new Rfc2898DeriveBytes(password, passwordSalt,
                Pbkdf2Iteration, HashAlgorithmName.SHA256);
            return key.GetBytes(AesKeyBytes);
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

        // ストリーム操作

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

        // その他

        private static Aes CreateInstance()
        {
            var aes = Aes.Create();
            aes.BlockSize = 128; // 既定値(AESの有効なブロックサイズは128bit固定)
            aes.KeySize = AesKeySize; // 既定値は256(128, 192, 256bitを使用可)
            aes.Mode = CipherMode.CBC; // 既定値
            aes.Padding = PaddingMode.PKCS7; // 既定値
            return aes;
        }
    }
}
