namespace HttpClientExample.Utils
{
    public class DumpDelegateHandler: DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var res = await base.SendAsync(request, cancellationToken);
            var d1 = await HttpDebugUtils.GetRawRequestAsync(request);
            var d2 = await HttpDebugUtils.GetRawResponseAsync(res);
            return res;
        }
    }
}
