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
    [Activity(Label = "Add Gamer Tag")]
    public class AddGamerTagActivity : Activity
    {
        FirebaseAuth auth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddGamerTag);

            // Adding Components
            EditText gameTitle = FindViewById<EditText>(Resource.Id.gameTitle);
            EditText gamerTag = FindViewById<EditText>(Resource.Id.gamerTag);
            EditText platform = FindViewById<EditText>(Resource.Id.platform);
            Button save = FindViewById<Button>(Resource.Id.saveGamerTag);

            save.Click += (sender, e) =>
            {
                if (gameTitle.Text != string.Empty && gamerTag.Text != string.Empty && platform.Text != string.Empty)
                {
                    // Later add the option to check if this gamer tag already exists or not
                    InsertIntoDB(new GameTagViewModel
                    {
                        Email = auth.CurrentUser.Email,
                        GameTitle = gameTitle.Text,
                        GamerTag = gamerTag.Text,
                        Platform = platform.Text

                    });

                    gameTitle.Text = string.Empty;
                    gamerTag.Text = string.Empty;
                    platform.Text = string.Empty;
                }
            };
        }

        private async void InsertIntoDB(GameTagViewModel obj)
        {
            var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/gamerTags/");
            //firebase.Child("users");
            var item = await firebase.Child("").PostAsync<GameTagViewModel>(obj);
        }
    }
}