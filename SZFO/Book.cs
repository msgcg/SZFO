using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SZFO
{
    using System.Data.Entity;

    public class Book
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string FullDescription { get; set; }
    }

    public class BooksContext : DbContext
    {
        public BooksContext() : base("BooksContext")
        {
        }

        public DbSet<Book> Books { get; set; }
    }

}