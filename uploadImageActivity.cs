using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Storage;
using System;
using System.IO;

namespace Firebase
{
    [Activity(Label = "XamarinFirebaseUpload")]
    public class uploadImageActivity : AppCompatActivity, IOnProgressListener, IOnSuccessListener, IOnFailureListener
    {
        private Button btnUpload, btnChoose;
        private ImageView imgView;
        private Android.Net.Uri filePath;
        private const int PICK_IMAGE_REQUSET = 71;
        ProgressDialog progressDialog;
        FirebaseStorage storage;
        StorageReference storageRef;
        FirebaseAuth auth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.uploadImage);
            //Firebase Init  
            //FirebaseApp.InitializeApp(this);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            
            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://fir-test-1bdb3.appspot.com/");
            //Init View  
            btnChoose = FindViewById<Button>(Resource.Id.btnChoose);
            btnUpload = FindViewById<Button>(Resource.Id.btnUpload);
            imgView = FindViewById<ImageView>(Resource.Id.imgView);
            //Events  
            btnChoose.Click += delegate
            {
                ChooseImage();
            };
            btnUpload.Click += delegate
            {
                UploadImage();
            };
        }
        private void UploadImage()
        {
            if (filePath != null)
                progressDialog = new ProgressDialog(this);
            progressDialog.SetTitle("Uploading...");
            progressDialog.Window.SetType(Android.Views.WindowManagerTypes.SystemAlert);
            //progressDialog.Show();
            //var images = storageRef.Child("images/" + Guid.NewGuid().ToString());
            var images = storageRef.Child("images/" + auth.CurrentUser.Uid);
            images.PutFile(filePath)
                .AddOnProgressListener(this)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);

            int a = 1;
        }
        private void ChooseImage()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PICK_IMAGE_REQUSET);
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
                try
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, filePath);
                    imgView.SetImageBitmap(bitmap);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        public void OnProgress(Java.Lang.Object snapshot)
        {
            var taskSnapShot = (UploadTask.TaskSnapshot)snapshot;
            double progress = (100.0 * taskSnapShot.BytesTransferred / taskSnapShot.TotalByteCount);
            progressDialog.SetMessage("Uploaded " + (int)progress + " %");
        }
        public void OnSuccess(Java.Lang.Object result)
        {
            progressDialog.Dismiss();
            Toast.MakeText(this, "Uploaded Successful", ToastLength.Short).Show();
            var snapshot = (UploadTask.TaskSnapshot)result;
            var url = "https://firebasestorage.googleapis.com" + snapshot.DownloadUrl.EncodedPath + "?" + snapshot.DownloadUrl.EncodedQuery; // download url for user image

            //int a = 1;

            var profileUpdates = new UserProfileChangeRequest.Builder()
        .SetDisplayName("Jane Q. User")
        .SetPhotoUri(Android.Net.Uri.Parse(url))
        .Build();
            auth.CurrentUser.UpdateProfile(profileUpdates);
        }
        public void OnFailure(Java.Lang.Exception e)
        {
            progressDialog.Dismiss();
            Toast.MakeText(this, "" + e.Message, ToastLength.Short).Show();
        }
    }
}