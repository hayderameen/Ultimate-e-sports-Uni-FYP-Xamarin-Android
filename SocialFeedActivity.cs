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
    [Activity(Label = "SocialFeedActivity")]
    public class SocialFeedActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SocialFeed);

            // Adding Components
            Button viewSocialPosts = FindViewById<Button>(Resource.Id.viewSocialFeedButton);
            Button publishPost = FindViewById<Button>(Resource.Id.publishPostButton);

            publishPost.Click += (sender, e) =>
            {
                Intent pubishPostActivity = new Intent(Application.Context, typeof(PublishPostActivity));
                StartActivity(pubishPostActivity);
            };

            viewSocialPosts.Click += (sender, e) =>
            {
                Intent viewSocialPostFeed = new Intent(Application.Context, typeof(ViewSocialFeedActivity));
                StartActivity(viewSocialPostFeed);
            };
        }
    }
}