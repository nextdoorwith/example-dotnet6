using System.Text;

namespace Utf8BomExample
{
    public static class ExtensionExamples
    {
        
        public static byte[] StripUtf8Bom(this byte[]? bytes)
        {
            _ = bytes ?? throw new ArgumentNullException(nameof(bytes));
            var bom = Encoding.UTF8.GetPreamble();
            return bom.Length <= bytes.Length && bom.SequenceEqual(bytes[..bom.Length])
                ? bytes[bom.Length..] : bytes;
        }

        public static string StripUtf8Bom(this string? str)
        {
            _ = str ?? throw new ArgumentNullException(nameof(str));
            var encoding = Encoding.UTF8;
            var bomChars = encoding.GetString(encoding.Preamble).ToCharArray(); // '\ufeff'
            return str.TrimStart(bomChars);
        }

        public static byte[] StripUtf16Bom(this byte[] bytes)
        {
            _ = bytes ?? throw new ArgumentNullException(nameof(bytes));
            var bom = Encoding.Unicode.GetPreamble();
            return bom.Length <= bytes.Length && bom.SequenceEqual(bytes[..bom.Length])
                ? bytes[bom.Length..] : bytes;
        }

        public static string StripUtf16Bom(this string str)
        {
            _ = str ?? throw new ArgumentNullException(nameof(str));
            var encoding = Encoding.Unicode;
            var bomChars = encoding.GetString(encoding.Preamble).ToCharArray();
            return str.TrimStart(bomChars);
        }

    }
}
