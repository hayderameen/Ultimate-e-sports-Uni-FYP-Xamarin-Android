using Android.App;
using Android.Widget;
using Android.OS;
using Firebase;
using Firebase.Auth;
using System;
using static Android.Views.View;
using Android.Views;
using Android.Gms.Tasks;
using Xamarin.Essentials;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Runtime;
//using Android.Preferences;


namespace Firebase
{
    [Activity(Label = "Login or Signup")]
    public class MainActivity : Activity, IOnClickListener, IOnCompleteListener
    {
        Button btnLogin;
        EditText input_email, input_password;
        TextView btnSignUp, btnForgetPassword;
        RelativeLayout activity_main;
        public static FirebaseApp app;
        FirebaseAuth auth;
        public static string decision;

        /*
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        */
        


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Initating Preferences for login
            //Platform.Init(this, savedInstanceState);



            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.Main);
            //Init Auth  
            InitFirebaseAuth();
            
            //Views 

            btnLogin = FindViewById<Button>(Resource.Id.login_btn_login);
            input_email = FindViewById<EditText>(Resource.Id.login_email);
            input_password = FindViewById<EditText>(Resource.Id.login_password);
            btnSignUp = FindViewById<TextView>(Resource.Id.login_btn_sign_up);
            btnForgetPassword = FindViewById<TextView>(Resource.Id.login_btn_forget_password);
            activity_main = FindViewById<RelativeLayout>(Resource.Id.activity_main);
            RadioButton adminRadio = FindViewById<RadioButton>(Resource.Id.adminRadio);
            RadioButton playerRadio = FindViewById<RadioButton>(Resource.Id.playerRadio);
            btnSignUp.SetOnClickListener(this);
            btnLogin.SetOnClickListener(this);
            btnForgetPassword.SetOnClickListener(this);

            // 1: Set Radio Buttons 
            // 2: Check DB based on radio button selections

            decision = "Login as Admin"; // Default Value // Other value = "Signup as Player" // Whether admin or player is signing up
            
            // Steps if user login info is saved already
           /* string emailStore = Preferences.Get("email", "none");
            if (!emailStore.Equals("none") && !emailStore.Equals(string.Empty))
            {
                string passwordStore = Preferences.Get("password", "none");
                string decisionStore = Preferences.Get("decision", "none");
                decision = decisionStore;

                LoginUser(emailStore, passwordStore);
            }
            */

            void RadioButtonClick(object sender, EventArgs e)
            {
                RadioButton rb = (RadioButton)sender;
                decision = rb.Text;
                
            }

            adminRadio.Click += RadioButtonClick;
            playerRadio.Click += RadioButtonClick;

        }
        private void InitFirebaseAuth()
        {
            var options = new FirebaseOptions.Builder()
               .SetApplicationId("appid")
               .SetApiKey("apikey")
               .Build();
            if (app == null)
                app = FirebaseApp.InitializeApp(this, options, "authentication");
            auth = FirebaseAuth.GetInstance(app);
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.login_btn_forget_password)
            {
                StartActivity(new Android.Content.Intent(this, typeof(ForgetPassword)));
                //Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_sign_up)
            {
                StartActivity(new Android.Content.Intent(this, typeof(SignUp)));
                //Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_login)
            {
                LoginUser(input_email.Text.Trim(), input_password.Text.Trim());
                
            }
        }
        private void LoginUser(string email, string password)
        {
            auth.SignInWithEmailAndPassword(email, password).AddOnCompleteListener(this);
        }
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                // Saving login details
                //Preferences.Set("email", input_email.Text.Trim());
                //Preferences.Set("password", input_password.Text.Trim());
                //Preferences.Set("decision", decision);
                StartActivity(new Android.Content.Intent(this, typeof(HomeActivity)));
                Finish();
            }
            else
            {
                
                //Snackbar snackbar = Snackbar.Make(activity_main, "Login Failed ", Snackbar.LengthShort);
                //snackbar.Show();
            }
        }

        
    }
}