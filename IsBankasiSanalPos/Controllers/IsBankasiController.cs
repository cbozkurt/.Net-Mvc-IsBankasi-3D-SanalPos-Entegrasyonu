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
            model.okUrl = "http://<SonucAdresi>/Isbank/Sonuc"; //Bankasnın dönüş Urlsi
            model.failUrl = "http://<SonucAdresi>/Isbank/Sonuc";
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


      
            public ActionResult Sonuc()
            {
                var e = Request.Form.GetEnumerator();
                while (e.MoveNext())
                {
                    String xkey = (String)e.Current;
                    String xval = Request.Form.Get(xkey);
                    Response.Write("<tr><td>" + xkey + "</td><td>" + xval + "</td></tr>");
                }

                String hashparams = Request.Form.Get("HASHPARAMS");
                String hashparamsval = Request.Form.Get("HASHPARAMSVAL");
                String storekey = "XXXX"; //Sizin Storkey Adresiniz
                String paramsval = "";
                int index1 = 0, index2 = 0;
                // hash hesaplamada kullanılacak değerler ayrıştırılıp değerleri birleştiriliyor.
                do
                {
                    index2 = hashparams.IndexOf(":", index1);
                    String val = Request.Form.Get(hashparams.Substring(index1, index2 - index1)) == null ? "" : Request.Form.Get(hashparams.Substring(index1, index2 - index1));
                    paramsval += val;
                    index1 = index2 + 1;
                }
                while (index1 < hashparams.Length);

                //out.println("hashparams="+hashparams+"<br/>");
                //out.println("hashparamsval="+hashparamsval+"<br/>");
                //out.println("paramsval="+paramsval+"<br/>");
                String hashval = paramsval + storekey;         //elde edilecek hash değeri için paramsval e store key ekleniyor. (işyeri anahtarı)
                String hashparam = Request.Form.Get("HASH");

                System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashval);
                byte[] inputbytes = sha.ComputeHash(hashbytes);

                String hash = Convert.ToBase64String(inputbytes); //Güvenlik ve kontrol amaçlı oluşturulan hash
                if (!paramsval.Equals(hashparamsval) || !hash.Equals(hashparam)) //oluşturulan hash ile gelen hash ve hash parametreleri değerleri ile ayrıştırılıp edilen edilen aynı olmalı.
                {
                    Response.Write("<h4>Güvenlik Uyarısı. Sayısal İmza Geçerli Değil</h4>");
                }
                // Ödeme için gerekli parametreler
                String nameval = "xxxx";              //İşyeri kullanıcı adı
                String passwordval = "xxxx";        //İşyeri şifresi
                String clientidval = Request.Form.Get("clientid"); // İşyeri numarası
                String modeval = "P";                   //P olursa gerçek işlem, T olursa test işlemi yapar.
                String typeval = "Auth";           //Auth PreAuth PostAuth Credit Void olabilir.
                String expiresval = Request.Form.Get("Ecom_Payment_Card_ExpDate_Month") + "/" + Request.Form.Get("Ecom_Payment_Card_ExpDate_Year"); //Kredi Kartı son kullanım tarihi mm/yy formatından olmalı
                String cv2val = Request.Form.Get("cv2");    //Güvenlik Kodu
                String totalval = Request.Form.Get("amount"); //Tutar
                String numberval = Request.Form.Get("md");    //Kart numarası olarak 3d sonucu dönem md parametresi kullanılır.
                String taksitval = "";                  //Taksit sayısı peşin satışlar da boş olarak gönderilmelidir.
                String currencyval = "949";           //ytl için
                String orderidval = "";                //Sipariş numarası


                String mdstatus = Request.Form.Get("mdStatus"); // mdStatus 3d işlemin sonucu ile ilgili bilgi verir. 1,2,3,4 başarılı, 5,6,7,8,9,0 başarısızdır.
                if (mdstatus.Equals("1") || mdstatus.Equals("2") || mdstatus.Equals("3") || mdstatus.Equals("4")) //3D Onayı alınmıştır.
                {

                    Response.Write("<h5>3D İşlemi Başarılı</h5><br/>");
                    String cardholderpresentcodeval = "13";
                    String payersecuritylevelval = Request.Form.Get("eci");
                    String payertxnidval = Request.Form.Get("xid");
                    String payerauthenticationcodeval = Request.Form.Get("cavv");




                    String ipaddressval = "";
                    String emailval = "";
                    String groupidval = "";
                    String transidval = "";
                    String useridval = "";

                    //Fatura Bilgileri
                    String billnameval = "";      //Fatur İsmi
                    String billstreet1val = "";   //Fatura adres 1
                    String billstreet2val = "";   //Fatura adres 2
                    String billstreet3val = "";   //Fatura adres 3
                    String billcityval = "";      //Fatura şehir
                    String billstateprovval = ""; //Fatura eyalet
                    String billpostalcodeval = ""; //Fatura posta kodu

                    //Teslimat Bilgileri
                    String shipnameval = "";      //isim
                    String shipstreet1val = "";   //adres 1
                    String shipstreet2val = "";   //adres 2
                    String shipstreet3val = "";   //adres 3
                    String shipcityval = "";      //şehir
                    String shipstateprovval = ""; //eyalet
                    String shippostalcodeval = "";//posta kodu


                    String extraval = "";



                    //Ödeme için gerekli xml yapısı oluşturuluyor

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    System.Xml.XmlDeclaration dec =
                        doc.CreateXmlDeclaration("1.0", "ISO-8859-9", "yes");

                    doc.AppendChild(dec);


                    System.Xml.XmlElement cc5Request = doc.CreateElement("CC5Request");
                    doc.AppendChild(cc5Request);

                    System.Xml.XmlElement name = doc.CreateElement("Name");
                    name.AppendChild(doc.CreateTextNode(nameval));
                    cc5Request.AppendChild(name);

                    System.Xml.XmlElement password = doc.CreateElement("Password");
                    password.AppendChild(doc.CreateTextNode(passwordval));
                    cc5Request.AppendChild(password);

                    System.Xml.XmlElement clientid = doc.CreateElement("ClientId");
                    clientid.AppendChild(doc.CreateTextNode(clientidval));
                    cc5Request.AppendChild(clientid);

                    System.Xml.XmlElement ipaddress = doc.CreateElement("IPAddress");
                    ipaddress.AppendChild(doc.CreateTextNode(ipaddressval));
                    cc5Request.AppendChild(ipaddress);

                    System.Xml.XmlElement email = doc.CreateElement("Email");
                    email.AppendChild(doc.CreateTextNode(emailval));
                    cc5Request.AppendChild(email);

                    System.Xml.XmlElement mode = doc.CreateElement("Mode");
                    mode.AppendChild(doc.CreateTextNode(modeval));
                    cc5Request.AppendChild(mode);

                    System.Xml.XmlElement orderid = doc.CreateElement("OrderId");
                    orderid.AppendChild(doc.CreateTextNode(orderidval));
                    cc5Request.AppendChild(orderid);

                    System.Xml.XmlElement groupid = doc.CreateElement("GroupId");
                    groupid.AppendChild(doc.CreateTextNode(groupidval));
                    cc5Request.AppendChild(groupid);

                    System.Xml.XmlElement transid = doc.CreateElement("TransId");
                    transid.AppendChild(doc.CreateTextNode(transidval));
                    cc5Request.AppendChild(transid);

                    System.Xml.XmlElement userid = doc.CreateElement("UserId");
                    userid.AppendChild(doc.CreateTextNode(useridval));
                    cc5Request.AppendChild(userid);

                    System.Xml.XmlElement type = doc.CreateElement("Type");
                    type.AppendChild(doc.CreateTextNode(typeval));
                    cc5Request.AppendChild(type);

                    System.Xml.XmlElement number = doc.CreateElement("Number");
                    number.AppendChild(doc.CreateTextNode(numberval));
                    cc5Request.AppendChild(number);

                    System.Xml.XmlElement expires = doc.CreateElement("Expires");
                    expires.AppendChild(doc.CreateTextNode(expiresval));
                    cc5Request.AppendChild(expires);

                    System.Xml.XmlElement cvv2val = doc.CreateElement("Cvv2Val");
                    cvv2val.AppendChild(doc.CreateTextNode(cv2val));
                    cc5Request.AppendChild(cvv2val);

                    System.Xml.XmlElement total = doc.CreateElement("Total");
                    total.AppendChild(doc.CreateTextNode(totalval));
                    cc5Request.AppendChild(total);

                    System.Xml.XmlElement currency = doc.CreateElement("Currency");
                    currency.AppendChild(doc.CreateTextNode(currencyval));
                    cc5Request.AppendChild(currency);

                    System.Xml.XmlElement taksit = doc.CreateElement("Taksit");
                    taksit.AppendChild(doc.CreateTextNode(taksitval));
                    cc5Request.AppendChild(taksit);

                    System.Xml.XmlElement payertxnid = doc.CreateElement("PayerTxnId");
                    payertxnid.AppendChild(doc.CreateTextNode(payertxnidval));
                    cc5Request.AppendChild(payertxnid);

                    System.Xml.XmlElement payersecuritylevel = doc.CreateElement("PayerSecurityLevel");
                    payersecuritylevel.AppendChild(doc.CreateTextNode(payersecuritylevelval));
                    cc5Request.AppendChild(payersecuritylevel);

                    System.Xml.XmlElement payerauthenticationcode = doc.CreateElement("PayerAuthenticationCode");
                    payerauthenticationcode.AppendChild(doc.CreateTextNode(payerauthenticationcodeval));
                    cc5Request.AppendChild(payerauthenticationcode);

                    System.Xml.XmlElement cardholderpresentcode = doc.CreateElement("CardholderPresentCode");
                    cardholderpresentcode.AppendChild(doc.CreateTextNode(cardholderpresentcodeval));
                    cc5Request.AppendChild(cardholderpresentcode);

                    System.Xml.XmlElement billto = doc.CreateElement("BillTo");
                    cc5Request.AppendChild(billto);

                    System.Xml.XmlElement billname = doc.CreateElement("Name");
                    billname.AppendChild(doc.CreateTextNode(billnameval));
                    billto.AppendChild(billname);

                    System.Xml.XmlElement billstreet1 = doc.CreateElement("Street1");
                    billstreet1.AppendChild(doc.CreateTextNode(billstreet1val));
                    billto.AppendChild(billstreet1);

                    System.Xml.XmlElement billstreet2 = doc.CreateElement("Street2");
                    billstreet2.AppendChild(doc.CreateTextNode(billstreet2val));
                    billto.AppendChild(billstreet2);

                    System.Xml.XmlElement billstreet3 = doc.CreateElement("Street3");
                    billstreet3.AppendChild(doc.CreateTextNode(billstreet3val));
                    billto.AppendChild(billstreet3);

                    System.Xml.XmlElement billcity = doc.CreateElement("City");
                    billcity.AppendChild(doc.CreateTextNode(billcityval));
                    billto.AppendChild(billcity);

                    System.Xml.XmlElement billstateprov = doc.CreateElement("StateProv");
                    billstateprov.AppendChild(doc.CreateTextNode(billstateprovval));
                    billto.AppendChild(billstateprov);

                    System.Xml.XmlElement billpostalcode = doc.CreateElement("PostalCode");
                    billpostalcode.AppendChild(doc.CreateTextNode(billpostalcodeval));
                    billto.AppendChild(billpostalcode);



                    System.Xml.XmlElement shipto = doc.CreateElement("ShipTo");
                    cc5Request.AppendChild(shipto);

                    System.Xml.XmlElement shipname = doc.CreateElement("Name");
                    shipname.AppendChild(doc.CreateTextNode(shipnameval));
                    shipto.AppendChild(shipname);

                    System.Xml.XmlElement shipstreet1 = doc.CreateElement("Street1");
                    shipstreet1.AppendChild(doc.CreateTextNode(shipstreet1val));
                    shipto.AppendChild(shipstreet1);

                    System.Xml.XmlElement shipstreet2 = doc.CreateElement("Street2");
                    shipstreet2.AppendChild(doc.CreateTextNode(shipstreet2val));
                    shipto.AppendChild(shipstreet2);

                    System.Xml.XmlElement shipstreet3 = doc.CreateElement("Street3");
                    shipstreet3.AppendChild(doc.CreateTextNode(shipstreet3val));
                    shipto.AppendChild(shipstreet3);

                    System.Xml.XmlElement shipcity = doc.CreateElement("City");
                    shipcity.AppendChild(doc.CreateTextNode(shipcityval));
                    shipto.AppendChild(shipcity);

                    System.Xml.XmlElement shipstateprov = doc.CreateElement("StateProv");
                    shipstateprov.AppendChild(doc.CreateTextNode(shipstateprovval));
                    shipto.AppendChild(shipstateprov);

                    System.Xml.XmlElement shippostalcode = doc.CreateElement("PostalCode");
                    shippostalcode.AppendChild(doc.CreateTextNode(shippostalcodeval));
                    shipto.AppendChild(shippostalcode);


                    System.Xml.XmlElement extra = doc.CreateElement("Extra");
                    extra.AppendChild(doc.CreateTextNode(extraval));
                    cc5Request.AppendChild(extra);
                    String xmlval = doc.OuterXml;     //Oluşturulan xml string olarak alınıyor.
                                                      // Ödeme için bağlantı kuruluyor. ve post ediliyor
                    String url = "https://<DonusApiAdresi>/fim/api";
                    System.Net.HttpWebResponse resp = null;
                    try
                    {
                        System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                        string postdata = "DATA=" + xmlval.ToString();
                        byte[] postdatabytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(postdata);
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = postdatabytes.Length;
                        System.IO.Stream requeststream = request.GetRequestStream();
                        requeststream.Write(postdatabytes, 0, postdatabytes.Length);
                        requeststream.Close();

                        resp = (System.Net.HttpWebResponse)request.GetResponse();
                        System.IO.StreamReader responsereader = new System.IO.StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding("ISO-8859-9"));




                        String gelenXml = responsereader.ReadToEnd(); //Gelen xml string olarak alındı.


                        System.Xml.XmlDocument gelen = new System.Xml.XmlDocument();
                        gelen.LoadXml(gelenXml);    //string xml dökumanına çevrildi.

                        System.Xml.XmlNodeList list = gelen.GetElementsByTagName("Response");
                        String xmlResponse = list[0].InnerText;
                        list = gelen.GetElementsByTagName("AuthCode");
                        String xmlAuthCode = list[0].InnerText;
                        list = gelen.GetElementsByTagName("HostRefNum");
                        String xmlHostRefNum = list[0].InnerText;
                        list = gelen.GetElementsByTagName("ProcReturnCode");
                        String xmlProcReturnCode = list[0].InnerText;
                        list = gelen.GetElementsByTagName("TransId");
                        String xmlTransId = list[0].InnerText;
                        list = gelen.GetElementsByTagName("ErrMsg");
                        String xmlErrMsg = list[0].InnerText;
                        if ("Approved".Equals(xmlResponse))
                        {
                            Response.Write("Ödeme başarıyla gerçekleştirildi");
                        }
                        else
                        {
                            Response.Write("Ödemede hata oluştu");
                        }
                        resp.Close();



                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                    }
                    finally
                    {
                        if (resp != null)
                            resp.Close();
                    }

                }
                else
                {
                    Response.Write("3D Onayı alınamadı");
                }
                return View();
        }
    }
}