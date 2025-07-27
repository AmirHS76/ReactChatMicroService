using Microsoft.EntityFrameworkCore;
using ReactChat.Login.Domain.Entities;

namespace ReactChat.Login.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
    }
}
