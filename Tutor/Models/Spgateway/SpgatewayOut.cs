using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace New_Tutor.Models.Spgateway
{
    public class SpgatewayOut
    {
        public string Status { get; set; }
        public string MerchantId { get; set; }
        public string TradeInfo { get; set; }
        public string TradeSha { get; set; }
        public string Version { get; set; }
        public string Key { get; set; }
        public string Vi { get; set; }
    }
}