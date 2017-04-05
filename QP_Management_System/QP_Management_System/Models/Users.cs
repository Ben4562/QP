using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QP_Management_System.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public System.DateTime LastLogin { get; set; }
        public Nullable<int> TrackId { get; set; }
    }
}