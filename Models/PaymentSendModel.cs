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

namespace Firebase.Models
{
    public class PaymentSendModel
    {
        public string TournamentID { get; set; }
        public string EntryFee { get; set; }
        public string PlayerEmail { get; set; }
        public string Participants { get; set; }
        public string NewParticipant { get; set; }
    }
}