using TodoProject.Models;

namespace TodoProject.DataContext
{
    public class TodoAppContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }
    }
}
