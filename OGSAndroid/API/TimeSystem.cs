using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using OGSAndroid.Game;

namespace OGSAndroid.API
{
    class TimeSystem
    {
        public enum SystemType { Fischer, Byoyomi, Simple, Canadian, Absolute, None }

        public SystemType ClockType;
        public Stone Player;

        public FischerTime Fischer;
        public ByoyomiTime Byoyomi;
        public SimpleTime Simple;
        public CanadianTime Canadian;
        public AbsoluteTime Absolute;

        public ClockObject Clock;

        public void PopulateClock(JObject clock)
        {
            var playerString = Player ? "black_time" : "white_time";

            switch (ClockType)
            {
                case SystemType.Fischer: case SystemType.Absolute:
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
            Clock.NowDelta = (int)epoch.TotalMilliseconds - clock["now"].Value<int>();
            Clock.LastMove = clock["last_move"].Value<int>();
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
            return "1";
        }


        public struct FischerTime
        {
            public int InitialTime;
            public int TimeIncrement;
            public int MaxTime;
        }

        public struct ByoyomiTime
        {
            public int MainTime;
            public int PeriodTime;
            public int Periods;
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

            //Used by Fischer, Boyoyomi, Canadian, Absolute.
            public double ThinkingTime;

            //Byoyomi.
            public int Periods;
            public int PeriodTime;

            //Simple
            public int SimpleTime;

            //Canadian
            public int MovesLeft;
            public int BlockTime;

            //Global
            public int NowDelta;
            public int LastMove;
            public int BaseTime;

        }

    }
}