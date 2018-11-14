using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDocker01.Data.Entities
{
    public class Tag : IdentifiableEntity
    {
        [Required]
        public string Value { get; set; }

        public IList<BookTag> Books { get; set; }
    }
}
