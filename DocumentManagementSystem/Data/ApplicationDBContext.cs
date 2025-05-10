using DocumentManagementSystem.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagementSystem.Data
{
    public class ApplicationDBContext : IdentityDbContext<User,Role,long,
        IdentityUserClaim<long>,UserRole,IdentityUserLogin<long>,
        IdentityRoleClaim<long>,IdentityUserToken<long>>
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<MyFile> MyFiles { get; set; }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(u => new { u.RoleId, u.UserId });

                userRole
                .HasOne(u => u.Role)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();

                userRole
                .HasOne(u => u.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(u => u.UserId)
                .IsRequired();
                
            });
            builder.Entity<MyFile>(entity =>
                {
                    entity.HasKey(a => a.Id);
                    entity.Property(a => a.Name)
                    .IsRequired();

                    entity.HasOne(a => a.user)
                    .WithMany(f => f.MyFiles)
                    .HasForeignKey(a => a.userId)
                    .OnDelete(DeleteBehavior.Cascade);
                });
          

        }
    }
}
