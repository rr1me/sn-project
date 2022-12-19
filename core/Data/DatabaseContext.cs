using Microsoft.EntityFrameworkCore;

namespace core.Data;

public class DatabaseContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public DatabaseContext(DbContextOptions options) : base(options){}
}