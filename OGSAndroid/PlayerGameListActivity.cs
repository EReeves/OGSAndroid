
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OGSAndroid
{
    [Activity (Label = "PlayerGameListActivity", Theme = "@android:style/Theme.Holo.Light", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]		
    public class PlayerGameListActivity : Activity
    {
        public static SGF<Move> CurrentSGF;
        public static OGSGame CurrentGame;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title.

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.PlayerGameList);

            FlatUI.FlatUI.SetActivityTheme(this, FlatUI.FlatTheme.Dark());

            var gameList = FindViewById<ListView>(Resource.Id.gameList);
            var pNameText = FindViewById<EditText>(Resource.Id.pNameText);
            var searchButton = FindViewById<Button> (Resource.Id.searchButton);

            OGSGame[] gameArray = null;

            searchButton.Click += (sender, e) =>  
            {
                    var pid = OGSAPI.GetPlayerID(pNameText.Text);
                    gameArray = OGSAPI.PlayerGameList(pid,1);

                    var lst = new List<string>();

                    foreach( var g in gameArray)
                    {
                        lst.Add(g.ToString());
                    }

                    gameList.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1,lst.ToArray());
            };

            gameList.ItemClick += (o,e) =>
            {
                var gamePos = e.Position;

                CurrentSGF = OGSAPI.IDToSGF(gameArray[gamePos].ID);
                CurrentGame = gameArray[gamePos];
                StartActivity(typeof(MainActivity));
            };



        }
    }
}

