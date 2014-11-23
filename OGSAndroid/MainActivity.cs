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
            SetContentView (Resource.Layout.Main);                    
            FlatUI.FlatUI.SetActivityTheme(this, FlatUI.FlatTheme.Dark());

            BoardView = FindViewById<SGFView>(Resource.Id.SGFView);
            //BoardView move text.
            var moveText = FindViewById<TextView>(Resource.Id.moveText);
            BoardView.MoveNumberText = moveText;
            BoardView.SetSGF(PlayerGameListActivity.CurrentSGF);
            moveText.TextChanged += (sender, e) =>
            {
                int i = 0;
                if (int.TryParse(moveText.Text, out i))
                    BoardView.PlaceUpTo(i);
            };

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

            //toolbar temp
            Button tbb = FindViewById<Button>(Resource.Id.toolbarButton);
            tbb.Text = "ToolbarTemp";


		}
	}
}


