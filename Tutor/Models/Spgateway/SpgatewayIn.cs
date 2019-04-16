using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.Spgateway
{
    public class SpgatewayIn
    {
        public string MerchantID { get; set; }
        public string TradeInfo { get; set; }
        public string TradeSha { get; set; }
        public string Version { get; set; }
    }
}