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
    [Activity(Label = "ViewSocialFeedActivity")]
    public class ViewSocialFeedActivity : Activity
    {
        FirebaseAuth auth;
        private ProgressBar circular_progress;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ViewSocialFeed);

            auth = FirebaseAuth.GetInstance(MainActivity.app);

            // Adding views
            Button timeStamp = FindViewById<Button>(Resource.Id.timeStampSocialFeed);
            ImageView profilePicture = FindViewById<ImageView>(Resource.Id.profilePictureSocialFeed);
            Button name = FindViewById<Button>(Resource.Id.nameSocialFeed);
            Button postBody = FindViewById<Button>(Resource.Id.SocialFeedPostBody);
            Button commentBody = FindViewById<Button>(Resource.Id.SocialFeedCommentBody);
            Button next = FindViewById<Button>(Resource.Id.nextPost);
            Button previous = FindViewById<Button>(Resource.Id.previousPost);
            EditText commentText = FindViewById<EditText>(Resource.Id.CommentBodySocialFeed);
            Button reply = FindViewById<Button>(Resource.Id.commentButton);
            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgressSocialFeed);

            timeStamp.Enabled = false;
            name.Enabled = false;
            postBody.Enabled = false;
            commentBody.Enabled = false;

            circular_progress.Visibility = ViewStates.Visible;
            // Loading Social Posts from DB
            var firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var query = await firebase
                .Child("socialPosts")
                .OnceAsync<SocialPostViewModel>();

            //query.Reverse();

            List<SocialPostViewModel> posts = new List<SocialPostViewModel>();

            foreach (var item in query)
            {
                posts.Add(item.Object);
            }

            posts.Reverse();

            // Loading users' data to get profile pictures
            var users = await firebase.Child("users").OnceAsync<UserViewModel>();

            // Getting comments from DB
            firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var comments = await firebase
                .Child("comments")
                .OnceAsync<CommentViewModel>();

            //comments.Reverse();

            List<CommentViewModel> commentsList = new List<CommentViewModel>();
            
            foreach (var item in comments)
            {
                commentsList.Add(item.Object);
            }



            int postNumber = 0; // To keep track

            // Loading first post into views
            timeStamp.Text = posts[postNumber].TimeStamp;
            //name.Text = posts[postNumber].Email;

            foreach (var item in users)
            {
                if (item.Object.Email.Equals(posts[postNumber].Email))
                {
                    if (!item.Object.PhotoURL.Equals(string.Empty))
                    {
                        Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                    }
                    name.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                }
            }

            postBody.Text = posts[postNumber].Body;

            string commentTextTemp = "Comments:\n";
            int commentCounter = 0;

            // Filtering
            foreach (var item in commentsList)
            {
                if (item.SocialPostID.Equals(posts[postNumber].SocialPostID))
                {
                    commentCounter++;
                    commentTextTemp += "\n" + item.Email + ":\n" + " - " + item.Body + "\n";
                }
            }
            // Done getting comments


            if (commentCounter > 0)
            {
                commentBody.Text = commentTextTemp;
            }
            else
            {
                commentBody.Text += "\nNo Comments Yet";
            }

            circular_progress.Visibility = ViewStates.Invisible;

            next.Click += (sender, e) =>
            {
                if (query.Count() - 1 == postNumber)
                {
                    // No new posts
                    Toast.MakeText(ApplicationContext, "No more posts", ToastLength.Short).Show();
                }
                else
                {
                    circular_progress.Visibility = ViewStates.Visible;

                    postNumber++;

                    timeStamp.Text = posts[postNumber].TimeStamp;
                    //name.Text = posts[postNumber].Email;

                    foreach (var item in users)
                    {
                        if (item.Object.Email.Equals(posts[postNumber].Email))
                        {
                            if (!item.Object.PhotoURL.Equals(string.Empty))
                            {
                                Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                            }
                            name.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                        }
                    }

                    postBody.Text = posts[postNumber].Body;

                    commentTextTemp = "Comments:\n";

                    commentCounter = 0;
                    // Filtering
                    foreach (var item in commentsList)
                    {
                        if (item.SocialPostID.Equals(posts[postNumber].SocialPostID))
                        {
                            commentCounter++;
                            commentTextTemp += "\n" + item.Email + ":\n" + " - " + item.Body + "\n";
                        }
                    }
                    // Done getting comments


                    if (commentCounter > 0)
                    {
                        commentBody.Text = commentTextTemp;
                    }
                    else
                    {
                        commentBody.Text = "Comments:\n";
                        commentBody.Text += "\nNo Comments Yet";
                    }

                    circular_progress.Visibility = ViewStates.Invisible;
                }
            };

            previous.Click += (sender, e) =>
            {
                if (0 == postNumber)
                {
                    // No new posts
                    Toast.MakeText(ApplicationContext, "No more posts", ToastLength.Short).Show();
                }
                else
                {
                    circular_progress.Visibility = ViewStates.Visible;

                    postNumber--;

                    timeStamp.Text = posts[postNumber].TimeStamp;
                    name.Text = posts[postNumber].Email;

                    foreach (var item in users)
                    {
                        if (item.Object.Email.Equals(posts[postNumber].Email))
                        {
                            if (!item.Object.PhotoURL.Equals(string.Empty))
                            {
                                Glide.With(this).Load(item.Object.PhotoURL).Apply(RequestOptions.CircleCropTransform()).Into(profilePicture);
                            }
                            name.Text = item.Object.FirstName + " " + item.Object.LastName ?? "";
                        }
                    }

                    postBody.Text = posts[postNumber].Body;

                    commentTextTemp = "Comments:\n";

                    commentCounter = 0;
                    // Filtering
                    foreach (var item in commentsList)
                    {
                        if (item.SocialPostID.Equals(posts[postNumber].SocialPostID))
                        {
                            commentCounter++;
                            commentTextTemp += "\n" + item.Email + ":\n" + " - " + item.Body + "\n";
                        }
                    }
                    // Done getting comments


                    if (commentCounter > 0)
                    {
                        commentBody.Text = commentTextTemp;
                    }
                    else
                    {
                        commentBody.Text = "Comments:\n";
                        commentBody.Text += "\nNo Comments Yet";
                    }

                    circular_progress.Visibility = ViewStates.Invisible;
                }
            };

            reply.Click += async (sender, e) =>
            {
                if (commentText.Text != String.Empty)
                {
                    firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
                    var item = await firebase.Child("comments")
                    .PostAsync<CommentViewModel>(new CommentViewModel
                    {
                        Email = auth.CurrentUser.Email,
                        Body = commentText.Text.Trim(),
                        SocialPostID = posts[postNumber].SocialPostID
                    });

                    commentText.Text = "";

                    Toast.MakeText(ApplicationContext, "Published", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Enter a comment first", ToastLength.Long).Show();
                }
            };

        }
    }
}