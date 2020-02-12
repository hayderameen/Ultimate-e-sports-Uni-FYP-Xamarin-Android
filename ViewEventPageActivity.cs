using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Models;
using Firebase.Views;
//using Firebase.Views;
using Firebase.Xamarin.Database;
using Newtonsoft.Json;

namespace Firebase
{
    [Activity(Label = "ViewEventPageActivity")]
    public class ViewEventPageActivity : Activity
    {
        FirebaseAuth auth;
        TournamentViewModel query;
        DatabaseReference mDatabase;
        int id; // Match id for updating results
        int matchWinnerID; // match winner's ID
        string matchScore;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            string tourneyID = Intent.GetStringExtra("tourneyID") ?? "None";
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            mDatabase = FirebaseDatabase.Instance.Reference;

            base.OnCreate(savedInstanceState);

            // Loading tournaments
            var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var items = await firebase
                .Child("tournaments")
                .OnceAsync<TournamentViewModel>();

            foreach (var item in items)
            {
                if (item.Object.TournamentID.Equals(tourneyID))
                {
                    query = item.Object;
                    break;
                }
            }

            

            SetContentView(Resource.Layout.ViewEventPage);

            // Adding Components
            TextView title = FindViewById<TextView>(Resource.Id.tournamentTitle);
            TextView startDate = FindViewById<TextView>(Resource.Id.startDateTitle);
            TextView finishDate = FindViewById<TextView>(Resource.Id.finishDateTitle);
            TextView awardMoney = FindViewById<TextView>(Resource.Id.awardMoneyTitle);
            TextView format = FindViewById<TextView>(Resource.Id.formatTitle);

            Button location = FindViewById<Button>(Resource.Id.location);
            Button adminProfile = FindViewById<Button>(Resource.Id.adminProfileView);
            Button register = FindViewById<Button>(Resource.Id.register);
            Button edit = FindViewById<Button>(Resource.Id.edit);
            Button bracketURL = FindViewById<Button>(Resource.Id.bracketLink);
            Button updateResult = FindViewById<Button>(Resource.Id.updateResult);
            Button searchOpponent = FindViewById<Button>(Resource.Id.searchOpponentButton);
            Button endTournament = FindViewById<Button>(Resource.Id.finishTournament);

            AutoCompleteTextView matchNumber = FindViewById<AutoCompleteTextView>(Resource.Id.MatchNumberText);
            AutoCompleteTextView matchWinner = FindViewById<AutoCompleteTextView>(Resource.Id.matchWinnerText);
            AutoCompleteTextView searchOpponentText = FindViewById<AutoCompleteTextView>(Resource.Id.searchOpponent);

            ImageView showCase = FindViewById<ImageView>(Resource.Id.tourneyImageShowCaseEventPage);
            string showCaseURL = "https://firebasestorage.googleapis.com/v0/b/fir-test-1bdb3.appspot.com/o/ShowCase.png?alt=media&token=f0c6e2e7-e9fc-46e8-a2ad-528ebf778aad";
            Glide.With(this).Load(showCaseURL).Apply(RequestOptions.CircleCropTransform()).Into(showCase);


            if (MainActivity.decision.Equals("Login as Admin"))
            {
                register.Enabled = false;
            }

            if (query.Finished.Equals("true"))
            {
                matchNumber.Enabled = false;
                matchWinner.Enabled = false;
                updateResult.Enabled = false;
            }

            if (!query.AdminID.Equals(auth.CurrentUser.Email))
            {
                edit.Enabled = false;
                matchNumber.Enabled = false;
                matchWinner.Enabled = false;
                updateResult.Enabled = false;
                endTournament.Enabled = false;
            }
            
            if (query.online.Equals("true"))
            {
                location.Enabled = false;
            }

            if (query.Live.Equals("false"))
            {
                bracketURL.Enabled = false;
                matchNumber.Enabled = false;
                matchWinner.Enabled = false;
                updateResult.Enabled = false;
                
            }
            else
            {
                register.Enabled = false;
            }

            if (query.Paid.Equals("false"))
            {
                register.Text = "Register for Free";
            }
            else
            {
                register.Text = "Register for " + query.EntryFee + "¢";
            }

            


            title.Text = query.Title ?? "";
            startDate.Text = query.StartDate ?? "";
            finishDate.Text = query.FinishDate ?? "";
            awardMoney.Text = query.AwardMoney + "" ?? "";
            format.Text = query.Format ?? "";

            endTournament.Click += async (sender, e) =>
            {
                if (query.Finished.Equals("true"))
                {
                    Toast.MakeText(ApplicationContext, "Tournament is already Over!", ToastLength.Short).Show();
                }
                else
                {
                    if (query.Live.Equals("true"))
                    {
                        await mDatabase.Child("tournaments").Child(tourneyID).Child("Finished").SetValueAsync("true");
                        Toast.MakeText(ApplicationContext, "Tournament Ended", ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "You cannot end a tournament before it goes live!", ToastLength.Short).Show();
                    }
                }
                
            };

            searchOpponent.Click += async (sender, e) =>
            {
                // Loading users' data for searching
                var users = await firebase.Child("users").OnceAsync<UserViewModel>();

                bool found = false;
                string userID = "";

                foreach (var user in users)
                {
                    if (user.Object.Email.Equals(searchOpponentText.Text.Trim().ToLower()))
                    {
                        found = true;
                        userID = user.Object.Email;
                        break;
                    }
                }

                if (found)
                {
                    Intent viewProfileActivity = new Intent(Application.Context, typeof(ViewProfileActivity));
                    viewProfileActivity.PutExtra("toEmail", userID); // Recipient email
                    HomeActivity.fromHome = false;
                    StartActivity(viewProfileActivity);
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Invalid user", ToastLength.Short).Show();
                }
            };

            updateResult.Click += async (sender, e) =>
            {
                
                int spo;
                bool validInputs = false;

                // Getting user input values

                int matchNo;
                bool result = int.TryParse(matchNumber.Text, out matchNo);

                int playerNo;
                bool result2 = int.TryParse(matchWinner.Text, out playerNo);

                if (result && result2)
                {
                    string url = "https://api.challonge.com/v1/tournaments/" + query.BracketID + "/matches.json?api_key=nzFuvS0FNlSVr7KWKdTpoCqP4EXhPAlyMccTfIyy";
                    // Create an HTTP web request using the URL:
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                    request.ContentType = "application/json";
                    request.Method = "GET";
                    request.Timeout = 20200000;

                    // Send the request to the server and wait for the response:
                    using (WebResponse response = await request.GetResponseAsync())
                    {
                        // Get a stream representation of the HTTP web response:
                        using (Stream stream = response.GetResponseStream())
                        {
                            // Use this stream to build a JSON document object:
                            JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                            foreach (JsonValue abc in jsonDoc)
                            {

                                JsonValue match = abc["match"];

                                spo = match["suggested_play_order"];

                                if (spo == matchNo)
                                {
                                    id = match["id"];
                                    validInputs = true;

                                    if (playerNo == 1)
                                    {
                                        matchWinnerID = match["player1_id"];
                                        matchScore = "1-0";
                                    }
                                    else if (playerNo == 2)
                                    {
                                        matchWinnerID = match["player2_id"];
                                        matchScore = "0-1";
                                    }
                                    else
                                        validInputs = false;
                                    
                                    break;
                                }
                            }
                        }
                    }

                    // Now updating the result of that particular match
                    if (validInputs)
                    {
                        url = "https://api.challonge.com/v1/tournaments/"+query.BracketID+"/matches/"+id+".json?api_key=nzFuvS0FNlSVr7KWKdTpoCqP4EXhPAlyMccTfIyy&match[winner_id]="+matchWinnerID+ "&match[scores_csv]="+matchScore;
                        // Create an HTTP web request using the URL:
                        request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                        request.ContentType = "application/json";
                        request.Method = "PUT";
                        request.ContentLength = 0;
                        request.Timeout = 20200000;

                        // Send the request to the server and wait for the response:
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
                    else
                    {
                        Toast.MakeText(ApplicationContext, "Either Match number or Player number is not valid. Try again.", ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Match Number and Player number must be whole numbers", ToastLength.Long).Show();
                }

                

                
            };

            bracketURL.Click += (sender, e) =>
            {
                Intent viewBracket = new Intent(Application.Context, typeof(ViewTournamentBracketActivity));
                viewBracket.PutExtra("url", query.BracketURL);
                StartActivity(viewBracket);
            };

            edit.Click += (sender, e) =>
            {
                Intent editTournament = new Intent(Application.Context, typeof(AddTournamentActivity));
                editTournament.PutExtra("tourneyID", tourneyID);
                StartActivity(editTournament);
            };
            
            location.Click += (sender, e) =>
            {
                Intent viewTournamentLocation = new Intent(Application.Context, typeof(ViewTournamentLocationActivity));
                viewTournamentLocation.PutExtra("coords", query.Location);
                StartActivity(viewTournamentLocation);
            };
            
            adminProfile.Click += (sender, e) =>
            {
                Intent viewProfileActivity = new Intent(Application.Context, typeof(ViewProfileActivity));
                viewProfileActivity.PutExtra("toEmail", query.AdminID); // Recipient email
                HomeActivity.fromHome = false;
                StartActivity(viewProfileActivity);
            };
            
            register.Click += async (sender, e) =>
            {
                string temp = query.Participants;
                string entryFee = query.EntryFee;

                // Temp change here from null to ""
                if (temp == null)
                {
                    if (query.Paid.Equals("false"))
                    {
                        temp = "";

                        temp += auth.CurrentUser.Email;

                        await mDatabase.Child("tournaments").Child(tourneyID).Child("Participants").SetValueAsync(temp);

                        Toast.MakeText(ApplicationContext, "Registered", ToastLength.Long).Show();
                    }
                    else
                    {
                        // Render the view from the type generated from RazorView.cshtml
                        var model = new PaymentSendModel() { TournamentID = tourneyID, EntryFee = entryFee, NewParticipant = "true", Participants = "none", PlayerEmail = auth.CurrentUser.Email };
                        //string query = "?tourneyID=" + tourneyID + "&entryFee=" + entryFee + "&NewParticipant=true&PlayerEmail=" + auth.CurrentUser.Email;
                        var template = new PaymentSendRazorView() { Model = model };
                        var page = template.GenerateString();

                        // Load the rendered HTML into the view with a base URL 
                        // that points to the root of the bundled Assets folder
                        // It is done in another activity
                        Intent makePaymentActivity = new Intent(Application.Context, typeof(MakePaymentActivity));
                        makePaymentActivity.PutExtra("page", page);
                        StartActivity(makePaymentActivity);


                    }
                }
                else
                {
                    bool registered = false;

                    string[] temps = temp.Split(',');

                    if (temps.Count() == int.Parse(query.ParticipantsLimit))
                    {
                        Toast.MakeText(ApplicationContext, "Registration is full!", ToastLength.Long).Show();
                    }
                    else
                    {
                        foreach (string item in temps)
                        {
                            if (item.Equals(auth.CurrentUser.Email))
                            {
                                registered = true;
                                Toast.MakeText(ApplicationContext, "Already Registered", ToastLength.Long).Show();
                                break;
                            }
                        }

                        if (!registered)
                        {
                            if (query.Paid.Equals("false"))
                            {
                                temp += "," + auth.CurrentUser.Email;

                                await mDatabase.Child("tournaments").Child(tourneyID).Child("Participants").SetValueAsync(temp);

                                Toast.MakeText(ApplicationContext, "Registered", ToastLength.Long).Show();
                            }
                            else
                            {
                                // Render the view from the type generated from RazorView.cshtml
                                var model = new PaymentSendModel() { TournamentID = tourneyID, EntryFee = entryFee, NewParticipant = "false", Participants = temp, PlayerEmail = auth.CurrentUser.Email };
                                //string query = "?tourneyID=" + tourneyID + "&entryFee=" + entryFee + "&NewParticipant=true&PlayerEmail=" + auth.CurrentUser.Email;
                                var template = new PaymentSendRazorView() { Model = model };
                                var page = template.GenerateString();

                                // Load the rendered HTML into the view with a base URL 
                                // that points to the root of the bundled Assets folder
                                // It is done in another activity
                                Intent makePaymentActivity = new Intent(Application.Context, typeof(MakePaymentActivity));
                                makePaymentActivity.PutExtra("page", page);
                                StartActivity(makePaymentActivity);
                            }
                        }
                    }                    
                }
                    

                /*
                if (query.Participants == null)
                    query.Participants = "";

                query.Participants += test + ",";
                db.Update(query);
                query = (from s in db.Table<TournamentViewModel>()
                         where s.TournamentID == tourneyID
                         select s).FirstOrDefault();
                         
                Intent viewTournamentsActivity = new Intent(Application.Context, typeof(ViewTournamentsActivity));
                viewTournamentsActivity.PutExtra("email", test);
                viewTournamentsActivity.PutExtra("userType", userType);
                StartActivity(viewTournamentsActivity);
                Toast.MakeText(ApplicationContext, "Registered", ToastLength.Long).Show();
                */
            };
            
        }
        
        
        
        /*
        public class HelloWebViewClient : WebViewClient
        {
            // For API level 24 and later
            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                view.LoadUrl(request.Url.ToString());
                return false;
            }
        }
        */
    }

    
}