using System.Timers;
using Android.App;
using Android.Widget;

namespace OGSAndroid.API
{
    public class TimeControl
    {
        public static TimeSystem Black = new TimeSystem(OGSAndroid.Game.Stone.Black);
        public static TimeSystem White = new TimeSystem(OGSAndroid.Game.Stone.White);

        private readonly static Timer countdown = new Timer(1000);

        public static void Init(TextView black, TextView white, Activity act)
        {
            countdown.Elapsed += (o,e) =>
            {
                    if(((OGSAndroid.Activities.BoardActivity)act).GameView.CurrentTurn.Equals(Game.Stone.Black))
                        Black.Clock.TimeLeft -= 1000;
                    else
                        White.Clock.TimeLeft -= 1000;

                act.RunOnUiThread(() =>
                {
                    black.Text = Black.GuessTime();
                    white.Text = White.GuessTime();
                });
            };
        }

        public static void StartEstimating()
        {
            countdown.Stop();
            countdown.Start();
        }

        public static void StopEstimating()
        {
            countdown.Stop();
        }
    }
}