namespace net.niceygy.eddatacollector.database
{
    using Microsoft.EntityFrameworkCore;
    using Serilog;
    public class Database
    {
        public static DbContextOptionsBuilder CreateOptions()
        {
            // string test = Config.GetConnectionString();
            // Log.Information($"Connecting to {Config.GetConnectionString()}");
            var optionsBuilder = new DbContextOptionsBuilder<EdDbContext>();
            optionsBuilder.UseMySql(
                Config.GetConnectionString(),
                ServerVersion.AutoDetect(Config.GetConnectionString())
            );

            return optionsBuilder;
        }
    }
}