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
            //bv.PlaceAllSGFMoves();

            foreach (var node in Node<Move>.Next(sgf.Tree.First()))
            {
                Console.WriteLine(node.ToString());
            }


            //Console.WriteLine(sgf.Tree.First());

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

            Button next = FindViewById<Button>(Resource.Id.button5);
            Node<Move>[] cnode = Node<Move>.Next(sgf.Tree.First()).ToArray();
            int i = 0;

            next.Click += (sender, e) =>  {

                if(i>cnode.Count()-1) return;            
                bv.PlaceStone(cnode[i].Data);
                i++;
            };
		}




	}

}


