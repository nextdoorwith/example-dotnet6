using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace Utf8BomExample
{
    [SuppressMessage("Naming", "CA1707:識別子はアンダースコアを含むことはできません", Justification = "<保留中>")]
    public class ExtensionExamplesTest
    {
        [Fact(DisplayName = "byte[].StripUtf8Bom(): 入力がnullの場合は例外がスローされること")]
        public void Test_ByteArray_StripUtf8Bom_Null()
        {
            byte[]? empty = null;
            var ex = Assert.Throws<ArgumentNullException>(() => empty.StripUtf8Bom());
            Assert.Equal("Value cannot be null. (Parameter 'bytes')", ex.Message);
        }

        [Theory (DisplayName = "byte[].StripUtf8Bom(): BOMが存在する場合は除外されること")]
        [InlineData(new byte[] { }, new byte[] { })]
        [InlineData(new byte[] { 0x78 }, new byte[] { 0x78 })]
        [InlineData(new byte[] { 0x78, 0x79, 0x7a }, new byte[] { 0x78, 0x79, 0x7a })]
        [InlineData(new byte[] { 0xef, 0xbb, 0xbf }, new byte[] { })]
        [InlineData(new byte[] { 0xef, 0xbb, 0xbf, 0x78, 0x79, 0x7a }, new byte[] { 0x78, 0x79, 0x7a })]
        public void Test_ByteArray_StripUtf8Bom(byte[] test, byte[] expected)
            => Assert.Equal(expected, test.StripUtf8Bom());

        [Fact(DisplayName = "string.StripUtf8Bom(): 入力がnullの場合は例外がスローされること")]
        public void Test_String_StripUtf8Bom_Null()
        {
            string? empty = null;
            var ex = Assert.Throws<ArgumentNullException>(() => empty.StripUtf8Bom());
            Assert.Equal("Value cannot be null. (Parameter 'str')", ex.Message);
        }

        [Theory(DisplayName = "string.StripUtf8Bom(): BOMが存在する場合は除外されること")]
        [InlineData("", "")]
        [InlineData("x", "x")]
        [InlineData("xyz", "xyz")]
        [InlineData("\ufeff", "")]
        [InlineData("\ufeffxyz", "xyz")]
        public void Test_String_StripUtf8Bom(string test, string expected)
            => Assert.Equal(expected, test.StripUtf8Bom());

    }
}