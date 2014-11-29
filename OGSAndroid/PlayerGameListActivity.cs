
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Diagnostics;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;


namespace OGSAndroid
{
    [Activity (Label = "PlayerGameListActivity", Theme = "@android:style/Theme.Holo.Light", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]		
    public class PlayerGameListActivity : Activity
    {
        public static SGF<Move> CurrentSGF;
        public static OGSGame CurrentGame;
        public static RelativeLayout LoadingPanel;

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
            LoadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);

            OGSGame[] gameArray = null;

            searchButton.Click += (sender, e) => 
            {
                    var lst = new List<string>();

                    LoadingPanel.Visibility = ViewStates.Visible;
                                   
                    InputMethodManager mgr = (InputMethodManager)GetSystemService (Context.InputMethodService);
                    mgr.HideSoftInputFromWindow(pNameText.WindowToken, HideSoftInputFlags.ImplicitOnly);

                    ThreadPool.QueueUserWorkItem( o =>{
                       
                        var pid = OGSAPI.GetPlayerID(pNameText.Text);
                        gameArray = OGSAPI.PlayerGameList(pid,1);

                        foreach( var g in gameArray)
                        {
                            lst.Add(g.ToString());
                        }

                        RunOnUiThread( () => { 
                            gameList.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1,lst.ToArray());
                            LoadingPanel.Visibility = ViewStates.Gone;
                        });   
                    });
                               
            };

            gameList.ItemClick += (o,e) =>
            {
                    LoadingPanel.Visibility = ViewStates.Visible;

                    ThreadPool.QueueUserWorkItem( thr => {
                    var gamePos = e.Position;
                        CurrentSGF = OGSAPI.IDToSGF(gameArray[gamePos].ID);
                    CurrentGame = gameArray[gamePos];
                        RunOnUiThread( () => StartActivity(typeof(MainActivity)));  
                    });                       
            };



        }

        protected override void OnResume()
        {
            LoadingPanel.Visibility = ViewStates.Gone;
            base.OnResume();
        }
    }
}

