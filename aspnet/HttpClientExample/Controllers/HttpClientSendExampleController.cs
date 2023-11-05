using HttpClientExample.Models;
using HttpClientExample.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HttpClientExample.Controllers
{
    public class HttpClientSendExampleController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly byte[] _image1bytes = Resource.Image1;
        private static readonly byte[] _image2bytes = Resource.Image2;

        public HttpClientSendExampleController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTest()
        {
            var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");
            var targetUri = new Uri(baseUri, "api/DummyApi?body=json");

            using var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = targetUri,
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var res = await httpClient.SendAsync(req);

            var rawreq = await HttpDebugUtils.GetRawRequestAsync(req);
            var rawres = await HttpDebugUtils.GetRawResponseAsync(res);
            return CreatePlainResponse(rawreq, rawres);
        }

        public async Task<IActionResult> PostFormTest()
        {
            var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");
            var targetUri = new Uri(baseUri, "api/DummyApi?body=image");

            var forms = new Dictionary<string, string>()
            {
                {"key1", "値１" }, { "key2", "1234"}
            };
            using var content = new FormUrlEncodedContent(forms); // "application/x-www-form-urlencoded"
            using var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = targetUri,
                Content = content
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var res = await httpClient.SendAsync(req);

            var rawreq = await HttpDebugUtils.GetRawRequestAsync(req);
            var rawres = await HttpDebugUtils.GetRawResponseAsync(res);
            return CreatePlainResponse(rawreq, rawres);
        }

        public async Task<IActionResult> PostTextTest()
        {
            var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");
            var targetUri = new Uri(baseUri, "api/DummyApi?body=json");

            var data = "key1=値１, key2=1234";
            using var content = new StringContent(data); // "text/plain; charset=utf-8"
            using var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = targetUri,
                Content = content
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var res = await httpClient.SendAsync(req);

            var rawreq = await HttpDebugUtils.GetRawRequestAsync(req);
            var rawres = await HttpDebugUtils.GetRawResponseAsync(res);
            return CreatePlainResponse(rawreq, rawres);
        }

        public async Task<IActionResult> PostJsonTest()
        {
            var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");
            var targetUri = new Uri(baseUri, "api/DummyApi?body=json");

            var data = new Dictionary<string, object>() {
                {"key1", "値１" }, {"key2", 1234}
            };
            using var content = JsonContent.Create(data); // "application/json; charset=utf-8"
            using var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = targetUri,
                Content = content
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var res = await httpClient.SendAsync(req);

            var rawreq = await HttpDebugUtils.GetRawRequestAsync(req);
            var rawres = await HttpDebugUtils.GetRawResponseAsync(res);
            return CreatePlainResponse(rawreq, rawres);
        }

        public async Task<IActionResult> PostBytesImageTest()
        {
            var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");
            var targetUri = new Uri(baseUri, "api/DummyApi");

            using var content = new ByteArrayContent(_image1bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            using var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = targetUri,
                Content = content
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var res = await httpClient.SendAsync(req);

            var rawreq = await HttpDebugUtils.GetRawRequestAsync(req);
            var rawres = await HttpDebugUtils.GetRawResponseAsync(res);
            return CreatePlainResponse(rawreq, rawres);
        }

        public async Task<IActionResult> PostStreamImageTest()
        {
            var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");
            var targetUri = new Uri(baseUri, "api/DummyApi?sc=400");

            using var stream = new MemoryStream(_image2bytes);
            using var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            using var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = targetUri,
                Content = content
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var res = await httpClient.SendAsync(req);

            var rawreq = await HttpDebugUtils.GetRawRequestAsync(req);
            var rawres = await HttpDebugUtils.GetRawResponseAsync(res);
            return CreatePlainResponse(rawreq, rawres);
        }

        public async Task<IActionResult> PostMultiPartTest()
        {

            var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}");
            var targetUri = new Uri(baseUri, "api/DummyApi?body=json");

            using var part1 = new StringContent("値１");
            using var part2 = new ByteArrayContent(_image1bytes);
            part2.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            using var part3 = new ByteArrayContent(_image2bytes);
            part3.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            using var multipartContent = new MultipartFormDataContent
            {
                { part1, "key1" },
                { part2, "key2", "sample1.png" },
                { part3, "key3", "サンプル２.jpg" }
            };
            using var req = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = targetUri,
                Content = multipartContent
            };

            var httpClient = _httpClientFactory.CreateClient();
            using var res = await httpClient.SendAsync(req);

            var rawreq = await HttpDebugUtils.GetRawRequestAsync(req);
            var rawres = await HttpDebugUtils.GetRawResponseAsync(res);
            return CreatePlainResponse(rawreq, rawres);
        }

        private IActionResult CreatePlainResponse(string rawreq, string rawres)
        {
            var sb = new StringBuilder();
            string line;
            using var reader1 = new StringReader(rawreq);
            while ((line = reader1.ReadLine()) != null) sb.AppendLine("> " + line);
            sb.AppendLine("=====");
            using var reader2 = new StringReader(rawres);
            while ((line = reader2.ReadLine()) != null) sb.AppendLine("< " + line);

            return new ContentResult()
            {
                ContentType = "text/plain",
                Content = sb.ToString()
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}