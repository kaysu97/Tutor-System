using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models
{
    public class Email
    {
        public DateTime create_time { get; set; }

        public DateTime Modify_time { get; set; }

        public string MailID { get; set; }

        public string MailTo { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string status { get; set; }
    }
}