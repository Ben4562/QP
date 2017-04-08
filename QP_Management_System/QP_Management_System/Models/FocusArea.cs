using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QP_Management_System.Models
{
    public class FocusArea
    {
        public int FAId { get; set; }
        public string FAName { get; set; }
        public Nullable<float> TotalMarks { get; set; }
        public Nullable<int> TrackId { get; set; }
        public System.DateTime CreationLog { get; set; }
        public Nullable<System.DateTime> UpdationLog { get; set; }
        public string Comments { get; set; }
    }
}