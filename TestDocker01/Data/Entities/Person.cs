using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestDocker01.Data.Entities
{
    public class Person : IdentifiableEntity
    {
        public string Name { get; set; }

        public IList<Book> Books { get; set; }
    }
}
