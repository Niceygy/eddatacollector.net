using System.Text;
using FluentFTP;
using Microsoft.EntityFrameworkCore;
using net.niceygy.eddatacollector.database;
using Serilog;

namespace net.niceygy.eddatacollector.handlers
{
    public class EDAM
    {
        const int INTERVAL = 1000 * 60 * 60;
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
                Thread.Sleep(INTERVAL);

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

                    //ftp
                    CreatePublicCSV(
                        Environment.GetEnvironmentVariable("FTP_HOST")!,
                        Environment.GetEnvironmentVariable("FTP_USERNAME")!,
                        Environment.GetEnvironmentVariable("FTP_PASSWORD")!
                    );

                    Log.Verbose($"Seen {uniqueUploaders} uploaders in the past hour.");

                }
            }
        }

        private void CreatePublicCSV(string host, string username, string password)
        {
            const string CSV_FILE_PATH = "/experiments/edam/data.csv";
            string csv = "";

            using (var context = new EdDbContext(this.options))
            {
                var topItems = context.Uploaders
                    .OrderByDescending(p => p.timestamp)
                    .Take(24)
                    .ToList();

                foreach (var item in topItems)
                {
                    csv += $"{item.timestamp},{item.uploaders}\n";
                }
            }

            byte[] bytes = Encoding.Unicode.GetBytes(csv);


            using var client = new FtpClient(host, username, password);
            client.AutoConnect();
            try
            {
                client.DeleteFile(CSV_FILE_PATH);
            }
            catch (Exception) { }

            client.UploadBytes(bytes, CSV_FILE_PATH);

            client.Disconnect();
            client.Dispose();
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