using System.Security.Cryptography;

namespace KdfBasicExample
{
    public static class Pbkdf2Example
    {
        /// <summary>
        /// パスワード用のソルトを生成する。
        /// </summary>
        /// <returns>ソルト</returns>
        public static byte[] CreatePasswordSalt()
        {
            // - 予測が難しい暗号用乱数ジェネレータを使用する必要あり。
            // - NIST(800-132)では、128ビットを推奨。
            // - RNGCryptoServiceProviderは非推奨になった。
            return RandomNumberGenerator.GetBytes(128 / 8);
        }

        /// <summary>
        /// パスワードからキーを生成する。
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <param name="passwordSalt">パスワード用ソルト</param>
        /// <returns>キー</returns>
        public static byte[] CreateKeyFromPassword(string password, byte[] passwordSalt)
        {
            // PBKDF2を使用してキーを生成
            // - イテレーション回数
            //   - NIST(800-132)では、最小1,000回、できれば10,000,000回を推奨
            //   - OWASPではSHA256の場合、310,000回を推奨
            //   - Microsoftでは、100,000回以上を推奨(コード分析: CA5387)
            // - 内部で使用するハッシュ関数として、NIST推奨で広く使われるSHA-256を使用
            //   - 既定はSHA-1だが安全性が低い(コード分析: CA5379)
            using var key = new Rfc2898DeriveBytes(
                password,
                passwordSalt,
                310_000,
                HashAlgorithmName.SHA256
            );
            return key.GetBytes(256/8);
        }

    }
}
