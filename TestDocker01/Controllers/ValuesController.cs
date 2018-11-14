using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestDocker01.Data;
using TestDocker01.Data.Entities;

namespace TestDocker01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IAppRepository _personRepository;

        public ValuesController(IAppRepository personRepository)
        {
            _personRepository = personRepository;
        }

        // GET api/values
        [HttpGet]
        public IList<string> Get()
        {
            return _personRepository.GetNames();
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Person Get(int id)
        {
            return _personRepository.Query<Person>().Include(x => x.Books).FirstOrDefault(x => x.Id == id);
        }

        // POST api/values
        [HttpPost]
        public int Post([FromBody] string value)
        {
            var result = _personRepository.Add(new Person { Name = value });
            return result;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
