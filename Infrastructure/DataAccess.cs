using Core.Auth;
using Core.Entities;
using DataTransferObject.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class DataAccess : IdentityDbContext<User>
    {
        public DataAccess(DbContextOptions<DataAccess> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users", "security");
            builder.Entity<IdentityRole>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "security");
            //builder.Entity<RefreshTokenDevCreed>().HasNoKey();
            //builder.Entity<IdentityRole>().HasData(
            //    new IdentityRole
            //    {
            //        Name = "Visitor",
            //        NormalizedName = "VISITOR"
            //    },
            //    new IdentityRole
            //    {
            //        Name = "Administrator",
            //        NormalizedName = "ADMINISTRATOR"
            //    });

            //builder.Entity<User>()
            //    .HasMany(u => u.RefreshTokens)
            //    .WithOne()
            //    .HasForeignKey(rt => rt.UserId);

        }
        public DbSet<Region> region { get; set; }
        public DbSet<RefreshTokenDevCreed> refreshTokenDevCreed { get; set; }
    }
}
