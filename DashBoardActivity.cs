using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using static Android.Views.View;
namespace Firebase
{
    [Activity(Label = "DashBoard")]
    public class DashBoard : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        TextView txtWelcome;
        EditText input_new_password, newName;
        Button btnChangePass, btnLogout, btnNewName, subscribeButton, viewDataButton, dashboard_btn_upload_image, dashboard_btn_view_image;
        //private ProgressBar circular_progress;
        RelativeLayout activity_dashboard;
        FirebaseAuth auth;
        private const string FirebaseURL = "https://fir-test-1bdb3.firebaseio.com/users/"; //Firebase Database URL
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.dashboard_btn_change_pass)
                ChangePassword(input_new_password.Text);
            else if (v.Id == Resource.Id.dashboard_btn_logout)
                LogoutUser();
            else if (v.Id == Resource.Id.dashboard_btn_new_name)
                InsertName();
            else if (v.Id == Resource.Id.dashboard_btn_subscribe)
            {
                StartActivity(new Intent(this, typeof(subscribeActivity)));
                Finish();
            }
            else if (v.Id == Resource.Id.dashboard_btn_view_data)
            {
                StartActivity(new Intent(this, typeof(viewDataActivity)));
                Finish();
            }
            else if (v.Id == Resource.Id.dashboard_btn_upload_image)
            {
                StartActivity(new Intent(this, typeof(uploadImageActivity)));
                Finish();
            }
            else if (v.Id == Resource.Id.dashboard_btn_view_image)
            {
                StartActivity(new Intent(this, typeof(profilePictureActivity)));
                Finish();
            }

        }

        private async void InsertName()
        {

            // Insert it into DB
            Account newItem = new Account
            {
                name = newName.Text,
                email = newName.Text + "@Dummy.com"
            };
        
            var firebase = new FirebaseClient(FirebaseURL).Child("");
            
            //firebase.Child("users");
            var item = await firebase.PostAsync<Account>(newItem);
        }

        private void LogoutUser()
        {
            auth.SignOut();
            if (auth.CurrentUser == null)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
        }
        private void ChangePassword(string newPassword)
        {
            FirebaseUser user = auth.CurrentUser;
            user.UpdatePassword(newPassword)
            .AddOnCompleteListener(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DashBoard);
            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            //Add Toolbar  
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Firebase Database";
            SetSupportActionBar(toolbar);
            //View
            //circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            dashboard_btn_view_image = FindViewById<Button>(Resource.Id.dashboard_btn_view_image);
            dashboard_btn_upload_image = FindViewById<Button>(Resource.Id.dashboard_btn_upload_image);
            viewDataButton = FindViewById<Button>(Resource.Id.dashboard_btn_view_data);
            subscribeButton = FindViewById<Button>(Resource.Id.dashboard_btn_subscribe);
            btnNewName = FindViewById<Button>(Resource.Id.dashboard_btn_new_name);
            newName = FindViewById<EditText>(Resource.Id.dashboard_newname);
            btnChangePass = FindViewById<Button>(Resource.Id.dashboard_btn_change_pass);
            txtWelcome = FindViewById<TextView>(Resource.Id.dashboard_welcome);
            btnLogout = FindViewById<Button>(Resource.Id.dashboard_btn_logout);
            input_new_password = FindViewById<EditText>(Resource.Id.dashboard_newpassword);
            activity_dashboard = FindViewById<RelativeLayout>(Resource.Id.activity_dashboard);
            btnChangePass.SetOnClickListener(this);
            btnLogout.SetOnClickListener(this);
            btnNewName.SetOnClickListener(this);
            subscribeButton.SetOnClickListener(this);
            viewDataButton.SetOnClickListener(this);
            dashboard_btn_upload_image.SetOnClickListener(this);
            dashboard_btn_view_image.SetOnClickListener(this);

            btnChangePass.Text = MainActivity.decision;
            

            //Check Session  
            if (auth != null)
            {
                FirebaseUser current = auth.CurrentUser;

                if (!current.IsEmailVerified)
                {
                    current.SendEmailVerification();
                    txtWelcome.Text = "Email not verified. Features unavailable";
                }
                else
                    txtWelcome.Text = "Welcome , " + auth.CurrentUser.Email;
            }
                
            
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_add)
            {
                Log.Debug("DashboardActivity", "Add Button");
            }
            else
            if (id == Resource.Id.menu_save) //Update  
            {
                Log.Debug("DashboardActivity", "Add Button");
            }
            else
            if (id == Resource.Id.menu_delete) //Delete  
            {
                Log.Debug("DashboardActivity", "Add Button");
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Snackbar snackbar = Snackbar.Make(activity_dashboard, "Password has been Changed!", Snackbar.LengthShort);
                snackbar.Show();
            }
        }
    }
}