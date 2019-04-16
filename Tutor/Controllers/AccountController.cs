using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using New_Tutor.DB;
using New_Tutor.Models;

namespace New_Tutor.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return RedirectToLocal(Url.Action("TutorLogin", "Account"));
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 需要使用者已透過使用者名稱/密碼或外部登入進行登入
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 下列程式碼保護兩個因素碼不受暴力密碼破解攻擊。 
            // 如果使用者輸入不正確的代碼來表示一段指定的時間，則使用者帳戶 
            // 會有一段指定的時間遭到鎖定。 
            // 您可以在 IdentityConfig 中設定帳戶鎖定設定
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代碼無效。");
                    return View(model);
            }
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // 不顯示使用者不存在或未受確認
                    return View("ForgotPasswordConfirmation");
                }

                // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                // 傳送包含此連結的電子郵件
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "重設密碼", "請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // 不顯示使用者不存在
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 要求重新導向至外部登入提供者
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 產生並傳送 Token
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 若使用者已經有登入資料，請使用此外部登入提供者登入使用者
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            Session["LoginName"] = loginInfo.Email;
            switch (result)
            {
                case SignInStatus.Success:
                    DB.DBFunction dBFunction = new DB.DBFunction();
                    //createTutor(loginInfo.Email);
                    if (!dBFunction.CheckpersonalStatus(loginInfo.Email))
                        return RedirectToLocal(Url.Action("Register", "Home")); //若個人狀態為非已付款導到填寫基本資料
                    else
                        return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // 若使用者沒有帳戶，請提示使用者建立帳戶
                    //ViewBag.ReturnUrl = returnUrl;
                    //ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    //return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });

                    string LINE_ID = "";
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    var user = new ApplicationUser { UserName = loginInfo.Email, Email = loginInfo.Email };
                    var result2 = await UserManager.CreateAsync(user);
                    if (result2.Succeeded)
                    {
                        result2 = await UserManager.AddLoginAsync(user.Id, info.Login);
                        if (result2.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            if (TempData["linekey"] != null)
                            {
                                LINE_ID = TempData["linekey"].ToString();
                                if (CheckLineidExit(LINE_ID, user.UserName))
                                    BindLingGoogle(LINE_ID, user.UserName);
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                createTutor(user.UserName);
                                return RedirectToLocal(Url.Action("Register", "Home"));
                            }
                        }
                    }
                    AddErrors(result2);
                    ViewBag.ReturnUrl = "Error/index"; //KAI 錯誤頁代補
                    return View();
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 從外部登入提供者處取得使用者資訊
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        [HttpGet]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult TutorLogin(string key)
        {
            Session["Workaround"] = 0;
            return ExternalLogin("Google", Url.Action("Index", "Home"));
        }

        [HttpGet]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult linkLogin(string key)
        {
            Session["Workaround"] = 0;
            ActionResult a;
            TempData["linekey"] = key;
            if (CheckLineidExit(key, ""))
            {
                a = ExternalLogin("Google", Url.Action("Index", "Home"));
            }
            else
            {
                a = View("Lockout");
            }
            //CheckLineidExit(key, "");

            // a=ExternalLogin("Google", null);
            return a;

        }
        /// <summary>
        /// 在舊資料中加入與google帳號的關連
        /// </summary>
        /// <param name="lineid">line id</param>
        /// <param name="googleid">google account</param>
        public void BindLingGoogle(string lineid, string googleid)
        {
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sql = "update tutorRegister set googleaccount=@googleid where line_id=@lineid";


            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand SqlCmd = new SqlCommand(sql, conn);
                SqlCmd.Parameters.AddWithValue("@lineid", lineid);
                SqlCmd.Parameters.AddWithValue("@googleid", googleid);
                SqlCmd.ExecuteNonQuery();
                //using (SqlDataReader reader = SqlCmd.ExecuteReader())
            }

        }

        // <summary>
        // 檢查LineID是否允許綁定
        //</summary>
        //<param name="id">line id</param>
        //<param name="googleid">google account</param>
        //<returns>TRUE(通過)/false(不通過)</returns>
        public bool CheckLineidExit(string id, string googleid)
        {

            bool ans = false;
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sql = "select googleaccount from tutorRegister where line_id=@id";
            string googlesql = "select * from tutorRegister where googleaccount=@googleid";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand SqlCmd = new SqlCommand(sql, conn);
                SqlCmd.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = SqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (string.IsNullOrEmpty(googleid))
                        {
                            if(reader[0]==null || string.IsNullOrEmpty(reader[0].ToString()))
                                ans = true;
                            else
                                ans = false;
                        }
                        else
                        {
                            ans = true;
                        }
                    }
                    else
                    {
                        ans = false;
                    }

                }
                SqlCmd.Dispose();
                if (!string.IsNullOrEmpty(googleid))
                {

                    SqlCmd = new SqlCommand(googlesql, conn);
                    SqlCmd.Parameters.AddWithValue("@googleid", googleid);
                    using (SqlDataReader reader = SqlCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ans = false;
                        }
                        else
                        {
                            ans = true;
                        }

                    }
                }

            }


            return ans;
        }
        private void createTutor(string GoogleAccount)
        {
            DBFunction DB = new DBFunction();
            Register register = DB.GetRegister(GoogleAccount);
            if (string.IsNullOrEmpty(register.googleaccount))
            {
                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string sql = "insert into tutorRegister(create_time,last_modify_time,googleaccount,personalStatus) VALUES(@create_time,@last_modify_time,@googleaccount,'未填寫基本資料')";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    DateTime nowDate = DateTime.Now;
                    conn.Open();
                    SqlCommand SqlCmd = new SqlCommand(sql, conn);
                    SqlCmd.Parameters.AddWithValue("@create_time", nowDate);
                    SqlCmd.Parameters.AddWithValue("@last_modify_time", nowDate);
                    SqlCmd.Parameters.AddWithValue("@googleaccount", GoogleAccount);
                    SqlCmd.ExecuteNonQuery();
                }
            }
        }
        #region Helper
        // 新增外部登入時用來當做 XSRF 保護
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}