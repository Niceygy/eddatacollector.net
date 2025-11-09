namespace net.niceygy.eddatacollector.database
{
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    public class Database
    {
        public readonly string CONNECTION_STRING;
        public Database()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "10.0.0.52",
                UserID = "assistant",
                Password = "6548",
                InitialCatalog = "elite",
            };

            CONNECTION_STRING = builder.ConnectionString;

        }

        public async Task UpdatePowerSystem()
        {
            
        }
    }
}