using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericServices;
using TestDocker01.Data.Entities;

namespace TestDocker01.Models
{
    public class BookModel : ILinkToEntity<Book>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int PageCount { get; set; }

        public int AuthorId { get; set; }

        public IList<BookPage> Pages { get; set; }

        public IList<int> TagIds { get; set; }
    }
}
