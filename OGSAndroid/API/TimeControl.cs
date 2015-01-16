using System.Timers;
using Android.App;
using Android.Widget;

namespace OGSAndroid.API
{
    public class TimeControl
    {
        public static TimeSystem Black = new TimeSystem(OGSAndroid.Game.Stone.Black);
        public static TimeSystem White = new TimeSystem(OGSAndroid.Game.Stone.White);

        private static Timer countdown;

        public static void Init(TextView black, TextView white, OGSAndroid.Activities.BoardActivity act)
        {
            if(countdown != null)
                countdown.Dispose();
            countdown = new Timer(1000);
            countdown.Elapsed += (o,e) =>
            {
                    if(((OGSAndroid.Activities.BoardActivity)act).GameView.CurrentAPIMove.Equals(Game.Stone.Black))
                        Black.Clock.TimeLeft -= 1000;
                    else
                        White.Clock.TimeLeft -= 1000;

                act.RunOnUiThread(() =>
                {
                    black.Text = Black.GuessTime();
                    white.Text = White.GuessTime();
                    act.SetByoyomiStrings(Black.ByoyomiString,White.ByoyomiString);

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