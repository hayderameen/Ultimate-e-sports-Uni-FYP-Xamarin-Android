﻿using System;
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
    class SocialPostViewModel
    {
        public string SocialPostID { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public string TimeStamp { get; set; }
    }
}