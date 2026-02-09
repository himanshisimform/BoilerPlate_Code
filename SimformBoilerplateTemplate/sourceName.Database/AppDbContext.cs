using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sourceName.Database.Tables;

namespace sourceName.Database;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Identity table names
        builder.Entity<ApplicationUser>().ToTable("Users", "Identity");
        builder.Entity<ApplicationRole>().ToTable("Roles", "Identity");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "Identity");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "Identity");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "Identity");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "Identity");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "Identity");

        // Configure relationships
        builder.Entity<UserProfile>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithOne(e => e.Profile)
                  .HasForeignKey<UserProfile>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany(e => e.RefreshTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed default roles
        SeedRoles(builder);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        var adminRoleId = Guid.Parse("2c5e174e-3b0e-446f-86af-483d56fd7210");
        var userRoleId = Guid.Parse("8e445865-a24d-4543-a6c6-9443d048cdb9");

        builder.Entity<ApplicationRole>().HasData(
            new ApplicationRole("Admin", "Administrator role with full access")
            {
                Id = adminRoleId,
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new ApplicationRole("User", "Standard user role")
            {
                Id = userRoleId,
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );
    }
}
