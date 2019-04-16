using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models
{
    public class QuestionnaireData
    {
        public string Questionnaire_id { get; set; }

        public DateTime Create_date { get; set; }

        public string Questionnaire_role { get; set; }

        public string grade_Type { get; set; }

        public string grade { get; set; }

        public string subject { get; set; }

        public string frequency { get; set; }

        public string week_Hours { get; set; }

        public string Class_time { get; set; }

        public string Address { get; set; }

        public string Address_mamo { get; set; }

        public string gender { get; set; }

        public string gender_Mamo { get; set; }

        public string budget { get; set; }

        public string mamo { get; set; }

        public string role_Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}