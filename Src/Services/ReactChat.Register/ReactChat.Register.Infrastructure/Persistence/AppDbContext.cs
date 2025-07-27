using Microsoft.EntityFrameworkCore;
using ReactChat.Register.Domain.Entities;

namespace ReactChat.Register.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
    }
}
