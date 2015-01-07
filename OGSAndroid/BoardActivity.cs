#region

using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using FlatUI;
using OGSAndroid.External.UrlImageHelper;


#endregion

namespace OGSAndroid
{
    [Activity(Label = "Main", Theme = "@android:style/Theme.Holo.Light", Icon = "@drawable/icon", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class BoardActivity : Activity
    {
        public GameView GameView;
        private ChatDrawer chatDrawer;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title. 
            SetContentView(Resource.Layout.Main);
            FlatUI.FlatUI.SetActivityTheme(this, FlatTheme.Dark());

            GameView = FindViewById<GameView>(Resource.Id.GameView);
            //GameView move text.
            var moveText = FindViewById<TextView>(Resource.Id.moveText);
            GameView.MoveNumberText = moveText;
            //GameView.SetSGF(PlayerGameListActivity.CurrentSGF);
            GameView.Connect(PlayerGameListActivity.CurrentGame.ID);
            moveText.TextChanged += (sender, e) =>
            {
                var i = 0;
                if (int.TryParse(moveText.Text, out i))
                    GameView.PlaceUpTo(i);
            };

            //MatchInfo
            var tvl = FindViewById<TextView>(Resource.Id.textBlack);
            tvl.Text = GameView.Moves.Info.LeftString();
            tvl.SetTextColor(Color.Black);
            tvl.Invalidate();

            var tvr = FindViewById<TextView>(Resource.Id.textWhite);
            tvr.Text = GameView.Moves.Info.RightString();
            tvr.SetTextColor(Color.Black);
            tvr.Invalidate();

            //Avatar
            var bimg = FindViewById<ImageView>(Resource.Id.blackImage);
            var wimg = FindViewById<ImageView>(Resource.Id.whiteImage);

            bimg.SetUrlDrawable(PlayerGameListActivity.CurrentGame.Black.Icon, Resource.Drawable.defaultuser);
            wimg.SetUrlDrawable(PlayerGameListActivity.CurrentGame.White.Icon, Resource.Drawable.defaultuser);

            //NextMove
            var next = FindViewById<Button>(Resource.Id.button5);
            var hbr = new HoldButtonRepeat(next, 400);
            hbr.Invoke += () => RunOnUiThread(() => GameView.Next());

            //SumbitButton
            var submitButton = FindViewById<Button>(Resource.Id.button4);
            submitButton.Click += (sender, args) => GameView.SubmitMove();

            //Previous
            var prev = FindViewById<Button>(Resource.Id.button3);
            prev.Click += (sender, e) => GameView.Previous();

            //Start
            var start = FindViewById<Button>(Resource.Id.button2);
            start.Click += (sender, e) => GameView.ToStart();

            //End
            var end = FindViewById<Button>(Resource.Id.button6);
            end.Click += (sender, e) => GameView.ToEnd();

            //toolbar temp
            var tbb = FindViewById<Button>(Resource.Id.toolbarButton);
            tbb.Text = "ToolbarTemp";

            //Chat drawer
            var chatDrawerScroll = FindViewById<ScrollView>(Resource.Id.rightDrawerScroll);
            var chatDrawerView = FindViewById<SlidingDrawer>(Resource.Id.rightDrawer);
            var chatDrawerText = FindViewById<TextView>(Resource.Id.chatText);
            chatDrawer = new ChatDrawer(chatDrawerView, chatDrawerText);
            GameView.boardTouch.OnTouchEvent += (e) =>
            {
                chatDrawer.GestureDetector.OnTouchEvent(e);
            };
            

            //Apply chat text 
            //chatDrawer.ChatText = ChatDrawer.StringListToString(PlayerGameListActivity.CurrentSGF.Info.ChatMessages);

            //Stop scrollview from consuming gesture events.
            chatDrawerScroll.SetOnTouchListener(chatDrawer);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            chatDrawer.InvokeMotionEvent(e);
            base.OnTouchEvent(e);
            return false;
        }
    }
}