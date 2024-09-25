using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movie.Models;
using System;
using System.Reflection.Emit;

namespace Movie.Data
{
    public class AppDbContext:IdentityDbContext<AppUser,AppRole,string>
    {
        //public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Genre> Genre { get; set; } 
        public DbSet<MOvie> movies { get; set; }
        public DbSet<Permission>permissions { get; set; }
        public DbSet<UserPermission> userPermissions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserPermission>().HasKey(t=>new {t.UserId,t.PermissionId});

            builder.Entity<UserPermission>()
             .HasOne(up => up.Permission)
             .WithMany(p => p.UserPermissions)
             .HasForeignKey(up => up.PermissionId);

            builder.Entity<UserPermission>()
             .HasOne(up => up.AppUser)
             .WithMany(p => p.UserPermissions)
             .HasForeignKey(up => up.UserId);



            // i don't know why seeding does not work
            builder.Entity<UserPermission>().HasData(PutUserPermissions());
            builder.Entity<Permission>().HasData(PutPermissions());
            builder.Entity<AppUser>().HasData(PutUsers());
            builder.Entity<AppRole>().HasData(PutRoles());
        }


        #region Seeding models
        private List<Permission> PutPermissions()
        {
            return new List<Permission>
            {
                new Permission{
                    Id=1,
                    PermissionName="GetMoview"
                },

                  new Permission{
                    Id=2,
                    PermissionName="AddMovie"
                    },

                    new Permission{
                    Id=3,
                    PermissionName="DeleteMovie"
                    },

                     new Permission{
                    Id=4,
                    PermissionName="EditMovie"
                    }

            };

        }

        private List<AppRole> PutRoles()
        {
            //add default role
            AppRole appRole = new AppRole
            {
                Id = "1",
                Name = "Admin"
            };
            return new List<AppRole> { appRole };
        }

        private List<AppUser> PutUsers()
        {
            var passwordHasher = new PasswordHasher<AppUser>();
            AppUser appUser = new AppUser
            {
                Id = "1",
                UserName = "ibrahim",
                Email = "ihany@gmail.com"
            };
            appUser.PasswordHash = passwordHasher.HashPassword(appUser, "Ibrahim1020+");
            return new List<AppUser>
            {
                appUser
            };

        }

        private List<UserPermission> PutUserPermissions()
        {
            var result = new List<UserPermission>{
                new UserPermission
                {
                    UserId = "1",
                    PermissionId = 1
                },
                 new UserPermission
                {
                    UserId = "1",
                    PermissionId = 2
                },
                   new UserPermission
                {
                    UserId = "1",
                    PermissionId =3
                },
                     new UserPermission
                {
                    UserId = "1",
                    PermissionId = 4
                }
            };
            return result;
        }
        #endregion
    }
}
