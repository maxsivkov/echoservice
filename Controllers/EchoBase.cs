using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebApp.Controllers
{
    public class EchoBase : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        public async Task<JsonResult> Get() =>
            new JsonResult(new
            {
                ServerDate = DateTime.Now,
                Path = HttpContext.Request.Path,
                RequestMethod = HttpContext.Request.Method,
                RequestBodyLength = await BodyLength(),
                RequestHeaders = HttpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                RequestQueryParams = HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
                RequestFormParams = HttpContext.Request.HasFormContentType ? HttpContext.Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString()) : new Dictionary<string, string>(),
                RequestFormFiles = HttpContext.Request.HasFormContentType ? HttpContext.Request.Form.Files.ToDictionary(x => x.Name, x =>
                {
                    using MemoryStream ms = new MemoryStream();
                    x.CopyTo(ms);
                    return Convert.ToBase64String(ms.ToArray());
                }) : new Dictionary<string, string>(),
                RequestHasFormContentType = HttpContext.Request.HasFormContentType,
                RequestCookies = HttpContext.Request.Cookies.ToDictionary(x => x.Key, x => x.Value.ToString())
            });

        protected async Task<long> BodyLength()
        {
            var request = HttpContext.Request;
            long bodyLen = 0;
            if (request.Body.CanSeek && request.Body.CanRead)
            {
                request.Body.Seek(0, SeekOrigin.Begin);

                using (MemoryStream ms = new MemoryStream())
                {
                    await request.Body.CopyToAsync(ms);
                    bodyLen = ms.Position;
                }
                request.Body.Seek(0, SeekOrigin.Begin);
            }
            return bodyLen;
        }
    }
}
