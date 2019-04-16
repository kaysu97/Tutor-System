using New_Tutor.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace New_Tutor.Controllers
{
    [AllowAnonymous]
    public class TutorApiController : ApiController
    {
        [HttpPost]
        public void Questionnaire(FormDataCollection form)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "QuestionnaireLOG\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = DateTime.Now.ToString("HHmmssfff") + ".txt";
            using (StreamWriter sw = new StreamWriter(path + "\\" + fileName))
            {
                sw.WriteLine("SVID : " + form.Get("svid"));
                sw.WriteLine("HASH : " + form.Get("hash"));
                sw.Close();
            }
            string QueryApiUrl = @"https://www.surveycake.com/webhook/v0/" + form.Get("svid") + "/" + form.Get("hash");
            string result = "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            WebRequest request = WebRequest.Create(QueryApiUrl);
            request.Method = "GET";
            using (var httpResponse = (HttpWebResponse)request.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            using (StreamWriter sw = new StreamWriter(path + "\\" + fileName, true))
            {
                sw.WriteLine("QueryApiUrl result : " + result);
                sw.Close();
            }
            string Ans = Functions.Functions.DecryptStringFromBytes_Aes(result, "243e5630417eadb6", "62105ce834325553");
            using (StreamWriter sw = new StreamWriter(path + "\\" + fileName, true))
            {
                sw.WriteLine("");
                sw.WriteLine("DecryptStringFromBytes_Aes : " + Ans);
                sw.Close();
            }
            try
            {
                Ans = Ans.Substring(0, Ans.LastIndexOf("}") + 1);
                //Ans = System.Web.HttpUtility.UrlEncode(Ans);
                //Ans = ReplaceCode(Ans);
                //Ans = System.Web.HttpUtility.UrlDecode(Ans);
                JObject json = JObject.Parse(Ans);
                DBFunction dBFunction = new DBFunction();
                dBFunction.saveQuestionnaireData(json, path);
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter(path + "\\" + fileName, true))
                {
                    sw.WriteLine("");
                    sw.WriteLine("錯誤 : " + e.StackTrace);
                    sw.Close();
                }
            }
        }
        
    }
}
