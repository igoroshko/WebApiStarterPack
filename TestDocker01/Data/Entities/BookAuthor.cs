using System.Collections.Generic;

namespace TestDocker01.Data.Entities
{
    public class BookAuthor
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public IList<Book> Books { get; set; }
    }
}
