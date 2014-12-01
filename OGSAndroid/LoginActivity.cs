using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FlatUI;

namespace OGSAndroid
{
    [Activity(Label = "LoginActivity", MainLauncher = true)]
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
            guestButton.Click += (o,e) => StartActivity(typeof (PlayerGameListActivity));
        }
    }
}