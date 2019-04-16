using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.Spgateway
{
    public class Spgateway
    {
        [DisplayName("商店代號")]
        public string MerchantID { get; set; }
        [DisplayName("回傳格式")]
        public string RespondType { get; set; }
        [DisplayName("時間戳記")]
        public string TimeStamp { get; set; }
        [DisplayName("串接程式版本")]
        public string Version { get; set; }
        [DisplayName("串接程式版本")]
        public string LangType { get; set; }
        [DisplayName("商店訂單編號")]
        public string MerchantOrderNo { get; set; }
        [DisplayName("訂單金額")]
        public int Amt { get; set; }
        [DisplayName("商品資訊")]
        public string ItemDesc { get; set; }
        [DisplayName("交易限制秒數")]
        public int TradeLimit { get; set; }
        [DisplayName("繳費有效期限")]
        public string ExpireDate { get; set; }
        [DisplayName("支付完成返回商店網址")]
        public string ReturnURL { get; set; }
        [DisplayName("支付通知網址")]
        public string NotifyURL { get; set; }
        [DisplayName("商店取號網址")]
        public string CustomerURL { get; set; }
        [DisplayName("擬付取消返回商店網址")]
        public string ClientBackURL { get; set; }
        [DisplayName("付款人電子信箱")]
        public string Email { get; set; }
        [DisplayName("付款人電子信箱是否開放修改")]
        public decimal EmailModify { get; set; }
        [DisplayName("智付通會員")]
        public decimal LoginType { get; set; }
        [DisplayName("商店備註")]
        public string OrderComment { get; set; }
        [DisplayName("信用卡一次付清啟用")]
        public int CREDIT { get; set; }
        [DisplayName("Google Pay啟用")]
        public int ANDROIDPAY { get; set; }
        [DisplayName("Samsung Pay啟用")]
        public int SAMSUNGPAY { get; set; }
        [DisplayName("信用卡分期付款啟用")]
        public string InstFlag { get; set; }
        [DisplayName("信用卡紅利啟用")]
        public int CreditRed { get; set; }
        [DisplayName("信用卡銀聯卡啟用")]
        public int UNIONPAY { get; set; }
        [DisplayName("WEBATM啟用")]
        public int WEBATM { get; set; }
        [DisplayName("ATM轉帳啟用")]
        public int VACC { get; set; }
        [DisplayName("超商代碼繳費啟用")]
        public int CVS { get; set; }
        [DisplayName("超商條碼繳費啟用")]
        public int BARCODE { get; set; }
        [DisplayName("Pay2go電子錢包啟用")]
        public int P2G { get; set; }
        [DisplayName("物流啟用")]
        public int CVSCOM { get; set; }
        private static object _thisLock = new object();
        void defaultinit()
        {
            lock (_thisLock)
            {
                MerchantID = ConfigurationManager.AppSettings["MerchantID"];
                RespondType = "JSON";
                MerchantOrderNo = "TH" + DateTime.Now.ToString("yyMMddhhHHmmssfff");
                var s = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                TimeStamp = s.ToString();
                Version = "1.4";
                LangType = "zh-tw";
                TradeLimit = 0;
                ReturnURL = ConfigurationManager.AppSettings["SpgatewayReturnURL"];
                NotifyURL = ConfigurationManager.AppSettings["SpgatewayNotifyURL"];
                EmailModify = 0;
                LoginType = 0;
                CREDIT = 0;
                InstFlag = "0";
                CreditRed = 0;
                UNIONPAY = 0;

                CustomerURL = ConfigurationManager.AppSettings["CustomerURL"];
                ANDROIDPAY = 0;
                SAMSUNGPAY = 0;
                WEBATM = 1;
                VACC = 1;
                CVS = 1;
                BARCODE = 1;
                P2G = 0;
                CVSCOM = 0;
            }

        }

        public Spgateway()
        {
            defaultinit();


        }
        public Spgateway(int amt)
        {
            defaultinit();

            ItemDesc = "測試商品";
            Amt = amt;
            // Email = "bb@cc.com";





        }

    }
}