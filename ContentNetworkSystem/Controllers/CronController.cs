using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ContentNetworkSystem.Data;

namespace ContentNetworkSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CronController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get([FromServices] SchedulerService schedulerService)
        {
            await schedulerService.ProcessProjectsAsync();
            return Ok();
        }
    }
}