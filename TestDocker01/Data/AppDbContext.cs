using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestDocker01.Data.Entities;

namespace TestDocker01.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public DbSet<Book> Books { get; set; }

        // public DbSet<BookPage> BookPages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        /// <summary>
        /// Customizes mappings between entity model and database.
        /// </summary>
        /// <param name="builder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<User>()
            //    .HasIndex(u => u.Username)
            //    .IsUnique();

            //builder.Entity<ProductInstructor>()
            //    .HasKey(x => new { x.ProductId, x.InstructorId });

            // many to many
            builder.Entity<BookTag>()
                   .HasKey(bc => new { bc.BookId, bc.TagId });
            builder.Entity<BookTag>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.Tags)
                .HasForeignKey(bc => bc.BookId);
            builder.Entity<BookTag>()
                .HasOne(bc => bc.Tag)
                .WithMany(b => b.Books)
                .HasForeignKey(bc => bc.BookId);

            base.OnModelCreating(builder);
        }
    }
}
