using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QP_Management_System.Models
{
    public class Track
    {
        public int TrackId { get; set; }
        public string TrackName { get; set; }
        public System.DateTime CreationLog { get; set; }
        public Nullable<System.DateTime> UpdationLog { get; set; }
        public string Comments { get; set; }
    }
}