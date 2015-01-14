#region

using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using FlatUI;
using Newtonsoft.Json;
using OGSAndroid.API;
using OGSAndroid.External.UrlImageHelper;
using OGSAndroid.Game;

#endregion

namespace OGSAndroid.Activities
{
    [Activity(Label = "Main", Theme = "@android:style/Theme.Holo.Light", Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class BoardActivity : Activity
    {
        private ChatDrawer chatDrawer;
        public GameView GameView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title. 
            SetContentView(Resource.Layout.Main);

            var theme = new FlatUI.FlatTheme () {
                DarkAccentColor = Android.Graphics.Color.ParseColor("#00103f"),
                BackgroundColor = Android.Graphics.Color.ParseColor("#424242"),
                LightAccentColor = Android.Graphics.Color.ParseColor("#a5a5a5"),
                VeryLightAccentColor = Android.Graphics.Color.ParseColor("#000000")
            };

            FlatUI.FlatUI.SetActivityTheme(this, theme);

            GameView = FindViewById<GameView>(Resource.Id.GameView);
            //GameView move text.
            var moveText = FindViewById<TextView>(Resource.Id.moveText);
            GameView.MoveNumberText = moveText;

            moveText.TextChanged += (sender, e) =>
            {
                if (!moveText.HasFocus) return;
                var i = 0;
                if (int.TryParse(moveText.Text, out i))
                    GameView.PlaceUpTo(i);
            };

            //MatchInfo
            var tvl = FindViewById<TextView>(Resource.Id.textBlack);
            tvl.Text = GameView.Moves.Info.PlayerString(Stone.Black);
            tvl.SetTextColor(Color.Black);
            tvl.Invalidate();

            var tvr = FindViewById<TextView>(Resource.Id.textWhite);
            tvr.Text = GameView.Moves.Info.PlayerString(Stone.White);
            tvr.SetTextColor(Color.Black);
            tvr.Invalidate();

            //Deserialize game object.
            var gameObject = JsonConvert.DeserializeObject<OGSGame>(Intent.GetStringExtra("game"));

            if (gameObject == null) throw new Exception("Gameobject not properly retrieved from ListViewActivity.");

            //Avatar
            var bimg = FindViewById<ImageView>(Resource.Id.blackImage);
            var wimg = FindViewById<ImageView>(Resource.Id.whiteImage);



            var bIcon = gameObject.Black.Icon;
            if (bIcon != null && !bIcon.Contains("default-user"))
                bimg.SetUrlDrawable(bIcon, Resource.Drawable.defaultuser);

            var wIcon = gameObject.White.Icon;
            if (wIcon != null && !wIcon.Contains("default-user"))
                wimg.SetUrlDrawable(gameObject.White.Icon, Resource.Drawable.defaultuser);

            //Player names/
            var blackText = FindViewById<TextView>(Resource.Id.textBlack);
            var whiteText = FindViewById<TextView>(Resource.Id.textWhite);

            blackText.Text = gameObject.Black.Username + " " + gameObject.Black.Rank;
            whiteText.Text = gameObject.White.Username + " " + gameObject.White.Rank;

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


            //Chat drawer
            var chatDrawerScroll = FindViewById<ScrollView>(Resource.Id.rightDrawerScroll);
            var chatDrawerView = FindViewById<SlidingDrawer>(Resource.Id.rightDrawer);
            var chatDrawerText = FindViewById<TextView>(Resource.Id.chatText);
            chatDrawer = new ChatDrawer(chatDrawerView, chatDrawerText);
            GameView.boardTouch.OnTouchEvent += e => { chatDrawer.GestureDetector.OnTouchEvent(e); };


            //Apply chat text 
            //chatDrawer.ChatText = ChatDrawer.StringListToString(PlayerGameListActivity.CurrentSGF.Info.ChatMessages);

            //Stop scrollview from consuming gesture events.
            chatDrawerScroll.SetOnTouchListener(chatDrawer);

            ALog.Info("BoardActivity", "Created, Connecting...");

            GameView.Connect(gameObject.ID);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            chatDrawer.InvokeMotionEvent(e);
            base.OnTouchEvent(e);
            return false;
        }

        protected override void OnStop()
        {
            //Disconnect from game.
            RealTimeAPI.I.Disconnect();
            base.OnStop();
        }
    }
}