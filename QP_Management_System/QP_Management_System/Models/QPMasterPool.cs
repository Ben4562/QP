using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QP_Management_System.Models
{
    public class QPMasterPool
    {
        public string QPDocId { get; set; }
        public byte[] Document { get; set; }
        public string DocumentName { get; set; }
        public string Author { get; set; }
        public string Reviewer { get; set; }
        public string QualityAnchor { get; set; }
        public string Status { get; set; }
        public Nullable<int> ModuleId { get; set; }
        public System.DateTime CreationLog { get; set; }
        public Nullable<System.DateTime> UpdationLog { get; set; }
        public string Comments { get; set; }
    }
}