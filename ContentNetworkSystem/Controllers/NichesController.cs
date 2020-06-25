using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentNetworkSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NichesController : ControllerBase
    {
        // GET: api/<NichesController>
        [HttpGet]
        public async Task<ActionResult>  Get([FromServices] INichesService nichesService)
        {
            return Ok( await nichesService.GetAsync());
        }

        // GET api/<NichesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id, [FromServices] INichesService nichesService)
        {
            var niche = await nichesService.GetAsync(id);
            if (niche == null)
            {
                return NotFound();
            }
            return Ok(niche);
        }

        // GET api/<NichesController>/5/KeywordsList
        [HttpGet("{id}/KeywordsList")]
        public async Task<ActionResult> GetKeywordsList(int id, [FromServices] INichesService nichesService)
        {
            var niche = await nichesService.GetAsync(id);
            if(niche==null)
            {
                return NotFound();
            }
            List<string> keywords = niche.Keywords.Select(e => e.Name).ToList();
            return Ok(keywords);
        }

        // POST api/<NichesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<NichesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<NichesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
