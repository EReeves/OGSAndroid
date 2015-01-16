#region

using System;
using Android.Text;
using Android.Text.Style;
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

        public bool HideByoyomi = false;
        public bool ByoyomiEstimateStart = false;
     

        public string ByoyomiString = "";

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

            bool byoyomi = false;

            switch (ClockType)
            {
                case SystemType.Fischer:

                    break;
                case SystemType.Absolute:

                    break;

                case SystemType.Byoyomi:
                    if (timeLeft < 0 || Clock.ThinkingTime == 0)
                    {
                        HideByoyomi = true;

                        var periodOffset = Math.Floor((-timeLeft/1000)/Clock.PeriodTime);
                        periodOffset = Math.Max(0, periodOffset);

                        while (timeLeft < 0)
                            timeLeft += Clock.PeriodTime*1000;

                        if (Clock.Periods - periodOffset - 1 < 0)
                            timeLeft = 0;

                        //Clock.Periods = (int) ((Clock.Periods - periodOffset) - 1);

                        byoyomi = true;
                    }
                    break;

                case SystemType.Simple:
                    //?
                    break;

                case SystemType.Canadian:
                    if (timeLeft < 0 || Clock.ThinkingTime == 0)
                    {
                        HideByoyomi = true;

                        timeLeft = Clock.BaseTime + (Clock.ThinkingTime + Clock.BlockTime)*1000 -
                                   epoch.TotalMilliseconds;
                        byoyomi = true;
                    }
                    break;
            }
                    
            var left = TimeSpan.FromMilliseconds(timeLeft);

            Clock.TimeLeft = timeLeft;

            //In case of desync
            if (ClockType == SystemType.Byoyomi && timeLeft > Clock.PeriodTime*1000)
                HideByoyomi = false;

            return TimespanToString(left, byoyomi);

        }

        public string TimespanToString(TimeSpan ts, bool byoyomi = false)
        {
            var result = "";

            if (ts.Days != 0)
            {
                result = ts.Days + "D:" + ts.Hours + "H";
            }
            else if (ts.Hours != 0)
            {
                result = ts.Hours + "H:" + PrefixZero(ts.Minutes.ToString()) + "M";
            }
            else if (byoyomi)
            {
                result = TimespanToString(ts, false) + "x" + Clock.Periods;
            }
            else result = ts.Minutes + ":" +  PrefixZero((ts.Seconds+1).ToString());

            if (ClockType == SystemType.Byoyomi )
            {
                ByoyomiString = "\n+" + Clock.PeriodTime + "x" + Clock.Periods ;
            }

            return result;
        }

        public string GuessTime()
        {
            var ts = TimeSpan.FromMilliseconds(Clock.TimeLeft);

            if (ClockType == SystemType.Byoyomi)
            {


                if (Clock.TimeLeft < 0)
                {
                    HideByoyomi = true;
                    Clock.TimeLeft = Clock.PeriodTime*1000;

                    if(ByoyomiEstimateStart)
                        Clock.Periods -= 1;

                    ByoyomiEstimateStart = true;
                }
                return Clock.Periods <= 0 ? TimespanToString(TimeSpan.FromMilliseconds(-1000)) : TimespanToString(ts, HideByoyomi);
            }
            return TimespanToString(ts);
        }

        private string PrefixZero(string str)
        {
            return str.Length == 1 ? "0" + str : str;              
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