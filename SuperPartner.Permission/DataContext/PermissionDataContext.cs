using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperPartner.Permission.DataContext
{
    public class PermissionDataContext : DbContext
    {
        public PermissionDataContext(DbContextOptions<PermissionDataContext> options)
           : base(options)
        { }

        public DbSet<P_Function> P_Functions { get; set; }
        public DbSet<P_User2Function> P_User2Functions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<P_User2Function>(entity =>
            {
                entity.HasKey(c => new { c.UserId, c.FuncCode });
                entity.Property(e => e.UserId).HasMaxLength(50);
                entity.Property(e => e.FuncCode).HasMaxLength(50);
            });

            modelBuilder.Entity<P_Function>(entity =>
            {
                entity.HasKey(c => new { c.FuncCode });
                entity.Property(e => e.FuncCode).HasMaxLength(50);
                entity.Property(e => e.FuncName).HasMaxLength(200);
                entity.Property(e => e.AssociateUrls).HasMaxLength(4000);
                entity.Property(e => e.FuncDesc).HasColumnType("ntext");
                entity.Property(c => c.ExtendProperties).HasColumnType("xml");
            });
        }
    }
}
