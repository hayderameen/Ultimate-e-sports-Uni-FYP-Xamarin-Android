using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace Firebase
{
    [Activity(Label = "Ultimate eSports", MainLauncher = true)]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //ActionBar.Hide();

            SetContentView(Resource.Layout.SplashScreen);
        }

        // Wait for loading
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { delay(); });
            startupWork.Start();
        }

        async void delay()
        {
            await Task.Delay(2000);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            Finish();
        }
    }
}