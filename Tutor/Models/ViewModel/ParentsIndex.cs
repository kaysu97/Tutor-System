using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.ViewModel
{
    public class ParentsIndex
    {
        public ParentsIndex()
        {
            this.ParentsIndexList = new List<Parents_QuestionnaireData>();
            //this.ParentsIndexDetail = new Parents_QuestionnaireData();
          
        }
        public List<Parents_QuestionnaireData> ParentsIndexList { get; set; }
        //public Parents_QuestionnaireData ParentsIndexDetail { get; set; }

       
       
           
       
     }
    //public class ParentsIndexDetail
    //{
    //    public ParentsIndexDetail()
    //    {
    //        this.Questionnaire_id = Questionnaire_id;
    //        this.Create_date = Create_date;
    //        this.school = school;
    //        this.gender = gender;
    //        this.subject = subject;
    //    }
            
    //    public string Questionnaire_id { get; set; }
    //    public DateTime Create_date { get; set; }
    //    public string school { get; set; }
    //    public string gender { get; set; }
    //    public string subject { get; set; }
    //}
}