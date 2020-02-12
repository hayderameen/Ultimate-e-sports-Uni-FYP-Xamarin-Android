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

namespace Firebase
{
    [Activity(Label = "ViewTournamentsActivity")]
    public class ViewTournamentsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ViewTournaments);

            // Adding Components
            Button viewOfflineTournaments = FindViewById<Button>(Resource.Id.offlineTournamentsButton);
            Button viewOnlineTournaments = FindViewById<Button>(Resource.Id.onlineTournamentsButton);

            viewOnlineTournaments.Click += (sender, e) =>
            {
                Intent viewOnlineTournamentsActivity = new Intent(Application.Context, typeof(ViewOnlineTournamentsActivity));
                StartActivity(viewOnlineTournamentsActivity);
            };

            viewOfflineTournaments.Click += (sender, e) =>
            {
                Intent viewOfflineTournamentsActivity = new Intent(Application.Context, typeof(ViewOfflineTournamentsActivity));
                StartActivity(viewOfflineTournamentsActivity);
            };
        }
    }
}