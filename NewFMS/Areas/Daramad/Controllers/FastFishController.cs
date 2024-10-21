using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using NewFMS.Models;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using System.Xml;
using System.Net;


namespace NewFMS.Areas.Daramad.Controllers
{
    public class FastFishController : Controller
    {
        //
        // GET: /Daramad/FastFish/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();

        public ActionResult Index(int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            var d=servic.GetDate(IP, out Err);
            result.ViewBag.DateNow = d.fldTarikh;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult GetTypePardakht()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetNahvePardakhtWithFilter("", "", 0, IP, out ErrD).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult CheckSerialFish(string SerialFish)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; string MablaghHoruf = ""; var fldId = 0; long Mablagh = 0;
            string fldShenase_CodeMeli = ""; string FName = ""; string shomareshabt_father = "";
            try
            {
                var FieldName = "fldId";
                if (SerialFish.Length == 26)
                    FieldName = "fldBarcode";
                var k = servicD.GetSodoorFishWithFilter(FieldName, SerialFish, 0, IP, out ErrD).FirstOrDefault();
                if (k == null)
                {
                    MsgTitle = "خطا";
                    Msg = "سریال وارد شده نامعتبر است.";
                    Er = 2;
                }
                else
                {
                    if (servicD.GetPardakhtFishWithFilter("fldFishId", k.fldId.ToString(), 0, IP, out ErrD).Any())
                    {
                        MsgTitle = "خطا";
                        Msg = "فیش موردنظر قبلا پرداخت شده است.";
                        Er = 2;
                    }
                    else if (servicD.GetEbtalWithFilter("fldFishId", k.fldId.ToString(), 0, IP, out ErrD).Any())
                    {
                        MsgTitle = "خطا";
                        Msg = "فیش موردنظر ابطال شده است.";
                        Er = 2;
                    }
                    else
                    {
                        Mablagh = k.fldMablaghAvarezGerdShode;
                        fldId = k.fldId;
                        MablaghHoruf = MyLib.NumberTool.Num2Str(Convert.ToUInt64(k.fldMablaghAvarezGerdShode), 1);

                        var e = servicD.GetElamAvarezWithFilter("fldId", k.fldElamAvarezId.ToString(), (Session["OrganId"]).ToString(), 0, IP, out ErrD).FirstOrDefault();

                        var h = servic.GetAshkhasDetail(e.fldAshakhasID, IP, out Err);
                        fldShenase_CodeMeli = h.fldShenase_CodeMeli;
                        FName = h.Name;
                        shomareshabt_father = h.shomareshabt_father;
                    }
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er,
                MablaghHoruf = MablaghHoruf,
                fldId = fldId,
                Mablagh=Mablagh,
                fldShenase_CodeMeli = fldShenase_CodeMeli,
                FName = FName,
                shomareshabt_father = shomareshabt_father
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SavePardakht(WCF_Daramad.OBJ_PardakhtFish Fish, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var Tarikh = MyLib.Shamsi.Shamsi2miladiDateTime(Fish.fldTarikh);
                var TarikhVariz = MyLib.Shamsi.Shamsi2miladiDateTime(Fish.fldTarikhVariz);
                //ذخیره
                MsgTitle = "ذخیره موفق";
                Msg = servicD.InsertPardakhtFish(Fish.fldFishId, Tarikh, Fish.fldNahvePardakhtId, null, TarikhVariz, Fish.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);

                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                else
                {
                    var company = servicD.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out ErrD).FirstOrDefault();
                    if (company != null)//ارسال به حسابرایان
                    {
                        HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                        var k = h.RecovrySendedFiche(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService)
                            , Fish.fldFishId.ToString(), Tarikh, "");
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "Pardakht daramad: " + k);
                    }
                    company = servicD.GetCompanyWithFilter("fldTitle", "Rasa", 0, IP, out ErrD).FirstOrDefault();
                    if (company != null)//ارسال به رسا
                    {
                        DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter m = new DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter();
                        DataSet.DataSet1.sp_selectFishForKhazaneDataTable mm = new DataSet.DataSet1.sp_selectFishForKhazaneDataTable();
                        m.Fill(mm, Convert.ToInt32(Session["OrganId"]), Fish.fldFishId);
                        RasaKhazane.FishDetial[] FDetial = new RasaKhazane.FishDetial[mm.Rows.Count];

                        RasaKhazane.KhazaneService rasa = new RasaKhazane.KhazaneService();
                        if (mm.Rows.Count > 0)
                        {
                            for (int i = 0; i < mm.Rows.Count; i++)
                            {
                                var code = servicD.GetCodhayeDaramdWithFilter("fldDaramadCode", mm.Rows[i]["fldDaramadCode"].ToString(),0, Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrD).FirstOrDefault();
                                var code_hesab = servicD.GetShomareHesabCodeDaramadWithFilter("fldCodeDaramadId", code.fldId.ToString(), "",FiscalYearId, 0, IP, out ErrD).FirstOrDefault();
                                var hesab = servicD.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId", code_hesab.fldId.ToString(), 0, IP, out ErrD).FirstOrDefault();
                                if (hesab != null)
                                {
                                    FDetial[i] =
                                        new RasaKhazane.FishDetial
                                        {
                                            CodeHesab = hesab.fldHesabId.ToString(),
                                            Mablagh = Convert.ToInt64(mm.Rows[i]["Mablagh"]),
                                            radif = i + 1,
                                            Desc = mm.Rows[i]["fldSharheCodeDaramad"].ToString()
                                        };
                                }
                            }
                            var fish = servicD.GetSodoorFishDetail(Fish.fldFishId, IP, out ErrD);
                            var hesabNum = servic.GetShomareHesabeOmoomiWithFilter("fldid", fish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err).FirstOrDefault();
                            var elamavarez = servicD.GetElamAvarezDetail(fish.fldElamAvarezId, Session["OrganId"].ToString(), IP, out ErrD);
                            string name = "", family = "", codeMeli = "", noeSherkat = "", shenasemeli = "", shSabt = "";
                            var ashkhas = servic.GetAshkhasWithFilter("fldid", elamavarez.fldAshakhasID.ToString(), "", 0, IP, out Err).FirstOrDefault();
                            if (elamavarez.fldNoeShakhs == "0")
                            {
                                var employe = servic.GetEmployeeWithFilter("fldid", ashkhas.fldHaghighiId.ToString(), 0,
                                    Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err).FirstOrDefault();
                                name = employe.fldName;
                                family = employe.fldFamily;
                                codeMeli = employe.fldCodemeli;
                            }
                            else
                            {
                                var hoghoghi = servic.GetAshkhaseHoghoghiWithFilter("fldid", ashkhas.fldHoghoghiId.ToString(), 0, IP, out Err).FirstOrDefault();
                                name = hoghoghi.fldName;
                                noeSherkat = hoghoghi.fldTypeShakhs.ToString();
                                codeMeli = hoghoghi.fldShenaseMelli;
                            }
                            string res = rasa.ReciveFish(company.fldUserNameService, company.fldPassService, Fish.fldFishId.ToString(), hesabNum.fldShomareHesab, FDetial, name, family, codeMeli, fish.fldJamKol, fish.fldMablaghAvarezGerdShode);
                            var result = res.Split('|');
                            if (result[1] != "-1")
                            {
                                servicD.UpdateSendToMali_Fish("SendToMali", true, fish.fldId, IP, out ErrD);
                                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + fish.fldId + "\n");
                            }
                        }

                    }
                    company = servicD.GetCompanyWithFilter("fldTitle", "Fara", 0, IP, out ErrD).FirstOrDefault();
                    if (company != null)//ارسال به فرارایانه
                    {
                        var payType = servicD.GetNahvePardakhtWithFilter("fldid", Fish.fldNahvePardakhtId.ToString(), 0, IP, out ErrD).FirstOrDefault();
                        var fish = servicD.GetSodoorFishDetail(Fish.fldFishId, IP, out ErrD);
                        var fishDetail = servicD.GetSodoorFish_DetailWithFilter("fldfishid", Fish.fldFishId.ToString(), 0, IP, out ErrD).ToList();
                        if (fishDetail.Count > 0)
                        {
                            var codeDaramadi = servicD.GetCodhayeDaramadiElamAvarez(fishDetail[0].fldCodeElamAvarezId, IP, out ErrD);
                            var hesabNum = servic.GetShomareHesabeOmoomiWithFilter("fldid", fish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err).FirstOrDefault();
                            var bank = servic.GetBankWithFilter("fldid", hesabNum.fldBankId.ToString(), 0, IP, out Err).FirstOrDefault();
                            var organ = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                            var elamavarez = servicD.GetElamAvarezDetail(fish.fldElamAvarezId, organ.fldId.ToString(), IP, out ErrD);
                            string name = "", family = "", codeMeli = "", noeSherkat = "", shenasemeli = "", shSabt = "";
                            var ashkhas = servic.GetAshkhasWithFilter("fldid", elamavarez.fldAshakhasID.ToString(), "", 0, IP, out Err).FirstOrDefault();
                            if (elamavarez.fldNoeShakhs == "0")
                            {
                                var employe = servic.GetEmployeeWithFilter("fldid", ashkhas.fldHaghighiId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err).FirstOrDefault();
                                name = employe.fldName;
                                family = employe.fldFamily;
                                codeMeli = employe.fldCodemeli;
                            }
                            else
                            {
                                var hoghoghi = servic.GetAshkhaseHoghoghiWithFilter("fldid", ashkhas.fldHoghoghiId.ToString(), 0, IP, out Err).FirstOrDefault();
                                name = hoghoghi.fldName;
                                noeSherkat = hoghoghi.fldTypeShakhs.ToString();
                                shenasemeli = hoghoghi.fldShenaseMelli;
                                shSabt = hoghoghi.fldShomareSabt;
                            }

                            StringBuilder sb = new StringBuilder();
                            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sb);
                            writer.WriteStartDocument();
                            writer.WriteStartElement("Fishs");
                            writer.WriteStartElement("Fish");//Start FishOut
                            writer.WriteAttributeString("Serial", Fish.fldFishId.ToString());
                            writer.WriteStartElement("ShenaseGhabz");
                            writer.WriteRaw(fish.fldShenaseGhabz.ToString());
                            writer.WriteEndElement();
                            writer.WriteStartElement("ShenasePardakht");
                            writer.WriteRaw(fish.fldShenasePardakht.ToString());
                            writer.WriteEndElement();
                            writer.WriteStartElement("Amount");
                            writer.WriteRaw(fish.fldJamKol.ToString());
                            writer.WriteEndElement();
                            long maliat = 0, avarez = 0;
                            foreach (var item in fishDetail)
                            {
                                codeDaramadi = servicD.GetCodhayeDaramadiElamAvarez(item.fldCodeElamAvarezId, IP, out ErrD);
                                avarez += codeDaramadi.fldAvarezValue;
                                maliat += codeDaramadi.fldMaliyatValue;
                            }
                            writer.WriteStartElement("Tax");
                            writer.WriteRaw(maliat.ToString());
                            writer.WriteEndElement();
                            writer.WriteStartElement("Complications");
                            writer.WriteRaw(avarez.ToString());
                            writer.WriteEndElement();
                            writer.WriteStartElement("Status");
                            writer.WriteRaw("1");//پرداخت شده
                            writer.WriteEndElement();
                            writer.WriteStartElement("IssueDate");
                            writer.WriteRaw(MyLib.Shamsi.Miladi2ShamsiString(fish.fldDate));
                            writer.WriteEndElement();
                            writer.WriteStartElement("PayDate");
                            writer.WriteRaw(Fish.fldTarikh);
                            writer.WriteEndElement();
                            writer.WriteStartElement("PayType");
                            writer.WriteRaw(payType.fldCodePardakht);
                            writer.WriteEndElement();
                            writer.WriteStartElement("TrackingCode");
                            writer.WriteRaw("");
                            writer.WriteEndElement();
                            writer.WriteStartElement("Referencecode");
                            writer.WriteRaw("");
                            writer.WriteEndElement();
                            writer.WriteStartElement("AccountPayDate");
                            writer.WriteRaw("");
                            writer.WriteEndElement();
                            writer.WriteStartElement("AccountNum");
                            writer.WriteRaw(hesabNum.fldShomareHesab);
                            writer.WriteEndElement();
                            writer.WriteStartElement("BankID");
                            writer.WriteRaw(bank.fldInfinitiveBank);
                            writer.WriteEndElement();
                            writer.WriteStartElement("BankName");
                            writer.WriteRaw(hesabNum.fldBankName + "_" + hesabNum.nameShobe);
                            writer.WriteEndElement();
                            writer.WriteStartElement("OrganName");
                            writer.WriteRaw(organ.fldName);
                            writer.WriteEndElement();
                            writer.WriteStartElement("OrganNationalID");
                            writer.WriteRaw(organ.fldShenaseMelli);
                            writer.WriteEndElement();

                            writer.WriteStartElement("Person");
                            writer.WriteStartElement("Type");
                            writer.WriteRaw(elamavarez.fldNoeShakhs + 1);
                            writer.WriteEndElement();
                            writer.WriteStartElement("Name");
                            writer.WriteRaw(name);
                            writer.WriteEndElement();
                            writer.WriteStartElement("Family");
                            writer.WriteRaw(family);
                            writer.WriteEndElement();
                            writer.WriteStartElement("NationalCode");
                            writer.WriteRaw(codeMeli);
                            writer.WriteEndElement();
                            writer.WriteStartElement("CompanyType");
                            writer.WriteRaw(noeSherkat);
                            writer.WriteEndElement();
                            writer.WriteStartElement("NationalID");
                            writer.WriteRaw(shenasemeli);
                            writer.WriteEndElement();
                            writer.WriteStartElement("RegistrationNum");
                            writer.WriteRaw(shSabt);
                            writer.WriteEndElement();
                            writer.WriteEndElement();

                            writer.WriteStartElement("DaramadGroup");
                            //writer.WriteAttributeString("Title", "نوسازی");
                            writer.WriteStartElement("param");
                            writer.WriteStartElement("name");
                            writer.WriteRaw("");
                            writer.WriteEndElement();
                            writer.WriteStartElement("value");
                            writer.WriteRaw("");
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();


                            writer.WriteStartElement("Detail");
                            foreach (var item in fishDetail)
                            {
                                codeDaramadi = servicD.GetCodhayeDaramadiElamAvarez(item.fldCodeElamAvarezId, IP, out ErrD);
                                writer.WriteStartElement("CodeDaramad");
                                writer.WriteAttributeString("Code", codeDaramadi.fldDaramadCode);
                                writer.WriteStartElement("CodeDescription");
                                writer.WriteRaw(codeDaramadi.fldDaramadTitle);
                                writer.WriteEndElement();
                                writer.WriteStartElement("Amount");
                                writer.WriteRaw((codeDaramadi.fldAsliValue - (codeDaramadi.fldAsliValue - codeDaramadi.fldTakhfifAsliValue)/*تخفیف*/).ToString());
                                writer.WriteEndElement();
                                writer.WriteStartElement("Tax");
                                writer.WriteRaw((codeDaramadi.fldMaliyatValue - (codeDaramadi.fldMaliyatValue - codeDaramadi.fldTakhfifMaliyatValue)/*تخفیف*/).ToString());
                                writer.WriteEndElement();
                                writer.WriteStartElement("Complications");
                                writer.WriteRaw((codeDaramadi.fldAvarezValue - (codeDaramadi.fldAvarezValue - codeDaramadi.fldTakhfifAvarezValue)/*تخفیف*/).ToString());
                                writer.WriteEndElement();
                                writer.WriteEndElement();
                                writer.WriteEndElement();

                                writer.WriteEndElement();
                            }
                            writer.WriteEndDocument();
                            writer.Flush();
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.LoadXml(sb.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").ToString());
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + company.fldURL + "?FormType=103&YearID=" + MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(2, 2) + "&XmlFishs=" + xmlDocument.InnerXml + "\n");
                            var request = (HttpWebRequest)WebRequest.Create(company.fldURL + "?FormType=103&YearID=" + MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(2, 2) + "&XmlFishs=" + xmlDocument.InnerXml);
                            var res = request.GetResponse();
                            Stream receiveStream = res.GetResponseStream();
                            // Pipes the stream to a higher level stream reader with the required encoding format. 
                            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                            string result = readStream.ReadToEnd();
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + result + "\n");
                            if (result.Contains("Fishs"))
                            {
                                if (result.Contains("<Status>1</Status>"))
                                {
                                    servicD.UpdateSendToMali_Fish("SendToMali", true, fish.fldId, IP, out ErrD);
                                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + fish.fldId + "\n");
                                }
                            }
                        }
                    }                    
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                servic.InsertWebServiceLog(Msg, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "error ersal daramad: " + Msg + "\n");
                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
