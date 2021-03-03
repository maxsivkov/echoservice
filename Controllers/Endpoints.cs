using Microsoft.AspNetCore.Authorization;
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
    public class Public : EchoBase
    {
    }

    [ApiController]
    [Authorize]
    [Route("/api/[controller]")]
    public class Protected : EchoBase
    {
    }
}
