using System.Text;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole;
using NetMQ.Sockets;
using NetMQ;

using net.niceygy.eddatacollector.constants;
using net.niceygy.eddatacollector.schemas;
using net.niceygy.eddatacollector.handlers;
using net.niceygy.eddatacollector.database;

using Serilog.Events;
using System.Net.NetworkInformation;

namespace net.niceygy.eddatacollector
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var levelSwitch = new Serilog.Core.LoggingLevelSwitch();
            if (Environment.GetEnvironmentVariable("LOG_LEVEL") == "DEBUG")
            {
                levelSwitch.MinimumLevel = LogEventLevel.Debug;
            }
            else
            {
                levelSwitch.MinimumLevel = LogEventLevel.Information;
            }


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Starting...");

            EDAM edam = new();

            while (true)
            {
                if (!await IsEliteOnline())
                {
                    Log.Information("Elite offline. Waiting 1 minute");
                    Thread.Sleep(60000);
                    //1m   
                }
                else if (!IsDatabaseReachable())
                {
                    Log.Information("Database unreachable, waiting 1 minute");
                    Thread.Sleep(5 * 1000);
                }
                else
                {
                    try
                    {
                        DbContextOptionsBuilder options = Database.CreateOptions();
                        Log.Information("Starting main loop");
                        await MainLoop(options, edam);
                        Thread.Sleep(10 * 1000);
                        //10s
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        public static async Task MainLoop(DbContextOptionsBuilder options, EDAM edam)
        {
            var utf8 = new UTF8Encoding();
            using var client = new SubscriberSocket();
            client.Options.ReceiveHighWatermark = 1000;
            client.Connect(Constants.EDDN_URI);
            client.SubscribeToAnyTopic();

            Log.Debug($"Connected to {Constants.EDDN_URI}");
            while (true)
            {
                var bytes = client.ReceiveFrameBytes();
                var uncompressed = DecompressZlib(bytes);
                var result = utf8.GetString(uncompressed);

                if (result == string.Empty | result == null)
                {
                    break;
                }

                UnknownMessage message = JsonConvert.DeserializeObject<UnknownMessage>(result!)!;
                string? eventType = message.message["event"];
                edam.AddUploader(message.header.uploaderID);

                if (eventType != null)
                {
                    switch (eventType)
                    {
                        case "FSSSignalDiscovered":
                            FSSSignalMessage msg_ = JsonConvert.DeserializeObject<FSSSignalMessage>(result!)!;
                            _ = Task.Factory.StartNew(() => FSSSignalHandler.Handle(msg_, options.Options));
                            break;

                        case "FSDJump":
                            FSDJumpMessage msg = JsonConvert.DeserializeObject<FSDJumpMessage>(result!)!;
                            _ = Task.Factory.StartNew(() => FSDJumpHandler.Handle(msg, options.Options));
                            break;

                        default:
                            continue;

                    }
                }
            }
        }

        public static async Task<bool> IsEliteOnline()
        {
            HttpClient client = new();
            var data = await client.GetAsync("https://ed-server-status.orerve.net");
            EDStatusReponse response = JsonConvert.DeserializeObject<EDStatusReponse>(await data.Content.ReadAsStringAsync()!)!;
            Log.Debug($"ED Status: {response.status}");
            return response.status == "Good";
        }

        public static bool IsDatabaseReachable()
        {
            Ping pingSender = new();
            PingReply reply = pingSender.Send(Environment.GetEnvironmentVariable("DATABASE_ADDR")!);

            return reply.Status == IPStatus.Success;
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
