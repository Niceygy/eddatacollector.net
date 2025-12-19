using Microsoft.EntityFrameworkCore;
using net.niceygy.eddatacollector.database;
using Serilog;

namespace net.niceygy.eddatacollector.handlers
{
    public class EDAM
    {
        const int ONE_HOUR = 1000 * 60 * 60;
        private List<string> uploaders;
        private Lock locker;
        private Thread thread;
        private readonly DbContextOptions options;
        public EDAM(DbContextOptions options)
        {
            this.locker = new Lock();
            lock (this.locker)
            {
                this.uploaders = [];
                this.options = options;
                this.thread = new Thread(Loop);
                thread.Start();
            }

        }

        private void Loop()
        {
            while (true)
            {
                Thread.Sleep(ONE_HOUR);

                lock (this.locker)
                {
                    int uniqueUploaders = this.uploaders.Count;
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    //save
                    using var ctx = new EdDbContext(options);
                    var newEntry = new database.schemas.Uploaders
                    {
                        timestamp = (int)unixTime,
                        uploaders = uniqueUploaders
                    };

                    ctx.Uploaders.Add(newEntry);

                    Log.Verbose($"Seen {uniqueUploaders} uploaders in the past hour.");

                }
            }
        }

        public void AddUploader(string ID)
        {
            lock (this.locker)
            {
                if (!this.uploaders.Contains(ID))
                {
                    this.uploaders.Add(ID);
                }
            }
        }
    }
}