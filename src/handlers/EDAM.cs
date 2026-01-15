using System.Text;
using FluentFTP;
using Microsoft.EntityFrameworkCore;
using net.niceygy.eddatacollector.database;
using Serilog;

namespace net.niceygy.eddatacollector.handlers
{
    public class EDAM
    {
        const int INTERVAL = 1000 * 60 * 15;
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
                Thread.Sleep(SecondsUntilTheHour());

                lock (this.locker)
                {
                    int uniqueUploaders = this.uploaders.Count;
                    this.uploaders.Clear();
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    //save
                    using var ctx = new EdDbContext(options);

                    var newEntry = new database.schemas.Uploaders
                    {
                        timestamp = (int)unixTime,
                        uploaders = uniqueUploaders
                    };

                    ctx.Uploaders.Add(newEntry);
                    ctx.SaveChanges();

                    //ftp
                    CreatePublicCSV(
                        Environment.GetEnvironmentVariable("FTP_HOST")!,
                        Environment.GetEnvironmentVariable("FTP_USERNAME")!,
                        Environment.GetEnvironmentVariable("FTP_PASSWORD")!
                    );

                    Log.Debug($"Seen {uniqueUploaders} uploaders in the past hour.");

                }
            }
        }

        private void CreatePublicCSV(string host, string username, string password)
        {
            const string CSV_FILE_PATH = "public_html/experiments/edam/data.csv";
            string csv = "";

            using var context = new EdDbContext(this.options);

            var topItems = context.Uploaders
                .OrderByDescending(p => p.timestamp)
                .ToList();

            foreach (var item in topItems)
            {
                csv += $"{item.timestamp},{item.uploaders}\n";
            }

            byte[] bytes = Encoding.UTF8.GetBytes(csv);

            using var client = new FtpClient(host, username, password);
            client.AutoConnect();

            client.UploadBytes(bytes, CSV_FILE_PATH);
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

        private static int SecondsUntilTheHour()
        {
            // Source - https://stackoverflow.com/a/5733562
            // Posted by dtb
            // Retrieved 2025-12-31, License - CC BY-SA 3.0

            var timeOfDay = DateTime.Now.TimeOfDay;
            var nextFullHour = TimeSpan.FromHours(Math.Ceiling(timeOfDay.TotalHours));
            int delta = Convert.ToInt32(Math.Round((nextFullHour - timeOfDay).TotalSeconds));
            Log.Debug($"EDAM Waiting {delta.ToString()} seconds");
            return delta * 1000;
        }
    }
}