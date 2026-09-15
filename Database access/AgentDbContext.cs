using Microsoft.EntityFrameworkCore;

namespace Database_access;

public class AgentDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data source=app.db");
    }
}