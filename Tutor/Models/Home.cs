using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models
{
    public class Home
    {
        public String memberStatus { get; set; }
        public String credit { get; set; }
        public Home()
        {
            memberStatus = "Free member";
            credit = "10000 points";
        }
    }
}