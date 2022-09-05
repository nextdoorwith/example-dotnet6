using System.Diagnostics;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace KdfBasicExample
{
    public class KdfExampleTest
    {
        private readonly ITestOutputHelper _logger;

        public KdfExampleTest(ITestOutputHelper logger)
            => _logger = logger;

        [Fact(DisplayName = "生成されたキーは256ビットであること。")]
        public void CreateKeyFromPasswordTest()
        {
            var salt = Pbkdf2Example.CreatePasswordSalt();
            Assert.True(Pbkdf2Example.CreateKeyFromPassword("password", salt).Length == (256 / 8));
        }

        [Fact(DisplayName = "パスワードからキー生成の性能評価")]
        public void CreateKeyFromPasswordPerformanceTest()
        {
            const int MaxCount = 1_000;
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < MaxCount; i++)
            {
                var salt = Pbkdf2Example.CreatePasswordSalt();
                _ = Pbkdf2Example.CreateKeyFromPassword("password", salt);
            }
            sw.Stop();

            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format(CultureInfo.InvariantCulture,
                "({0:n0}) {1:00}:{2:00}:{3:00}.{4:00}",
                MaxCount,
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            _logger.WriteLine("RunTime " + elapsedTime);
        }

    }
}
