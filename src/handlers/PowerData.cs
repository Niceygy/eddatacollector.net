using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using net.niceygy.eddatacollector.database;
using net.niceygy.eddatacollector.schemas.FSDJump;

namespace net.niceygy.eddatacollector.handlers
{
    static class PowerDataHandler
    {
        /// <summary>
        /// Min number of points for the top two powers to trigger
        /// a conflict for control of the system.
        /// </summary>
        private const decimal CONFLICT_THRESHOLD = (decimal)0.3;
        public static async Task UpdatePowerData(FSDJumpMessage msg, EdDbContext ctx)
        {
            if (!HasPowerPresence(msg.message))
            {
                return;
            }
            await UpdateConflictData(msg.message, ctx);

            var entry = await ctx.PowerDatas.FindAsync(msg.message.StarSystem.Replace("'", "."));

            decimal journalControlPoints = 0;
            try
            {
                journalControlPoints = msg.message.PowerplayConflictProgress == null ? (decimal)msg.message.PowerplayStateControlProgress! : msg.message.PowerplayConflictProgress![0].ConflictProgress;
            }
            catch (Exception)
            {//no powers present
                return;
            }

            decimal controlPoints = CorrectControlPoints(journalControlPoints, msg.message.PowerplayState);

            if (entry != null)
            {
                entry.state = msg.message.PowerplayState;
                entry.control_points = (float)controlPoints;
                entry.shortcode = PowersInfo.PowerShortCodes[msg.message.ControllingPower!];
            }
            else
            {
                var newEntry = new database.schemas.PowerData
                {
                    shortcode = PowersInfo.PowerShortCodes[msg.message.ControllingPower!],
                    system_name = msg.message.StarSystem.Replace("'", "."),
                    control_points = (float)controlPoints,
                    state = msg.message.PowerplayState
                };

                await ctx.PowerDatas.AddAsync(newEntry);
            }

            await ctx.SaveChangesAsync();

        }

        /// <summary>
        /// Updates the conflicts table
        /// </summary>
        /// <param name="msg">FSDJump Message</param>
        /// <param name="ctx">Current database context</param>
        /// <returns></returns>
        private static async Task UpdateConflictData(Message msg, EdDbContext ctx)
        {
            if (msg.PowerplayConflictProgress != null && msg.PowerplayConflictProgress.Count >= 2)
            {//any power conflict data is there
                PowerplayConflictEntry firstPlace = msg.PowerplayConflictProgress[0];
                PowerplayConflictEntry secondPlace = msg.PowerplayConflictProgress[1];

                foreach (PowerplayConflictEntry entry in msg.PowerplayConflictProgress!)
                {//can assert "!", because of inital IsNullOrEmpty check
                    if (entry.ConflictProgress > firstPlace.ConflictProgress)
                    {
                        firstPlace = entry;
                    }
                    else if (entry.ConflictProgress > secondPlace.ConflictProgress)
                    {
                        secondPlace = entry;
                    }
                }


                database.schemas.Conflict? sys = await ctx.Conflicts.FindAsync(msg.StarSystem.Replace("'", "."));

                bool has_conflic_zones = (
                    firstPlace.ConflictProgress > CONFLICT_THRESHOLD &&
                    secondPlace.ConflictProgress > CONFLICT_THRESHOLD
                    );

                if (sys == null)
                {// no logged wars, add one
                    var newConflict = new database.schemas.Conflict
                    {
                        system_name = msg.StarSystem.Replace("'", "."),
                        first_place = firstPlace.Power,
                        second_place = secondPlace.Power,
                        has_czs = has_conflic_zones,
                        cycle = Cycles.GetCycleNo()

                    };

                    await ctx.Conflicts.AddAsync(newConflict);
                }
                else
                {//exists, update it
                    sys.first_place = firstPlace.Power;
                    sys.second_place = secondPlace.Power;
                    sys.has_czs = has_conflic_zones;
                }

                await ctx.SaveChangesAsync();
            }
        }

        private static decimal CorrectControlPoints(decimal progress, SystemStates.SystemState state)
        {
            int scale = 0;
            if (progress > 4000)
            {
                scale = state switch
                {
                    SystemStates.SystemState.Exploited => 349999,
                    SystemStates.SystemState.Fortified => 650000,
                    SystemStates.SystemState.Stronghold => 1000000,
                    _ => 120000,
                };
                progress -= 4294967296 / scale;
            }
            return progress;
        }

        private static bool HasPowerPresence(Message msg)
        {
            try
            {
                bool result = false;

                result = result || msg.ControllingPower == null;

                result = result || msg.PowerplayConflictProgress == null;

                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}