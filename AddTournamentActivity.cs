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
using Firebase.Database;
using Firebase.Xamarin.Database;
using System.Json;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Firebase
{
    [Activity(Label = "AddTournamentActivity")]
    public class AddTournamentActivity : Activity
    {
        FirebaseAuth auth;
        string tourneyID;
        DatabaseReference mDatabase;
        string[] players;
        string tourneyTitle;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            mDatabase = FirebaseDatabase.Instance.Reference;

            tourneyID = Intent.GetStringExtra("tourneyID") ?? "None"; // It will be initiated if existing tournament is being edited
            
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddTournament);

            // Adding Components
            Button addTournament = FindViewById<Button>(Resource.Id.addTournamentInfo);
            Button addLocation = FindViewById<Button>(Resource.Id.addLocation);
            CheckBox LANEvent = FindViewById<CheckBox>(Resource.Id.LANEvent);
            Button saveInfoButton = FindViewById<Button>(Resource.Id.saveInfo);
            Button cancelInfoButton = FindViewById<Button>(Resource.Id.cancelInfo);
            Button liveButton = FindViewById<Button>(Resource.Id.tournamentLiveButton);
            Button noOfParticipants = FindViewById<Button>(Resource.Id.participantsNumberTourneyPage);

            cancelInfoButton.Text = "Delete";
            noOfParticipants.Enabled = false;

            if (tourneyID.Equals("None")) // New tournament creation
            {
                LANEvent.Checked = false;
                addLocation.Enabled = false;
               // tourneyID = Guid.NewGuid().ToString();

                var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
                var item = await firebase.Child("tournaments")
                .PostAsync<TournamentViewModel>(new TournamentViewModel
                {
                    AdminID = auth.CurrentUser.Email,
                    TournamentID = "",
                    online = "true",
                    Format = "Knockout",
                    Live = "false", // shows that is is not published yet
                    Description = "",
                    Game = "Game not set yet",
                    Finished = "false"
                });

                await mDatabase.Child("tournaments").Child(item.Key).Child("TournamentID").SetValueAsync(item.Key);
                tourneyID = item.Key;

            }
            else
            {
                FirebaseClient firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/tournaments/");
                var items = await firebase
                    .Child("")
                    .OnceAsync<TournamentViewModel>();

                foreach (var item in items)
                {
                    if (item.Key.Equals(tourneyID))
                    {
                        if (item.Object.online.Equals("true"))
                        {
                            addLocation.Enabled = false;
                            LANEvent.Checked = false;
                        }
                        if (item.Object.Live.Equals("true"))
                        {
                            addLocation.Enabled = false;
                            LANEvent.Enabled = false;
                            addTournament.Enabled = false;
                        }

                        int number;
                        bool result = int.TryParse(item.Object.ParticipantsLimit, out number);

                        if (result)
                        {
                            if (item.Object.Participants != null)
                            {
                                players = item.Object.Participants.Split(',');
                                noOfParticipants.Text = "Registered: " + (players.Count()) + "/" + number;
                                tourneyTitle = item.Object.Title;

                                if (players.Count() != number)
                                    liveButton.Enabled = false;
                            }
                            else
                            {
                                noOfParticipants.Text = "Registered: " + "0/" + number;
                                liveButton.Enabled = false;
                            }
                            
                        }
                        else
                        {
                            liveButton.Enabled = false;
                        }

                        break;
                    }
                }
            }
            

            liveButton.Click += async (sender, e) =>
            {
                await mDatabase.Child("tournaments").Child(tourneyID).Child("Live").SetValueAsync("true");
                Toast.MakeText(ApplicationContext, "Tournament is now live! You cannot change any info now.", ToastLength.Long).Show();
                addLocation.Enabled = false;
                LANEvent.Enabled = false;
                addTournament.Enabled = false;

                // 1: GENERATE BRACKET WHEN TOURNAMENT GOES LIVE HERE!!
                var url = "https://api.challonge.com/v1/tournaments.json?api_key=nzFuvS0FNlSVr7KWKdTpoCqP4EXhPAlyMccTfIyy&tournament[name]="+tourneyTitle.Trim()+"&tournament[url]="+tourneyTitle.Trim()+"_"+ tourneyTitle.Trim()+"UltimateESports"+ "&tournament[open_signup]=false&tournament[description]="+"To be added here. This space is for rules"+"&tournament[show_rounds]=true&tournament[signup_cap]="+players.Count();

                // Create an HTTP web request using the URL:
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ContentLength = 0;
                request.Timeout = 300000;

                // For use in next function
                string urlFull;

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        //JsonValue a = jsonDoc[0];

                        JsonValue tourney = jsonDoc["tournament"];

                        string BracketID = tourney["full_challonge_url"];
                        urlFull = tourney["url"];

                        await mDatabase.Child("tournaments").Child(tourneyID).Child("BracketID").SetValueAsync(urlFull);
                        
                        await mDatabase.Child("tournaments").Child(tourneyID).Child("BracketURL").SetValueAsync(BracketID);


                        //Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
                    }
                }

                // Add Players to the bracket
                foreach (var name in players)
                {
                    string addParticipant = "https://api.challonge.com/v1/tournaments/" + urlFull + "/participants.json?participant[name]=" + name + "&api_key=nzFuvS0FNlSVr7KWKdTpoCqP4EXhPAlyMccTfIyy";
                    request = (HttpWebRequest)HttpWebRequest.Create(new Uri(addParticipant));
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.ContentLength = 0;

                    using (WebResponse response = await request.GetResponseAsync())
                    {
                        // Get a stream representation of the HTTP web response:
                        using (Stream stream = response.GetResponseStream())
                        {
                            // Use this stream to build a JSON document object:
                            JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                            Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
                        }
                    }
                }

                // Starting the tournament
                url = "https://api.challonge.com/v1/tournaments/"+urlFull+ "/start.json?api_key=nzFuvS0FNlSVr7KWKdTpoCqP4EXhPAlyMccTfIyy";

                request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "POST";
                request.ContentLength = 0;

                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
                    }
                }

                // 2: SEND MESSAGE TO ALL PARTICIPANTS FOR TOURNAMENT START !!
                var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
                foreach (var name in players)
                {
                    string message = tourneyTitle + "has just started.";

                    await firebase.Child("messages")
                        .PostAsync<MessageViewModel>(new MessageViewModel
                        {
                            ToEmail = name,
                            FromEmail = auth.CurrentUser.Email,
                            Body = message,
                            Timestamp = DateTime.Now.ToString()
                        });
                }
            };

            cancelInfoButton.Click += async (sender, e) =>
            {
                // Heading to Home Page
                var homeActivity = new Intent(Application.Context, typeof(HomeActivity));
                await mDatabase.Child("tournaments").Child(tourneyID).RemoveValueAsync();
                StartActivity(homeActivity);
                Finish();

            };

            saveInfoButton.Click += (sender, e) =>
            {
                // Heading to Home Page
                var homeActivity = new Intent(Application.Context, typeof(HomeActivity));
                StartActivity(homeActivity);
                Finish();
            };

            // Button actions
            addTournament.Click += (sender, e) =>
            {
                Intent addTournamentInfoActivity = new Intent(Application.Context, typeof(AddTournamentInfoActivity));
                addTournamentInfoActivity.PutExtra("tourneyID", tourneyID);
                StartActivity(addTournamentInfoActivity);
            };

            addLocation.Click += (sender, e) =>
            {
                // setting tournament as LAN
               
                Intent addLocationActivity = new Intent(Application.Context, typeof(AddLocationActivity));
                addLocationActivity.PutExtra("tourneyID", tourneyID);
                StartActivity(addLocationActivity);
            };

            // Checkbox action
            LANEvent.Click += async (sender, e) =>
            {
                if (LANEvent.Checked)
                {
                    addLocation.Enabled = true;
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("online").SetValueAsync("false");

                }
                else
                {
                    addLocation.Enabled = false;
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("online").SetValueAsync("true");

                }
            };
        }
    }
}