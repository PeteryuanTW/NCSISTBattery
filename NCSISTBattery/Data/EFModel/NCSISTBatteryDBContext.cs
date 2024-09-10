using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace NCSISTBattery.Data.EFModel
{
	public partial class NCSISTBatteryDBContext: DbContext
	{
		public NCSISTBatteryDBContext()
		{
		}

		public NCSISTBatteryDBContext(DbContextOptions<NCSISTBatteryDBContext> options)
			: base(options)
		{
		}

		
		public virtual DbSet<BatteryConfig> BatteryConfigs { get; set; }
		public virtual DbSet<BatteryMaterial> BatteryMaterials { get; set; }
		public virtual DbSet<BatteryRecipe> BatteryRecipes { get; set; }
		public virtual DbSet<JigConfig> JigConfigs { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
	=> optionsBuilder.UseSqlServer("Server=localhost;Database=NCSISTBatteryDB;Trusted_Connection=True; trustServerCertificate=true;");







		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<BatteryConfig>(entity =>
			{
				entity.HasKey(e => e.Type);
				entity.ToTable("BatteryConfigs");

				entity.Property(e => e.Type).HasColumnName("Type");
				entity.Property(e => e.MinWeight).HasColumnName("MinWeight");
				entity.Property(e => e.MaxWeight).HasColumnName("MaxWeight");
				entity.Property(e => e.LimitedPieces).HasColumnName("LimitedPieces");
			});

			modelBuilder.Entity<BatteryMaterial>(entity =>
			{
				entity.HasKey(e => e.Name);
				entity.ToTable("BatteryMaterial");

				entity.Property(e => e.Name).HasColumnName("Name");
			});

			modelBuilder.Entity<BatteryRecipe>(entity =>
			{
				entity.HasKey(e => new {e.BatteryName, e.RecipeIndex });
				entity.ToTable("BatteryRecipes");

				entity.Property(e => e.BatteryName).HasColumnName("BatteryName");
				entity.Property(e => e.RecipeIndex).HasColumnName("RecipeIndex");
				entity.Property(e => e.Type).HasColumnName("Type");
			});

			modelBuilder.Entity<JigConfig>(entity =>
			{
				entity.HasKey(e => e.Name);
				entity.ToTable("JigConfigs");

				entity.Property(e => e.Name).HasColumnName("Name");
				entity.Property(e => e.Type).HasColumnName("Type");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

	}
}
