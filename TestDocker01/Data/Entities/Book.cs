using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestDocker01.Data.Entities
{
    public class Book : IdentifiableEntity
    {
        [Required]
        public string Name { get; set; }

        public int PageCount { get; set; }

        public int AuthorId { get; set; }

        public BookAuthor Author { get; set; }

        public IList<BookPage> Pages { get; set; }

        public IList<BookTag> Tags { get; set; }
    }
}
