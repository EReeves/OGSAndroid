#region

using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.InputMethodServices;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FlatUI;
using Javax.Security.Auth;
using OGSAndroid.Activities.Ext;
using OGSAndroid.API;
using OGSAndroid.Game;

#endregion

namespace OGSAndroid.Activities
{
    [Activity(Label = "PlayerGameListActivity", Theme = "@android:style/Theme.Holo.Light",
        Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PlayerGameListActivity : Activity
    {
        public static SGF<Move> CurrentSGF;
        public static OGSGame CurrentGame;
        public static RelativeLayout LoadingPanel;

        private ListView gameListView;
        private bool infiniteLoading;
        private RelativeLayout infiniteLoadingPanel;
        private ListViewInfiniteScroll infiniteScroll;
        private int pages = 1;
        private EditText playerNameText;
        private Button searchButton;
        private readonly List<OGSGame> gameList = new List<OGSGame>();
        private readonly List<string> gameStringList = new List<string>();

        public int Pages
        {
            get { return pages; }
            private set { pages = value; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.PlayerGameList);
            FlatUI.FlatUI.SetActivityTheme(this, FlatTheme.Dark());

            gameListView = FindViewById<ListView>(Resource.Id.gameList);
            playerNameText = FindViewById<EditText>(Resource.Id.pNameText);
            searchButton = FindViewById<Button>(Resource.Id.searchButton);
            LoadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            infiniteLoadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanelInfinite);

            searchButton.Click += (sender, e) =>
            {
                gameList.Clear();
                gameStringList.Clear();
                LoadPage(1);
                LoadingPanel.Visibility = ViewStates.Visible;
                pages = 1;
            };

            gameListView.ItemClick += (o, e) =>
            {
                if (gameList.Count <= 1) return; //Sorry if you have only played one game. //Todo: fix.

                LoadingPanel.Visibility = ViewStates.Visible;

                var gamePos = e.Position;
                CurrentGame = gameList[gamePos];
                StartActivity(typeof (BoardActivity))););
            };

            //Infinite scroll.
            infiniteScroll = new ListViewInfiniteScroll(gameListView);
            infiniteScroll.HitBottom += () =>
            {
                if (infiniteLoading) return;
                infiniteLoadingPanel.Visibility = ViewStates.Visible;
                pages++;
                infiniteLoading = true;
                LoadPage(pages);
            };

            ALog.Info("PlayerGameListActivity", "Created");
        }

        protected override void OnResume()
        {
            LoadingPanel.Visibility = ViewStates.Gone;
            base.OnResume();
        }

        public void LoadPage(int page)
        {
            //This is a little messy because of the threading, separate if you need to add much more.
            var mgr = (InputMethodManager) GetSystemService(InputMethodService);
            mgr.HideSoftInputFromWindow(playerNameText.WindowToken, HideSoftInputFlags.ImplicitOnly);

            //Run heavy stuff on another thread.
            ThreadPool.QueueUserWorkItem(o =>
            {
                //Find player.
                var pid = OGSAPI.I.GetPlayerID(playerNameText.Text);
                if (string.IsNullOrEmpty(pid))
                    RunOnUiThread(() => { PlayerNotFound(); return; });

                //Load games.
                foreach (var gam in OGSAPI.I.PlayerGameList(pid, page))
                {
                    gameList.Add(gam);
                    gameStringList.Add(gam.ToString());
                }

                //Show games.
                RunOnUiThread(PopulateList);

            });

        }


        private void PlayerNotFound()
        {
            gameListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
            new[] { "Could not find player, remember, it's case sensitive." });
            FinishLoading();
        }

        private void PopulateList()
        {
            var scrollBarPosition = gameListView.FirstVisiblePosition;
            gameListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, gameStringList.ToArray());
            gameListView.SetSelectionFromTop(scrollBarPosition, 0);
            FinishLoading();
        }

        private void FinishLoading()
        {
            LoadingPanel.Visibility = ViewStates.Gone;
            infiniteLoadingPanel.Visibility = ViewStates.Gone;
            infiniteLoading = false;
        }
    }
}