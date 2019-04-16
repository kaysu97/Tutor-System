using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using New_Tutor.Models;
using New_Tutor.Models.ViewModel;
using New_Tutor.DB;

namespace New_Tutor.Controllers
{
    public class ParentsController : Controller
    {
        // GET: Parents
        public ActionResult Index(string gender = null, string school = null, string subject = null)
        {
            
            ParentsIndex model = new ParentsIndex();
            DBFunction DB = new DBFunction();
            model.ParentsIndexList = DB.ParentsGetTutorData(gender, school, subject);
            
            return View(model);
        }
        public ActionResult Detail(string ID)
        {

            DBFunction DB = new DBFunction();
            ParentsDetail model = new ParentsDetail();
            model.ParentsIndexDetail = DB.ParentsGetTutorData(ID);
            return View(model);
           
        }
        public ActionResult Detail2(string ID)
        {


            DBFunction DB = new DBFunction();
            ParentsDetail model = new ParentsDetail();
            model.ParentsIndexDetail = DB.ParentsGetTutorData(ID);
            return View(model);
       
        }
    }
}