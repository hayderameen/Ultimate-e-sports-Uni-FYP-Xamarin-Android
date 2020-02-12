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
    [Activity(Label = "PostCommentActivity")]
    public class PostCommentActivity : Activity
    {
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            string postID = Intent.GetStringExtra("post") ?? "None";

            SetContentView(Resource.Layout.PostComment);

            // Adding Components
            EditText commentBodyText = FindViewById<EditText>(Resource.Id.commentBody);
            Button postCommentButton = FindViewById<Button>(Resource.Id.postCommentButton);

            postCommentButton.Click += async (sender, e) =>
            {
                if (commentBodyText.Text != String.Empty)
                {
                    var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
                    var item = await firebase.Child("comments")
                    .PostAsync<CommentViewModel>(new CommentViewModel
                    {
                        Email = auth.CurrentUser.Email,
                        Body = commentBodyText.Text.Trim(),
                        SocialPostID = postID
                    });

                    commentBodyText.Text = "";
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Enter a comment first", ToastLength.Long).Show();
                }
            };

        }
    }
}