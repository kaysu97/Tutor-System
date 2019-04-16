using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.ViewModel
{
    public class HomeDetail
    {
        public HomeDetail()
        {
            this.QuestionnaireData = new QuestionnaireData();
        }
        public QuestionnaireData QuestionnaireData { get; set; }

    }
}