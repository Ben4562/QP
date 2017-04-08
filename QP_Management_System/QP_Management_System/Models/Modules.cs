using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QP_Management_System.Models
{
    public class Modules
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public Nullable<int> FAId { get; set; }
        public Nullable<float> Marks { get; set; }
        public System.DateTime CreationLog { get; set; }
        public Nullable<System.DateTime> UpdationLog { get; set; }
        public string Comments { get; set; }
    }
}