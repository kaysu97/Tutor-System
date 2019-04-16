using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models
{
    public class tutorInformation
    {
        public String subject { get; set; }
        public String salary { get; set; }
        public String grade { get; set; }
        public String region { get; set; }
        public String gender { get; set; }
        public String transportation { get; set; }
        public String schedule { get; set; }
        public String contactInfo { get; set; }
        public String frequency { get; set; }
        public String note { get; set; }
        public tutorInformation()
        {
            subject = "數學";
            salary = "100000";
            grade = "one";
            region = "taipei";
            gender = "boy";
            transportation = "bus";
            schedule = "wed,mon";
            contactInfo = "09xxxx";
            frequency = "once a week";
            note = "no";
        }
    }
}