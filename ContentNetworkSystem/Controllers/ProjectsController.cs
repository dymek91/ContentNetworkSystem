using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ContentNetworkSystem.ModelsExtensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentNetworkSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        // GET: api/<ProjectsController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProjectsView, ProjectsManage")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProjectsController>/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProjectsView, ProjectsManage")]
        public async Task<ActionResult> Get(int id, [FromServices] IProjectsService projectsService)
        {
            var project = await projectsService.GetAsync(id);
            return Ok(project);
        }

        // POST api/<ProjectsController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProjectsAdd, ProjectsManage")]
        public async Task<ActionResult> Post([FromBody] Project project, [FromServices] IServiceProvider serviceProvider, [FromServices] IProjectsService projectsService)
        {
            project.Content.EncryptPassword(serviceProvider);
            project = await projectsService.AddAsync(project);
            return Ok(project);
        }

        // PUT api/<ProjectsController>/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProjectsAdd, ProjectsUpdate, ProjectsManage")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProjectsController>/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ProjectsDelete, ProjectsManage")]
        public void Delete(int id)
        {
        }
    }
}
