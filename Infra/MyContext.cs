using Microsoft.EntityFrameworkCore;
using g2soir.Models;

namespace g2soir.Infra
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Sessionf> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Formation>()
                .HasOne(f => f.Categorie)
                .WithMany(c => c.Formations)
                .HasForeignKey(f => f.IdCategorie)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sessionf>()
                .HasOne(s => s.Formation)
                .WithMany(f => f.Sessions)
                .HasForeignKey(s => s.IdFormation)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sessionf>()
                .HasMany(s => s.Users)
                .WithMany(u => u.Sessions)
                .UsingEntity(j => j.ToTable("Inscription"));
        }
    }
}
