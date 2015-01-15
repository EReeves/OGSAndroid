#region

using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FlatUI;
using Newtonsoft.Json;
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
        private readonly List<OGSGame> gameList = new List<OGSGame>();
        private readonly List<string> gameStringList = new List<string>();
        private OGSGame currentGame;
        private ListView gameListView;
        private bool infiniteLoading;
        private RelativeLayout infiniteLoadingPanel;
        private RelativeLayout loadingPanel;
        private int pages = 1;
        private EditText playerNameText;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.PlayerGameList);
            FlatUI.FlatUI.SetActivityTheme(this, FlatTheme.Dark());

            gameListView = FindViewById<ListView>(Resource.Id.gameList);
            playerNameText = FindViewById<EditText>(Resource.Id.pNameText);
            loadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            infiniteLoadingPanel = FindViewById<RelativeLayout>(Resource.Id.loadingPanelInfinite);

            var searchButton = FindViewById<Button>(Resource.Id.searchButton);

            RegisterButtons(ref searchButton, ref gameListView);

            ALog.Info("PlayerGameListActivity", "Created");
        }

        private void RegisterButtons(ref Button search, ref ListView lView)
        {
            search.Click += (sender, e) =>
            {
                gameList.Clear();
                gameStringList.Clear();
                LoadPage(1);
                loadingPanel.Visibility = ViewStates.Visible;
                pages = 1;
            };

            lView.ItemClick += (o, e) =>
            {
                if (gameList.Count <= 1) return; //Sorry if you have only played one game. //Todo: fix.

                loadingPanel.Visibility = ViewStates.Visible;

                //Prepare to start game.
                var gamePos = e.Position;
                currentGame = gameList[gamePos];
                var intent = new Intent(this, typeof (BoardActivity));
                var b = new Bundle();

                //Serialize and send game object.
                var gameJson = JsonConvert.SerializeObject(currentGame);
                intent.PutExtra("game", gameJson);
                intent.PutExtras(b);

                //Start game.
                StartActivity(intent);
            };
        }

        private void InitInfiniteScroll(ref ListViewInfiniteScroll scroll)
        {
            //Infinite scroll.
            scroll = new ListViewInfiniteScroll(gameListView);
            scroll.HitBottom += () =>
            {
                if (infiniteLoading) return;
                infiniteLoadingPanel.Visibility = ViewStates.Visible;
                pages++;
                infiniteLoading = true;
                LoadPage(pages);
            };
        }

        protected override void OnResume()
        {
            loadingPanel.Visibility = ViewStates.Gone;
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
                    RunOnUiThread(() => { PlayerNotFound(); });

                //Load games.
                var games = OGSAPI.I.PlayerGameList(pid, page);
                if (games == null) return;
                foreach (var game in games)
                {
                    gameList.Add(game);
                    gameStringList.Add(game.ToString());
                }

                //Show games.
                RunOnUiThread(PopulateList);
            });
        }

        private void PlayerNotFound()
        {
            gameListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                new[] {"Could not find player, remember, it's case sensitive."});
            FinishLoading();
        }

        private void PopulateList()
        {
            var scrollBarPosition = gameListView.FirstVisiblePosition;
            gameListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                gameStringList.ToArray());
            gameListView.SetSelectionFromTop(scrollBarPosition, 0);
            FinishLoading();
        }

        private void FinishLoading()
        {
            loadingPanel.Visibility = ViewStates.Gone;
            infiniteLoadingPanel.Visibility = ViewStates.Gone;
            infiniteLoading = false;
        }
    }
}