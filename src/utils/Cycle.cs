namespace net.niceygy.eddatacollector
{
    static class Cycles
    {

        public static int GetCycleNo()
        {
            DateTime PowerplayStartDate = new(2024, 10, 31);
            PowerplayStartDate.AddHours(8);

            DateTime now = DateTime.Now;

            TimeSpan difference = now - PowerplayStartDate;

            int weeks = difference.Days / 7;

            return weeks;
        }

        public static int GetMegashipCycle()
        {
            int cycle = GetCycleNo();

            while (cycle > 6)
            {
                cycle -= 6;
            }

            return cycle;
        }
    }
}