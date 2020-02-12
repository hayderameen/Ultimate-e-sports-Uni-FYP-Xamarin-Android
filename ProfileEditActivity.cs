using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Xamarin.Database;
using Newtonsoft.Json;

namespace Firebase
{
    [Activity(Label = "Edit Profile")]
    public class ProfileEditActivity : AppCompatActivity, IOnSuccessListener
    {
        FirebaseAuth auth;
        private Android.Net.Uri filePath;
        private const int PICK_IMAGE_REQUSET = 71;
        FirebaseStorage storage;
        StorageReference storageRef;
        string userKey;
        FirebaseClient firebase;
        bool changeImage = false;
        string PhotoURL;

        EditText firstName;
        EditText lastName;
        EditText Bio;
        EditText City;
        EditText Country;
        ImageView profilePicturePreview;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            auth = FirebaseAuth.GetInstance(MainActivity.app);
            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://fir-test-1bdb3.appspot.com/");

            SetContentView(Resource.Layout.ProfileEdit);

            EditText newPassword = FindViewById<EditText>(Resource.Id.newPassword);
            Button changePWButton = FindViewById<Button>(Resource.Id.ChangePasswordButton);
            firstName = FindViewById<EditText>(Resource.Id.FirstName);
            lastName = FindViewById<EditText>(Resource.Id.LastName); 
            profilePicturePreview = FindViewById<ImageView>(Resource.Id.ProfilePicturePreview);
            ImageButton selectProfilePicture = FindViewById<ImageButton>(Resource.Id.ProfilePicture);
            Button SaveButton = FindViewById<Button>(Resource.Id.SaveButton);
            Bio = FindViewById<EditText>(Resource.Id.Bio);
            City = FindViewById<EditText>(Resource.Id.City);
            Country = FindViewById<EditText>(Resource.Id.Country);

            

            // Populating fields with pre-existing data
            loadData();

            selectProfilePicture.Click += (sender, e) =>
            {
                ChooseImage();
            };

            SaveButton.Click += (sender, e) =>
            {
                if (changeImage)
                    UploadImage();
                saveData();
                
            };

        }

        private async void saveData()
        {
            /*
            UserViewModel newUser = new UserViewModel
            {
                Email = auth.CurrentUser.Email,
                FirstName = firstName.Text.Trim(),
                LastName = lastName.Text.Trim(),
                City = City.Text.Trim(),
                Country = Country.Text.Trim(),
                Bio = Bio.Text.Trim()
            };
            */

            //firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/users/");
            //var item = firebase.Child("").PostAsync<UserViewModel>(newUser);
            DatabaseReference mDatabase = FirebaseDatabase.Instance.Reference;
            await mDatabase.Child("users").Child(userKey).Child("FirstName").SetValueAsync(firstName.Text.Trim());
            await mDatabase.Child("users").Child(userKey).Child("LastName").SetValueAsync(lastName.Text.Trim());
            await mDatabase.Child("users").Child(userKey).Child("City").SetValueAsync(City.Text.Trim());
            await mDatabase.Child("users").Child(userKey).Child("Country").SetValueAsync(Country.Text.Trim());
            await mDatabase.Child("users").Child(userKey).Child("Bio").SetValueAsync(Bio.Text.Trim());
            await mDatabase.Child("users").Child(userKey).Child("PhotoURL").SetValueAsync(PhotoURL);
            

            // SO MANY CHECKS LEFT HERE !!!!!!

        }

        private void ChangePassword(string newPassword)
        {
            auth.CurrentUser.UpdatePassword(newPassword);
        }

        private async void loadData()
        {
            firebase = new FirebaseClient("https://fir-test-1bdb3.firebaseio.com/");
            var items = await firebase
                .Child("users")
                .OnceAsync<UserViewModel>();

            foreach (var item in items)
            {
                if (item.Object.Email.Equals(auth.CurrentUser.Email))
                {
                    userKey = item.Key;
                    firstName.Text = item.Object.FirstName ?? "";
                    lastName.Text = item.Object.LastName ?? "";
                    Bio.Text = item.Object.Bio ?? "";
                    City.Text = item.Object.City ?? "";
                    Country.Text = item.Object.Country ?? "";

                    if (auth.CurrentUser.PhotoUrl != null)
                    {
                        var uri = auth.CurrentUser.PhotoUrl;

                        var url = "https://firebasestorage.googleapis.com" + uri.EncodedPath + "?" + uri.EncodedQuery; // download url for user image

                        PhotoURL = url;

                        Glide.With(this).Load(url).Into(profilePicturePreview);
                    }

                    break;
                }

                
            }
        }

        private void UploadImage()
        {
            
            var images = storageRef.Child("images/" + auth.CurrentUser.Uid);
            images.PutFile(filePath).AddOnSuccessListener(this); // Add progress listeneres later
        }
        private void ChooseImage()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Profile Picture"), PICK_IMAGE_REQUSET);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PICK_IMAGE_REQUSET &&
                resultCode == Result.Ok &&
                data != null &&
                data.Data != null)
            {
                filePath = data.Data;
                changeImage = true;
                try
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, filePath);
                    profilePicturePreview.SetImageBitmap(bitmap);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            
            Toast.MakeText(this, "Picture Uploaded Successfully", ToastLength.Short).Show();
            var snapshot = (UploadTask.TaskSnapshot)result;
            var url = "https://firebasestorage.googleapis.com" + snapshot.DownloadUrl.EncodedPath + "?" + snapshot.DownloadUrl.EncodedQuery; // download url for user image
            PhotoURL = url; // To save in db
            //int a = 1;

            var profileUpdates = new UserProfileChangeRequest.Builder().SetPhotoUri(Android.Net.Uri.Parse(url)).Build();
            auth.CurrentUser.UpdateProfile(profileUpdates);
        }
    }
}