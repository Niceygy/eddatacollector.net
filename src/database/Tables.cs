namespace net.niceygy.eddatacollector.database
{
    using Microsoft.EntityFrameworkCore;

    using net.niceygy.eddatacollector.database.schemas;

    /*
    modelBuilder.Entity<YourNewEntity>()
        .ToTable("your_table_name")
        .HasKey(e => e.Id);  // replace Id with your primary key property

    // Optional: Map specific column names if they differ from property names
    modelBuilder.Entity<YourNewEntity>()
        .Property(e => e.SomeProperty)
        .HasColumnName("some_column_name");*/

    public class EdDbContext(DbContextOptions ctx) : DbContext(ctx)
    {
        public DbSet<StarSystem> StarSystems { get; set; }
        public DbSet<Megaship> Megaships { get; set; }
        public DbSet<PowerData> PowerDatas { get; set; }
        public DbSet<Conflict> Conflicts { get; set; }
        public DbSet<FleetCarrier> FleetCarriers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //star systems
            modelBuilder.Entity<StarSystem>()
                .ToTable("star_systems")
                .HasKey(s => s.system_name);

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

            //powerdata
            modelBuilder.Entity<PowerData>()
                        .ToTable("powerdata")
                        .HasKey(s => s.system_name);

            // modelBuilder.Entity<FleetCarrier>()
            //     .Property(s => s.system_name)
            //     .HasKey("system_name");
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