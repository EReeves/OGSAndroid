#region

using System;
using Newtonsoft.Json.Linq;
using OGSAndroid.Game;

#endregion

namespace OGSAndroid.API
{
    public class TimeSystem
    {
        public enum SystemType
        {
            Fischer,
            Byoyomi,
            Simple,
            Canadian,
            Absolute,
            None
        }

        public TimeSystem(Stone player)
        {
            Player = player;
        }

        public AbsoluteTime Absolute;
        public ByoyomiTime Byoyomi;
        public CanadianTime Canadian;
        public ClockObject Clock;
        public SystemType ClockType;
        public FischerTime Fischer;
        public Stone Player;
        public SimpleTime Simple;

        public void SetTimeSystem(string time)
        {
            switch(time)
            {
                case "simple":
                    break;
                case "byoyomi":
                    ClockType = SystemType.Byoyomi;
                    break;
                case "canadian":
                    break;
                case "absolute":
                    break;
                case "fischer":
                    break;
            }
        }

        public void PopulateClock(JObject clock)
        {
            var playerString = Player ? "black_time" : "white_time";

            switch (ClockType)
            {
                case SystemType.Fischer:
                case SystemType.Absolute:
                    Clock.ThinkingTime = clock[playerString]["thinking_time"].Value<double>();
                    break;

                case SystemType.Byoyomi:
                    Clock.ThinkingTime = clock[playerString]["thinking_time"].Value<double>();
                    Clock.Periods = clock[playerString]["periods"].Value<int>();
                    Clock.PeriodTime = clock[playerString]["period_time"].Value<int>();
                    break;

                case SystemType.Simple:
                    //?
                    break;

                case SystemType.Canadian:
                    Clock.ThinkingTime = clock[playerString]["thinking_time"].Value<double>();
                    Clock.MovesLeft = clock[playerString]["moves_left"].Value<int>();
                    Clock.BlockTime = clock[playerString]["block_time"].Value<int>();
                    break;
            }

            var epoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            Clock.NowDelta = epoch.TotalMilliseconds - clock["now"].Value<double>();
            Clock.LastMove = clock["last_move"].Value<double>();
            Clock.BaseTime = Clock.LastMove + Clock.NowDelta;
        }

        public string ClockString()
        {
            var epoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var timeLeft = Clock.BaseTime + Clock.ThinkingTime*1000 - epoch.TotalMilliseconds;

            switch (ClockType)
            {
                case SystemType.Fischer:

                    break;
                case SystemType.Absolute:

                    break;

                case SystemType.Byoyomi:
                    if (timeLeft < 0 || Clock.ThinkingTime == 0)
                    {
                        var periodOffset = Math.Floor((-timeLeft/1000)/Clock.PeriodTime);
                        periodOffset = Math.Max(0, periodOffset);

                        while (timeLeft < 0)
                            timeLeft += Clock.PeriodTime*1000;

                        if (Clock.Periods - periodOffset - 1 < 0)
                            timeLeft = 0;

                        Clock.Periods = (int) ((Clock.Periods - periodOffset) - 1);
                    }
                    break;

                case SystemType.Simple:
                    //?
                    break;

                case SystemType.Canadian:
                    if (timeLeft < 0 || Clock.ThinkingTime == 0)
                    {
                        timeLeft = Clock.BaseTime + (Clock.ThinkingTime + Clock.BlockTime)*1000 -
                                   epoch.TotalMilliseconds;
                    }
                    break;
            }
                    
            var left = TimeSpan.FromMilliseconds(timeLeft);

            Clock.TimeLeft = timeLeft;

            return TimespanToString(left);

        }

        public string TimespanToString(TimeSpan ts)
        {
            var result = "";
           
            if (ts.Days != 0)
            {
                return ts.Days + "D:" + ts.Hours + "H";
            }

            if (ts.Hours != 0)
            {
                return ts.Hours + "H:" + ts.Minutes + "M";
            }

            return ts.Minutes + ":" + ts.Seconds;
        }

        public string GuessTime()
        {
            var ts = TimeSpan.FromMilliseconds(Clock.TimeLeft);
            return TimespanToString(ts);
        }

        public struct FischerTime
        {
            public int InitialTime;
            public int MaxTime;
            public int TimeIncrement;
        }

        public struct ByoyomiTime
        {
            public int MainTime;
            public int Periods;
            public int PeriodTime;
        }

        public struct SimpleTime
        {
            public int PerMove;
        }

        public struct CanadianTime
        {
            public int MainTime;
            public int PeriodTime;
            public int StonesPerPeriod;
        }

        public struct AbsoluteTime
        {
            public int TotalTime;
        }

        public struct ClockObject
        {
            public double BaseTime;
            public double BlockTime;
            public double LastMove;
            //Canadian
            public int MovesLeft;
            //Global
            public double NowDelta;
            //Byoyomi.
            public int Periods;
            public double PeriodTime;
            //Simple
            public double SimpleTime;
            //Used by Fischer, Boyoyomi, Canadian, Absolute.
            public double ThinkingTime;

            public double TimeLeft;
        }
    }
}