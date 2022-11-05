using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Xunit;

namespace Utf8BomExample
{
    [SuppressMessage("Naming", "CA1707:識別子はアンダースコアを含むことはできません", Justification = "<保留中>")]
    public class ExampleTest
    {
        [Fact(DisplayName = "ファイル読み取り(UTF-8 BOMなし)")]
        public void Test_FileRead_Utf8NoBom()
        {
            // "xyz_*.txt"は、"xyz"を指定の文字符号化形式で保存したファイル
            // ("xyz"はUTF-8で{0x78, 0x79, 0x7a}のバイト列)

            var str1 = File.ReadAllText("xyz_utf8.txt");
            var bytes1 = Encoding.UTF8.GetBytes(str1); // {0x78, 0x79, 0x7a}

            using var sr2 = new StreamReader("xyz_utf8.txt");
            var str2 = sr2.ReadToEnd();
            var bytes2 = Encoding.UTF8.GetBytes(str2); // {0x78, 0x79, 0x7a}

            using var sr3 = new StreamReader("xyz_utf8.txt", new UTF8Encoding(false));
            string str3 = sr3.ReadToEnd();
            var bytes3 = Encoding.UTF8.GetBytes(str3); // {0x78, 0x79, 0x7a}

            using var sr4 = new StreamReader("xyz_utf8.txt", new UTF8Encoding(true));
            string str4 = sr4.ReadToEnd();
            var bytes4 = Encoding.UTF8.GetBytes(str4); // {0x78, 0x79, 0x7a}

            Assert.Equal("xyz", str1);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes1);
            Assert.Equal("xyz", str2);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes2);
            Assert.Equal("xyz", str3);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes3);
            Assert.Equal("xyz", str4);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes4);
        }

        [Fact(DisplayName = "ファイル読み取り(UTF-8 BOMあり)")]
        public void Test_FileRead_Utf8Bom()
        {
            var str1 = File.ReadAllText("xyz_utf8_bom.txt");
            var bytes1 = Encoding.UTF8.GetBytes(str1); // {0x78, 0x79, 0x7a}

            using var sr2 = new StreamReader("xyz_utf8_bom.txt");
            var str2 = sr2.ReadToEnd();
            var bytes2 = Encoding.UTF8.GetBytes(str2); // {0x78, 0x79, 0x7a}

            using var sr3 = new StreamReader("xyz_utf8_bom.txt", new UTF8Encoding(false));
            string str3 = sr3.ReadToEnd();
            var bytes3 = Encoding.UTF8.GetBytes(str3); // {0x78, 0x79, 0x7a}

            using var sr4 = new StreamReader("xyz_utf8_bom.txt", new UTF8Encoding(true));
            string str4 = sr4.ReadToEnd();
            var bytes4 = Encoding.UTF8.GetBytes(str4); // {0x78, 0x79, 0x7a}

            Assert.Equal("xyz", str1);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes1);
            Assert.Equal("xyz", str2);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes2);
            Assert.Equal("xyz", str3);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes3);
            Assert.Equal("xyz", str4);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes4);
        }

        [Fact(DisplayName = "ファイル書き込み(UTF-8 BOMなし)")]
        public void Test_FileWrite_Utf8NoBom()
        {
            File.WriteAllText("output.txt", "xyz");
            var bytes1 = File.ReadAllBytes("output.txt"); // {0x78, 0x79, 0x7a}

            using var sw2 = new StreamWriter("output.txt");
            sw2.Write("xyz");
            sw2.Close();
            var bytes2 = File.ReadAllBytes("output.txt"); // {0x78, 0x79, 0x7a}

            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes1);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes2);
        }

        [Fact(DisplayName = "ファイル書き込み(UTF-8 BOMあり)")]
        public void Test_FileWrite_Utf8Bom()
        {
            File.WriteAllText("output.txt", "xyz", Encoding.UTF8);
            var bytes1 = File.ReadAllBytes("output.txt"); // {0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a}

            File.WriteAllText("output.txt", "xyz", new UTF8Encoding(true));
            var bytes2 = File.ReadAllBytes("output.txt"); // {0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a}

            File.WriteAllText("output.txt", "xyz", new UTF8Encoding(false));
            var bytes3 = File.ReadAllBytes("output.txt"); // {0x78, 0x79, 0x7a}

            using var sw4 = new StreamWriter("output.txt", false, Encoding.UTF8);
            sw4.Write("xyz");
            sw4.Close();
            var bytes4 = File.ReadAllBytes("output.txt"); // {0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a}

            using var sw5 = new StreamWriter("output.txt", false, new UTF8Encoding(true));
            sw5.Write("xyz");
            sw5.Close();
            var bytes5 = File.ReadAllBytes("output.txt"); // {0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a}

            using var sw6 = new StreamWriter("output.txt", false, new UTF8Encoding(false));
            sw6.Write("xyz");
            sw6.Close();
            var bytes6 = File.ReadAllBytes("output.txt"); // {0x78, 0x79, 0x7a}

            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, bytes1);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, bytes2);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes3);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, bytes4);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, bytes5);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes6);
        }

        [Fact(DisplayName = "バイト列・文字列変換にEncodingのBOM出力有無は影響しない")]
        public void Test_EffectOfEncodingBom()
        {
            // UTF8Encoding(Boolean)
            // https://learn.microsoft.com/en-us/dotnet/api/system.text.utf8encoding.-ctor?view=net-6.0#system-text-utf8encoding-ctor(system-boolean)

            var bytes1 = File.ReadAllBytes("xyz_utf8.txt");
            var n1 = Encoding.UTF8.GetString(bytes1);           // "xyz"
            var n2 = new UTF8Encoding(true).GetString(bytes1);  // "xyz"
            var n3 = new UTF8Encoding(false).GetString(bytes1); // "xyz"
            var n4 = Encoding.UTF8.GetBytes(n1);           // {0x78, 0x79, 0x7a}
            var n5 = new UTF8Encoding(true).GetBytes(n2);  // {0x78, 0x79, 0x7a}
            var n6 = new UTF8Encoding(false).GetBytes(n3); // {0x78, 0x79, 0x7a}

            var bytes2 = File.ReadAllBytes("xyz_utf8_bom.txt");
            var b1 = Encoding.UTF8.GetString(bytes2);           // "\ufeffxyz" ※誤変換
            var b2 = new UTF8Encoding(true).GetString(bytes2);  // "\ufeffxyz" ※誤変換
            var b3 = new UTF8Encoding(false).GetString(bytes2); // "\ufeffxyz" ※誤変換
            var b4 = Encoding.UTF8.GetBytes(b1);           // {0x78, 0x79, 0x7a}
            var b5 = new UTF8Encoding(true).GetBytes(b2);  // {0x78, 0x79, 0x7a}
            var b6 = new UTF8Encoding(false).GetBytes(b3); // {0x78, 0x79, 0x7a}

            Assert.Equal("xyz", n1);
            Assert.Equal("xyz", n2);
            Assert.Equal("xyz", n3);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, n4);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, n5);
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, n6);

            Assert.Equal("\ufeffxyz", b1);
            Assert.Equal("\ufeffxyz", b2);
            Assert.Equal("\ufeffxyz", b3);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, b4);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, b5);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, b6);
        }

        [Fact(DisplayName = "バイト列の文字列化(UTF-8 BOMなし)")]
        public void Test_BytesToString()
        {
            var image = File.ReadAllBytes("xyz_utf8.txt");
            var str1 = Encoding.UTF8.GetString(image);
            var bytes1 = Encoding.UTF8.GetBytes(str1); // {0x78, 0x79, 0x7a}

            Assert.Equal("xyz", str1);
            Assert.Equal(new char[] { 'x', 'y', 'z' }, str1.ToCharArray());
            Assert.Equal(new byte[] { 0x78, 0x79, 0x7a }, bytes1);
        }

        [Fact(DisplayName = "バイト列の文字列化(UTF-8 BOMあり)")]
        public void Test_BomBytesToString()
        {
            var image = File.ReadAllBytes("xyz_utf8_bom.txt");
            var str1 = Encoding.UTF8.GetString(image); // "\ufeffxyz" ※誤変換
            var bytes1 = Encoding.UTF8.GetBytes(str1); // {0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a}

            Assert.Equal("\ufeffxyz", str1);
            Assert.Equal(new char[] { '\ufeff', 'x', 'y', 'z' }, str1.ToCharArray());
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, bytes1);
        }

        // その他

        [Fact(DisplayName = "EncodingのBOM出力有無")]
        public void Test_Encoding_Preamble()
        {
            // GetPreamble()はbyte[]型を返却
            var byte1 = Encoding.UTF8.GetPreamble();           // {0xef, 0xbb, 0xbf}
            var byte2 = new UTF8Encoding(true).GetPreamble();  // {0xef, 0xbb, 0xbf}
            var byte3 = new UTF8Encoding(false).GetPreamble(); // {}

            // PreambleプロパティはReadOnlySpan<byte>型を返却
            var byte4 = Encoding.UTF8.Preamble;           // {0xef, 0xbb, 0xbf}
            var byte5 = new UTF8Encoding(true).Preamble;  // {0xef, 0xbb, 0xbf}
            var byte6 = new UTF8Encoding(false).Preamble; // {}

            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf }, byte1);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf }, byte2);
            Assert.Empty(byte3);
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf }, byte4.ToArray());
            Assert.Equal(new byte[] { 0xef, 0xbb, 0xbf }, byte5.ToArray());
            Assert.Empty(byte6.ToArray());
        }

        [Fact(DisplayName = "BOMを含むバイト列から文字列を生成した場合の結果")]
        public void Test_Encoding_BomChar()
        {
            var bytesUtf8 = File.ReadAllBytes("xyz_utf8_bom.txt");
            var bytesUtf16Le = File.ReadAllBytes("xyz_utf16le_bom.txt");
            var bytesUtf16Be = File.ReadAllBytes("xyz_utf16be_bom.txt");
            var bytesUtf32Le = File.ReadAllBytes("xyz_utf32le_bom.txt");
            var bytesUtf32Be = File.ReadAllBytes("xyz_utf32be_bom.txt");

            var c1 = Encoding.UTF8.GetString(bytesUtf8); // "\ufeffxyz"
            var c2 = Encoding.Unicode.GetString(bytesUtf16Le); // "\ufeffxyz"
            var c3 = Encoding.BigEndianUnicode.GetString(bytesUtf16Be); // "\ufeffxyz"
            var c4 = Encoding.UTF32.GetString(bytesUtf32Le); // "\ufeffxyz"
            var c5 = new UTF32Encoding(true, true).GetString(bytesUtf32Be); // "\ufeffxyz"

            Assert.Equal("\ufeffxyz", c1);
            Assert.Equal("\ufeffxyz", c2);
            Assert.Equal("\ufeffxyz", c3);
            Assert.Equal("\ufeffxyz", c4);
            Assert.Equal("\ufeffxyz", c5);
        }

        [Fact(DisplayName = "ファイル読み込み時はどのエンコーディングでもBOMは除外される")]
        public void Test_Unicode_Read()
        {
            var results = new List<string>();
            Action<string, Encoding> test = (f, e) => results.Add(File.ReadAllText(f, e));

            test("xyz_utf8.txt", Encoding.UTF8);
            test("xyz_utf8_bom.txt", Encoding.UTF8);
            test("xyz_utf8.txt", new UTF8Encoding(true));
            test("xyz_utf8_bom.txt", new UTF8Encoding(true));
            test("xyz_utf8.txt", new UTF8Encoding(false));
            test("xyz_utf8_bom.txt", new UTF8Encoding(false));

            test("xyz_utf16le.txt", Encoding.Unicode);
            test("xyz_utf16le_bom.txt", Encoding.Unicode);
            test("xyz_utf16le.txt", new UnicodeEncoding(false, false));
            test("xyz_utf16le_bom.txt", new UnicodeEncoding(false, false));
            test("xyz_utf16le.txt", new UnicodeEncoding(false, true));
            test("xyz_utf16le_bom.txt", new UnicodeEncoding(false, true));

            test("xyz_utf16be.txt", Encoding.BigEndianUnicode);
            test("xyz_utf16be_bom.txt", Encoding.BigEndianUnicode);
            test("xyz_utf16be.txt", new UnicodeEncoding(true, false));
            test("xyz_utf16be_bom.txt", new UnicodeEncoding(true, false));
            test("xyz_utf16be.txt", new UnicodeEncoding(true, true));
            test("xyz_utf16be_bom.txt", new UnicodeEncoding(true, true));

            test("xyz_utf32le.txt", Encoding.UTF32);
            test("xyz_utf32le_bom.txt", Encoding.UTF32);
            test("xyz_utf32le.txt", new UTF32Encoding(false, false));
            test("xyz_utf32le_bom.txt", new UTF32Encoding(false, false));
            test("xyz_utf32le.txt", new UTF32Encoding(false, true));
            test("xyz_utf32le_bom.txt", new UTF32Encoding(false, true));

            test("xyz_utf32be.txt", new UTF32Encoding(true, false));
            test("xyz_utf32be_bom.txt", new UTF32Encoding(true, false));
            test("xyz_utf32be.txt", new UTF32Encoding(true, true));
            test("xyz_utf32be_bom.txt", new UTF32Encoding(true, true));

            results.ForEach(v => Assert.Equal("xyz", v));
        }

        [Fact(DisplayName = "ファイル書き込み時はエンコーディングの設定に基づいてBOM出力される")]
        public void Test_Unicode_Write()
        {
            // 指定のエンコーディングで"xyz"を書込み、その結果をHexダンプで返却
            Func<Encoding, string> test = e =>
            {
                File.WriteAllText("output.txt", "xyz", e);
                return BitConverter.ToString(File.ReadAllBytes("output.txt"));
            };

            Assert.Equal("EF-BB-BF-78-79-7A", test(Encoding.UTF8));
            Assert.Equal("EF-BB-BF-78-79-7A", test(new UTF8Encoding(true)));
            Assert.Equal("78-79-7A" /*    */, test(new UTF8Encoding(false)));

            Assert.Equal("FF-FE-78-00-79-00-7A-00", test(Encoding.Unicode));
            Assert.Equal("FF-FE-78-00-79-00-7A-00", test(new UnicodeEncoding(false, true)));
            Assert.Equal("78-00-79-00-7A-00" /* */, test(new UnicodeEncoding(false, false)));

            Assert.Equal("FE-FF-00-78-00-79-00-7A", test(Encoding.BigEndianUnicode));
            Assert.Equal("FE-FF-00-78-00-79-00-7A", test(new UnicodeEncoding(true, true)));
            Assert.Equal("00-78-00-79-00-7A" /* */, test(new UnicodeEncoding(true, false)));

            Assert.Equal("FF-FE-00-00-78-00-00-00-79-00-00-00-7A-00-00-00", test(Encoding.UTF32));
            Assert.Equal("FF-FE-00-00-78-00-00-00-79-00-00-00-7A-00-00-00", test(new UTF32Encoding(false, true)));
            Assert.Equal("78-00-00-00-79-00-00-00-7A-00-00-00" /*       */, test(new UTF32Encoding(false, false)));

            Assert.Equal("00-00-FE-FF-00-00-00-78-00-00-00-79-00-00-00-7A", test(new UTF32Encoding(true, true)));
            Assert.Equal("00-00-00-78-00-00-00-79-00-00-00-7A" /*       */, test(new UTF32Encoding(true, false)));
        }

        [Fact(DisplayName = "UTF32ファイルの作成", Skip = "ユーティリティのためテスト対象外")]
        public void CreateUtf32File()
        {
            File.WriteAllText("xyz_utf32le.txt", "xyz", new UTF32Encoding(false, false));
            File.WriteAllText("xyz_utf32le_bom.txt", "xyz", new UTF32Encoding(false, true));
            File.WriteAllText("xyz_utf32be.txt", "xyz", new UTF32Encoding(true, false));
            File.WriteAllText("xyz_utf32be_bom.txt", "xyz", new UTF32Encoding(true, true));
        }
    }
}