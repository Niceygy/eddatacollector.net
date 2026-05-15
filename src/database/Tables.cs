namespace net.niceygy.eddatacollector.database
{
    using Microsoft.EntityFrameworkCore;
    using net.niceygy.eddatacollector.database.schemas;
    public class EdDbContext(DbContextOptions ctx) : DbContext(ctx)
    {
        public DbSet<StarSystem> StarSystems { get; set; }
        public DbSet<Megaship> Megaships { get; set; }
        // public DbSet<PowerData> PowerDatas { get; set; }
        public DbSet<Conflict> Conflicts { get; set; }
        public DbSet<FleetCarrier> FleetCarriers { get; set; }
        // public DbSet<Uploaders> Uploaders { get; set; }
        // public DbSet<Systems> Systems_DBSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FleetCarrier>()
                .ToTable("FleetCarriers")
                .HasKey(s => s.system_name);

            //confilcts 
            modelBuilder.Entity<Conflict>()
                .ToTable("conflicts")
                .HasKey(s => s.system_name);

            modelBuilder.Entity<Conflict>()
                .Property(e => e.first_place)
                .HasConversion<string>();

            modelBuilder.Entity<Conflict>()
                .Property(e => e.second_place)
                .HasConversion<string>();

            //megaships
            modelBuilder.Entity<Megaship>()
                .ToTable("megaships")
                .HasKey(s => s.name);

            modelBuilder.Entity<StarSystem>(entity =>
                {
                    entity.ToTable("systems");
                    entity.HasKey(e => e.system_name);
                    entity.Property<string>("_state")
                        .HasColumnName("state");
                    entity.Property<string>("_shortcode")
                        .HasColumnName("shortcode");
                    entity.Ignore(e => e.state);
                    entity.Ignore(e => e.shortcode);
                    entity.Property(e => e.control_points)
                        .HasColumnName("control_points");
                    entity.Property(e => e.points_change)
                        .HasColumnName("points_change");
                });
        }
    }

    public static class Config
    {
        public static string GetConnectionString()
        {

            return $"Server={Environment.GetEnvironmentVariable("DATABASE_ADDR")};" +
                   "Database=elite;" +
                   $"User={Environment.GetEnvironmentVariable("DATABASE_USER")};" +
                   $"Password={Environment.GetEnvironmentVariable("DATABASE_PASSWD")};" +
                   "Convert Zero Datetime=True;";
        }
    }
}