using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace Firebase
{
    [Activity(Label = "ViewTournamentBracketActivity")]
    public class ViewTournamentBracketActivity : Activity
    {

        WebView web_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            string url = Intent.GetStringExtra("url") ?? "None";

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ViewTournamentBracket);

            web_view = FindViewById<WebView>(Resource.Id.webView);
            web_view.Settings.JavaScriptEnabled = true;
            web_view.SetWebViewClient(new HelloWebViewClient());
            
            web_view.LoadUrl(url);


        }
    }

    public class HelloWebViewClient : WebViewClient
    {
        // For API level 24 and later
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            view.LoadUrl(request.Url.ToString());
            return false;
        }
    }
}