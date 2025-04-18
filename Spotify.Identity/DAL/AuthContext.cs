using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spotify.Identity.Entity;

namespace Spotify.Identity.DAL;

public class AuthContext(IConfiguration configuration) : IdentityDbContext<UserEntity>
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(configuration.GetConnectionString("Postgres"));
    }
    public DbSet<UserEntity> UserEntities { get; set; }
}