using New_Tutor.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace New_Tutor.DB
{
    public class DBFunction
    {
        Object thisLock = new Object();
        /// <summary>
        /// 取得Connection
        /// </summary>
        /// <param name="sqlConnectionStr">若傳入連線字串則優先使用傳入的字串 無傳入使用預設字串</param>
        /// <returns></returns>
        private SqlConnection getConnection(string sqlConnectionStr = null)
        {
            if (string.IsNullOrEmpty(sqlConnectionStr))
                sqlConnectionStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return new SqlConnection(sqlConnectionStr);
        }
        /// <summary>
        /// 更新註冊資料
        /// </summary>
        /// <param name="register">註冊Model</param>
        /// <param name="LoginName">登入帳號</param>
        /// <param name="type">更新模式(目前針對第二次修改資料圖檔空值不變更現有資料)</param>
        public void Register(Register register, string LoginName, string type)
        {
            SqlConnection dataConnection = getConnection();
            string innerSQL = "";
            if (type == "Firist")
                innerSQL += ", personalStatus = @personalStatus, recommendCode = @recommendCode";
            if (type == "Firist" || !string.IsNullOrEmpty(register.photo))
                innerSQL += ", photo = @photo";
            string SQL = "UPDATE tutorRegister SET degree = @degree, department = @department, email = @email, gender = @gender, grade = @grade, introduction = @introduction," +
                " last_modify_time = @last_modify_time, motivation = @motivation, name = @name, school = @school," +
                " status = @status, sub_department = @sub_department, subject = @subject, SuggestCode = @SuggestCode," +
                " telephone = @telephone,BankCode = @BankCode,BankAcct = @BankAcct, tutor_student_id = @tutor_student_id" + innerSQL +
                " where googleaccount =@googleaccount";

            //kai 無存入 : personal_quality self_expection , subject_extra, teaching_method
            SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
            try
            {
                dataConnection.Open();
                SqlCmd.Parameters.AddWithValue("@degree", (register.degree == null) ? "" : register.degree);
                SqlCmd.Parameters.AddWithValue("@department", (register.department == null) ? "" : register.department);
                SqlCmd.Parameters.AddWithValue("@email", (register.email == null) ? "" : register.email);
                SqlCmd.Parameters.AddWithValue("@gender", (register.gender == null) ? "" : register.gender);
                SqlCmd.Parameters.AddWithValue("@grade", (register.grade == null) ? "" : register.grade);
                SqlCmd.Parameters.AddWithValue("@introduction", (register.introduction == null) ? "" : register.introduction);
                SqlCmd.Parameters.AddWithValue("@last_modify_time", DateTime.Now);
                SqlCmd.Parameters.AddWithValue("@motivation", (register.motivation == null) ? "" : register.motivation);
                SqlCmd.Parameters.AddWithValue("@name", (register.name == null) ? "" : register.name);
                SqlCmd.Parameters.AddWithValue("@school", (register.school == null) ? "" : register.school);
                SqlCmd.Parameters.AddWithValue("@status", "active");
                SqlCmd.Parameters.AddWithValue("@sub_department", (register.sub_department == null) ? "" : register.sub_department);
                SqlCmd.Parameters.AddWithValue("@telephone", (register.telephone == null) ? "" : register.telephone);
                SqlCmd.Parameters.AddWithValue("@tutor_student_id", (register.tutor_student_id == null) ? "" : register.tutor_student_id);
                if (type == "Firist")
                    SqlCmd.Parameters.AddWithValue("@recommendCode", Functions.Functions.GetRandomString(12));//個人推薦碼
                SqlCmd.Parameters.AddWithValue("@SuggestCode", (register.SuggestCode == null) ? "" : register.SuggestCode);//他人推薦碼
                SqlCmd.Parameters.AddWithValue("@BankCode", (register.BankCode == null) ? "" : register.BankCode);//銀行代碼
                SqlCmd.Parameters.AddWithValue("@BankAcct", (register.BankAcct == null) ? "" : register.BankAcct);//銀行帳號
                if (type == "Firist")
                    SqlCmd.Parameters.AddWithValue("@personalStatus", "待付款");
                SqlCmd.Parameters.AddWithValue("@googleaccount", LoginName);
                if (type == "Firist" || !string.IsNullOrEmpty(register.photo))
                    SqlCmd.Parameters.AddWithValue("@photo", (register.photo == null) ? "" : register.photo);
                string subject = "";
                foreach (string value in register.subject)
                {
                    subject += "," + value;
                }
                SqlCmd.Parameters.AddWithValue("@subject", subject.Substring(1));

                SqlCmd.ExecuteNonQuery();
                dataConnection.Close();
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
            finally
            {
                SqlCmd.Cancel();
                dataConnection.Close();
                dataConnection.Dispose();
            }
        }
        /// <summary>
        /// 取得註冊資料
        /// </summary>
        /// <param name="LoginName">登入帳號</param>
        /// <returns></returns>
        public Register GetRegister(string LoginName)
        {
            Register register = new Register();
            SqlConnection dataConnection = getConnection();
            string SQL = "select * from tutorRegister where googleaccount =@googleaccount";
            SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
            try
            {
                dataConnection.Open();
                SqlCmd.Parameters.AddWithValue("@googleaccount", LoginName);
                SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                DataTable dataTable = new DataTable();
                da.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    if (dataTable.Rows[0]["degree"] != null)
                        register.degree = dataTable.Rows[0]["degree"].ToString();
                    if (dataTable.Rows[0]["department"] != null)
                        register.department = dataTable.Rows[0]["department"].ToString();
                    if (dataTable.Rows[0]["email"] != null)
                        register.email = dataTable.Rows[0]["email"].ToString();
                    if (dataTable.Rows[0]["gender"] != null)
                        register.gender = dataTable.Rows[0]["gender"].ToString();
                    if (dataTable.Rows[0]["grade"] != null)
                        register.grade = dataTable.Rows[0]["grade"].ToString();
                    if (dataTable.Rows[0]["introduction"] != null)
                        register.introduction = dataTable.Rows[0]["introduction"].ToString();
                    if (dataTable.Rows[0]["motivation"] != null)
                        register.motivation = dataTable.Rows[0]["motivation"].ToString();
                    if (dataTable.Rows[0]["name"] != null)
                        register.name = dataTable.Rows[0]["name"].ToString();
                    if (dataTable.Rows[0]["school"] != null)
                        register.school = dataTable.Rows[0]["school"].ToString();
                    if (dataTable.Rows[0]["sub_department"] != null)
                        register.sub_department = dataTable.Rows[0]["sub_department"].ToString();
                    if (dataTable.Rows[0]["telephone"] != null)
                        register.telephone = dataTable.Rows[0]["telephone"].ToString();
                    if (dataTable.Rows[0]["tutor_student_id"] != null)
                        register.tutor_student_id = dataTable.Rows[0]["tutor_student_id"].ToString();
                    if (dataTable.Rows[0]["photo"] != null)
                        register.photoName = dataTable.Rows[0]["photo"].ToString();
                    if (dataTable.Rows[0]["subject"] != null)
                    {
                        register.subject = dataTable.Rows[0]["subject"].ToString().Split(',').ToList();
                    }
                    if (dataTable.Rows[0]["last_modify_time"] != null)
                    {
                        register.last_modify_time = (DateTime)dataTable.Rows[0]["last_modify_time"];
                    }
                    if (dataTable.Rows[0]["recommendCode"] != null)
                        register.recommendCode = dataTable.Rows[0]["recommendCode"].ToString();
                    if (dataTable.Rows[0]["SuggestCode"] != null)
                        register.SuggestCode = dataTable.Rows[0]["SuggestCode"].ToString();
                    if (dataTable.Rows[0]["BankAcct"] != null)
                        register.BankAcct = dataTable.Rows[0]["BankAcct"].ToString();
                    if (dataTable.Rows[0]["BankCode"] != null)
                        register.BankCode = dataTable.Rows[0]["BankCode"].ToString();
                    if (dataTable.Rows[0]["personalStatus"] != null)
                        register.personalStatus = dataTable.Rows[0]["personalStatus"].ToString();
                }
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
            finally
            {
                SqlCmd.Cancel();
                dataConnection.Close();
                dataConnection.Dispose();
            }
            return register;
        }
        /// <summary>
        /// 查詢問券資料
        /// </summary>
        /// <param name="subject">科目</param>
        /// <param name="grade">年級</param>
        /// <param name="gender">老師性別</param>
        /// <returns></returns>
        public List<QuestionnaireData> GetQuestionnaireData(string subject, string grade, string gender)
        {

            List<QuestionnaireData> QuestionnaireDatas = new List<QuestionnaireData>();
            SqlConnection dataConnection = getConnection();
            string SQL = "select * from Questionnaire where (status='已發佈' and Commission is null or Commission <> '有需要')  ";
            List<string> subjectList = new List<string>();
            List<string> gradeList = new List<string>();
            if (!string.IsNullOrEmpty(subject))
            {
                subjectList = subject.Substring(1).Split(',').ToList();
                int subjectCount = subjectList.Count;
                SQL = SQL + "and ";
                string innerSql = "";
                for (int i = 0; i < subjectCount; i++)
                {
                    innerSql += "or subject like @subject" + (i + 1) + " ";
                }
                SQL = SQL + "(" + innerSql.Substring(2) +")";
            }
            if (!string.IsNullOrEmpty(grade))
            {
                gradeList = grade.Substring(1).Split(',').ToList();
                int gradeCount = gradeList.Count;
                SQL = SQL + "and grade_Type in (";
                string innerSql = "";
                for (int i = 0; i < gradeCount; i++)
                {
                    innerSql += ", @grade" + (i + 1);
                }
                SQL = SQL + innerSql.Substring(1) + ")";
            }
            if (!string.IsNullOrEmpty(gender))
            {
                SQL = SQL + "and gender_Mamo =@gender_Mamo";
            }
            SqlCommand SqlCmd = new SqlCommand(SQL + "  order by Create_date desc ", dataConnection);
            try
            {
                dataConnection.Open();
                if (subjectList.Count > 0)
                {
                    int subjectCount = subjectList.Count;
                    for (int i = 0; i < subjectCount; i++)
                    {
                        SqlCmd.Parameters.AddWithValue("@subject" + (i + 1), "%" + subjectList[i] + "%");
                    }
                }
                if (gradeList.Count > 0)
                {
                    int gradeCount = gradeList.Count;
                    for (int i = 0; i < gradeCount; i++)
                    {
                        SqlCmd.Parameters.AddWithValue("@grade" + (i + 1), gradeList[i]);
                    }
                }
                if (!string.IsNullOrEmpty(gender))
                {
                    SqlCmd.Parameters.AddWithValue("@gender_Mamo", gender);
                }
                SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                DataTable dataTable = new DataTable();
                da.Fill(dataTable);
                foreach (DataRow item in dataTable.Rows)
                {
                    QuestionnaireData questionnaireData = new QuestionnaireData();
                    questionnaireData.Questionnaire_id = item["Questionnaire_id"].ToString();
                    if (item["subject"] != null)
                        questionnaireData.subject = item["subject"].ToString();
                    if (item["budget"] != null)
                        questionnaireData.budget = item["budget"].ToString();
                    if (item["grade_Type"] != null)
                        questionnaireData.grade_Type = item["grade_Type"].ToString();
                    if (item["grade"] != null)
                        questionnaireData.grade = item["grade"].ToString();
                    if (item["Address"] != null)
                        questionnaireData.Address = item["Address"].ToString();
                    if (item["gender_Mamo"] == null || item["gender_Mamo"].ToString() == "")
                        questionnaireData.gender_Mamo = "不限";
                    else
                        questionnaireData.gender_Mamo = item["gender_Mamo"].ToString();
                    QuestionnaireDatas.Add(questionnaireData);
                }
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
            finally
            {
                SqlCmd.Cancel();
                dataConnection.Close();
                dataConnection.Dispose();
            }
            return QuestionnaireDatas;
        }
        /// <summary>
        /// 查詢問券詳細資料
        /// </summary>
        /// <param name="ID">問券ID</param>
        /// <returns></returns>
        public QuestionnaireData GetQuestionnaireData(string ID)
        {
            SqlConnection dataConnection = getConnection();
            string SQL = "select * from Questionnaire where Questionnaire_id=@Questionnaire_id";
            SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
            QuestionnaireData questionnaireData = new QuestionnaireData();
            try
            {
                dataConnection.Open();
                SqlCmd.Parameters.AddWithValue("@Questionnaire_id", ID);
                SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                DataTable dataTable = new DataTable();
                da.Fill(dataTable);
                foreach (DataRow item in dataTable.Rows)
                {
                    questionnaireData.Questionnaire_id = item["Questionnaire_id"].ToString();
                    questionnaireData.Create_date = (DateTime)item["Create_date"];
                    questionnaireData.Questionnaire_role = item["Questionnaire_role"].ToString();
                    questionnaireData.subject = item["subject"].ToString();
                    questionnaireData.frequency = item["frequency"].ToString();
                    questionnaireData.grade_Type = item["grade_Type"].ToString();
                    if (item["grade"] != null)
                        questionnaireData.grade = item["grade"].ToString();
                    questionnaireData.week_Hours = item["week_Hours"].ToString();
                    questionnaireData.Class_time = item["Class_time"].ToString();
                    questionnaireData.Address = item["Address"].ToString();
                    if (item["Address_mamo"] != null)
                        questionnaireData.Address_mamo = item["Address_mamo"].ToString();
                    questionnaireData.gender = item["gender"].ToString();
                    if (item["gender_Mamo"] == null || item["gender_Mamo"].ToString() == "")
                        questionnaireData.gender_Mamo = "不限";
                    else
                        questionnaireData.gender_Mamo = item["gender_Mamo"].ToString();
                    questionnaireData.budget = item["budget"].ToString();
                    if (item["mamo"] != null)
                        questionnaireData.mamo = item["mamo"].ToString();
                    questionnaireData.role_Name = item["role_Name"].ToString();
                    if (item["Email"] != null)
                        questionnaireData.Email = item["Email"].ToString();
                    questionnaireData.Phone = item["Phone"].ToString();
                }
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
            finally
            {
                SqlCmd.Cancel();
                dataConnection.Close();
                dataConnection.Dispose();
            }
            return questionnaireData;
        }
        /// <summary>
        /// 存取問券資料
        /// </summary>
        /// <param name="data">存入資料</param>
        /// <param name="path">文字LOG存入路徑</param>
        public void saveQuestionnaireData(JObject data, string path)
        {
            lock (thisLock)
            {
                int Seq = 1;
                string seqFileName = "Seq.txt";
                if (System.IO.File.Exists(path + "\\" + seqFileName))
                {
                    using (StreamReader sr = new StreamReader(path + "\\" + seqFileName))
                    {
                        Seq = int.Parse(sr.ReadLine());
                        sr.Close();
                    }
                    using (StreamWriter sw = new StreamWriter(path + "\\" + seqFileName))
                    {
                        Seq++;
                        sw.WriteLine(Seq);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(path + "\\" + seqFileName))
                    {
                        sw.WriteLine(Seq);
                        sw.Close();
                    }
                }

                string insSQL = @"INSERT INTO [dbo].[Questionnaire]([Questionnaire_id], [Create_date], [Questionnaire_role],[grade_Type], [grade], [subject], [frequency], [week_Hours], [Class_time], [Address], [Address_mamo], [gender], [gender_Mamo], [budget], [mamo], [role_Name], [Email], [Phone],[status],[Commission],[choose]) 
	                        VALUES(@Questionnaire_id, @Create_date, @Questionnaire_role,@grade_Type, @grade, @subject, @frequency, @week_Hours, @Class_time, @Address, @Address_mamo, @gender, @gender_Mamo, @budget, @mamo, @role_Name, @Email, @Phone,'待審核',@Commission,@choose)";
                using (SqlConnection Conn = getConnection())
                using (SqlCommand cmd = new SqlCommand())
                {
                    Conn.Open();
                    DateTime nowDate = DateTime.Now;
                    cmd.Connection = Conn;
                    cmd.CommandText = insSQL;
                    cmd.Parameters.Add("Questionnaire_id", SqlDbType.VarChar).Value = nowDate.ToString("yyyyMMdd") + Seq.ToString("000");
                    cmd.Parameters.Add("Create_date", SqlDbType.DateTime).Value = nowDate;
                    cmd.Parameters.Add("Questionnaire_role", SqlDbType.VarChar).Value = data["result"][0]["answer"][0].ToString();
                    string grade_Type = data["result"][1]["answer"][0].ToString();
                    cmd.Parameters.Add("grade_Type", SqlDbType.VarChar).Value = data["result"][1]["answer"][0].ToString();//學生年級
                    string subject = "";
                    int subjectCount = 0;
                    if (grade_Type.IndexOf("國小") >= 0)
                    {
                        cmd.Parameters.Add("grade", SqlDbType.VarChar).Value = data["result"][2]["answer"][0].ToString();//國小年級
                        subjectCount = 5;
                    }
                    else if (grade_Type.IndexOf("國中") >= 0)
                    {
                        cmd.Parameters.Add("grade", SqlDbType.VarChar).Value = data["result"][3]["answer"][0].ToString();//國中年級
                        subjectCount = 6;
                    }
                    else if (grade_Type.IndexOf("高中") >= 0)
                    {
                        cmd.Parameters.Add("grade", SqlDbType.VarChar).Value = data["result"][4]["answer"][0].ToString();//高中年級
                        subjectCount = 7;
                    }
                    else
                    {
                        cmd.Parameters.Add("grade", SqlDbType.VarChar).Value = "";
                        if (grade_Type.IndexOf("學齡前") >= 0)
                            subjectCount = 5;
                        else
                            subjectCount = 8;
                    }
                    for (int i = 0; i < ((JArray)data["result"][subjectCount]["answer"]).Count; i++)
                    {
                        subject = subject + "," + data["result"][subjectCount]["answer"][i].ToString();
                    }
                    cmd.Parameters.Add("subject", SqlDbType.VarChar).Value = subject.Substring(1);//需求科目
                    string frequency = "";
                    for (int i = 0; i < ((JArray)data["result"][9]["answer"]).Count; i++)
                    {
                        frequency = frequency + "," + data["result"][9]["answer"][i].ToString();
                    }
                    cmd.Parameters.Add("frequency", SqlDbType.VarChar).Value = frequency;//一週上課幾次
                    cmd.Parameters.Add("week_Hours", SqlDbType.VarChar).Value = data["result"][10]["answer"][0].ToString();//一次上課幾小時
                    string Class_time = "";
                    for (int i = 0; i < ((JArray)data["result"][11]["answer"]).Count; i++)
                    {
                        Class_time = Class_time + "," + data["result"][11]["answer"][i].ToString();
                    }
                    cmd.Parameters.Add("Class_time", SqlDbType.VarChar).Value = Class_time.Substring(1);//可以上課的時間
                    cmd.Parameters.Add("Address", SqlDbType.VarChar).Value = data["result"][12]["answer"][0].ToString();//上課地點
                    if (((JArray)data["result"][13]["answer"]).Count > 0)
                        cmd.Parameters.Add("Address_mamo", SqlDbType.VarChar).Value = data["result"][13]["answer"][0].ToString();//附近的公車或捷運站
                    else
                        cmd.Parameters.Add("Address_mamo", SqlDbType.VarChar).Value = "";//附近的公車或捷運站
                    cmd.Parameters.Add("gender", SqlDbType.VarChar).Value = data["result"][14]["answer"][0].ToString();//是否需指定老師性別
                    if (((JArray)data["result"][15]["answer"]).Count > 0)
                        cmd.Parameters.Add("gender_Mamo", SqlDbType.VarChar).Value = data["result"][15]["answer"][0].ToString();//老師性別
                    else
                        cmd.Parameters.Add("gender_Mamo", SqlDbType.VarChar).Value = "";//老師性別
                    cmd.Parameters.Add("budget", SqlDbType.VarChar).Value = data["result"][16]["answer"][0].ToString();//預算金額
                    cmd.Parameters.Add("Commission", SqlDbType.VarChar).Value = data["result"][17]["answer"][0].ToString();//需要協助篩選老師嗎
                    if (((JArray)data["result"][18]["answer"]).Count > 0)
                        cmd.Parameters.Add("choose", SqlDbType.VarChar).Value = data["result"][18]["answer"][0].ToString();//希望自己選擇老師嗎 
                    else
                        cmd.Parameters.Add("choose", SqlDbType.VarChar).Value = "";//希望自己選擇老師嗎
                    if (((JArray)data["result"][19]["answer"]).Count > 0)
                        cmd.Parameters.Add("mamo", SqlDbType.VarChar).Value = data["result"][19]["answer"][0].ToString();//其他備註
                    else
                        cmd.Parameters.Add("mamo", SqlDbType.VarChar).Value = "";//其他備註
                    cmd.Parameters.Add("role_Name", SqlDbType.VarChar).Value = data["result"][20]["answer"][0].ToString();//您的稱呼
                    cmd.Parameters.Add("Email", SqlDbType.VarChar).Value = data["result"][21]["answer"][0].ToString();//聯絡 E-mail
                    cmd.Parameters.Add("Phone", SqlDbType.VarChar).Value = data["result"][22]["answer"][0].ToString();//聯絡電話
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 修改問券資料狀態
        /// </summary>
        public void EditQuestionnaireStatus()
        {
            lock (thisLock)
            {
                SqlConnection dataConnection = getConnection();
                string SQL = "select * from Questionnaire where status=@status";
                DateTime editDate = DateTime.Now;
                SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
                try
                {
                    dataConnection.Open();
                    SqlCmd.Parameters.AddWithValue("@status", "已審核");
                    SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);
                    foreach (DataRow item in dataTable.Rows)
                    {
                        string ID = dataTable.Rows[0]["Questionnaire_id"].ToString();
                        string Status = dataTable.Rows[0]["choose"].ToString();
                        switch (Status)
                        {
                            case "希望":
                                Email email = new Email();
                                email.MailTo = dataTable.Rows[0]["Email"].ToString();
                                email.Subject = "自行尋找教師通知";
                                //email.Body = System.Configuration.ConfigurationManager.AppSettings["PaymentUrl"] +"?ID=" + Id;
                                email.Body = "待提供";
                                NewMail(email);
                                break;
                            default:
                                break;
                        }
                        SQL = "update Questionnaire SET status =@NewStatus,Edit_Date =@Edit_Date where Questionnaire_id=@Questionnaire_id";
                        SqlCmd.CommandText = SQL;
                        SqlCmd.Parameters.Clear();
                        SqlCmd.Parameters.AddWithValue("@Questionnaire_id", ID);
                        SqlCmd.Parameters.AddWithValue("@NewStatus", "已發佈");
                        SqlCmd.Parameters.AddWithValue("Edit_Date", editDate);
                        SqlCmd.ExecuteNonQuery();
                    }

                }
                catch (Exception e)
                {
                    string ee = e.ToString();
                }
                finally
                {
                    SqlCmd.Cancel();
                    dataConnection.Close();
                    dataConnection.Dispose();
                }
            }
        }

        /// <summary>
        /// 查詢個人狀態是否為已付款
        /// </summary>
        /// <param name="googleid">登入帳號</param>
        /// <returns></returns>
        public bool CheckpersonalStatus(string googleid)
        {

            bool ans = false;
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sql = "select * from tutorRegister where googleaccount=@googleaccount and personalStatus in('已驗證','舊會員已驗證')";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand SqlCmd = new SqlCommand(sql, conn);
                SqlCmd.Parameters.AddWithValue("@googleaccount", googleid);
                using (SqlDataReader reader = SqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ans = true;
                    }
                    else
                    {
                        ans = false;
                    }

                }
                SqlCmd.Dispose();
            }
            return ans;
        }
        /// <summary>
        /// 檢查推薦碼是否存在
        /// </summary>
        /// <param name="SuggestCode"></param>
        /// <returns></returns>
        public bool checkRecommendCode(string SuggestCode)
        {
            if (string.IsNullOrEmpty(SuggestCode))
                return false;
            bool ans = false;
            SqlConnection conn = getConnection();
            string sql = "select * from tutorRegister where recommendCode=@recommendCode and personalStatus = '已付款'";

            using (conn)
            {
                conn.Open();
                SqlCommand SqlCmd = new SqlCommand(sql, conn);
                SqlCmd.Parameters.AddWithValue("@recommendCode", SuggestCode);
                using (SqlDataReader reader = SqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ans = true;
                    }
                    else
                    {
                        ans = false;
                    }

                }
                SqlCmd.Dispose();
            }
            return ans;
        }
        /// <summary>
        /// 製作信件給待付款會員
        /// </summary>
        public void ReadRegisterToMail(string personalStatus)
        {
            lock (thisLock)
            {
                SqlConnection dataConnection = getConnection();
                string SQL = "select a.Id,b.googleaccount,b.name from AspNetUsers a ,tutorRegister b where a.UserName=b.googleaccount and b.personalStatus=@personalStatus";
                SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
                try
                {
                    dataConnection.Open();
                    SqlCmd.Parameters.AddWithValue("@personalStatus", personalStatus);
                    SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);
                    string googleaccount = "";
                    string EmailBody = "";
                    string newStatus = "";
                    if (dataTable.Rows.Count > 0 && personalStatus == "待付款")
                    {
                        string path = System.AppDomain.CurrentDomain.BaseDirectory + "EmailBody\\ReadRegisterToMail.html";
                        using (StreamReader sr = new StreamReader(path))
                        {
                            EmailBody = sr.ReadToEnd();
                        }
                    }

                    foreach (DataRow item in dataTable.Rows)
                    {
                        Email email = new Email();
                        googleaccount = dataTable.Rows[0]["googleaccount"].ToString();
                        switch (personalStatus)
                        {
                            case "待付款":
                                string Id = dataTable.Rows[0]["Id"].ToString();
                                email.MailTo = googleaccount;
                                email.Subject = "付款通知";
                                //email.Body = System.Configuration.ConfigurationManager.AppSettings["PaymentUrl"] +"?ID=" + Id;
                                email.Body = EmailBody.Replace("@NAME@", dataTable.Rows[0]["name"].ToString()).Replace("@URL@", System.Configuration.ConfigurationManager.AppSettings["PaymentUrl"] + "?ID=" + Id);
                                NewMail(email);
                                SQL = "update tutorRegister SET personalStatus =@personalStatus,PayMethod =@PayMethod where googleaccount=@googleaccount";
                                newStatus = "已通知付款";
                                break;
                            case "待驗證":
                                email.MailTo = System.Configuration.ConfigurationManager.AppSettings["RegisterMailTo"];
                                email.Subject = "待驗證通知";
                                email.Body = "待驗證會員 : " + googleaccount;
                                NewMail(email);
                                SQL = "update tutorRegister SET personalStatus =@personalStatus where googleaccount=@googleaccount";
                                newStatus = "已通知驗證";
                                break;
                            default:
                                break;
                        }
                        SqlCmd.CommandText = SQL;
                        SqlCmd.Parameters.Clear();
                        SqlCmd.Parameters.AddWithValue("@personalStatus", newStatus);
                        SqlCmd.Parameters.AddWithValue("@googleaccount", googleaccount);
                        if (personalStatus == "待付款")
                            SqlCmd.Parameters.AddWithValue("@PayMethod", "家教老師終身開通費");
                        SqlCmd.ExecuteNonQuery();
                    }

                }
                catch (Exception e)
                {
                    string ee = e.ToString();
                }
                finally
                {
                    SqlCmd.Cancel();
                    dataConnection.Close();
                    dataConnection.Dispose();
                }
            }
        }
        /// <summary>
        /// 發信給舊會員登入
        /// </summary>
        public void OldmemberRegisterToMail()
        {
            lock (thisLock)
            {
                SqlConnection dataConnection = getConnection();
                string SQL = "select * from tutorRegister where status='active' and personalStatus=@personalStatus";
                SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
                try
                {
                    dataConnection.Open();
                    SqlCmd.Parameters.AddWithValue("@personalStatus", "舊會員");
                    SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);
                    string LineId = "";
                    foreach (DataRow item in dataTable.Rows)
                    {
                        LineId = item["line_id"].ToString();
                        string OldmemberEmail = item["email"].ToString();
                        Email email = new Email();
                        email.MailTo = OldmemberEmail;
                        email.Subject = "舊會員通知";
                        //email.Body = System.Configuration.ConfigurationManager.AppSettings["PaymentUrl"] +"?ID=" + Id;
                        string EmailBody = "";
                        string path = System.AppDomain.CurrentDomain.BaseDirectory + "EmailBody\\OldmemberRegisterToMail.html";
                        using (StreamReader sr = new StreamReader(path))
                        {
                            EmailBody = sr.ReadToEnd();
                        }
                        email.Body = EmailBody.Replace("@URL@", System.Configuration.ConfigurationManager.AppSettings["OldmemberLoginUrl"] + "?key=" + LineId);
                        NewMail(email);

                        SQL = "update tutorRegister SET personalStatus=@personalStatus,PayMethod=@PayMethod,recommendCode=@recommendCode where line_id=@line_id";
                        SqlCmd.CommandText = SQL;
                        SqlCmd.Parameters.Clear();
                        SqlCmd.Parameters.AddWithValue("@personalStatus", "舊會員已驗證");
                        SqlCmd.Parameters.AddWithValue("@line_id", LineId);
                        SqlCmd.Parameters.AddWithValue("@PayMethod", "家教老師終身開通費");
                        SqlCmd.Parameters.AddWithValue("@recommendCode", Functions.Functions.GetRandomString(12));
                        SqlCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    string ee = e.ToString();
                }
                finally
                {
                    SqlCmd.Cancel();
                    dataConnection.Close();
                    dataConnection.Dispose();
                }
            }
        }
        /// <summary>
        /// 讀取待發信EMAIL 發EMAIL
        /// </summary>
        public void SendMail()
        {
            lock (thisLock)
            {
                SqlConnection dataConnection = getConnection();
                string SQL = "select * from SendMail where status=@status";
                SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
                try
                {
                    dataConnection.Open();
                    SqlCmd.Parameters.AddWithValue("@status", "Padding");
                    SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                    DataTable dataTable = new DataTable();
                    da.Fill(dataTable);
                    foreach (DataRow item in dataTable.Rows)
                    {
                        Email email = new Email();
                        email.MailID = item["MailID"].ToString();
                        email.MailTo = item["MailTo"].ToString();
                        email.Subject = item["Subject"].ToString();
                        email.Body = item["Body"].ToString();
                        string status = Functions.Functions.SendMail(email);
                        SQL = "update SendMail SET status =@status,Modify_time =@Modify_time where MailID=@MailID";
                        SqlCmd.CommandText = SQL;
                        SqlCmd.Parameters.Clear();
                        SqlCmd.Parameters.AddWithValue("@status", status);
                        SqlCmd.Parameters.AddWithValue("@Modify_time", DateTime.Now);
                        SqlCmd.Parameters.AddWithValue("@MailID", email.MailID);
                        SqlCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    string ee = e.ToString();
                }
                finally
                {
                    SqlCmd.Cancel();
                    dataConnection.Close();
                    dataConnection.Dispose();
                }
            }
        }
        /// <summary>
        /// 產生新的MAIL 待發送
        /// </summary>
        /// <param name="email"></param>
        public void NewMail(Email email)
        {
            lock (thisLock)
            {
                SqlConnection dataConnection = getConnection();
                string SQL = "INSERT INTO [dbo].[SendMail]([create_time], [MailTo], [Subject], [Body], [status], [MailID]) " +
                            "VALUES(@create_time, @MailTo, @Subject, @Body, @status, @MailID)";
                SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
                try
                {
                    dataConnection.Open();
                    SqlCmd.Parameters.AddWithValue("@create_time", DateTime.Now);
                    SqlCmd.Parameters.AddWithValue("@MailTo", email.MailTo);
                    SqlCmd.Parameters.AddWithValue("@Subject", email.Subject);
                    SqlCmd.Parameters.AddWithValue("@Body", email.Body);
                    SqlCmd.Parameters.AddWithValue("@status", "Padding");
                    SqlCmd.Parameters.AddWithValue("@MailID", Guid.NewGuid());
                    SqlCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    string ee = e.ToString();
                }
                finally
                {
                    SqlCmd.Cancel();
                    dataConnection.Close();
                    dataConnection.Dispose();
                }
            }
        }

        /// <summary>
        /// 查詢個人推薦碼是否已產生過
        /// </summary>
        /// <param name="recommendCode">推薦碼</param>
        /// <returns></returns>
        public bool CheckrecommendCode(string recommendCode)
        {

            bool ans = false;
            SqlConnection conn = getConnection();
            string sql = "select * from tutorRegister where recommendCode=@recommendCode";

            using (conn)
            {
                conn.Open();
                SqlCommand SqlCmd = new SqlCommand(sql, conn);
                SqlCmd.Parameters.AddWithValue("@recommendCode", recommendCode);
                using (SqlDataReader reader = SqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ans = true;
                    }
                    else
                    {
                        ans = false;
                    }

                }
                SqlCmd.Dispose();
            }
            return ans;
        }
        public List<Parents_QuestionnaireData> ParentsGetTutorData(string gender, string school, string subject)
        {

            List<Parents_QuestionnaireData> ParentsGetTutorDatas = new List<Parents_QuestionnaireData>();
            SqlConnection dataConnection = getConnection();
            string SQL = "select * from tutorRegister where (personalStatus='已驗證' or personalStatus='舊會員已驗證' )  ";
            List<string> subjectList = new List<string>();
            List<string> schoolList = new List<string>();
            if (!string.IsNullOrEmpty(subject))
            {
                subjectList = subject.Substring(1).Split(',').ToList();
                int subjectCount = subjectList.Count;
                SQL = SQL + "and ";
                string innerSql = "";
                for (int i = 0; i < subjectCount; i++)
                {
                    innerSql += "or subject like @subject" + (i + 1) + " ";
                }
                SQL = SQL + "("+innerSql.Substring(2) +")";
            }
            if (!string.IsNullOrEmpty(school))
            {
                //schoolList = school;
                //int schoolCount = schoolList.Count;
                SQL = SQL + " and school = @school ";
                //string innerSql = "";
                //for (int i = 0; i < schoolCount; i++)
                //{
                   
                //}
               
            }
            if (!string.IsNullOrEmpty(gender))
            {
                SQL = SQL + " and gender =@gender ";
            }
            SqlCommand SqlCmd = new SqlCommand(SQL + "  order by Create_time desc ", dataConnection);
            try
            {
                dataConnection.Open();
                if (subjectList.Count > 0)
                {
                    int subjectCount = subjectList.Count;
                    for (int i = 0; i < subjectCount; i++)
                    {
                        SqlCmd.Parameters.AddWithValue("@subject" + (i + 1), "%" + subjectList[i] + "%");
                    }
                }
                if (!string.IsNullOrEmpty(school))
                {
                    //int schoolCount = schoolList.Count;
                    //for (int i = 0; i < schoolCount; i++)
                    //{
                        SqlCmd.Parameters.AddWithValue("@school" ,school);
                    //}
                }
                if (!string.IsNullOrEmpty(gender))
                {
                    SqlCmd.Parameters.AddWithValue("@gender", gender);
                }
                SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                DataTable dataTable = new DataTable();
                da.Fill(dataTable);
                foreach (DataRow item in dataTable.Rows)
                {
                    Parents_QuestionnaireData GetTutorDatas = new Parents_QuestionnaireData();
                    GetTutorDatas.tutor_id = item["tutor_id"].ToString();
                    GetTutorDatas.name = item["name"].ToString();
                    if (item["subject"] != null)
                        GetTutorDatas.subject = item["subject"].ToString();
                    if (item["department"] != null)
                        GetTutorDatas.department = item["department"].ToString();
                    if (item["school"] != null)
                        GetTutorDatas.school = item["school"].ToString();
                    if (item["gender"] != null)
                        GetTutorDatas.gender = item["gender"].ToString();
                    
                    ParentsGetTutorDatas.Add(GetTutorDatas);
                }
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
            finally
            {
                SqlCmd.Cancel();
                dataConnection.Close();
                dataConnection.Dispose();
            }
            return ParentsGetTutorDatas;
        }
        public Parents_QuestionnaireData ParentsGetTutorData(string ID)
        {

            SqlConnection dataConnection = getConnection();
            string SQL = "select * from tutorRegister where tutor_id = @tutor_id ";
            SqlCommand SqlCmd = new SqlCommand(SQL, dataConnection);
            Parents_QuestionnaireData tutorData = new Parents_QuestionnaireData();
            try
            {
                dataConnection.Open();
                SqlCmd.Parameters.AddWithValue("@tutor_id", ID);
                SqlDataAdapter da = new SqlDataAdapter(SqlCmd);
                DataTable dataTable = new DataTable();
                da.Fill(dataTable);
                foreach (DataRow item in dataTable.Rows)
                {
                    tutorData.tutor_id = item["tutor_id"].ToString();
                    tutorData.create_time = (DateTime)item["create_time"];
                    tutorData.name = item["name"].ToString();
                    tutorData.subject = item["subject"].ToString();
                    tutorData.gender = item["gender"].ToString();
                    tutorData.school = item["school"].ToString();
                    if (item["grade"] != null)
                        tutorData.grade = item["grade"].ToString();
                    
                    if (item["department"] != null)
                        tutorData.department = item["department"].ToString();
                    tutorData.sub_department = item["sub_department"].ToString();
                    tutorData.introduction = item["introduction"].ToString();
                    tutorData.motivation = item["motivation"].ToString();
                    if (item["email"] != null)
                        tutorData.email = item["email"].ToString();
                    tutorData.telephone = item["telephone"].ToString();
                }
            }
            catch (Exception e)
            {
                string ee = e.ToString();
            }
            finally
            {
                SqlCmd.Cancel();
                dataConnection.Close();
                dataConnection.Dispose();
            }
            return tutorData;
        }
    }
}