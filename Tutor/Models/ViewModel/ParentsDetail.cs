using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.ViewModel
{
    public class ParentsDetail
    {
        public ParentsDetail()
        {
            
            this.ParentsIndexDetail = new Parents_QuestionnaireData();
        }
       
        public Parents_QuestionnaireData ParentsIndexDetail { get; set; }
    }
}