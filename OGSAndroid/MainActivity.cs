using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

using UrlImageViewHelper;

namespace OGSAndroid
{
    [Activity (Label = "Main", Theme = "@android:style/Theme.Holo.Light", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{
        public SGFView BoardView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title.


			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            var customJabbrTheme = new FlatUI.FlatTheme () {
                DarkAccentColor = Android.Graphics.Color.ParseColor("#7F5E42"),
                BackgroundColor = Android.Graphics.Color.ParseColor("#0A6887"),
                LightAccentColor = Android.Graphics.Color.ParseColor("#875018"),
                VeryLightAccentColor = Android.Graphics.Color.ParseColor("#52DB64")
            };

            FlatUI.FlatUI.SetActivityTheme(this, FlatUI.FlatTheme.Dark());

            BoardView = FindViewById<SGFView>(Resource.Id.SGFView);


           /* OGSAndroid.SGFParser ps = new SGFParser();

            var sgf = ps.Parse(
                @"
(;FF[4]
GM[1]
DT[2014-10-30]
PC[OGS: http://online-go.com/game/1035215]
PB[Rvzy]
PW[sashaib]
BR[20k]
WR[13k]
CP[online-go.com]
RE[B+3.5]
SZ[13]
KM[5.5]
RU[japanese]
HA[3]
AB[jj][jd][dj]
C[Rvzy: Hey and good lick
]
C[Rvzy: luck*
]
;W[cd]
C[Rvzy: Hey and good lick
]
C[Rvzy: luck*
]
;B[gc]
;W[ci]
;B[cg]
;W[ck]
;B[dc]
;W[cc]
;B[kg]
;W[dh]
;B[gj]
;W[fk]
;B[gk]
;W[fj]
;B[fi]
;W[ei]
;B[fh]
;W[dd]
;B[ff]
;W[ec]
;B[fd]
;W[ef]
;B[eg]
;W[dg]
;B[ee]
;W[df]
;B[kc]
;W[fc]
;B[gd]
;W[ed]
;B[kk]
;W[gl]
;B[hl]
;W[gm]
;B[hm]
C[Rvzy: Considering I was ahead I assume it was worth sacrificing a few points on the main wall to cover the 3-3 points and prevent an invade? 
]
;W[el]
;B[fb]
;W[eb]
;B[fa]
;W[ea]
;B[hb]
;W[hk]
;B[hj]
;W[ik]
;B[ij]
;W[jl]
;B[jk]
;W[il]
;B[kl]
;W[km]
;B[lm]
;W[im]
;B[jm]
;W[eh]
;B[fg]
;W[km]
;B[ll]
;W[de]
;B[fe]
;W[jm]
;B[]
;W[]
C[Rvzy: Thanks for the game
]
C[Rvzy: you nearly came back with my mistake at the end :p
]
)
                "
                );

            BoardView.SetSGF(sgf);*/

            BoardView.SetSGF(PlayerGameListActivity.CurrentSGF);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
            //Clear Board
			button.Click += delegate {
                BoardView.ClearBoard();
                BoardView.ToStart();
			};

            //Debug Pass
           /* Button button1 = FindViewById<Button> (Resource.Id.button1);
            button1.Click += delegate {

                Stone[] stones =  BoardView.stones.Select(x => (Stone)x.Clone()).ToArray();
                foreach(Stone bs in stones)
                {
                    BoardView.CapturePass(bs);
                }
                BoardView.Invalidate();
            };*/

            //MatchInfo
            TextView tvl = FindViewById<TextView>(Resource.Id.textBlack);
            tvl.Text = BoardView.Moves.Info.LeftString();
            tvl.SetTextColor(Android.Graphics.Color.Black);
            tvl.Invalidate();

            TextView tvr = FindViewById<TextView>(Resource.Id.textWhite);
            tvr.Text = BoardView.Moves.Info.RightString();
            tvr.SetTextColor(Android.Graphics.Color.Black);
            tvr.Invalidate();

            //Avatar
            var bimg = FindViewById<ImageView>(Resource.Id.blackImage);
            var wimg = FindViewById<ImageView>(Resource.Id.whiteImage);

            bimg.SetUrlDrawable(PlayerGameListActivity.CurrentGame.Black.Icon, Resource.Drawable.Icon);
            wimg.SetUrlDrawable(PlayerGameListActivity.CurrentGame.White.Icon, Resource.Drawable.Icon);


            //NextMove
            Button next = FindViewById<Button>(Resource.Id.button5);
            next.Click += (sender, e) =>   BoardView.Next();

            //Previous
            Button prev = FindViewById<Button>(Resource.Id.button3);
            prev.Click += (sender, e) =>   BoardView.Previous();

            //Start
            Button start = FindViewById<Button>(Resource.Id.button2);
            start.Click += (sender, e) =>   BoardView.ToStart();
            
            //End
            Button end = FindViewById<Button>(Resource.Id.button6);
            end.Click += (sender, e) =>  BoardView.ToEnd();  




            var pid = OGSAPI.GetPlayerID("Rvzy");
            Console.WriteLine(pid);

            var arr = OGSAPI.PlayerGameList(pid,1);
            Console.WriteLine(arr.Count());

		}
	}
}


