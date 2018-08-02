using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsBankasiSanalPos.Models.IsBankasi
{
    public class IsBankasiVm
    {
        public string clientId { get; set; }
        public string amount { get; set; }
        public string oid { get; set; }
        public string okUrl { get; set; }
        public string failUrl { get; set; }
        public string rnd { get; set; }
        public string storekey { get; set; }
        public string storetype { get; set; }

        public string hash { get; set; }
    }
}