using HttpClientExample.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HttpClientExample.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyApiController : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        public IActionResult Respose([FromQuery] string body = null, [FromQuery] int sc = 200)
        {
            IActionResult result;
            switch (body)
            {
                case "image":
                    result = new FileContentResult(Resource.Image1, "image/png");
                    break;
                case "json":
                    var vals = new Dictionary<string, object>()
                    {
                        ["result"] = 1,
                        ["description"] = "success"
                    };
                    result = new JsonResult(vals) { StatusCode = sc };
                    break;
                default:
                    result = new StatusCodeResult(sc);
                    break;
            };
            return result;
        }
    }
}
