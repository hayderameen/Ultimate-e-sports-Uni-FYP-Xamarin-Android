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
    [Activity(Label = "Send Message")]
    public class MessageSendActivity : Activity
    {
        string toEmail; // Email of person to whom message is being sent
        string fromEmail; // sender
        FirebaseAuth auth;
        private ProgressBar circular_progress;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            auth = FirebaseAuth.GetInstance(MainActivity.app);
            
            SetContentView(Resource.Layout.MessageSend);

            EditText recipient = FindViewById<EditText>(Resource.Id.recipient);
            EditText message = FindViewById<EditText>(Resource.Id.message);
            Button send = FindViewById<Button>(Resource.Id.sendButton);
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgressMessageSend);

            if (HomeActivity.fromHome)
            {
                fromEmail = auth.CurrentUser.Email;
                // toEmail will be entered by user
            }
            else
            {
                fromEmail = auth.CurrentUser.Email;
                // 1: Receive toEmail from incoming intent which could either be through profile or through inbox
                // 2: Then insert it into variable
                toEmail = Intent.GetStringExtra("toEmail") ?? "None";
                recipient.Text = toEmail;
            }

            send.Click += async (sender, e) =>
            {
                if (!recipient.Text.Equals(string.Empty) && !message.Text.Equals(string.Empty))
                {
                    circular_progress.Visibility = ViewStates.Visible;
                    // Check if recipient Email exists or not

                    toEmail = recipient.Text.ToLower().Trim();

                    bool check = false;
                    var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
                    var items = await firebase
                        .Child("users")
                        .OnceAsync<UserViewModel>();

                    foreach (var item in items)
                    {
                        if (item.Object.Email.Equals(toEmail))
                        {
                            check = true;
                            break;
                        }
                    }

                    if (check)
                    {
                        var item = firebase.Child("messages")
                        .PostAsync<MessageViewModel>(new MessageViewModel {
                            ToEmail = toEmail,
                            FromEmail = fromEmail,
                            Body = message.Text.Trim(),
                            Timestamp = DateTime.Now.ToString()
                        });


                        message.Text = string.Empty;

                        Toast.MakeText(ApplicationContext, "Message Sent", ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "Enter a valid recipient", ToastLength.Long).Show();
                        
                    }
                    circular_progress.Visibility = ViewStates.Invisible;
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Fill both fields first", ToastLength.Long).Show();
                    
                }
            };
        }
    }
}