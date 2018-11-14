using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestDocker01.Data.Entities
{
    public abstract class IdentifiableEntity
    {
        public int Id { get; set; }
    }
}
