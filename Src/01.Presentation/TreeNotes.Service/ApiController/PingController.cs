using Apps.Sdk.DependencyInjection;
using Apps.Sdk.Exceptions;
using Auth.Domain.RequestsResponses;
using Auth.Domain.Handlers;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using TreeNotes.Service.Authorization;
using Apps.Sdk.Helpers;
using Microsoft.Extensions.Configuration;

namespace TreeNotes.Service.ApiController
{
    [Route("api/ping")]    
    public class PingController : ApiBaseController
    {
        public PingController()
        {
        }

        [HttpGet]
        public IActionResult Ping(string echo="pong")
        {
            return Ok(echo);
        }

        [HttpGet]
        [Route("Config")]
        public IActionResult PingConfig(string configName)
        {
             return Ok(Configuration.GetValue<string>(configName));
        }
    }
}
