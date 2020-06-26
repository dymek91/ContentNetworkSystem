using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentNetworkSystem.Data;
using ContentNetworkSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentNetworkSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        // GET: api/<GroupsController>
        [HttpGet]
        public async Task<ActionResult> Get([FromServices] IGroupsService groupsService)
        {
            return Ok(await groupsService.GetAsync());
        }

        // GET api/<GroupsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id, [FromServices] IGroupsService groupsService)
        {
            var group = await groupsService.GetAsync(id);
            if(group ==null)
            { 
                return NotFound(group);
            }
            return Ok(group);
        }

        // POST api/<GroupsController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProjectsAdd, ProjectsManage")]
        public async Task<ActionResult> Post([FromBody] Group group, [FromServices] IGroupsService groupsService)
        {
            group = await groupsService.AddAsync(group);
            return Ok(group);
        }

        // PUT api/<GroupsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
         
        // DELETE api/<GroupsController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProjectsDelete, ProjectsManage")]
        public async Task<ActionResult> Delete(int id, [FromServices] IGroupsService groupsService)
        {
            var group = await groupsService.GetAsync(id);
            if(group ==null)
            {
                return NotFound("Not Found");
            }
            await groupsService.DeleteAsync(group);
            return Ok("Ok");
        }
    }
}
