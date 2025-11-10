using System.Data;
using Microsoft.Data.SqlClient;
using net.niceygy.eddatacollector.schemas.FSSSignalDiscovered;
using net.niceygy.eddatacollector;
using System.IO.Enumeration;
using net.niceygy.eddatacollector.database;

namespace net.niceygy.eddatacollector.handlers
{
    static class FSSSignalHandler
    {
        public static async Task Handle(FSSSignalMessage msg, EdDbContext ctx)
        {
            string systemName = msg.message.StarSystem;

            int megashipCycle = Cycles.GetMegashipCycle();

            foreach (Signal signal in msg.message.signals)
            {
                if (signal.SignalType == "Megaship")
                {
                    Console.WriteLine($"Updating {signal.SignalName} to {systemName} for week {megashipCycle}");

                    // Example usage

                    // Find existing system
                    var entry = await ctx.Megaships.FindAsync(signal.SignalName);
                    if (entry != null)
                    {
                        // Update properties

                        switch (megashipCycle)
                        {
                            case 1:
                                entry.SYSTEM1 = systemName;
                                break;
                            case 2:
                                entry.SYSTEM2 = systemName;
                                break;
                            case 3:
                                entry.SYSTEM3 = systemName;
                                break;
                            case 4:
                                entry.SYSTEM4 = systemName;
                                break;
                            case 5:
                                entry.SYSTEM5 = systemName;
                                break;
                            case 6:
                                entry.SYSTEM6 = systemName;
                                break;
                            default:
                                break;
                        }
                        
                        // Save changes
                        await ctx.SaveChangesAsync();

                    }
                }
            }

            return;
        }


    }
}