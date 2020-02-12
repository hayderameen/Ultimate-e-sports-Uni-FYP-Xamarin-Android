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

namespace Firebase
{
    class GameTagViewModel
    {
        public string Email { get; set; }
        public string GameTitle { get; set; }
        public string GamerTag { get; set; }
        public string Platform { get; set; }
    }
}