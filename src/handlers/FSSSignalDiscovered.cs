using net.niceygy.eddatacollector.schemas.FSSSignalDiscovered;
using net.niceygy.eddatacollector.database;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace net.niceygy.eddatacollector.handlers
{
    static class FSSSignalHandler
    {
        public static async Task Handle(FSSSignalMessage msg, DbContextOptions options)
        {
            if (!MessageCheck.IsValid(msg.header))
            {
                return;
            }
            string systemName = msg.message.StarSystem;

            int megashipCycle = Cycles.GetMegashipCycle();

            using var ctx = new EdDbContext(options);

            foreach (Signal signal in msg.message.signals)
            {
                if (signal.SignalType == "Megaship")
                {
                    if (MessageCheck.IsNameBlocked(signal.SignalName))
                    {
                        continue;
                    }
                    Log.Verbose($"Updating '{signal.SignalName}' to {systemName} for week {megashipCycle}");
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