
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

        private int pages = 1;
        public int Pages { get { return pages; } private set { pages = value; } }

        private List<OGSGame> GameList = new List<OGSGame>();
        private List<string> GameStringList = new List<string>();
        private ListViewInfiniteScroll infiniteScroll;

        private EditText playerNameText;
        private Button searchButton;
        private ListView gameListView;
        private RelativeLayout infiniteLoadingPanel;

        private bool infiniteLoading = false;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);          
            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title.
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.PlayerGameList);
            FlatUI.FlatUI.SetActivityTheme(this, FlatUI.FlatTheme.Dark());

            gameListView = FindViewById<ListView>(Resource.Id.gameList);
            playerNameText = FindViewById<EditText>(Resource.Id.pNameText);
            searchButton = FindViewById<Button> (Resource.Id.searchButton);
            LoadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            infiniteLoadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanelInfinite);

            searchButton.Click += (sender, e) => {

                GameList.Clear();
                GameStringList.Clear();
                LoadPage(1);
                LoadingPanel.Visibility = ViewStates.Visible;
                pages = 1;
            };
                

            gameListView.ItemClick += (o,e) =>
            {
                if(GameList.Count <= 1) return; //Sorry if you have only played one game.

                LoadingPanel.Visibility = ViewStates.Visible;

                ThreadPool.QueueUserWorkItem( thr => {
                    var gamePos = e.Position;
                    CurrentSGF = OGSAPI.IDToSGF(GameList[gamePos].ID);
                    CurrentGame = GameList[gamePos];
                    RunOnUiThread( () => StartActivity(typeof(MainActivity)));  
                });                       
            };

            //Infinite scroll.
            infiniteScroll = new ListViewInfiniteScroll(gameListView);
            infiniteScroll.HitBottom += () =>
            {
                if(!infiniteLoading)
                {        
                    infiniteLoadingPanel.Visibility = ViewStates.Visible;
                    pages++;
                    infiniteLoading = true;
                    LoadPage(pages);
                }
            };
        }

        protected override void OnResume()
        {
            LoadingPanel.Visibility = ViewStates.Gone;
            base.OnResume();
        }

        public void LoadPage(int page)
        {
            InputMethodManager mgr = (InputMethodManager)GetSystemService (Context.InputMethodService);
            mgr.HideSoftInputFromWindow(playerNameText.WindowToken, HideSoftInputFlags.ImplicitOnly);

            ThreadPool.QueueUserWorkItem( o =>{

                var pid = OGSAPI.GetPlayerID(playerNameText.Text);
                if(string.IsNullOrEmpty(pid))
                {
                    RunOnUiThread( () => {
                        gameListView.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1,new [] {"Could not find player, remember, it's case sensitive."});
                        FinishLoading();
                    });
                    return;
                }
                foreach(var gam in OGSAPI.PlayerGameList(pid,page))
                {
                    GameList.Add(gam);
                    GameStringList.Add(gam.ToString());
                }

                RunOnUiThread( () => { 
                    var scrollBarPosition = gameListView.FirstVisiblePosition;
                    gameListView.Adapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleListItem1,GameStringList.ToArray());
                    gameListView.SetSelectionFromTop(scrollBarPosition,0);
                    FinishLoading();
                });   
            });
        }

        private void FinishLoading()
        {
            LoadingPanel.Visibility = ViewStates.Gone;
            infiniteLoadingPanel.Visibility = ViewStates.Gone;
            infiniteLoading = false;
        }
    }
}

