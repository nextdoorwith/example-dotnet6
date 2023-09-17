using System.Text;
using Xunit;
using Xunit.Sdk;

namespace XunitMoqExample
{

    public class XunitHowto
    {
        [Fact]
        public void UsingEqualExceptionTest()
        {
            var chars = Enumerable.Range(0, 1024).Select(c => (char)('A' + c % 26)).ToArray();
            var expected = new string(chars);
            var actual = expected.Remove(1000, 1); // 1000文字目を削除

            if (!string.Equals(expected, actual))
                // throw new EqualException(expected, actual); // xunit 2.2-2.4
                throw EqualException.ForMismatchedValues(expected, actual); // xunit 2.5+

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UsingXunitExceptionTest()
        {
            var strs = Enumerable.Range(0, 32).Select(c => $"{c},{c + 1},{c + 2}").ToArray();
            var expected = string.Join("\r\n", strs);
            var actual = expected.Remove(98, 1);

            AssertCsvEqual(expected, actual);
        }
        private static void AssertCsvEqual(string expected, string actual)
        {
            if (string.Equals(expected, actual)) return;

            // 実際には、null考慮や、行数の不一致等の考慮が必要です。
            var exps = expected.Split("\r\n");
            var acts = actual.Split("\r\n");
            var sb = new StringBuilder();
            for (var i = 0; i < exps.Length; i++)
            {
                var e = i < exps.Length ? exps[i] : null;
                var a = i < acts.Length ? acts[i] : null;
                var matched = string.Equals(e, a) ? "--" : "NG";
                sb.AppendLine($"{matched}: \"{e}\" <-> \"{a}\"");
            }
            throw new XunitException(sb.ToString());
        }

    }
}

