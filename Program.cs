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
using Microsoft.IdentityModel.Tokens;

namespace net.niceygy.eddatacollector
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            int waitsec = 1;
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

            while (AreEnvVarsOK())
            {
                if (!await EDStatus.IsEliteOnline())
                {
                    Log.Information($"Elite offline. Waiting {waitsec} minutes");
                    Thread.Sleep(waitsec * 1000);
                    waitsec++;
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
                        await MainLoop(options);
                        Thread.Sleep(10 * 1000);
                        //10s
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        Thread.Sleep(waitsec * 1000);
                        waitsec++;
                    }
                }
            }
        }

        public static async Task MainLoop(DbContextOptionsBuilder options)
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

                byte[]? uncompressed;
                try
                {
                    uncompressed = DecompressZlib(bytes);
                }
                catch (Exception e)
                {
                    Log.Error($"Decompression failed: {e.Message}");
                    continue;  // Skip this message and continue
                }

                if (uncompressed == null)
                {
                    continue;
                }

                var result = utf8.GetString(uncompressed);

                if (result == string.Empty | result == null)
                {
                    break;
                }

                UnknownMessage message = JsonConvert.DeserializeObject<UnknownMessage>(result!)!;
                string? eventType = message.message["event"];

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

        /// <summary>
        /// Checks to see if all env 
        /// vars are present.
        /// </summary>
        /// <returns></returns>
        public static bool AreEnvVarsOK()
        {
            string[] ENV_VAR_NAMES = [
                "DATABASE_ADDR",
                "DATABASE_USER",
                "DATABASE_PASSWD",
                "LOG_LEVEL",
                "FTP_USERNAME",
                "FTP_PASSWORD",
                "FTP_HOST"
            ];

            bool result = true;

            foreach (string envvar in ENV_VAR_NAMES)
            {
                bool exists = Environment.GetEnvironmentVariable(envvar) != null;
                result = result && exists;
                if (!exists)
                {
                    Log.Error($"Env Var {envvar} is null!");
                }
            }

            if (result) Log.Information("All env vars OK");

            return result;
        }



        public static bool IsDatabaseReachable()
        {
            Thread.Sleep(1000 * 5);
            return true;
        }

        public static byte[]? DecompressZlib(byte[] compressed)
        {
            if (compressed.IsNullOrEmpty())
            {
                return null;
            }
            using var input = new MemoryStream(compressed);
            using var zlib = new ZLibStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            zlib.CopyTo(output);
            return output.ToArray();
        }
    }
}
