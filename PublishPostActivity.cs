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
using Firebase.Auth;
using Firebase.Xamarin.Database;

namespace Firebase
{
    [Activity(Label = "PublishPostActivity")]
    public class PublishPostActivity : Activity
    {
        FirebaseAuth auth;
        private ProgressBar circular_progress;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            SetContentView(Resource.Layout.PublishPost);

            // Adding components
            EditText socialPostBody = FindViewById<EditText>(Resource.Id.socialPostBodyText);
            Button publishButton = FindViewById<Button>(Resource.Id.publishPostButton);
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgressPublishPost);

            publishButton.Click += async (sender, e) =>
            {
                if (socialPostBody.Text != string.Empty)
                {
                    circular_progress.Visibility = ViewStates.Visible;
                    var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
                    var item = await firebase.Child("socialPosts")
                    .PostAsync<SocialPostViewModel>(new SocialPostViewModel
                    {
                        Email = auth.CurrentUser.Email,
                        Body = socialPostBody.Text.Trim(),
                        SocialPostID = Guid.NewGuid().ToString(),
                        TimeStamp = DateTime.Now.ToString()
                    });

                    socialPostBody.Text = "";
                    circular_progress.Visibility = ViewStates.Invisible;
                    Toast.MakeText(ApplicationContext, "Published", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Write something first", ToastLength.Long).Show();
                }

            };
        }
    }
}