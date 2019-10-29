using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace echoservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EchoController : ControllerBase
    {
        // GET api/values
        [HttpGet("headers")]
        public JsonResult Get()
        {
            //return new string[] { "value1", "value2" };
            NameValueCollection headers = new NameValueCollection();
            foreach (var h in Request.Headers)
                headers.Add(h.Key, h.Value);
            return new JsonResult(new
            {
                headers = Request.Headers,
                query = Request.QueryString
            });
        }
        [HttpPost("file")]
        public JsonResult Post(IFormFile file)
        {
            return new JsonResult(new
            {
                filename = file.FileName,
                filesize = file.Length
            });
        }
    }
}