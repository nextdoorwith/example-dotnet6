using Xunit.Abstractions;

namespace DbExample.common
{
    public static class TestHelper
    {
        private static ITestOutputHelper _output = null;

        // テストが並列実行されると出力が崩れるので、各テストでは[Collection()]で直列実行する前提
        public static void SetOutput(ITestOutputHelper output)
            => _output = output;

        public static class Console
        {
            public static void WriteLine(string format, params object[] args)
            {
                if (args.Length <= 0)
                    WriteLine((object)format);
                else
                    _output?.WriteLine(format, args);
            }
            public static void WriteLine(object o)
                => _output?.WriteLine(o?.ToString());
        }
    }

}
