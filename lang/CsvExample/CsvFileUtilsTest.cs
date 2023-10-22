using Xunit;

namespace CsvExample
{
    public class CsvFileUtilsTest
    {
        [Fact(DisplayName = "引数readerがnullの場合、例外が発生すること。")]
        public void Parse_Arg_Reader_Null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => CsvFileUtils.Parse(null));
            Assert.Equal("Value cannot be null. (Parameter 'reader')", ex.Message);
        }

        [Fact(DisplayName = "引用符の閉じ忘れがある場合、例外が発生すること。")]
        public void Parse_NoEndingQuote()
        {
            var reader = new StringReader("abc,\"def,ghi\r\njkl");
            var ex = Assert.Throws<FormatException>(() => CsvFileUtils.Parse(reader));
            Assert.Equal("no ending quotation", ex.Message);
        }

        [Theory(DisplayName = "CSVファイルのバリエーション")]
        [MemberData(nameof(ParseVariation))]
        public void Parse_Variation(string csv, List<string[]> expected)
        {
            var reader = new StringReader(csv);
            var actual = CsvFileUtils.Parse(reader);
            Assert.Equal(expected, actual);
        }

        private static List<string[]> ToList(params string[][] s2) => s2.ToList();
        private static string[] ToR(params string[] s1) => s1;
        private static readonly string[] EmpRow = new string[] {""};
        public static object[][] ParseVariation = new object[][]
        {
            // ★基本
            new object[]{ "", ToList(ToR("")) },
            new object[]{ "a", ToList(ToR("a")) },
            new object[]{ "a,", ToList(ToR("a", "")) },
            new object[]{ "a,b", ToList(ToR("a", "b")) },
            new object[]{ "a,,c", ToList(ToR("a", "", "c")) },
            new object[]{ " a , b , c ", ToList(ToR(" a ", " b ", " c ")) },
            new object[]{ "いち,に,さん", ToList(ToR("いち", "に", "さん")) },
            new object[]{ "\n,\n,,", ToList(ToR(""), ToR("", ""), ToR("", "", "")) },
            // ★引用符
            new object[]{ "\"a1\"", ToList(ToR("a1")) },
            new object[]{ "\"a1\",b", ToList(ToR("a1", "b")) },
            new object[]{ " \"a1\" ,b", ToList(ToR("a1", "b")) },
            new object[]{ "\"a1,a2\",b", ToList(ToR("a1,a2", "b")) },
            new object[]{ "\"a1\"\"a2\",b", ToList(ToR("a1\"a2", "b")) },
            new object[]{ "\"a1\"\",\r\na2\",b", ToList(ToR("a1\",\r\na2", "b")) },
            new object[]{ "a,\"b1\r\nb2\",c", ToList(ToR("a", "b1\r\nb2", "c")) },
            new object[]{ "a,b,\"c1\r\nc2\"", ToList(ToR("a", "b", "c1\r\nc2")) },
            // ★引用符（引用符前後の冗長文字をスキップ）
            new object[]{ " aaa\"a1\ra2\"a\"\"aa", ToList(ToR("a1\ra2")) },
            new object[]{ "a,b, ccc\"c1\r\nc2\"cc\"\"c", ToList(ToR("a", "b", "c1\r\nc2")) },
            // ★改行
            new object[]{ "a,b,c\r", ToList(ToR("a", "b", "c"), EmpRow) },
            new object[]{ "a,b,c\n", ToList(ToR("a", "b", "c"), EmpRow) },
            new object[]{ "a,b,c\r\n", ToList(ToR("a", "b", "c"), EmpRow) },
            new object[]{ "a,b,c\r\r", ToList(ToR("a", "b", "c"), EmpRow, EmpRow) },
            new object[]{ "a,b,c\n\n", ToList(ToR("a", "b", "c"), EmpRow, EmpRow) },
            new object[]{ "a,b,c\r\n\r\n", ToList(ToR("a", "b", "c"), EmpRow, EmpRow) },
            new object[]{ "a,b,c\rd,e\n,f,\r\ng",
                ToList(ToR("a", "b", "c"), ToR("d", "e"), ToR("", "f", ""), ToR("g")) },
            // ★標準的なCSV
            new object[]{
                "col1,col2,col3\r\n" + 
                "値１－１,値１－２,\r\n" + 
                ",値２－２,値２－３",
                ToList(
                    ToR("col1", "col2", "col3"),
                    ToR("値１－１", "値１－２", ""),
                    ToR("", "値２－２", "値２－３")
                )
            },
            new object[]{
                "col1,col2,col3\r\n" +
                ",\"ああ\rいい\", \r\n" +
                "ううう,,\"ええ,\r\nおお\"",
                ToList(
                    ToR("col1", "col2", "col3"),
                    ToR("", "ああ\rいい", " "),
                    ToR("ううう", "", "ええ,\r\nおお")
                )
            },
        };

    }
}
