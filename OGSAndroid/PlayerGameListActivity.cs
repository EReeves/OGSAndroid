#region

using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FlatUI;

#endregion

namespace OGSAndroid
{
    [Activity(Label = "PlayerGameListActivity", Theme = "@android:style/Theme.Holo.Light", MainLauncher = true,
        Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PlayerGameListActivity : Activity
    {
        public static SGF<Move> CurrentSGF;
        public static OGSGame CurrentGame;
        public static RelativeLayout LoadingPanel;

        private readonly List<OGSGame> gameList = new List<OGSGame>();
        private readonly List<string> gameStringList = new List<string>();
        private ListView gameListView;
        private bool infiniteLoading;
        private RelativeLayout infiniteLoadingPanel;
        private ListViewInfiniteScroll infiniteScroll;
        private int pages = 1;

        private EditText playerNameText;
        private Button searchButton;

        public int Pages
        {
            get { return pages; }
            private set { pages = value; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title.
            // Set our view from the "main" layout resource
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
                if (gameList.Count <= 1) return; //Sorry if you have only played one game.

                LoadingPanel.Visibility = ViewStates.Visible;

                ThreadPool.QueueUserWorkItem(thr =>
                {
                    int gamePos = e.Position;
                    CurrentSGF = OGSAPI.IDToSGF(gameList[gamePos].ID);
                    CurrentGame = gameList[gamePos];
                    RunOnUiThread(() => StartActivity(typeof (MainActivity)));
                });
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

            ThreadPool.QueueUserWorkItem(o =>
            {
                string pid = OGSAPI.GetPlayerID(playerNameText.Text);
                if (string.IsNullOrEmpty(pid))
                {
                    RunOnUiThread(() =>
                    {
                        gameListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                            new[] {"Could not find player, remember, it's case sensitive."});
                        FinishLoading();
                    });
                    return;
                }
                foreach (var gam in OGSAPI.PlayerGameList(pid, page))
                {
                    gameList.Add(gam);
                    gameStringList.Add(gam.ToString());
                }

                RunOnUiThread(() =>
                {
                    int scrollBarPosition = gameListView.FirstVisiblePosition;
                    gameListView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1,
                        gameStringList.ToArray());
                    gameListView.SetSelectionFromTop(scrollBarPosition, 0);
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