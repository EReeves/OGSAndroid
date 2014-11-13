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

namespace OGSAndroid
{
    [Activity (Label = "Go", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title.

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            SGFView bv = FindViewById<SGFView>(Resource.Id.SGFView);

            OGSAndroid.SGFParser ps = new SGFParser();

            var sgf = ps.Parse(
                @"
;FF[4]
GM[1]
PC[OGS: http://online-go.com/game/review/38888]
BR[8d]
WR[8d]
CP[online-go.com]
RE[?]
SZ[19]
KM[6.5]
RU[japanese]

;B[aa]
(;W[ba]
;B[ca]
(;W[da]
;B[ea]
;W[fa]
;B[ga]
;W[ha]
;B[ia]
;W[ja]
)(;W[ab]
;B[ac]
;W[ad]
(;B[ae]
;W[af]
;B[ag]
)(;B[bd]
;W[cd]
;B[dd]
;W[ed]
)))(;W[ab]
;B[ba]
)
                "
                );
            //bv.PlaceAllSGFMoves();

            Console.WriteLine(sgf.Tree.First());

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
                bv.ClearBoard();
			};

            Button button1 = FindViewById<Button> (Resource.Id.button1);

            button1.Click += delegate {

                Stone[] stones = bv.stones.Select(x => (Stone)x.Clone()).ToArray();
                foreach(Stone bs in stones)
                {
                    bv.CapturePass(bs);
                }
                bv.Invalidate();
            };

            TextView tv = FindViewById<TextView>(Resource.Id.statText);
            tv.Text = bv.Info.String();
            tv.Invalidate();
		}




	}

}


