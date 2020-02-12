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
    class MessageViewModel
    {
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public string Body { get; set; }
        public string Flagged { get; set; }
        public string Timestamp { get; set; }
    }
}