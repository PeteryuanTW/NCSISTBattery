using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NCSISTBattery.Controller
{
    [Route("shopfloor/[controller]")]
    [ApiController]
    public class ShopfloorController : ControllerBase
    {
        // GET: api/<ShopfloorController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ShopfloorController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ShopfloorController>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<ShopfloorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<ShopfloorController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
