using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartClass.Common;

namespace SmartClass.Web.Api
{
    [Authorize]
    [Route("Api/Common/Auth")]
    [ApiController]
    public class AuthApiController
    {
        //用户名密码的登录
        [AllowAnonymous]
        [HttpPost("LoginOn")]
        public MessageResult UserLogin(UserLoginVo vo)
        {
            return MessageResult.Create(false, "TODO");
        }

        //pod登录的场景
        [AllowAnonymous]
        [HttpGet("GetDeviceToken")]
        public MessageResult GetDeviceToken(GetDeviceTokenArgs args)
        {
            return MessageResult.Create(false, "TODO");
        }
        
        //用户登录的场景
        [HttpGet("GetClientToken")]
        public MessageResult GetClientToken(GetClientTokenArgs args)
        {
            return MessageResult.Create(false, "TODO");
        }
    }

    public class UserLoginVo
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class GetClientTokenArgs
    {
        /// <summary>
        /// 入口ClientId
        /// </summary>
        public string EntryClientId { get; set; }
        /// <summary>
        /// 自身ClientId
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 连接验证码
        /// </summary>
        public string ValidateCode { get; set; }

        /// <summary>
        /// 预留扩展项
        /// </summary>
        public IDictionary<string, string> Items { get; set; } = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public class GetDeviceTokenArgs
    {
        public string ClientId { get; set; }
    }
}
