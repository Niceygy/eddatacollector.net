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

            int carriers = 0;

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
                    else
                    {
                        var newEntry = new database.schemas.Megaship
                        {
                            name = signal.SignalName,
                            SYSTEM1 = "",
                            SYSTEM2 = "",
                            SYSTEM3 = "",
                            SYSTEM4 = "",
                            SYSTEM5 = "",
                            SYSTEM6 = "",

                        };
                        await ctx.Megaships.AddAsync(newEntry);
                    }
                }
                else if (signal.SignalType == "FleetCarrier")
                {
                    carriers++;
                }
            }

            if (carriers > 0)
            {
                var fcentry = await ctx.FleetCarriers.FindAsync(msg.message.StarSystem.Replace("'", "."));
                if (fcentry == null)
                {
                    var newEntry = new database.schemas.FleetCarrier
                    {
                        system_name = msg.message.StarSystem.Replace("'", "."),
                        carriers = carriers
                    };

                    await ctx.FleetCarriers.AddAsync(newEntry);
                }
                else
                {
                    fcentry.carriers = carriers;
                }
                await ctx.SaveChangesAsync();
            }
            return;
        }
    }
}