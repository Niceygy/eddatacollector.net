namespace net.niceygy.eddatacollector.database
{
    using Microsoft.EntityFrameworkCore;

    public class Database
    {
        public static DbContextOptionsBuilder CreateOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EdDbContext>();
            optionsBuilder.UseMySql(
                Config.GetConnectionString(),
                ServerVersion.AutoDetect(Config.GetConnectionString())
            );

            return optionsBuilder;
        }
    }
}