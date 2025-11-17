using net.niceygy.eddatacollector.database;
using net.niceygy.eddatacollector.schemas.FSDJump;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace net.niceygy.eddatacollector.handlers
{
    static class FSDJumpHandler
    {
        const int BUBBLE_LIMIT_HIGH = 600;
        const int BUBBLE_LIMIT_LOW = -600;
        public static async Task Handle(FSDJumpMessage msg, DbContextOptions options)
        {
            if (!MessageCheck.IsValid(msg.header))
            {
                return;
            }
            else if (!SystemWithinRange(msg.message.StarPos))
            {
                return;
            }

            using var ctx = new EdDbContext(options);

            var entry = await ctx.StarSystems.FindAsync(msg.message.StarSystem.Replace("'", "."));
            if (entry != null)
            {
                entry.is_anarchy = msg.message.SystemSecurity == "$GAlAXY_MAP_INFO_state_anarchy;";
                entry.frequency++;
            }
            else
            {
                var newSystem = new database.schemas.StarSystem
                {
                    system_name = msg.message.StarSystem.Replace("'", "."),
                    is_anarchy = msg.message.SystemSecurity == "$GAlAXY_MAP_INFO_state_anarchy;",
                    frequency = 1,
                    latitude = (float)msg.message.StarPos[0],
                    longitude = (float)msg.message.StarPos[1],
                    height = (float)msg.message.StarPos[2]
                };

                await ctx.StarSystems.AddAsync(newSystem);
            }

            await ctx.SaveChangesAsync();
            return;
        }

        
        
        /// <summary>
        /// Are the inputted coords within the BUBBLE_LIMIT_HIGH
        /// and BUBBLE_LIMIT_LOW?
        /// </summary>
        /// <param name="StarPos"></param>
        /// <returns></returns>
        private static bool SystemWithinRange(List<double> StarPos)
        {
            bool res = true;

            res = res & StarPos[0] <= BUBBLE_LIMIT_HIGH & StarPos[0] >= BUBBLE_LIMIT_LOW;
            res = res & StarPos[1] <= BUBBLE_LIMIT_HIGH & StarPos[1] >= BUBBLE_LIMIT_LOW;
            res = res & StarPos[2] <= BUBBLE_LIMIT_HIGH & StarPos[2] >= BUBBLE_LIMIT_LOW;

            if (!res)
            {
                Log.Verbose($"System rejected. Coords of {StarPos[0]}/{StarPos[1]}/{StarPos[2]} (x/y/z)");
            }

            return res;
        }
    }
}