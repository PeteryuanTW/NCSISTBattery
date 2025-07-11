using Microsoft.EntityFrameworkCore;
using SharedMod;

namespace NCSISTBattery.EFModel
{
    public partial class NCSISTBatteryDBContext : DbContext
    {
        public NCSISTBatteryDBContext(DbContextOptions<NCSISTBatteryDBContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeContent> RecipeContents { get; set; }
        public virtual DbSet<Jig> Jigs { get; set; }
        public virtual DbSet<JigContent> JigContents { get; set; }
        public virtual DbSet<OrderCommand> OrderCommands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Name).IsUnique();

                entity.HasMany(d => d.RecipeContents).WithOne(p => p.Material)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Name).IsUnique();

                entity.HasMany(d => d.RecipeContents).WithOne(p => p.Recipe)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RecipeContent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Material).WithMany(p => p.RecipeContents)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeContents)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Jig>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<JigContent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<OrderCommand>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
