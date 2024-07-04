using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace BlogAPI.Data
{
    public class BlogContext : DbContext
    {  
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }
        public DbSet<Autor> Autors { get; set; }
        public DbSet<Publicacion> Publicaciones { get; set; }

    }
}
