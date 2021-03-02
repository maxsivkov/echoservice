using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApp.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class EchoController : Controller
    {
        [HttpGet]
        public JsonResult Get() => new JsonResult( new
        {
            ServerDate = DateTime.Now,
            Headers = HttpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            QueryParams = HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
            FormParams = HttpContext.Request.HasFormContentType ? HttpContext.Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString()) : new Dictionary<string, string>(),
            Cookies = HttpContext.Request.Cookies.ToDictionary(x => x.Key, x => x.Value.ToString())
        });
    }
}
