using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TestDocker01.Data;
using TestDocker01.Data.Entities;
using TestDocker01.Models;

namespace TestDocker01.Controllers
{
    public class TagsController : CrudController<Tag, BaseSearchCriteria>
    {
        public TagsController(AppDbContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        protected override void ResolveChildEntities(Tag entity)
        {
            entity.Books = null;
        }

        protected override void ValidateEntityCommon(Tag entity)
        {
            base.ValidateEntityCommon(entity);

            if (string.IsNullOrWhiteSpace(entity.Value))
            {
                AddValidationError();
            }
        }
    }
}
