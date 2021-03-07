using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebApp.Controllers
{
    public class EchoBase : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        public async Task<JsonResult> Get() => new JsonResult( new
        {
            ServerDate = DateTime.Now,
            Path = HttpContext.Request.Path,
            Method = HttpContext.Request.Method,
            BodyLength = await BodyLength(),
            Headers = HttpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            QueryParams = HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
            FormParams = HttpContext.Request.HasFormContentType ? HttpContext.Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString()) : new Dictionary<string, string>(),
            Cookies = HttpContext.Request.Cookies.ToDictionary(x => x.Key, x => x.Value.ToString())
        });

        protected async Task<long> BodyLength()
        {
            var request = HttpContext.Request;
            long bodyLen = 0;
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                if (request.Body.CanRead)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        if (request.Body.CanRead)
                        {
                            await request.Body.CopyToAsync(ms);
                            bodyLen = ms.Position;
                        }
                    }
                }
            }
            return bodyLen;
        }
    }
}
