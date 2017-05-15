using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QP_Management_System.Models
{
    public class Editor
    {
        [AllowHtml]
        public string HtmlContent { get; set; }

        public string DocId { get; set; }

    }
}