using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.ViewModel
{
    public class HomeIndex
    {
        public HomeIndex()
        {
            this.QuestionnaireDatas = new List<QuestionnaireData>();
        }
        public List<QuestionnaireData> QuestionnaireDatas { get; set; }
    }
}