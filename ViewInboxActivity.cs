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
    [Activity(Label = "View Inbox")]
    public class ViewInboxActivity : Activity
    {
        // NOTE: Add this to make inbox dynamic later await mDatabase.Child("messages").AddValueEventListener

        string toEmail; // Inbox of person opening this activity
        FirebaseAuth auth;
        private ProgressBar circular_progress;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ViewInbox);

            Button timeStamp = FindViewById<Button>(Resource.Id.timeStamp);
            ImageView profilePicture = FindViewById<ImageView>(Resource.Id.profilePictureInbox);
            Button nameAndEmail = FindViewById<Button>(Resource.Id.nameAndEmailInbox);
            Button messageBody = FindViewById<Button>(Resource.Id.messageBodyInbox);
            Button next = FindViewById<Button>(Resource.Id.nextMessage);
            Button previous = FindViewById<Button>(Resource.Id.previousMessage);
            EditText replyText = FindViewById<EditText>(Resource.Id.replyBodyInbox);
            Button reply = FindViewById<Button>(Resource.Id.replyButton);
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgressInbox);

            nameAndEmail.Enabled = false;
            messageBody.Enabled = false;
            timeStamp.Enabled = false;

            auth = FirebaseAuth.GetInstance(MainActivity.app);
            toEmail = auth.CurrentUser.Email;

            circular_progress.Visibility = ViewStates.Visible;
            // Loading messages
            var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var items = await firebase
                .Child("messages")
                .OnceAsync<MessageViewModel>();

            // Loading users' data to get profile pictures
            var users = await firebase.Child("users").OnceAsync<UserViewModel>();

            List<MessageViewModel> query = new List<MessageViewModel>();

            // Filtering
            foreach (var item in items)
            {
                if (item.Object.ToEmail.Equals(toEmail))
                {
                    query.Add(item.Object);
                }
            }

            query.Reverse();

            int messageNumber = 0; // To keep track of messages

            timeStamp.Text = query[messageNumber].Timestamp;
            

            foreach (var item in users)
            {
                if (item.Object.Email.Equals(query[messageNumber].FromEmail))
                {
                    if (!item.Object.PhotoURL.Equals(string.Empty))
                    {
                        Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                    }
                    nameAndEmail.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                }
            }

            messageBody.Text = query[messageNumber].FromEmail + " says: \n\n" + query[messageNumber].Body;

            circular_progress.Visibility = ViewStates.Invisible;

            next.Click += (sender, e) =>
            {
                int a = query.Count();
                if (query.Count()-1 == messageNumber)
                {
                    // No new messages
                    Toast.MakeText(ApplicationContext, "No more messages", ToastLength.Short).Show();
                }
                else
                {
                    circular_progress.Visibility = ViewStates.Visible;

                    messageNumber++;

                    timeStamp.Text = query[messageNumber].Timestamp;


                    foreach (var item in users)
                    {
                        if (item.Object.Email.Equals(query[messageNumber].FromEmail))
                        {
                            if (!item.Object.PhotoURL.Equals(string.Empty))
                            {
                                Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                            }
                            nameAndEmail.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                        }
                    }

                    messageBody.Text = query[messageNumber].FromEmail + " says: \n\n" + query[messageNumber].Body;

                    circular_progress.Visibility = ViewStates.Invisible;

                }
            };

            previous.Click += (sender, e) =>
            {
                int a = query.Count();
                if (messageNumber == 0)
                {
                    // No new messages
                    Toast.MakeText(ApplicationContext, "No more messages", ToastLength.Short).Show();
                }
                else
                {
                    circular_progress.Visibility = ViewStates.Visible;

                    messageNumber--;

                    timeStamp.Text = query[messageNumber].Timestamp;


                    foreach (var item in users)
                    {
                        if (item.Object.Email.Equals(query[messageNumber].FromEmail))
                        {
                            if (!item.Object.PhotoURL.Equals(string.Empty))
                            {
                                Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                            }
                            nameAndEmail.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                        }
                    }

                    messageBody.Text = query[messageNumber].FromEmail + " says: \n\n" + query[messageNumber].Body;

                    circular_progress.Visibility = ViewStates.Invisible;

                }
            };

            reply.Click += (sender, e) =>
            {
                circular_progress.Visibility = ViewStates.Visible;
                if (!replyText.Text.Equals(string.Empty))
                {
                    var item = firebase.Child("messages")
                        .PostAsync<MessageViewModel>(new MessageViewModel
                        {
                            ToEmail = query[messageNumber].FromEmail,
                            FromEmail = auth.CurrentUser.Email,
                            Body = replyText.Text.Trim(),
                            Timestamp = DateTime.Now.ToString()
                        });


                        replyText.Text = string.Empty;

                        Toast.MakeText(ApplicationContext, "Message Sent", ToastLength.Short).Show();  
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Enter a reply first", ToastLength.Short).Show();
                }
                circular_progress.Visibility = ViewStates.Invisible;
            };
            /*
            string[] messages = new string[query.Count()];

            for (int i = 0; i < query.Count(); i++)
            {
                messages[i] = query.ToList().ElementAt(i).Timestamp + "\n" + query.ToList().ElementAt(i).Body + "\n  -  " + query.ToList().ElementAt(i).FromEmail;

            }
            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.ViewInbox, messages);

            ListView.ItemClick += (sender, e) =>
            {
                string recipient = string.Empty;
                recipient = ((TextView)e.View).Text;
                string[] temps = recipient.Split(' ');
                recipient = temps[temps.Length - 1];
                Intent sendMessageActivity = new Intent(ApplicationContext, typeof(MessageSendActivity));
                sendMessageActivity.PutExtra("toEmail", recipient);
                HomeActivity.fromHome = false;
                StartActivity(sendMessageActivity);
            };
            */
        }
    }
}