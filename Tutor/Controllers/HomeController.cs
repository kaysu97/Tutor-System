using New_Tutor.DB;
using New_Tutor.Models;
using New_Tutor.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace New_Tutor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(string subject = null, string grade = null, string gender = null)
        {
            DBFunction DB = new DBFunction();
            HomeIndex model = new HomeIndex();
            model.QuestionnaireDatas = DB.GetQuestionnaireData(subject, grade, gender);
            return View(model);
        }

        public ActionResult Detail(string ID)
        {
            DBFunction DB = new DBFunction();
            HomeDetail model = new HomeDetail();
            model.QuestionnaireData = DB.GetQuestionnaireData(ID);
            return View(model);
        }

        public ActionResult Register(string Email)
        {
            string LoginName = Session["LoginName"].ToString();
            DBFunction DB = new DBFunction();
            Register register = DB.GetRegister(LoginName);
            if(register.personalStatus == "已驗證")
                return RedirectToAction("Index");
            return View(register);
        }
        public ActionResult RegisterView(string act)
        {
            DBFunction DB = new DBFunction();
            string LoginName = Session["LoginName"].ToString();
            Register register = DB.GetRegister(LoginName);
            ViewBag.act = act;
            return View(register);
        }

        public string checkRecommendCode(string Code)
        {
            string result = "";
            DBFunction DB = new DBFunction();
            if (!DB.checkRecommendCode(Code))
                result = "該推薦碼不存在";
            return result;
        }
        public ActionResult Information(Register register, HttpPostedFileBase photoFile,string type)
        {
            if (photoFile != null && photoFile.ContentLength > 0)//使用者有選擇照片檔案
            {
                //存放檔案路徑
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "TutorPhoto";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                //新的檔案名稱
                string strFileName = register.name + "_" + register.tutor_student_id + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(photoFile.FileName);
                //檔案存放在Server
                photoFile.SaveAs(path + @"/" + strFileName);
                register.photo = strFileName;
            }
            DBFunction DB = new DBFunction();
            string LoginName = Session["LoginName"].ToString();
            if (!DB.checkRecommendCode(register.SuggestCode))
                register.SuggestCode = "";
            DB.Register(register, LoginName, type);
            if (type == "NOT_Firist")
                return RedirectToAction("RegisterView", new { act = "View" });
            //else
            //{
            //    Email email = new Email();
            //    email.MailTo = System.Configuration.ConfigurationManager.AppSettings["RegisterMailTo"];
            //    email.Subject = "待驗證通知";
            //    email.Body = "待驗證會員 : "+ LoginName;
            //    DB.NewMail(email);
            //}
            return RedirectToAction("Index");
        }
    }
}