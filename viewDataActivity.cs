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
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace Firebase
{
    [Activity(Label = "viewDataActivity")]
    public class viewDataActivity : Activity
    {

        TextView data;
        private ProgressBar circular_progress;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.viewData);

            data = FindViewById<TextView>(Resource.Id.dataText);
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgress);

            loadData();
        }

        private async void loadData()
        {
            circular_progress.Visibility = ViewStates.Visible;

            var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var items = await firebase
                .Child("users")
                .OnceAsync<Account>();

            foreach (var item in items)
            {
                data.Text += item.Key + "\n" + item.Object.name + "\n" + item.Object.email + "\n\n";
            }

            circular_progress.Visibility = ViewStates.Invisible;
        }
    }
}