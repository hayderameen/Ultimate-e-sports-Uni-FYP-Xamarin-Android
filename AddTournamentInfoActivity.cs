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
using Firebase.Database;
using Firebase.Xamarin.Database;

namespace Firebase
{
    [Activity(Label = "AddTournamentInfoActivity")]
    public class AddTournamentInfoActivity : Activity
    {
        string tourneyID;
        DatabaseReference mDatabase;
        string tourneyFormat = "Double Elimination Knockout";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            mDatabase = FirebaseDatabase.Instance.Reference;

            tourneyID = Intent.GetStringExtra("tourneyID") ?? "None";

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddTournamentInfo);

            // Adding Components
            AutoCompleteTextView tournamentTitleText = FindViewById<AutoCompleteTextView>(Resource.Id.tournamentTitleText);
            AutoCompleteTextView awardMoney = FindViewById<AutoCompleteTextView>(Resource.Id.awardMoneyText);
            AutoCompleteTextView participantsLimit = FindViewById<AutoCompleteTextView>(Resource.Id.participantsLimitText);
            AutoCompleteTextView entryFee = FindViewById<AutoCompleteTextView>(Resource.Id.entryFeeText);
            AutoCompleteTextView gameTitle = FindViewById<AutoCompleteTextView>(Resource.Id.gameTitleText);
            AutoCompleteTextView description = FindViewById<AutoCompleteTextView>(Resource.Id.descriptionText);
            RadioButton knockout = FindViewById<RadioButton>(Resource.Id.knockout);
            //RadioButton doubleEliminationKnockout = FindViewById<RadioButton>(Resource.Id.doubleEliminationKnockout);
            Button saveTournamentInfoButton = FindViewById<Button>(Resource.Id.tournamentInfoSaveButton);
            Button startDateButton = FindViewById<Button>(Resource.Id.startDateButton);
            Button finishDateButton = FindViewById<Button>(Resource.Id.finishDateButton);
            ImageView showCase = FindViewById<ImageView>(Resource.Id.tourneyImageShowCase);

            string showCaseURL = "https://firebasestorage.googleapis.com/v0/b/fir-test-1bdb3.appspot.com/o/ShowCase.png?alt=media&token=f0c6e2e7-e9fc-46e8-a2ad-528ebf778aad";
            Glide.With(this).Load(showCaseURL).Apply(RequestOptions.CircleCropTransform()).Into(showCase);

            // Loading data from DB

            FirebaseClient firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/tournaments/");
            var items = await firebase
                .Child("")
                .OnceAsync<TournamentViewModel>();

            foreach (var item in items)
            {
                if (item.Key.Equals(tourneyID))
                {
                    tournamentTitleText.Text = item.Object.Title ?? "";
                    startDateButton.Text = item.Object.StartDate ?? "Start Date";
                    finishDateButton.Text = item.Object.FinishDate ?? "Finish Date";
                    awardMoney.Text = item.Object.AwardMoney + "" ?? "";
                    participantsLimit.Text = item.Object.ParticipantsLimit ?? "";
                    entryFee.Text = item.Object.EntryFee ?? "0";
                    gameTitle.Text = item.Object.Game ?? "";
                    description.Text = item.Object.Description ?? "";

                   //if (item.Object.Format.Equals("Double Elimination Knockout"))
                   //{
                   //     doubleEliminationKnockout.Checked = true;
                   //     tourneyFormat = "Double Elimination Knockout";
                   //}
                   if (item.Object.Format.Equals("Knockout"))
                   {
                        knockout.Checked = true;
                        tourneyFormat = "Knockout";
                   }

                   break;
                }
            }

            startDateButton.Click += (sender, e) =>
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    startDateButton.Text = time.ToLongDateString();
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };

            finishDateButton.Click += (sender, e) =>
            {

                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    finishDateButton.Text = time.ToLongDateString();
                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };

            async void RadioButtonClickAsync(object sender, EventArgs e)
            {
                RadioButton rb = (RadioButton)sender;
                tourneyFormat = rb.Text;
                await mDatabase.Child("tournaments").Child(tourneyID).Child("Format").SetValueAsync(rb.Text);
                
            }

            knockout.Click += RadioButtonClickAsync;
            //doubleEliminationKnockout.Click += RadioButtonClickAsync;

            saveTournamentInfoButton.Click += async (sender, e) =>
            {
                if (gameTitle.Text.Equals(string.Empty) || tournamentTitleText.Equals(string.Empty) || startDateButton.Text.Equals("Start Date") || finishDateButton.Text.Equals("Finish Date") || awardMoney.Equals(""))
                {
                    Toast.MakeText(ApplicationContext, "Enter data in all fields first", ToastLength.Long).Show();
                }
                else
                {
                    string titleTemp = tournamentTitleText.Text.Replace(' ', '_');
                    titleTemp = tournamentTitleText.Text.Replace('-', '_');
                    titleTemp = tournamentTitleText.Text.Replace('.', '_');
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("Title").SetValueAsync(titleTemp); 
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("StartDate").SetValueAsync(startDateButton.Text);
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("FinishDate").SetValueAsync(finishDateButton.Text); 
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("AwardMoney").SetValueAsync(awardMoney.Text);
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("Game").SetValueAsync(gameTitle.Text.ToUpper());
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("Description").SetValueAsync(description.Text);



                    // Inserting Participants limit if it meets certain conditions
                    int limit;
                    bool result = int.TryParse(participantsLimit.Text, out limit);
                    if (result)
                    {
                        if (limit > 3 && limit < 33)
                        {
                            await mDatabase.Child("tournaments").Child(tourneyID).Child("ParticipantsLimit").SetValueAsync(participantsLimit.Text);
                        }
                        else
                        {
                            await mDatabase.Child("tournaments").Child(tourneyID).Child("ParticipantsLimit").SetValueAsync("6");
                            Toast.MakeText(ApplicationContext, "Limit should be above 3 or less than 33, so set to 6", ToastLength.Long).Show();
                        }

                    } // ELSE statement if number is not correct then set it to 6 by default
                    else
                    {
                        await mDatabase.Child("tournaments").Child(tourneyID).Child("ParticipantsLimit").SetValueAsync("6");
                        Toast.MakeText(ApplicationContext, "Invalid participants limit, set to 6 automatically", ToastLength.Long).Show();
                    }

                    // Checking and inserting the entry fee
                    int entryFeeNumber;
                    result = int.TryParse(entryFee.Text, out entryFeeNumber);
                    if (result)
                    {
                        if (entryFeeNumber == 0)
                        {
                            await mDatabase.Child("tournaments").Child(tourneyID).Child("EntryFee").SetValueAsync(entryFee.Text);
                            await mDatabase.Child("tournaments").Child(tourneyID).Child("Paid").SetValueAsync("false");
                        }
                        else
                        {
                            await mDatabase.Child("tournaments").Child(tourneyID).Child("EntryFee").SetValueAsync(entryFee.Text);
                            await mDatabase.Child("tournaments").Child(tourneyID).Child("Paid").SetValueAsync("true");
                        }
                    }
                    else
                    {
                        await mDatabase.Child("tournaments").Child(tourneyID).Child("EntryFee").SetValueAsync("0");
                        await mDatabase.Child("tournaments").Child(tourneyID).Child("Paid").SetValueAsync("false");

                        Toast.MakeText(ApplicationContext, "Entered Entry Fee number was invalid so it is set to 0 and Free", ToastLength.Long).Show();
                    }


                    // Going back to Tournament Main page
                    Intent addTournamentActivity = new Intent(Application.Context, typeof(AddTournamentActivity));
                    addTournamentActivity.PutExtra("tourneyID", tourneyID);
                    StartActivity(addTournamentActivity);
                    Finish();
                }
            };
        }
    }
}