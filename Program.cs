using System.Text;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole;
using NetMQ.Sockets;
using NetMQ;

using net.niceygy.eddatacollector.constants;
using net.niceygy.eddatacollector.schemas.FSDJump;
using net.niceygy.eddatacollector.schemas.FSSSignalDiscovered;
using net.niceygy.eddatacollector.handlers;
using net.niceygy.eddatacollector.database;

namespace net.niceygy.eddatacollector
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Starting");

            var optionsBuilder = new DbContextOptionsBuilder<EdDbContext>();
            optionsBuilder.UseMySql(
                Config.GetConnectionString(),
                ServerVersion.AutoDetect(Config.GetConnectionString())
            );

            // using var context = new EdDbContext(optionsBuilder.Options);


            var utf8 = new UTF8Encoding();

            using var client = new SubscriberSocket();
            client.Options.ReceiveHighWatermark = 1000;
            client.Connect(Constants.EDDN_URI);
            client.SubscribeToAnyTopic();
            while (true)
            {
                var bytes = client.ReceiveFrameBytes();
                var uncompressed = DecompressZlib(bytes);
                var result = utf8.GetString(uncompressed);

                if (result == "" | result == null)
                {
                    break;
                }

                dynamic message = JsonConvert.DeserializeObject<dynamic>(result!)!;
                string? eventType = message!["message"]["event"];
                // Console.WriteLine(eventType);
                if (eventType != null)
                {
                    switch (eventType)
                    {
                        case "FSSSignalDiscovered":
                            FSSSignalMessage msg_ = JsonConvert.DeserializeObject<FSSSignalMessage>(result!)!;
                            _ = Task.Factory.StartNew(() => FSSSignalHandler.Handle(msg_, optionsBuilder.Options));
                            break;

                        case "FSDJump":
                            FSDJumpMessage msg = JsonConvert.DeserializeObject<FSDJumpMessage>(result!)!;
                            _ = Task.Factory.StartNew(() =>  FSDJumpHandler.Handle(msg, optionsBuilder.Options));
                            break;

                        default:
                            continue;

                    }
                }
            }
        }

        public static byte[] DecompressZlib(byte[] compressed)
        {
            using var input = new MemoryStream(compressed);
            using var zlib = new ZLibStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            zlib.CopyTo(output);
            return output.ToArray();
        }
    }
}
