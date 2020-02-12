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
using Com.Bumptech.Glide;
using Firebase.Auth;

namespace Firebase
{
    [Activity(Label = "profilePictureActivity")]
    public class profilePictureActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.profilePicture);

            FirebaseAuth auth = FirebaseAuth.GetInstance(MainActivity.app);

            ImageView pic = FindViewById<ImageView>(Resource.Id.profileView);

            var uri = auth.CurrentUser.PhotoUrl;

            var url = "https://firebasestorage.googleapis.com" + uri.EncodedPath + "?" + uri.EncodedQuery; // download url for user image

            Glide.With(this).Load(url).Into(pic);
        }
    }
}