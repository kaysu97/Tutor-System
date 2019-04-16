using New_Tutor.DB;
using New_Tutor.Functional;
using New_Tutor.Models;
using New_Tutor.Models.Spgateway;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace New_Tutor.Controllers
{
    public class PayPageController : Base
    {
        // GET: PayPage
        /// <summary>
        /// 模擬頁進入點
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {


            // WriteToPaymentLog("Amt", 66);
            Spgateway sp = new Spgateway(500);
            string json = JsonConvert.SerializeObject(sp);
            Dictionary<string, object> j = new Dictionary<string, object>();
            j.Add("OrderId", sp.MerchantOrderNo);
            j.Add("OriginalData", json);
            WriteToPaymentLog(j);
            return View(sp);
        }


        /// <summary>
        /// 送往第三方支付
        /// </summary>
        /// <param name="sp"></param>
        public void Paypay(Spgateway sp)
        {
            try
            {
                string json = JsonConvert.SerializeObject(sp);

                var ss = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                string AES256string = GetAesString(ss);
                JObject jjson = JObject.Parse(json);
                //JObject j = new JObject();
                Dictionary<string, object> j = new Dictionary<string, object>();
                j.Add("OrderId", ss["MerchantOrderNo"].ToString());
                j.Add("SendData", json);
                j.Add("SendDateTime", DateTime.Now);
                WriteToPaymentLog(j);




                string hSha256string = getHashSha256("HashKey=" + ConfigurationManager.AppSettings["HashKey"] + "&" + AES256string + "&" + "HashIV=" + ConfigurationManager.AppSettings["HashIV"]);
                jjson.Add("TradeInfo", AES256string);
                jjson.Add("TradeSha", hSha256string);
                RedirectWithData(jjson, ConfigurationManager.AppSettings["SpgatewayURL"]);
            }
            catch (Exception e)
            {
                WriteData(e.ToString());
            }
            //return View();
        }

        /// <summary>
        /// MAIL呼叫產生付款相關資訊
        /// </summary>
        /// <param name="id">Guid</param>

        [AllowAnonymous]
        public void Pay(string ID)
        {

            try
            {

                // WriteData("ID:"+ID);
                ////
                ///加入記算金額，取姓名，產品名稱
                /// amt,tutor_id,productname
                /// 
                string tutor_id = "1";
                int amt = 500;
                string productname = "古學浩，請付錢";
                string SuggestCode = "";
                string SuggestName = "";
                string Suggestgender = "";
                string Sql = @"select a.Id,name,b.tutor_id,b.PayMethod,b.personalStatus,b.SuggestCode,b.gender,
                             case when b.SuggestCode is null then @amt1 else
                             case when b.SuggestCode = '' then @amt1 else @amt2 end end
                             amt from AspNetUsers a, tutorRegister b where a.UserName = b.googleaccount and b.personalStatus = '已通知付款' and b.PayMethod = '家教老師終身開通費' and a.Id=@Id";
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    conn.Open();
                   // var tx = conn.BeginTransaction();
                    SqlCommand SqlCmd = new SqlCommand(Sql, conn);
                    SqlCmd.Parameters.AddWithValue("@Id", ID);
                    SqlCmd.Parameters.AddWithValue("@amt1", int.Parse(ConfigurationManager.AppSettings["vipamt"].ToString()));
                    int amt2 = int.Parse(ConfigurationManager.AppSettings["vipamt"].ToString()) - int.Parse(ConfigurationManager.AppSettings["discount"].ToString());
                    SqlCmd.Parameters.AddWithValue("@amt2", amt2);
                    using (SqlDataReader reader = SqlCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //name

                            //id
                            tutor_id = reader[2].ToString();
                            //pruduct
                            productname = reader[1].ToString() + " " + reader[3].ToString();
                            //amt
                            amt = int.Parse(reader[7].ToString());
                            SuggestCode = reader[5].ToString();
                            SuggestName = reader[1].ToString();
                            if (reader[6] != null)
                            {
                                Suggestgender = reader[6].ToString();
                                if (Suggestgender == "男")
                                    Suggestgender = "先生";
                                else
                                    Suggestgender = "小姐";
                            }
                        }
                        else
                        {
                            // RedirectToAction("TutorLogin", "Account");
                            // new  RedirectResult("/Account/TutorLogin");
                            throw new Exception("非有效id");
                        }
                    }
                }

                //var ss = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                //var ss = JsonConvert.DeserializeObject<Dictionary<string, object>>(dd);
                Dictionary<string, object> ss = new Dictionary<string, object>();
                Spgateway sp = new Spgateway();

                ss.Add("OrderId", sp.MerchantOrderNo);
                ss.Add("tutor_id", tutor_id);
                ss.Add("amt", amt);


                //ss.Add("OriginalData", json);
                WriteToPaymentLog(ss);

                //新增通知信功能
                string recommendName = "";
                string gender = "";
                string mail = "";
                Sql = @"select name,gender,googleaccount from tutorRegister where recommendCode=@SuggestCode";
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    conn.Open();
                    // var tx = conn.BeginTransaction();
                    SqlCommand SqlCmd = new SqlCommand(Sql, conn);
                    SqlCmd.Parameters.AddWithValue("@SuggestCode", SuggestCode);
                    using (SqlDataReader reader = SqlCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            recommendName = reader[0].ToString();
                            if (reader[1] != null)
                            {
                                gender = reader[1].ToString();
                                if (gender == "男")
                                    gender = "先生";
                                else
                                    gender = "小姐";
                            }
                            mail = reader[2].ToString();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recommendName))
                {
                    DBFunction DB = new DBFunction();
                    Email email = new Email();
                    email.MailTo = mail;
                    email.Subject = "推薦碼被使用通知";
                    email.Body = recommendName+ gender+"您的推薦碼已被"+ SuggestName +Suggestgender+ "使用";
                    DB.NewMail(email);
                }

                sp = setObjVal(sp, ss);
                //sp.ItemDesc = ConfigurationManager.AppSettings["ItmeDesc"];
                sp.ItemDesc = productname;
                sp.Amt = amt;
                Paypay(sp);
            }
            catch (Exception e)
            {
                WriteData(e.ToString());
            }
            // return View();
        }

        [HttpPost]
        public void Return()
        {
            try
            {
                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string data = new StreamReader(req).ReadToEnd();
                var dict = HttpUtility.ParseQueryString(data);
                var json = new JavaScriptSerializer().Serialize(
                                    dict.AllKeys.ToDictionary(k => k, k => dict[k])
                           );
                JObject job = JObject.Parse(json);
                if (ConfigurationManager.AppSettings["MerchantID"] == job["MerchantID"].ToString())
                {
                    string hSha256string = getHashSha256("HashKey=" + ConfigurationManager.AppSettings["HashKey"] + "&" + job["TradeInfo"] + "&" + "HashIV=" + ConfigurationManager.AppSettings["HashIV"]);
                    if (hSha256string == job["TradeSha"].ToString())
                    {
                        string dd = DecryptAesString(job["TradeInfo"].ToString());


                        var ss = JsonConvert.DeserializeObject<Dictionary<string, object>>(dd);

                        string AES256string = GetAesString(ss);
                        JObject jjson = JObject.Parse(json);
                        //JObject j = new JObject();
                        var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(ss["Result"].ToString());
                        Dictionary<string, object> j = new Dictionary<string, object>();
                        j.Add("OrderId", result["MerchantOrderNo"].ToString());
                        j.Add("GetData", dd);
                        j.Add("GetDateTime", DateTime.Now);
                        j.Add("Status", ss["Status"].ToString());
                        WriteToPaymentLog(j);
                       // string ToURL = "";
                        if (ss["Status"].ToString() == "SUCCESS")
                        {
                            JObject jo1 = ReadtoPaymentLog(result["MerchantOrderNo"].ToString());
                            string sql = "update  tutorRegister set [personalStatus]='待驗證' where tutor_id=@tutor_id";
                            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                            {
                                conn.Open();
                                //var tx = conn.BeginTransaction();
                                SqlCommand SqlCmd = new SqlCommand(sql, conn);
                                    SqlCmd.Parameters.AddWithValue("@tutor_id", int.Parse(jo1["tutor_id"].ToString()));
                                SqlCmd.ExecuteNonQuery();
                            }
                            //string Sql = @"select a.Id,name,b.tutor_id,b.PayMethod,b.personalStatus,b.SuggestCode,
                            // case  when b.SuggestCode is null then @amt1 else
                            // case when b.SuggestCode = '' then @amt1 else @amt2 end end
                            // amt from AspNetUsers a, tutorRegister b where a.UserName = b.googleaccount and b.personalStatus = '已通知付款' and b.PayMethod = '家教老師終身開通費' and a.Id=@Id";
                            //using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                            //{
                            //    conn.Open();
                            //    var tx = conn.BeginTransaction();
                            //    SqlCommand SqlCmd = new SqlCommand(Sql, conn, tx);
                            //    SqlCmd.Parameters.AddWithValue("@Id", ID);
                            //    SqlCmd.Parameters.AddWithValue("@amt1", int.Parse(ConfigurationManager.AppSettings["vipamt"].ToString()));
                            //    int amt2 = int.Parse(ConfigurationManager.AppSettings["vipamt"].ToString()) - int.Parse(ConfigurationManager.AppSettings["discount"].ToString());
                            //    SqlCmd.Parameters.AddWithValue("@amt2", amt2);
                            //    using (SqlDataReader reader = SqlCmd.ExecuteReader())
                            //    {
                            //    }
                        }
                        else
                        {
                           // ToURL = ConfigurationManager.AppSettings["FailRUL"];
                        }
                        //JObject jo1 = ReadtoPaymentLog(result["MerchantOrderNo"].ToString());
                        //string stringjson = JsonConvert.SerializeObject(jo1);
                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ToURL);
                        //request.Method = "POST";
                        //request.ContentType = "application/json";
                        //var stream = request.GetRequestStream();
                        //var writer = new StreamWriter(stream);
                        ////放資料
                        //byte[] bytesToPost = System.Text.Encoding.Default.GetBytes(stringjson);
                        //writer.Write(stringjson);
                        //writer.Flush();
                        //var twitpicResponse = (HttpWebResponse)request.GetResponse();
                        //var reader = new StreamReader(twitpicResponse.GetResponseStream());
                        //var objText = reader.ReadToEnd();

                    }



                }

            }
            catch (Exception e)
            {
                WriteData(e.ToString());
            }
        }


    }
}