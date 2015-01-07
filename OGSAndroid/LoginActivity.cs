using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using FlatUI;


namespace OGSAndroid
{
    [Activity(Label = "LoginActivity", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
            if(guestButton != null)
                guestButton.Click += (o, e) => StartActivity(typeof (PlayerGameListActivity));

            //OGSAPI.Authenticate("7283b79ff359ec0e4e97", "2c4f4abfd49846363f26e748c11964d8e86cf126", "Rvzy", "84b11ca7f7c24e6174a3e2f6ecaa7d1f");

            OGSAPI.DebugSetAccessToken("8c201183a4332c3cc39d00614d7a8ff764214e7e");

            RealTimeAPI.I.Start(true, "Rvzy");

            
        }
    }
}