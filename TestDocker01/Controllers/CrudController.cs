using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestDocker01.Common;
using TestDocker01.Data;
using TestDocker01.Data.Entities;
using TestDocker01.Extensions;
using TestDocker01.Models;

namespace TestDocker01.Controllers
{
    public abstract class CrudController<T, S> : BaseCrudController<T, S>
        where T : IdentifiableEntity
        where S : BaseSearchCriteria
    {
        public CrudController(AppDbContext db)
            : base(db)
        {
        }

        public CrudController(AppDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        [HttpPost("")]
        [ValidateModel]
        public virtual IActionResult Create(T entity)
        {
            var result = CreateEntity(entity);
            return GetResult(result);
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public virtual IActionResult Update(int id, T entity)
        {
            UpdateEntity(id, entity);
            return GetResult();
        }

        [HttpGet("{id}")]
        public virtual IActionResult Get(int id)
        {
            var result = GetEntity(id);
            return GetResult(result);
        }

        [HttpGet("")]
        public virtual IActionResult GetAll()
        {
            var result = GetAllEntities();
            return GetResult(result);
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(int id)
        {
            DeleteEntity(id);
            return GetResult();
        }

        [HttpPost("search")]
        public virtual IActionResult Search(S criteria)
        {
            var result = SearchEntities(criteria);
            return GetResult(result);
        }
    }
}
