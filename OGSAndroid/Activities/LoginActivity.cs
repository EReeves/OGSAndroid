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

            //OGSAPI.I.Authenticate("f1925834cd5d6f74ccbf", "c3c0ccac6bd38f1ddb8bdd55e4ebe4f6bf77f53d", "Rvzy", "54126bb282c46d0043dcb178644b4da1"); //Beta

            OGSAPI.I.Authenticate("7283b79ff359ec0e4e97", "2c4f4abfd49846363f26e748c11964d8e86cf126", "Rvzy", "84b11ca7f7c24e6174a3e2f6ecaa7d1f"); //Norm

            //OGSAPI.I.DebugSetAccessToken("7112c1a5a00f200d05a869e435e0bfd3439254c4"); //Beta

            //OGSAPI.I.DebugSetAccessToken(""d4d2f16a82e000e66200e75dde5864e90d14c8f5""); //Norm

            RealTimeAPI.I.Start(true, "Rvzy");

            ALog.Info("LoginActivity", "Created");
        }
    }
}