using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContentNetworkSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IApplicationLifetime ApplicationLifetime { get; set; }

        public AdminController(IApplicationLifetime appLifetime)
        {
            ApplicationLifetime = appLifetime;
        }

        [HttpGet("status")]
        public async Task<ActionResult> Status()
        {
            return Ok("Ok");
        }

        [HttpGet("shutdown")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Shutdown()
        {
            ApplicationLifetime.StopApplication();
            return Content("Done");
        }
    }
}