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
                    var system = await ctx.Megaships.FindAsync(signal.SignalName);
                    if (system != null)
                    {
                        // Update properties
                        
                        // Save changes
                        await ctx.SaveChangesAsync();

                    }
                }
            }

            return;
        }


    }
}