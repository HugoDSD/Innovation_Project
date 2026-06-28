using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InnovationProject.Model;

namespace InnovationProject.Persist;

public class AppDb : IdentityDbContext<AppUser>
{
    public AppDb(DbContextOptions<AppDb> options) : base(options)
    {
    }

    public DbSet<EvaluationHistory>  EvaluationHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // When a user is deleted, their history is deleted too (Cascade)
        modelBuilder.Entity<EvaluationHistory>()
            .HasOne(h => h.User)
            .WithMany(u => u.Histories)
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}



/*
                        -- Technology choice --
We use IdentityDbContext<AppUser> to benefit from all the ASP.NET Core Identity features (user management, roles, etc.)
while adding our own EvaluationHistory table to store each user's environmental evaluation history.
DbContext is the standard Entity Framework Core class. It is a blank slate: it contains no table by default.

By inheriting from IdentityDbContext<AppUser>, we automatically add all the tables needed to manage users (AspNetUsers, AspNetRoles, etc.)
so passwords, login tokens, roles (administrator, user), etc.

                        -- Role of this file --

It is the single gateway between the C# world (object-oriented) and the SQL world (PostgreSQL, relational).
    - Mapping: it tells Entity Framework which C# classes should become SQL tables,
thanks to the line public DbSet<EvaluationHistory> EvaluationHistories { get; set; }
    - The SQL translator
    - Change tracking: as soon as you fetch, modify, add or delete a C# object, the AppDbContext keeps it in memory.
Nothing is sent to the database until you explicitly call SaveChanges()
    - Business rule definition (OnModelCreating): this is where you define complex relationships.
Here, a cascade delete behavior has been configured (DeleteBehavior.Cascade)
*/