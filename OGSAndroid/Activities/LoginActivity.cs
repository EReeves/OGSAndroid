#region

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using FlatUI;
using OGSAndroid.API;

#endregion

namespace OGSAndroid.Activities
{
    [Activity(Label = "LoginActivity", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle); //Remove title.
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.Login);
            FlatUI.FlatUI.SetActivityTheme(this, FlatTheme.Dark());

            var guestButton = FindViewById<Button>(Resource.Id.loginGuestButton);
            if (guestButton != null)
                guestButton.Click += (o, e) => StartActivity(typeof (PlayerGameListActivity));

            //OGSAPI.I.Authenticate("f1925834cd5d6f74ccbf", "c3c0ccac6bd38f1ddb8bdd55e4ebe4f6bf77f53d", "Rvzy", "54126bb282c46d0043dcb178644b4da1");

            OGSAPI.I.DebugSetAccessToken("7112c1a5a00f200d05a869e435e0bfd3439254c4");

            RealTimeAPI.I.Start(false, "Rvzy");

            ALog.Info("LoginActivity", "Created");
        }
    }
}