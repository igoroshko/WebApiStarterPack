using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GenericServices;
using Microsoft.AspNetCore.Mvc;
using TestDocker01.Common;
using TestDocker01.Data;
using TestDocker01.Data.Entities;
using TestDocker01.Models;

namespace TestDocker01.Controllers
{
    public class BooksController : BaseCrudController<Book, BaseSearchCriteria>
    {
        public BooksController(AppDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        [HttpPost("")]
        public virtual IActionResult Create(BookModel model)
        {
            var entity = Map<Book>(model);
            entity.Tags = new List<BookTag> { };
            foreach (var id in model.TagIds.AsNullable())
            {
                var tag = Single<Tag>(id);
                entity.Tags.Add(new BookTag
                {
                    TagId = id
                });
            }

            var result = CreateEntity(entity);
            return GetResult(result);
        }

        [HttpPut("{id}")]
        public virtual IActionResult Update(int id, BookModel model)
        {
            var entity = Map<Book>(model);

            UpdateEntity(id, entity);
            return GetResult();
        }

        [HttpGet("{id}")]
        public virtual IActionResult Get(int id)
        {
            var entity = GetEntity(id);
            var model = Map<BookModel>(entity);
            return GetResult(model);
        }

        [HttpGet("")]
        public virtual IActionResult GetAll()
        {
            var entities = GetAllEntities();
            var models = MapList<BookModel>(entities);
            return GetResult(models);
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(int id)
        {
            DeleteEntity(id);
            return GetResult();
        }

        [HttpPost("search")]
        public virtual IActionResult Search(BaseSearchCriteria criteria)
        {
            var entityResult = SearchEntities(criteria);
            var modelResult = MapSearchResult<Book, BookModel>(entityResult);
            return GetResult(modelResult);
        }
    }
}
