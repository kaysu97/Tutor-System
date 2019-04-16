using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models
{
    public class Register
    {
        public Register()
        {
            this.last_modify_time = DateTime.Now;
            this.subject = new List<string>();
        }
        public string tutor_id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string school { get; set; }
        public string department { get; set; }
        public string degree { get; set; }
        public string grade { get; set; }
        public string tutor_student_id { get; set; }
        public string sub_department { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string photo { get; set; }
        public string introduction { get; set; }
        public string motivation { get; set; }
        public string photoName { get; set; }
        public DateTime last_modify_time { get; set; }
        public List<string> subject { get; set; }
        public string googleaccount { get; set; }
        public string recommendCode { get; set; }
        public string SuggestCode { get; set; }
        public string BankCode { get; set; }
        public string BankAcct { get; set; }
        public string personalStatus { get; set; }
        public string subjectSTR
        {
            get
            {
                string subjectstr = "";
                foreach (string item in this.subject)
                {
                    subjectstr += ","+item;
                }
                if (subjectstr.Length == 0)
                    return "";
                else
                    return subjectstr.Substring(1);
            }
            set
            {
                this.subjectSTR = value;
            }
        }
    }
}