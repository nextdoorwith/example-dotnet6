using System.Text;

namespace CsvExample
{
    public class CsvFileUtils
    {
        /// <summary>
        /// CSVファイルの内容を解析する。
        /// </summary>
        /// <param name="reader">CSVファイル内容</param>
        /// <returns>文字列配列リスト</returns>
        /// <exception cref="ArgumentNullException">readerがnullの場合</exception>
        /// <exception cref="FormatException">終端の引用符がない場合</exception>
        public static List<string[]> Parse(TextReader reader)
        {
            _ = reader ?? throw new ArgumentNullException(nameof(reader));

            var rowList = new List<string[]>();
            string val; int deli;
            do
            {
                // １行分のカラム値を取得
                var colList = new List<string>();
                do
                {
                    // 区切り文字・行終端(または引用符開始)までの値を取得
                    (val, deli) = ScanValue(reader, c => c == ',' || c == '"' || c == '\r' || c == '\n');

                    // 引用符だった場合、次の引用符までの値を取得
                    if (deli == '"')
                    {
                        // 次の引用符までを値とする。
                        // 終端の引用符が存在しない場合は例外とする。
                        (val, deli) = ScanValue(reader, c => c == '"');
                        if (deli != '"') throw new FormatException("no ending quotation");

                        // 終端となる引用符の次にある区切り文字まで読み飛ばす
                        (_, deli) = ScanValue(reader, c => c == ',' || c == '\r' || c == '\n');
                    }

                    // 取得した値をカラム値リストに追加
                    colList.Add(val);

                } while (deli == ','); // 区切り文字の場合、次のカラム値を取得

                // 改行読み飛ばし(\r\n時は追加で\nを読み飛ばし)
                if (deli == '\r' && reader.Peek() == '\n') deli = reader.Read();

                rowList.Add(colList.ToArray());

            } while (deli >= 0); // ファイル終端(-1)でない場合は繰り返し

            return rowList;
        }

        /// <summary>
        /// 停止条件まで値を読み取る
        /// </summary>
        /// <param name="reader">リーダー</param>
        /// <param name="stopFunc">停止条件</param>
        /// <returns>対象行に含まれる値群</returns>
        private static (string, int) ScanValue(TextReader reader, Func<int, bool> stopFunc)
        {
            StringBuilder sb = new StringBuilder();
            int ch;
            while ((ch = reader.Read()) >= 0)
            {
                // 連続する引用符は単一の引用符と解釈
                if (ch == '"' && reader.Peek() == '"')
                {
                    ch = reader.Read();
                    sb.Append((char)ch);
                    continue;
                }

                if (stopFunc(ch)) break;

                sb.Append((char)ch);
            }
            return (sb.ToString(), ch);
        }
    }
}
