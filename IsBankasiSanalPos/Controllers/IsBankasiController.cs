using IsBankasiSanalPos.Models.IsBankasi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsBankasiSanalPos.Controllers
{
    public class IsBankasiController : Controller
    {
        // GET: IsBankasi
        public ActionResult Index()
        {
            IsBankasiVm model = new IsBankasiVm();

            model.clientId = "700655000100";
            model.amount = "9.95";
            model.oid = "";
            model.okUrl = "http://<SonucAdresi>";
            model.failUrl = "http://<SonucAdresi>";
            model.rnd = DateTime.Now.ToString();
            model.storekey = "TRPS1234";
            model.storetype = "3d";
            String hashstr = model.clientId + model.oid + model.amount + model.okUrl + model.failUrl + model.rnd + model.storekey;
            System.Security.Cryptography.SHA1 sha = new
            System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashstr);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            model.hash = Convert.ToBase64String(inputbytes);
            String description = "";
            String xid = "";
            String lang = "";
            String email = "";
            String userid = "";


            return View(model);
        }
    }
}