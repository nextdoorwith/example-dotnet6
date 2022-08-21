using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace AesBasicExample
{
    public class AesExampleTest
    {
        // テストで使用する平文例
        private const string TestText = @"１２３４５６７８９０壱弐参四五六七八九〇";

        // テストで使用する平文例(UTF-8バイト列)
        private static readonly byte[] TestCleartext = Encoding.UTF8.GetBytes(TestText);

        private readonly ITestOutputHelper _logger;

        public AesExampleTest(ITestOutputHelper logger)
            => _logger = logger;

        [Fact(DisplayName = "生成されたキーは256ビットであること。")]
        public void CreateKeyTest()
        {
            Assert.True(AesExample.CreateKey().Length == (256/8));
        }

        [Fact(DisplayName = "生成された初期ベクトルは128ビットであること。")]
        public void CreateIvTest()
        {
            Assert.True(AesExample.CreateIv().Length == (128 / 8));
        }

        [Fact(DisplayName = "生成されたソルトは128ビットであること。")]
        public void CreatePasswordSaltTest()
        {
            Assert.True(AesExample.CreatePasswordSalt().Length == (128/8));
        }

        [Fact(DisplayName = "生成されたキーは256ビットであること。")]
        public void CreateKeyFromPasswordTest()
        {
            var salt = AesExample.CreatePasswordSalt();
            Assert.True(AesExample.CreateKeyFromPassword("password", salt).Length == (256/8));
        }

        [Fact(DisplayName = "パスワードからキー生成の性能評価")]
        public void CreateKeyFromPasswordPerformanceTest()
        {
            const int MaxCount = 10_000;
            var sw = new Stopwatch();
            sw.Start();
            for(var i=0; i< MaxCount; i++)
            {
                var salt = AesExample.CreatePasswordSalt();
                _ = AesExample.CreateKeyFromPassword("password", salt);
            }
            sw.Stop();

            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("({0:n0}) {1:00}:{2:00}:{3:00}.{4:00}",
                MaxCount,
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            _logger.WriteLine("RunTime " + elapsedTime);
        }

        [Fact(DisplayName = "暗号化・復号化できること。(ストリーム)")]
        public void EncryptDecryptByStreamTest()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();
            var key = aes.Key;
            var iv = aes.IV;

            using var output = new MemoryStream();
            AesExample.Encrypt(output, TestCleartext, key, iv);
            using var input = new MemoryStream(output.ToArray());
            var decrypted = AesExample.Decrypt(input, key, iv);

            Assert.Equal(TestText, Encoding.UTF8.GetString(decrypted));
        }

        [Fact(DisplayName ="暗号化・復号化できること。(バイト列)")]
        public void EncryptDecryptByBytesTest()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();
            var key = aes.Key;
            var iv = aes.IV;

            var ciphertext = AesExample.Encrypt(TestCleartext, key, iv);
            var decrypted = AesExample.Decrypt(ciphertext, key, iv);

            Assert.True(ciphertext.Length % (128 / 8) == 0);
            Assert.Equal(TestText, Encoding.UTF8.GetString(decrypted));
        }

        [Fact(DisplayName = "暗号化・復号化できること。(バイト列・ストリーム)")]
        public void EncryptDecryptBytesStreamTest()
        {
            var key = AesExample.CreateKey();
            var iv = AesExample.CreateIv();

            var ciphertext = AesExample.Encrypt(TestCleartext, key, iv);
            using var input = new MemoryStream(ciphertext);
            var decrypted = AesExample.Decrypt(input, key, iv);

            Assert.True(ciphertext.Length % (128 / 8) == 0);
            Assert.Equal(TestText, Encoding.UTF8.GetString(decrypted));
        }

        [Fact(DisplayName = "暗号化・復号化できること。(ストリーム・バイト列)")]
        public void EncryptDecryptStreamBytesTest()
        {
            var key = AesExample.CreateKey();
            var iv = AesExample.CreateIv();

            using var output = new MemoryStream();
            AesExample.Encrypt(output, TestCleartext, key, iv);
            var ciphertext = output.ToArray();
            var decrypted = AesExample.Decrypt(ciphertext, key, iv);

            Assert.True(ciphertext.Length % (128 / 8) == 0);
            Assert.Equal(TestText, Encoding.UTF8.GetString(decrypted));
        }

        [Fact(DisplayName = "パスワードから暗号化・復号化できること。")]
        public void EncryptDecryptByPasswordTest()
        {
            var salt = AesExample.CreatePasswordSalt();
            var key = AesExample.CreateKeyFromPassword("password", salt);
            var iv = AesExample.CreateIv();

            var ciphertext = AesExample.Encrypt(TestCleartext, key, iv);
            var decrypted = AesExample.Decrypt(ciphertext, key, iv);

            Assert.True(ciphertext.Length % (128 / 8) == 0);
            Assert.Equal(TestText, Encoding.UTF8.GetString(decrypted));
        }

    }
}
