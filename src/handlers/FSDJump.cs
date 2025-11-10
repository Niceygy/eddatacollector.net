using net.niceygy.eddatacollector.database;
using net.niceygy.eddatacollector.schemas.FSDJump;

namespace net.niceygy.eddatacollector.handlers
{
    static class FSDJumpHandler
    {
        const int BUBBLE_LIMIT_HIGH = 600;
        const int BUBBLE_LIMIT_LOW = -600;
        public static async Task Handle(FSDJumpMessage msg, EdDbContext ctx)
        {
            var entry = await ctx.StarSystems.FindAsync(msg.message.StarSystem.Replace("'", "."));
            if (entry != null)
            {
                entry.isanarchy = (msg.message.SystemSecurity == "$GAlAXY_MAP_INFO_state_anarchy;");
                
            }
        }
    }
}