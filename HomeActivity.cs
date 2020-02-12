using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Firebase.Auth;
using Xamarin.Essentials;

namespace Firebase
{
    [Activity(Label = "Home", Theme = "@style/AppTheme")]
    public class HomeActivity : AppCompatActivity
    {
        FirebaseAuth auth;
        public static bool fromHome = false; // This is used in viewing profile activity

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Home);

            // Initating Preferences for Logout
            Platform.Init(this, bundle);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            Button addTournament = FindViewById<Button>(Resource.Id.addTournament);
            Button logout = FindViewById<Button>(Resource.Id.logout);
            Button editProfile = FindViewById<Button>(Resource.Id.editProfile);
            Button sendMessage = FindViewById<Button>(Resource.Id.sendMessage);
            Button viewInbox = FindViewById<Button>(Resource.Id.viewInbox);
            Button viewProfile = FindViewById<Button>(Resource.Id.viewProfile);
            Button viewTournaments = FindViewById<Button>(Resource.Id.viewTournaments);
            Button viewSocialFeed = FindViewById<Button>(Resource.Id.viewSocialFeed);
            TextView homeMessage = FindViewById<TextView>(Resource.Id.welcomeMessageHome);

            // Modifying HomeMessage text
            homeMessage.Text += "\n" + auth.CurrentUser.Email;

            //if (!MainActivity.decision.Equals("Login as Admin")) // A player cannot add a tournament
               // addTournament.Text = "Add Gamer Tag";

            // Checking email verification
            if (auth != null)
            {
                if (!auth.CurrentUser.IsEmailVerified)
                {
                    auth.CurrentUser.SendEmailVerification();
                    homeMessage.Text = "\nEmail not verified yet! Check your inbox";
                    addTournament.Enabled = false;
                    editProfile.Enabled = false;
                    sendMessage.Enabled = false;
                    viewInbox.Enabled = false;
                    viewProfile.Enabled = false;
                    viewTournaments.Enabled = false;
                    viewSocialFeed.Enabled = false;

                }
            }

            viewSocialFeed.Click += (sender, e) =>
            {
                Intent viewSocialActivity = new Intent(Application.Context, typeof(SocialFeedActivity));
                StartActivity(viewSocialActivity);
            };

            viewTournaments.Click += (sender, e) =>
            {
                Intent viewTournamentsActivity = new Intent(Application.Context, typeof(ViewTournamentsActivity));
                StartActivity(viewTournamentsActivity);
            };

            addTournament.Click += (sender, e) =>
            {
                if (!MainActivity.decision.Equals("Login as Admin")) // Player is adding gamer tags
                {
                    Intent addGamerTag = new Intent(Application.Context, typeof(AddGamerTagActivity));
                    StartActivity(addGamerTag);
                }
                else // Admin is adding tournaments
                {
                    fromHome = true;
                    Intent addTournamentActivity = new Intent(Application.Context, typeof(AddTournamentActivity));
                    StartActivity(addTournamentActivity);
                }
            };

            viewInbox.Click += (sender, e) =>
            {
                fromHome = true;
                Intent viewInboxActivity = new Intent(Application.Context, typeof(ViewInboxActivity));
                StartActivity(viewInboxActivity);
            };

            sendMessage.Click += (sender, e) =>
            {
                fromHome = true;
                Intent sendMessageActivity = new Intent(Application.Context, typeof(MessageSendActivity));
                StartActivity(sendMessageActivity);
            };

            logout.Click += (sender, e) =>
            {
                auth.SignOut();
                if (auth.CurrentUser == null)
                {
                    Preferences.Remove("email");
                    Preferences.Remove("password");
                    Preferences.Remove("decision");
                    StartActivity(new Intent(this, typeof(MainActivity)));
                    Finish();
                }
                
            };

            viewProfile.Click += (sender, e) =>
            {
                fromHome = true;
                Intent viewProfileActivity = new Intent(Application.Context, typeof(ViewProfileActivity));
                StartActivity(viewProfileActivity);
                 
            };

            editProfile.Click += (sender, e) =>
            {
                 Intent editProfileActivity = new Intent(Application.Context, typeof(ProfileEditActivity));
                 StartActivity(editProfileActivity);
            };

        }
    }
}