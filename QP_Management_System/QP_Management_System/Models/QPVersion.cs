using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QP_Management_System.Models
{
    public class QPVersion
    {
        [Required(ErrorMessage ="DocId is Mandatory")]
        public string DocId { get; set; }

        [Required(ErrorMessage = "Doc is Mandatory")]
        public byte[] Document { get; set; }

        [Required(ErrorMessage = "DocName is Mandatory")]
        public string DocumentName { get; set; }

        [Required(ErrorMessage = "VersionId is Mandatory")]
        public string VersionId { get; set; }

        public System.DateTime CreationLog { get; set; }

        public Nullable<System.DateTime> UpdationLog { get; set; }
        public string Comments { get; set; }
    }
}