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
using Firebase.Xamarin.Database;

namespace Firebase
{
    [Activity(Label = "ViewOfflineTournamentsActivity")]
    public class ViewOfflineTournamentsActivity : Activity
    {
        private ProgressBar circular_progress;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ViewOfflineTournaments);

            // Adding views
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgressOfflineTournaments);
            Button gameTitle = FindViewById<Button>(Resource.Id.gameTitleOffline);
            ImageView profilePicture = FindViewById<ImageView>(Resource.Id.profilePictureOfflineTournament);
            Button adminName = FindViewById<Button>(Resource.Id.AdminNameOfflineTournament);
            Button tourneyTitle = FindViewById<Button>(Resource.Id.TitleOfflineTournament);
            Button registrationStatus = FindViewById<Button>(Resource.Id.LiveStatusOfflineTournament);
            Button finishedStatus = FindViewById<Button>(Resource.Id.FinishedStatusOfflineTournament);
            Button description = FindViewById<Button>(Resource.Id.DescriptionOfflineTournament);
            Button previous = FindViewById<Button>(Resource.Id.previousTourneyOffline);
            Button next = FindViewById<Button>(Resource.Id.nextTourneyOffline);
            Button eventPageButton = FindViewById<Button>(Resource.Id.ViewEventPageButtonOfflineTournament);

            circular_progress.Visibility = ViewStates.Visible;
            eventPageButton.Enabled = false;
            // Loading tournaments
            var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var items = await firebase
                .Child("tournaments")
                .OnceAsync<TournamentViewModel>();

            List<TournamentViewModel> query = new List<TournamentViewModel>();


            // Filtering
            foreach (var item in items)
            {
                if (item.Object.online.Equals("false"))
                {
                    query.Add(item.Object);
                }
            }

            query.Reverse();

            // Loading users' data to get profile pictures
            var users = await firebase.Child("users").OnceAsync<UserViewModel>();

            // Loading first item
            int tourneyCounter = 0; // To keep track of opened tournament

            gameTitle.Text = query[0].Game;

            foreach (var item in users)
            {
                if (item.Object.Email.Equals(query[tourneyCounter].AdminID))
                {
                    if (!item.Object.PhotoURL.Equals(string.Empty))
                    {
                        Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                    }
                    adminName.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                }
            }

            tourneyTitle.Text = query[0].Title;

            if (query[0].Live.Equals("false"))
                registrationStatus.Text = "Registrations Open!";
            else
                registrationStatus.Text = "Registrations Closed!";

            if (query[0].Finished.Equals("true"))
                finishedStatus.Text = "Tournament is Over..";
            else
                finishedStatus.Text = "Tournament is still going on!";

            description.Text = "Description:\n\n" + query[0].Description;

            eventPageButton.Enabled = true;
            circular_progress.Visibility = ViewStates.Invisible;

            next.Click += (sender, e) =>
            {
                if (query.Count() - 1 == tourneyCounter)
                {
                    // No new Tournaments
                    Toast.MakeText(ApplicationContext, "No more tournaments", ToastLength.Short).Show();
                }
                else
                {
                    tourneyCounter++;

                    gameTitle.Text = query[tourneyCounter].Game;

                    foreach (var item in users)
                    {
                        if (item.Object.Email.Equals(query[tourneyCounter].AdminID))
                        {
                            if (!item.Object.PhotoURL.Equals(string.Empty))
                            {
                                Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                            }
                            adminName.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                        }
                    }

                    tourneyTitle.Text = query[tourneyCounter].Title;

                    if (query[tourneyCounter].Live.Equals("false"))
                        registrationStatus.Text = "Registrations Open!";
                    else
                        registrationStatus.Text = "Registrations Closed!";

                    if (query[tourneyCounter].Finished.Equals("true"))
                        finishedStatus.Text = "Tournament is Over..";
                    else
                        finishedStatus.Text = "Tournament is still going on!";

                    description.Text = "Description:\n\n" + query[tourneyCounter].Description;
                }
            };

            previous.Click += (sender, e) =>
            {
                if (0 == tourneyCounter)
                {
                    // No new Tournaments
                    Toast.MakeText(ApplicationContext, "No more tournaments", ToastLength.Short).Show();
                }
                else
                {
                    tourneyCounter--;

                    gameTitle.Text = query[0].Game;

                    foreach (var item in users)
                    {
                        if (item.Object.Email.Equals(query[tourneyCounter].AdminID))
                        {
                            if (!item.Object.PhotoURL.Equals(string.Empty))
                            {
                                Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                            }
                            adminName.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                        }
                    }

                    tourneyTitle.Text = query[tourneyCounter].Title;

                    if (query[tourneyCounter].Live.Equals("false"))
                        registrationStatus.Text = "Registrations Open!";
                    else
                        registrationStatus.Text = "Registrations Closed!";

                    if (query[tourneyCounter].Finished.Equals("true"))
                        finishedStatus.Text = "Tournament is Over..";
                    else
                        finishedStatus.Text = "Tournament is still going on!";

                    description.Text = "Description:\n\n" + query[tourneyCounter].Description;
                }
            };

            eventPageButton.Click += (sender, e) =>
            {
                Intent viewEventPageActivity = new Intent(ApplicationContext, typeof(ViewEventPageActivity));
                viewEventPageActivity.PutExtra("tourneyID", query[tourneyCounter].TournamentID);
                StartActivity(viewEventPageActivity);
            };

            /*
            string[] events = new string[query.Count()];

            for (int i = 0; i < query.Count(); i++)
            {
                events[i] = query.ToList().ElementAt(i).Title + "\n  Tournament ID -  " + query.ToList().ElementAt(i).TournamentID;
            }
            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.ViewOfflineTournaments, events);

            ListView.ItemClick += (sender, e) =>
            {
                string tourneyID = string.Empty;
                tourneyID = ((TextView)e.View).Text;
                string[] temps = tourneyID.Split(' ');
                tourneyID = temps[temps.Length - 1];
                 Intent viewEventPageActivity = new Intent(ApplicationContext, typeof(ViewEventPageActivity));
                 viewEventPageActivity.PutExtra("tourneyID", tourneyID);
                 StartActivity(viewEventPageActivity);
            };
            */
        }
    }
}