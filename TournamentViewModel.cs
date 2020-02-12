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
    class TournamentViewModel
    {
        public string TournamentID { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string AdminID { get; set; }
        public string Participants { get; set; }
        public string Finished { get; set; }
        public string Format { get; set; }
        public string BracketID { get; set; }
        public string BracketURL { get; set; }
        public string online { get; set; }
        public string Location { get; set; }
        public string PicturePath { get; set; }
        public string AwardMoney { get; set; }
        public string Live { get; set; }
        public string ParticipantsLimit { get; set; }
        public string EntryFee { get; set; }
        public string ParticipantsPayments { get; set; }
        public string Paid { get; set; }
        public string Game { get; set; }
        public string Description { get; set; }
    }
}