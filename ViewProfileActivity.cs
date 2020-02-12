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
using Com.Bumptech.Glide.Request;
using Firebase.Auth;
using Firebase.Xamarin.Database;

namespace Firebase
{
    [Activity(Label = "Profile")]
    public class ViewProfileActivity : Activity
    {
        FirebaseAuth auth;
        FirebaseClient firebase;
        string fromEmail; // Message Sender | Viewer Email
        string toEmail; // Recipient | the email of person being viewed
        private ProgressBar circular_progress;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            auth = FirebaseAuth.GetInstance(MainActivity.app);
            fromEmail = auth.CurrentUser.Email; // Loading email of person opening this profile
            
            SetContentView(Resource.Layout.ViewProfile);

            // Adding components
            ImageView profilePicture = FindViewById<ImageView>(Resource.Id.profilePicture);
            Button sendMessageButton = FindViewById<Button>(Resource.Id.sendMessage);
            TextView name = FindViewById<TextView>(Resource.Id.name);
            TextView location = FindViewById<TextView>(Resource.Id.location);
            TextView bio = FindViewById<TextView>(Resource.Id.bio);
            TextView email = FindViewById<TextView>(Resource.Id.type);
            TextView gamerTagsListText = FindViewById<TextView>(Resource.Id.gamerTagsListText);
            Button gamerTagsList = FindViewById<Button>(Resource.Id.gamerTagsList);
            // A button for Organaized tourneys here
            // A button for participated tourneys here
            GridLayout parent = FindViewById<GridLayout>(Resource.Id.gridLayoutAdminProfile);
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgressProfile);


            if (HomeActivity.fromHome)
            {
                toEmail = fromEmail;
                sendMessageButton.Enabled = false;
            }
            else
            {
                // bring toEmail in the intent extra here when someone else is visiting
                // Then toEmail and fromEmail will be different, for messageSend activity
                toEmail = Intent.GetStringExtra("toEmail") ?? "None";
            }


            circular_progress.Visibility = ViewStates.Visible;
            firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var items = await firebase
                .Child("users")
                .OnceAsync<UserViewModel>();

            

            foreach (var item in items)
            {
                if (item.Object.Email.Equals(toEmail))
                {
                    name.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                    bio.Text = item.Object.Bio ?? "";
                    location.Text = item.Object.City + ", " + item.Object.Country ?? "";
                    email.Text = toEmail ?? "";

                    if (!item.Object.PhotoURL.Equals(string.Empty))
                        Glide.With(this).Load(item.Object.PhotoURL).Into(profilePicture);


                    break;
                }
                
            }
            circular_progress.Visibility = ViewStates.Invisible;

            // sendMessage Click method here
            sendMessageButton.Click += (sender, e) =>
            {
                HomeActivity.fromHome = false;
                Intent sendMessageActivity = new Intent(Application.Context, typeof(MessageSendActivity));
                sendMessageActivity.PutExtra("toEmail", toEmail);
                StartActivity(sendMessageActivity);
            };

            // Gamer Tags button
            gamerTagsList.Click += async (sender, e) =>
            {
                circular_progress.Visibility = ViewStates.Visible;
                var tags = await firebase
                .Child("gamerTags")
                .OnceAsync<GameTagViewModel>();

                string temp = "";
                int count = 1;

                foreach (var tag in tags)
                {
                    if (tag.Object.Email.Equals(toEmail))
                    {
                        temp += "#" + count + ": " + tag.Object.GameTitle + " (" + tag.Object.Platform + "): " + tag.Object.GamerTag + "\n";
                        count++;
                    }
                }
                gamerTagsListText.Text = temp.Substring(0, temp.Length - 1);
                circular_progress.Visibility = ViewStates.Invisible;
            };
        }
    }
}