using System;
using Microsoft.AspNetCore.Mvc;
using SmartClass.Web.Chats.AppServices;

namespace SmartClass.Web.Chats.Api
{
    [Route("api/chat")]
    [ApiController]
    public class SeedApiController : ControllerBase
    {
        private readonly ISeedAppService _seedAppService;

        public SeedApiController(ISeedAppService seedAppService)
        {
            _seedAppService = seedAppService;
        }

        [Route("getDate")]
        [HttpGet]
        public string GetDate()
        {
            return DateTime.Now.ToString("s");
        }

        //for test only
        [Route("seed")]
        [HttpGet]
        public string Seed([FromQuery]SeedArgs args)
        {
            _seedAppService.Seed(args);
            return "DONE";
        }
    }
}
