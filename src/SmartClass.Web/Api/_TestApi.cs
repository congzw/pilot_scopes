using System;
using Microsoft.AspNetCore.Mvc;

namespace SmartClass.Web.Api
{
    [Route("api/test")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        [Route("GetDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }
    }
}
