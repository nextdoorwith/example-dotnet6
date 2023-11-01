using System.Net.Http.Headers;
using System.Text;

namespace HttpClientExample.Utils
{
    public static class HttpDebugUtils
    {
        /// <summary>
        /// HTTP要求の生データを取得する。
        /// </summary>
        /// <remarks>Hostヘッダは含まれません。</remarks>
        /// <param name="req">HTTP要求メッセージ</param>
        /// <returns>HTTP要求の生データ</returns>
        public static async Task<string> GetRawRequestAsync(HttpRequestMessage req)
        {
            var sb = new StringBuilder();

            // スタートライン
            sb.AppendLine($"{req.Method} {req.RequestUri.PathAndQuery} HTTP/{req.Version}");

            // HTTPヘッダ(HttpRequestMessageとHttpContentを対象)
            sb.Append(GetHeadersString(req.Headers));
            sb.Append(GetHeadersString(req.Content?.Headers));

            // HTTPヘッダ・ボディを分離する空行
            sb.AppendLine();

            // HTTPボディ
            if( req.Content != null)
            {
                var bytes = await req.Content.ReadAsByteArrayAsync();
                sb.Append(GetContentAsPrintable(bytes));
            }

            return sb.ToString();
        }

        /// <summary>
        /// HTTP応答の生データを取得する。
        /// </summary>
        /// <param name="res">HTTP応答メッセージ</param>
        /// <returns>HTTP応答の生データ</returns>
        public static async Task<string> GetRawResponseAsync(HttpResponseMessage res)
        {
            var sb = new StringBuilder();

            // スタートライン
            sb.AppendLine($"HTTP/{res.Version} {(int)res.StatusCode} {res.ReasonPhrase}");

            // HTTPヘッダ(HttpRequestMessageとHttpContentを対象)
            sb.Append(GetHeadersString(res.Headers));
            sb.Append(GetHeadersString(res.Content.Headers));

            // HTTPヘッダ・ボディを分離する空行
            sb.AppendLine();

            // HTTPボディ
            var bytes = await res.Content.ReadAsByteArrayAsync();
            sb.Append(GetContentAsPrintable(bytes));

            return sb.ToString();
        }

        private static string GetHeadersString(HttpHeaders headers)
        {
            var sb = new StringBuilder();
            if (headers == null) return sb.ToString();

            foreach (var header in headers)
                foreach (var val in header.Value)
                    sb.AppendLine($"{header.Key}: {val}");
            return sb.ToString();
        }

        private static string GetContentAsPrintable(byte[] bytes)
        {
            var sb = new StringBuilder();
            if (bytes == null) return sb.ToString();

            foreach (var c in bytes.Select(e => (char)e))
                sb.Append(char.IsAscii(c) &&
                    (c == '\r' || c == '\n' || c == ' ' || !char.IsControl((char)c)) ? c : ".");

            return sb.ToString();
        }

    }
}
