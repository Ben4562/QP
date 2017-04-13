using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QP_Management_System.Models
{
    public class Users
    {
        [Required(ErrorMessage ="UserName is Mandatory")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",

            ErrorMessage = "Invalid email address.")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Password is Mandatory")]
        public string UserPassword { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> TrackId { get; set; }
        public System.DateTime CreationLog { get; set; }
        public Nullable<System.DateTime> UpdationLog { get; set; }
        public string Comments { get; set; }
    }
}