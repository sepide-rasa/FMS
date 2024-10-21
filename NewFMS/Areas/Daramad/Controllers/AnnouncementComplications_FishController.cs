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
using System.Threading;
using System.Net.Sockets;
using System.Xml;
using System.Net;
 

namespace NewFMS.Areas.Daramad.Controllers
{ 
    public class AnnouncementComplications_FishController : Controller
    {
        //
        // GET: /Daramad/AnnouncementComplications_Fish/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        WCF_Accounting.AccountingService servic_Acc = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err_Acc = new WCF_Accounting.ClsError();
        public ActionResult Index(string containerId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            //this.GetCmp<TabPanel>(containerId).SetActiveTab(1);
            //this.GetCmp<TabPanel>(containerId).SetActiveTab(0);
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult GroupPayIndex(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.ElamAvarezId = Id;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SabtPardakht(string FishId, string Mablagh, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            var Tarikh = servic_Com.GetDate(IP, out Err_Com).fldTarikh;
            result.ViewBag.FishId = FishId;
            result.ViewBag.Tarikh = Tarikh;
            result.ViewBag.Mablagh = Mablagh;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }

        public ActionResult GetPSPModel()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPspModelWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitleModel });
            return this.Store(q);
        }
        public ActionResult DetailMoadi(int fldAshakhasID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetAshkhasDetail(fldAshakhasID, IP, out Err_Com);
            var MoadiType = 0; var Status = 0;
            if (q.fldHaghighiId != null)
            {
                var haghighi = servic_Com.GetEmployee_DetailWithFilter("fldEmployeeId", q.fldHaghighiId.ToString(), 0, IP, out Err_Com).FirstOrDefault();
               
                if (haghighi != null)
                {
                    Status = 1;
                }
                return Json(new
                {
                    Id = q.fldHaghighiId,
                    Status = Status,
                    MoadiType = MoadiType
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var hoghoghi = servic_Com.GetAshkhaseHoghoghi_DetailWithFilter("fldAshkhaseHoghoghiId", q.fldHoghoghiId.ToString(), 1, IP, out Err_Com).FirstOrDefault();
                MoadiType = 1;
                if (hoghoghi != null)
                {
                    Status = 1;
                }
                return Json(new
                {
                    Id = q.fldHoghoghiId,
                    Status = Status,
                    MoadiType = MoadiType
                }, JsonRequestBehavior.AllowGet);
            }
    
        }
        public ActionResult GetTypePardakht()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetNahvePardakhtWithFilter("", "", 0,IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }

        public ActionResult SavePardakht(WCF_Daramad.OBJ_PardakhtFish Fish,int FiscalYearId)
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
                if (Fish.fldId == 0)
                {
                    Msg = servic.InsertPardakhtFish(Fish.fldFishId, Tarikh, Fish.fldNahvePardakhtId, null,TarikhVariz, Fish.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                    if (company != null)//ارسال به حسابرایان
                    {
                        HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                        var k = h.RecovrySendedFiche(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService)
                            , Fish.fldFishId.ToString(), Tarikh, "");
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "Pardakht daramad: " + k);
                    }
                    company = servic.GetCompanyWithFilter("fldTitle", "Rasa", 0, IP, out Err).FirstOrDefault();
                    if (company != null)//ارسال به رسا
                    {
                        DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter m = new DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter();
                        DataSet.DataSet1.sp_selectFishForKhazaneDataTable mm = new DataSet.DataSet1.sp_selectFishForKhazaneDataTable();
                        m.Fill(mm, Convert.ToInt32(Session["OrganId"]), Fish.fldFishId);
                        RasaKhazane.FishDetial[] FDetial = new RasaKhazane.FishDetial[mm.Rows.Count];
                        
                        RasaKhazane.KhazaneService rasa=new RasaKhazane.KhazaneService();
                        if (mm.Rows.Count > 0)
                        {
                            for (int i = 0; i < mm.Rows.Count; i++)
                            {
                                var code = servic.GetCodhayeDaramdWithFilter("fldId", mm.Rows[i]["Idcode"].ToString(), 0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                var code_hesab = servic.GetShomareHesabCodeDaramadWithFilter("fldCodeDaramadId", code.fldId.ToString(), "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
                                var hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId", code_hesab.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
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
                            var fish = servic.GetSodoorFishDetail(Fish.fldFishId, IP, out Err);
                            var hesabNum = servic_Com.GetShomareHesabeOmoomiWithFilter("fldid", fish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err_Com).FirstOrDefault();
                            var elamavarez = servic.GetElamAvarezDetail(fish.fldElamAvarezId, Session["OrganId"].ToString(), IP, out Err);
                            string name = "", family = "", codeMeli = "", noeSherkat = "", shenasemeli = "", shSabt = "";
                            var ashkhas = servic_Com.GetAshkhasWithFilter("fldid", elamavarez.fldAshakhasID.ToString(), "", 0, IP, out Err_Com).FirstOrDefault();
                            if (elamavarez.fldNoeShakhs == "0")
                            {
                                var employe = servic_Com.GetEmployeeWithFilter("fldid", ashkhas.fldHaghighiId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err_Com).FirstOrDefault();
                                name = employe.fldName;
                                family = employe.fldFamily;
                                codeMeli = employe.fldCodemeli;
                            }
                            else
                            {
                                var hoghoghi = servic_Com.GetAshkhaseHoghoghiWithFilter("fldid", ashkhas.fldHoghoghiId.ToString(), 0, IP, out Err_Com).FirstOrDefault();
                                name = hoghoghi.fldName;
                                noeSherkat = hoghoghi.fldTypeShakhs.ToString();
                                codeMeli = hoghoghi.fldShenaseMelli;
                            }
                            string res = rasa.ReciveFish(company.fldUserNameService, company.fldPassService, Fish.fldFishId.ToString(), hesabNum.fldShomareHesab, FDetial, name, family, codeMeli, fish.fldJamKol, fish.fldMablaghAvarezGerdShode);
                            var result = res.Split('|');
                            if (result[1] != "-1")
                            {
                                servic.UpdateSendToMali_Fish("SendToMali", true, fish.fldId, IP, out Err);
                                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + fish.fldId + "\n");
                            }
                        }
                        
                    }
                    company = servic.GetCompanyWithFilter("fldTitle", "Fara", 0, IP, out Err).FirstOrDefault();
                    if (company != null)//ارسال به فرارایانه
                    {
                        var payType = servic.GetNahvePardakhtWithFilter("fldid", Fish.fldNahvePardakhtId.ToString(), 0, IP, out Err).FirstOrDefault();
                        var fish = servic.GetSodoorFishDetail(Fish.fldFishId, IP, out Err);
                        var fishDetail = servic.GetSodoorFish_DetailWithFilter("fldfishid", Fish.fldFishId.ToString(), 0, IP, out Err).ToList();
                        if (fishDetail.Count > 0)
                        {
                            var codeDaramadi = servic.GetCodhayeDaramadiElamAvarez(fishDetail[0].fldCodeElamAvarezId, IP, out Err);
                            var hesabNum = servic_Com.GetShomareHesabeOmoomiWithFilter("fldid", fish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err_Com).FirstOrDefault();
                            var bank = servic_Com.GetBankWithFilter("fldid", hesabNum.fldBankId.ToString(), 0, IP, out Err_Com).FirstOrDefault();
                            var organ = servic_Com.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com);
                            var elamavarez = servic.GetElamAvarezDetail(fish.fldElamAvarezId, organ.fldId.ToString(), IP, out Err);
                            string name = "", family = "", codeMeli = "", noeSherkat = "", shenasemeli = "", shSabt = "";
                            var ashkhas = servic_Com.GetAshkhasWithFilter("fldid", elamavarez.fldAshakhasID.ToString(), "", 0, IP, out Err_Com).FirstOrDefault();
                            if (elamavarez.fldNoeShakhs == "0")
                            {
                                var employe = servic_Com.GetEmployeeWithFilter("fldid", ashkhas.fldHaghighiId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err_Com).FirstOrDefault();
                                name = employe.fldName;
                                family = employe.fldFamily;
                                codeMeli = employe.fldCodemeli;
                            }
                            else
                            {
                                var hoghoghi = servic_Com.GetAshkhaseHoghoghiWithFilter("fldid", ashkhas.fldHoghoghiId.ToString(), 0, IP, out Err_Com).FirstOrDefault();
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
                                codeDaramadi = servic.GetCodhayeDaramadiElamAvarez(item.fldCodeElamAvarezId, IP, out Err);
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
                                codeDaramadi = servic.GetCodhayeDaramadiElamAvarez(item.fldCodeElamAvarezId, IP, out Err);
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
                                    servic.UpdateSendToMali_Fish("SendToMali", true, fish.fldId, IP, out Err);
                                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + fish.fldId + "\n");
                                }
                            }
                        }
                    } 
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                    }
                }
                else
                {
                    Msg = servic.UpdatePardakhtFish(Fish.fldId, Fish.fldFishId, Tarikh, Fish.fldNahvePardakhtId, null,TarikhVariz, Fish.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                    }
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                servic_Com.InsertWebServiceLog(Msg, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com);
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
        public ActionResult EbtalPardakht(int FishId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //ذخیره
                MsgTitle = "عملیات موفق";
                Msg = "ابطال پرداخت با موفقیت انجام شد.";
                var HaveAccFile = servic_Acc.GetDocumentRecord_DetailsWithFilter("FishId", FishId.ToString(), 1, IP, out Err_Acc).FirstOrDefault();
                if (HaveAccFile != null)
                {
                    return Json(new
                    {
                        Msg = "برای فیش مورد نظر سند ثبت شده و امکان ابطال پرداخت وجود ندارد.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                servic.DeletePcPosTransaction(FishId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                servic.DeletePardakhtFish(FishId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);                
                var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                if (company != null)//ارسال به حسابرایان
                {
                    HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                    var k = h.VitiationRecovrySendedFiche(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService), FishId.ToString());
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "Ebtal Pardakht daramad: " + k);
                }
                company = servic.GetCompanyWithFilter("fldTitle", "Fara", 0, IP, out Err).FirstOrDefault();
                if (company != null)//ارسال به فرا
                {
                    //درحال پیاده سازی
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int FishId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPardakhtFishWithFilter("fldFishId", FishId.ToString(), 1, IP, out Err).FirstOrDefault();
            string TarikhVariz = "";

            if (q != null)
            {
                if(q.fldTarikhVariz==null ||  q.fldTarikhVariz=="" )
                {
                    if (q.fldNahvePardakhtId == 9 || q.fldNahvePardakhtId == 10)
                    {
                        TarikhVariz = MyLib.Shamsi.ShamsiAddDay(q.fldTarikh,1);
                    }
                    else
                    {
                        TarikhVariz = q.fldTarikh;
                    }
                }
                else
                {
                    TarikhVariz = q.fldTarikhVariz;
                }
                return Json(new
                {
                    fldId = q.fldId,
                    fldFishId = q.fldFishId,
                    fldNahvePardakhtId = q.fldNahvePardakhtId.ToString(),
                    fldTarikh = q.fldTarikh,
                    fldTarikhVariz = TarikhVariz,
                    fldDateVariz=q.fldDateVariz,
                    fldDesc = q.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                fldId = 0,
                fldFishId = 0,
                fldNahvePardakhtId = 0,
                fldTarikh = "",
                fldDesc = ""
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTarikh(string Tarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var date=MyLib.Shamsi.ShamsiAddDay(Tarikh,1);
            return Json(new
            {
                date = date
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Print(int FishId, int? ElamAvarezId)
        {
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            if (ElamAvarezId == null)
            {
                ElamAvarezId = servic.GetSodoorFishDetail(FishId, IP, out Err).fldElamAvarezId;
            }
            result.ViewBag.FishId = FishId;
            result.ViewBag.ElamAvarezId = ElamAvarezId;
            return result;
        }

        public ActionResult GeneratePDFFish(int FishId, int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter logo = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptFishTableAdapter Fish = new DataSet.DataSet1TableAdapters.spr_RptFishTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptMablaghDaramdTableAdapter CodeDaramd = new DataSet.DataSet1TableAdapters.spr_RptMablaghDaramdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblPcPosTransactionSelectTableAdapter PCPos = new DataSet.DataSet1TableAdapters.spr_tblPcPosTransactionSelectTableAdapter();
                dt.EnforceConstraints = false;
                PCPos.Fill(dt.spr_tblPcPosTransactionSelect, "fldFishId", FishId.ToString(), 1);
                var PCPosCount = dt.spr_tblPcPosTransactionSelect.Rows.Count;
                var E= servic.GetElamAvarezDetail(ElamAvarezId, (Session["OrganId"]).ToString(), IP, out Err);
                var OrganId =E.fldOrganId;
                var Organ = servic_Com.GetOrganizationDetail(OrganId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com);
                logo.Fill(dt_Com.spr_tblFileSelect, "fldId", Organ.fldFileId.ToString(), 1);
                Fish.Fill(dt.spr_RptFish,FishId);
                CodeDaramd.Fill(dt.spr_RptMablaghDaramd,FishId);

                int? FileId = 0;
                if (E.fldDaramadGroupId == null)
                {
                    var k = servic.GetGozareshatFileWithFilter("fldGozareshatId","1", (Session["OrganId"]).ToString(), 0, IP, out Err).FirstOrDefault();
                    if (k != null)
                        FileId = k.fldReportFileId;
                }
                else
                {
                    var k = servic.GetPatternFish_DaramadGroupWithFilter("fldDaramadGroupId", E.fldDaramadGroupId.ToString(), 0, IP, out Err).FirstOrDefault();
                    var kk = servic.GetPatternFishWithFilter("fldId", k.fldPatternFishId.ToString(), 0, IP, out Err).FirstOrDefault();
                    FileId = kk.fldFileId;
                }

                var reportFile = servic_Com.GetFileWithFilter("fldId", FileId.ToString(), 1, IP, out Err_Com).FirstOrDefault();

                

                FastReport.Report Report = new FastReport.Report();
                MemoryStream m = new MemoryStream(reportFile.fldImage);
                Report.Load(m);
                //Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\DaramadFish.frx");
                Report.RegisterData(dt, "rasaFMSDaramad");
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
                string tarikh = servic_Com.GetDate(IP, out Err_Com).fldTarikh;
                string sal = tarikh.Substring(0, 4);
                string TaTarikh = "";
                if (MyLib.Shamsi.Iskabise(Convert.ToInt32(sal)))
                    TaTarikh = sal + "/12/30";
                else
                    TaTarikh = sal + "/12/29";
                Report.SetParameterValue("AzTarikh", sal + "/01/01");
                Report.SetParameterValue("TaTarikh", TaTarikh);
                Report.SetParameterValue("Mohlat", "مهلت پرداخت فیش تا پایان سال "+sal+"میباشد و بعد از آن فاقد اعتبار است.");
                Report.SetParameterValue("OrganName", Organ.fldName);
                Report.SetParameterValue("CodeSh", ElamAvarezId);
                Report.SetParameterValue("PCPosCount", PCPosCount);

                if (E.fldDaramadGroupId != null)
                {
                    var Params = servic.GetDaramadGroup_ParametrWithFilter("fldDaramadGroupId", E.fldDaramadGroupId.ToString(), 0, IP, out Err).ToList();
                    foreach (var item in Params)
                    {
                        var ParamValue = servic.GetDaramadGroup_ParametrValuesWithFilter("fldParametrGroupDaramadId", item.fldId.ToString(), 0, IP, out Err).Where(l => l.fldElamAvarezId == ElamAvarezId).FirstOrDefault();
                        if (ParamValue != null)
                            Report.SetParameterValue(item.fldEnName, ParamValue.fldValue);
                    }
                }
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
               // pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintDaraei(int FishId, int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.FishId = FishId;
            result.ViewBag.ElamAvarezId = ElamAvarezId;
            return result;
        }

        public ActionResult PSPModels(int state, int FishId, string ShGhabz, string ShPardakht, int Price, string Sheba)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.state = state;
            result.ViewBag.FishId = FishId;
            result.ViewBag.ShGhabz = ShGhabz;
            result.ViewBag.ShPardakht = ShPardakht;
            result.ViewBag.Price = Price;
            result.ViewBag.Sheba = Sheba;
            return result;
        }

        public ActionResult GeneratePDFPrintDaraei(int FishId, int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                servic.InsertFactor(FishId, "", Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err);
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_tblFactorSelectTableAdapter Factor = new DataSet.DataSet1TableAdapters.spr_tblFactorSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptForooshande_KharidarInfoTableAdapter Forooshandeh = new DataSet.DataSet1TableAdapters.spr_RptForooshande_KharidarInfoTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptForooshande_KharidarInfo1TableAdapter Kharidar = new DataSet.DataSet1TableAdapters.spr_RptForooshande_KharidarInfo1TableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptSooratHesabForooshTableAdapter SooratHesab = new DataSet.DataSet1TableAdapters.spr_RptSooratHesabForooshTableAdapter();
                dt.EnforceConstraints = false;
                Factor.Fill(dt.spr_tblFactorSelect, "fldFishId", FishId.ToString(), 1);
                Kharidar.Fill(dt.spr_RptForooshande_KharidarInfo1,"Kharidar", FishId.ToString());
                Forooshandeh.Fill(dt.spr_RptForooshande_KharidarInfo, "Forooshande", FishId.ToString());
                SooratHesab.Fill(dt.spr_RptSooratHesabForoosh,ElamAvarezId);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptDaramadValueAdd.frx");
                Report.RegisterData(dt, "rasaFMSDaramad");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FullSearchFish(string AzTarikh, string TaTarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Daramad.OBJ_FishSaderShode> data = null;
             data = servic.GetFishSaderShodeWithFilter("", "", AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 100, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();

            return Json(new { data = data.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadFish(StoreRequestParameters parameters,string AzTarikh,string TATarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_FishSaderShode> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_FishSaderShode> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldElamAvarezId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldElamAvarezId";
                            break;
                        case "fldNameShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameShakhs";
                            break;
                        case "fldTypeAvarez":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeAvarez";
                            break;
                        case "fldNoeShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeShakhs";
                            break;
                        case "fldNationalCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldFather_ShomareSabt":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFather_ShomareSabt";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldShenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseGhabz";
                            break;
                        case "fldShenasePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenasePardakht";
                            break;
                        case "fldJamKol":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldJamKol";
                            break;
                        case "fldMablaghAvarezGerdShode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghAvarezGerdShode";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetFishSaderShodeWithFilter(field, searchtext, AzTarikh, TATarikh, Convert.ToInt32(Session["OrganId"]),100, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
                    else
                        data = servic.GetFishSaderShodeWithFilter(field, searchtext, AzTarikh, TATarikh, Convert.ToInt32(Session["OrganId"]),100, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetFishSaderShodeWithFilter("", "", AzTarikh, TATarikh, Convert.ToInt32(Session["OrganId"]),100, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Daramad.OBJ_FishSaderShode> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadFishElamAvarez(StoreRequestParameters parameters, string ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_FishSaderShode> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_FishSaderShode> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldElamAvarezId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldElamAvarezId";
                            break;
                        case "fldNameShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameShakhs";
                            break;
                        case "fldTypeAvarez":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeAvarez";
                            break;
                        case "fldNoeShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeShakhs";
                            break;
                        case "fldNationalCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldFather_ShomareSabt":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFather_ShomareSabt";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldShenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseGhabz";
                            break;
                        case "fldShenasePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenasePardakht";
                            break;
                        case "fldJamKol":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldJamKol";
                            break;
                        case "fldMablaghAvarezGerdShode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghAvarezGerdShode";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetFishSaderShodeWithFilter(field, searchtext, ElamAvarezId, "", Convert.ToInt32(Session["OrganId"]), 100, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
                    else
                        data = servic.GetFishSaderShodeWithFilter(field, searchtext, ElamAvarezId, "", Convert.ToInt32(Session["OrganId"]), 100, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetFishSaderShodeWithFilter("PcPos", ElamAvarezId,"", "", Convert.ToInt32(Session["OrganId"]), 100, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !oValue.ToString().Contains(value.ToString());
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Daramad.OBJ_FishSaderShode> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult PcPosPay(int FishId, string ShGhabz, string ShPardakht, long Price, int PspId, int state, List<WCF_Daramad.OBJ_FishSaderShode> FishVal)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = ""; string MsgTitle = ""; byte Er = 0; int Id = 0;
            try
            {
                var sherkat = servic.GetPspModelWithFilter("fldId", PspId.ToString(), 0, IP, out Err).FirstOrDefault();
                var pcposinfo = servic.GetPcPosInfoWithFilter("fldPspId", sherkat.fldId.ToString(), Session["OrganId"].ToString(), 0, IP, out Err).FirstOrDefault();
                var PcPosIp = servic.GetPcPosIPWithFilter("fldPcPosInfoId", pcposinfo.fldId.ToString(), Session["OrganId"].ToString(), 1, IP, out Err).FirstOrDefault();
               // var User = servic_Com.GetUserWithFilter("fldUserName", Session["Username"].ToString(), "", 1, IP, out Err_Com).FirstOrDefault();
                //var PcPosUser = servic.GetPcPosUserWithFilter("fldPosIpId", User.fldId.ToString(), 1, IP, out Err).FirstOrDefault();
                if (state == 1)
                {
                    Id = servic.InsertPcPosTransaction(FishId, Price, false, "", ShGhabz, ShPardakht, "", PcPosIp.fldSerial, "", "", "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            Msgtitle = "خطا",
                            Er = 1
                        });
                    }
                    Session["PcPosTransActionId"] = Id.ToString();
                }
                else if (state == 2)
                {
                    var TransActionIds = "";
                    var TransFishIds = "";
                    foreach (var item in FishVal)
                    {
                        Id = servic.InsertPcPosTransaction(item.fldId, Convert.ToInt64(item.fldMablaghAvarezGerdShode), false, "", item.fldShenaseGhabz, item.fldShenasePardakht, "", PcPosIp.fldSerial, "", "", "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                Msgtitle = "خطا",
                                Er = 1
                            });
                        }
                        TransActionIds = TransActionIds + "," + Id.ToString();
                        TransFishIds = TransFishIds + "," + item.fldId.ToString();
                    }
                    Session["PcPosTransActionId"] = TransActionIds;
                    Session["TransFishIds"] = TransFishIds;
                }

                if (sherkat.fldPspId == 1)/*شهر*/
                {
                    PAX.PCPOS.ActiveX objDOM = new PAX.PCPOS.ActiveX();
                    objDOM.InitSocket(PcPosIp.fldIp);
                    JavaScriptSerializer json = new JavaScriptSerializer();
                    var r = objDOM.BillPayment(Id.ToString(), ShGhabz, ShPardakht, true);
                    NewFMS.Helper.Response res = json.Deserialize<NewFMS.Helper.Response>(r);
                    var Currdate = servic_Com.GetDate(IP, out Err_Com);
                    var TarikhVariz = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(Currdate.fldTarikh, 1));
                    if (res.Success && res.TransactionInfo.ResponseCode == "00")
                    {
                        Msg = servic.UpdatePcPosTransaction_Status(Convert.ToInt32(res.AdditionalData), res.TransactionInfo.Stan,
                            res.PosInformation.TerminalId, PcPosIp.fldSerial, res.TransactionInfo.PAN, res.TransactionInfo.DateTime, "شماره شناسایی در سمت بانک:" + res.TransactionInfo.SVC + " شماره ارجاع تراکنش:" + res.TransactionInfo.RRN
                            + " شماره دارنده ترمینال:" + res.PosInformation.MerchantId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                Msgtitle = "خطا در پرداخت شماره " + res.AdditionalData,
                                Er = 1
                            });
                        }
                        Msg = servic.InsertPardakhtFish(FishId, Currdate.fldDateTime, 9, null,TarikhVariz, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                Msgtitle = "خطا در ثبت پرداخت",
                                Er = 1
                            });
                        }
                        return Json(new { Msg = "پرداخت با موفقیت انجام شد و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                    }
                    else
                    {
                        return Json(new { Msg = "پرداخت انجام نشد. کد خطا: " + res.ErrorCode, MsgTitle = "عملیات ناموفق", Er = 1 });
                    }
                }
                else if (sherkat.fldPspId == 2 )/*سامان */
                {
                    long[] P = new long[10] ;
                    string ErrSH = "";
                    string mm="";
                    if (state == 2)
                    {
                        foreach (var item in FishVal)
                        {
                            var s = servic.GetPSPModel_ShomareHesabWithFilter("fldShHesabId", item.fldShomareHesabId.ToString(), 0, IP, out Err).Where(l => l.fldPSPModelId == Convert.ToInt32(PspId)).FirstOrDefault();
                            if (s != null)
                                P[s.fldOrder] = item.fldMablaghAvarezGerdShode;
                            else
                                ErrSH = ErrSH + "،" + item.fldShomareHesab;
                        }
                        if(ErrSH!="")
                            return Json(new { Msg = "شماره حساب " + ErrSH + " برای بانک سامان ثبت نشده است.", MsgTitle = "خطا", Er = 1, sherkat = sherkat.fldPspId });

                        for (int i = 1; i < 10; i++) 
                            mm = mm + P[i] + ",";
                    }
                    return Json(new { Msg = "", MsgTitle = "", Er = 0, sherkat = sherkat.fldPspId, Price = mm });
                }
                else if (sherkat.fldPspId == 3)/*  پارسیان*/
                {
                    string mm = "";
                    string sh = "";
                    if (state == 2)
                    {
                        foreach (var item in FishVal)
                        {
                            mm = mm  +  item.fldMablaghAvarezGerdShode+",";
                            sh = sh  + item.fldShomareSheba+"," ;
                        }
                    }
                    return Json(new { Msg = "", MsgTitle = "", Er = 0, sherkat = sherkat.fldPspId, Price = mm ,Sheba=sh});

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SamanPcPosPay(string PspId, int FishId, string ShGhabz, string ShPardakht, string Price,string GpPrice, int state, List<WCF_Daramad.OBJ_FishSaderShode> FishVal)
        {
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
         
            var PayTypeId = 0;
            var radifId = 0;
            var AccNumId = 0;
            var PayMethodId = 0;

            var PayTypeVal = "";
            var radifVal = "";
            var AccNumVal = "";
            var PayMethodVal = "";


            var Pos = servic.GetPcPosInfoWithFilter("fldPspId", PspId, Session["OrganId"].ToString(), 0, IP, out Err).FirstOrDefault();
            var PosIp = servic.GetPcPosIPWithFilter("fldPcPosInfoId", Pos.fldId.ToString(), Session["OrganId"].ToString(), 0, IP, out Err).FirstOrDefault();
            var param = servic.GetPcPosParametrWithFilter("fldPspId", PspId,Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                foreach (var item in param)
                {
                    if (item.fldEnName == "PayType")
                    {
                        PayTypeId = item.fldId;
                    }
                    else if (item.fldEnName == "radif")
                    {
                        radifId = item.fldId;
                    }
                    else if (item.fldEnName == "AccountNum")
                    {
                        AccNumId = item.fldId;
                    }
                    else if (item.fldEnName == "PayMethod")
                    {
                        PayMethodId = item.fldId;
                    }
                }
                PayTypeVal = servic.GetPcPosParam_DetailWithFilter("fldPcPosParamId", PayTypeId.ToString(), 0, IP, out Err).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                radifVal = servic.GetPcPosParam_DetailWithFilter("fldPcPosParamId", radifId.ToString(), 0, IP, out Err).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                AccNumVal = servic.GetPcPosParam_DetailWithFilter("fldPcPosParamId", AccNumId.ToString(), 0, IP, out Err).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
                PayMethodVal = servic.GetPcPosParam_DetailWithFilter("fldPcPosParamId", PayMethodId.ToString(), 0, IP, out Err).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
            
                var q = servic_Com.GetOrganizationWithFilter("fldId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Com).FirstOrDefault();

             
                result.ViewBag.fishid = FishId;
            result.ViewBag.ip = PosIp.fldIp;
            result.ViewBag.mablagh = Price;
            result.ViewBag.ShGhabz = ShGhabz;
            result.ViewBag.ShPardakht = ShPardakht;
            result.ViewBag.organ = q.fldName;
            result.ViewBag.PayTypeVal = PayTypeVal;
            result.ViewBag.radifVal = radifVal;
            result.ViewBag.AccNumVal = AccNumVal;
            result.ViewBag.PayMethodVal = PayMethodVal;
            result.ViewBag.state = state;
            result.ViewBag.GpPrice = GpPrice;

            return result;
        }
        public ActionResult SamanPosVerify(int fishid, string TerminalID, string ResponseCode,
            string SerialId, string TxnDate, string RRN, string ResponseDescription, int state,int FiscalYearId)
        {
            string Msg = ""; string MsgTitle = ""; byte Er = 0; int Id = 0;
            try
            {
                if (ResponseCode == "00")
                {
                    var Currdate = servic_Com.GetDate(IP, out Err_Com);
                    var TarikhVariz = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(Currdate.fldTarikh, 1));
                    if (state == 1)
                    {
                        Msg = servic.UpdatePcPosTransaction_Status(Convert.ToInt32(Session["PcPosTransActionId"]), RRN, TerminalID, SerialId, "", TxnDate, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                Msgtitle = "خطا در پرداخت شماره " + Session["PcPosTransActionId"].ToString(),
                                Er = 1
                            });
                        }
                        Msg = servic.InsertPardakhtFish(fishid, Currdate.fldDateTime, 9, null,TarikhVariz, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                        if (company != null)//ارسال به حسابرایان
                        {
                            HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                            var k = h.RecovrySendedFiche(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService)
                                , fishid.ToString(), Currdate.fldDateTime, "");
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "Pardakht daramad: " + k);
                        }
                        company = servic.GetCompanyWithFilter("fldTitle", "Rasa", 0, IP, out Err).FirstOrDefault();
                        if (company != null)//ارسال به رسا
                        {
                            DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter m = new DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter();
                            DataSet.DataSet1.sp_selectFishForKhazaneDataTable mm = new DataSet.DataSet1.sp_selectFishForKhazaneDataTable();
                            m.Fill(mm, Convert.ToInt32(Session["OrganId"]), fishid);
                            RasaKhazane.FishDetial[] FDetial = new RasaKhazane.FishDetial[mm.Rows.Count];

                            RasaKhazane.KhazaneService rasa = new RasaKhazane.KhazaneService();
                            if (mm.Rows.Count > 0)
                            {
                                for (int i = 0; i < mm.Rows.Count; i++)
                                {
                                    var code = servic.GetCodhayeDaramdWithFilter("fldDaramadCode", mm.Rows[i]["fldDaramadCode"].ToString(),0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                                    var code_hesab = servic.GetShomareHesabCodeDaramadWithFilter("fldCodeDaramadId", code.fldId.ToString(), "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
                                    var hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId", code_hesab.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
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
                                var fish = servic.GetSodoorFishDetail(fishid, IP, out Err);
                                var hesabNum = servic_Com.GetShomareHesabeOmoomiWithFilter("fldid", fish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err_Com).FirstOrDefault();
                                var elamavarez = servic.GetElamAvarezDetail(fish.fldElamAvarezId, Session["OrganId"].ToString(), IP, out Err);
                                string name = "", family = "", codeMeli = "", noeSherkat = "", shenasemeli = "", shSabt = "";
                                var ashkhas = servic_Com.GetAshkhasWithFilter("fldid", elamavarez.fldAshakhasID.ToString(), "", 0, IP, out Err_Com).FirstOrDefault();
                                if (elamavarez.fldNoeShakhs == "0")
                                {
                                    var employe = servic_Com.GetEmployeeWithFilter("fldid", ashkhas.fldHaghighiId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err_Com).FirstOrDefault();
                                    name = employe.fldName;
                                    family = employe.fldFamily;
                                    codeMeli = employe.fldCodemeli;
                                }
                                else
                                {
                                    var hoghoghi = servic_Com.GetAshkhaseHoghoghiWithFilter("fldid", ashkhas.fldHoghoghiId.ToString(), 0, IP, out Err_Com).FirstOrDefault();
                                    name = hoghoghi.fldName;
                                    noeSherkat = hoghoghi.fldTypeShakhs.ToString();
                                    codeMeli = hoghoghi.fldShenaseMelli;
                                }
                                string res = rasa.ReciveFish(company.fldUserNameService, company.fldPassService, fishid.ToString(), hesabNum.fldShomareHesab, FDetial, name, family, codeMeli, fish.fldJamKol, fish.fldMablaghAvarezGerdShode);
                                var result = res.Split('|');
                                if (result[1] != "-1")
                                {
                                    servic.UpdateSendToMali_Fish("SendToMali", true, fish.fldId, IP, out Err);
                                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + fish.fldId + "\n");
                                }
                            }

                        }
                        company = servic.GetCompanyWithFilter("fldTitle", "Fara", 0, IP, out Err).FirstOrDefault();
                        if (company != null)//ارسال به فرارایانه
                        {
                            var Fish = servic.GetSodoorFishDetail(fishid, IP, out Err);
                            var payType = servic.GetNahvePardakhtWithFilter("fldid", "9", 0, IP, out Err).FirstOrDefault();
                            var fishDetail = servic.GetSodoorFish_DetailWithFilter("fldfishid", Fish.fldId.ToString(), 0, IP, out Err).ToList();
                            if (fishDetail.Count > 0)
                            {
                                var codeDaramadi = servic.GetCodhayeDaramadiElamAvarez(fishDetail[0].fldCodeElamAvarezId, IP, out Err);
                                var hesabNum = servic_Com.GetShomareHesabeOmoomiWithFilter("fldid", Fish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err_Com).FirstOrDefault();
                                var bank = servic_Com.GetBankWithFilter("fldid", hesabNum.fldBankId.ToString(), 0, IP, out Err_Com).FirstOrDefault();
                                var organ = servic_Com.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com);
                                var elamavarez = servic.GetElamAvarezDetail(Fish.fldElamAvarezId, organ.fldId.ToString(),IP, out Err);
                                string name = "", family = "", codeMeli = "", noeSherkat = "", shenasemeli = "", shSabt = "";
                                var ashkhas = servic_Com.GetAshkhasWithFilter("fldid", elamavarez.fldAshakhasID.ToString(), "", 0, IP, out Err_Com).FirstOrDefault();
                                if (elamavarez.fldNoeShakhs == "0")
                                {
                                    var employe = servic_Com.GetEmployeeWithFilter("fldid", ashkhas.fldHaghighiId.ToString(), 0,
                                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]),
                                        out Err_Com).FirstOrDefault();
                                    name = employe.fldName;
                                    family = employe.fldFamily;
                                    codeMeli = employe.fldCodemeli;
                                }
                                else
                                {
                                    var hoghoghi = servic_Com.GetAshkhaseHoghoghiWithFilter("fldid", ashkhas.fldHoghoghiId.ToString(), 0, IP, out Err_Com).FirstOrDefault();
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
                                writer.WriteAttributeString("Serial", Fish.fldId.ToString());
                                writer.WriteStartElement("Amount");
                                writer.WriteRaw(Fish.fldJamKol.ToString());
                                writer.WriteEndElement();
                                long maliat = 0, avarez = 0;
                                foreach (var item in fishDetail)
                                {
                                    codeDaramadi = servic.GetCodhayeDaramadiElamAvarez(item.fldCodeElamAvarezId, IP, out Err);
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
                                writer.WriteRaw(MyLib.Shamsi.Miladi2ShamsiString(Fish.fldDate));
                                writer.WriteEndElement();
                                writer.WriteStartElement("PayDate");
                                writer.WriteRaw(MyLib.Shamsi.Miladi2ShamsiString(Currdate.fldDateTime));
                                writer.WriteEndElement();
                                writer.WriteStartElement("PayType");
                                writer.WriteRaw(payType.fldCodePardakht);
                                writer.WriteEndElement();
                                writer.WriteStartElement("TrackingCode");
                                writer.WriteRaw(RRN);
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
                                    codeDaramadi = servic.GetCodhayeDaramadiElamAvarez(item.fldCodeElamAvarezId, IP, out Err);
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
                                var request = (HttpWebRequest)WebRequest.Create(company.fldURL + "?FormType=103&YearID=" + MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(2, 2) + "&XmlFishs=" + xmlDocument.InnerXml);
                                var res = request.GetResponse();
                                Stream receiveStream = res.GetResponseStream();

                                // Pipes the stream to a higher level stream reader with the required encoding format. 
                                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                string result = readStream.ReadToEnd();
                                if (result.Contains("Fishs"))
                                {
                                    if (result.Contains("<Status>1</Status>"))
                                    {
                                        servic.UpdateSendToMali_Fish("SendToMali", true, Fish.fldId, IP, out Err);
                                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + Fish.fldId + "\n");
                                    }
                                }
                            }
                        }
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                Msgtitle = "خطا در ثبت پرداخت",
                                Er = 1
                            });
                        }
                        return Json(new { Msg = "پرداخت با موفقیت انجام شد و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                    }
                    else if (state == 2)
                    {
                        var Trans = Session["PcPosTransActionId"].ToString().Split(',');
                        var Fish = Session["TransFishIds"].ToString().Split(',');
                        for (int i = 1; i < Trans.Length; i++)
                        {
                            Msg = servic.UpdatePcPosTransaction_Status(Convert.ToInt32(Trans[i]), RRN, TerminalID, SerialId, "", TxnDate, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);

                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    Msgtitle = "خطا در پرداخت شماره " + Trans[i].ToString(),
                                    Er = 1
                                });
                            }
                            Msg = servic.InsertPardakhtFish(Convert.ToInt32(Fish[i]), Currdate.fldDateTime, 9, null,TarikhVariz, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    Msgtitle = "خطا در ثبت پرداخت",
                                    Er = 1
                                });
                            }
                        }
                        return Json(new { Msg = "پرداخت با موفقیت انجام شد و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                    }
                }
                else
                {
                    return Json(new { Msg = "پرداخت انجام نشد.(خطا: " + ResponseDescription + ")", MsgTitle = "عملیات ناموفق", Er = 1 });
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
                Msg = ResponseDescription,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ParsianPcPosPay(string PspId, int FishId, string ShGhabz, string ShPardakht, string Price, string Sheba, int state)
        {
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();

            var PayMethodVal = "";
            var pcpos_url = "";
            var PayMethodId = 0;
            var pcpos_urlId = 0;

            var Pos = servic.GetPcPosInfoWithFilter("fldPspId", PspId, Session["OrganId"].ToString(), 0, IP, out Err).FirstOrDefault();
            var PosIp = servic.GetPcPosIPWithFilter("fldPcPosInfoId", Pos.fldId.ToString(), Session["OrganId"].ToString(), 0, IP, out Err).FirstOrDefault();
            var param = servic.GetPcPosParametrWithFilter("fldPspId", PspId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            foreach (var item in param)
            {
                 if (item.fldEnName == "PayMethod")
                {
                    PayMethodId = item.fldId;
                }
                else if (item.fldEnName == "pcpos_url")
                {
                    pcpos_urlId = item.fldId;
                }
            }
            PayMethodVal = servic.GetPcPosParam_DetailWithFilter("fldPcPosParamId", PayMethodId.ToString(), 0, IP, out Err).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
            pcpos_url = servic.GetPcPosParam_DetailWithFilter("fldPcPosParamId", pcpos_urlId.ToString(), 0, IP, out Err).Where(l => l.fldPcPosInfoId == Pos.fldId).FirstOrDefault().fldValue;
            
            //long[] Mablgh=new long[10];
            //Mablgh[1] = Price;


            result.ViewBag.fishid = FishId;
            result.ViewBag.ip = PosIp.fldIp;
            result.ViewBag.mablagh = Price;
            result.ViewBag.ShGhabz = ShGhabz;
            result.ViewBag.ShPardakht = ShPardakht;
            result.ViewBag.PayMethodVal = PayMethodVal;
            result.ViewBag.pcpos_url = pcpos_url;
            result.ViewBag.Sheba = Sheba;
            result.ViewBag.state = state;
            
            return result;
        }
        public ActionResult ParsianPcPosVerify(int fishid, string TerminalID, string ResponseCode,
            string SerialId, string TxnDate, string RRN, string ResponseDescription,int state)
        {
            string Msg = ""; string MsgTitle = ""; byte Er = 0; int Id = 0;
            try
            {
                if (ResponseCode == "0")
                {
                    var Currdate = servic_Com.GetDate(IP, out Err_Com);
                    var TarikhVariz = MyLib.Shamsi.Shamsi2miladiDateTime(MyLib.Shamsi.ShamsiAddDay(Currdate.fldTarikh, 1));
                    if (state == 1)
                    {
                        Msg = servic.UpdatePcPosTransaction_Status(Convert.ToInt32(Session["PcPosTransActionId"]), RRN, TerminalID, SerialId, "", TxnDate, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                Msgtitle = "خطا در پرداخت شماره " + Session["PcPosTransActionId"].ToString(),
                                Er = 1
                            });
                        }
                        Msg = servic.InsertPardakhtFish(fishid, Currdate.fldDateTime, 9, null,TarikhVariz, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                Msgtitle = "خطا در ثبت پرداخت",
                                Er = 1
                            });
                        }
                        return Json(new { Msg = "پرداخت با موفقیت انجام شد و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                    }
                    else if (state == 2)
                    {
                        var Trans = Session["PcPosTransActionId"].ToString().Split(',');
                        var Fish = Session["TransFishIds"].ToString().Split(',');
                        for (int i = 1; i < Trans.Length; i++)
                        {
                            Msg = servic.UpdatePcPosTransaction_Status(Convert.ToInt32(Trans[i]), RRN, TerminalID, SerialId, "", TxnDate, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);

                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    Msgtitle = "خطا در پرداخت شماره " + Trans[i].ToString(),
                                    Er = 1
                                });
                            }
                            Msg = servic.InsertPardakhtFish(Convert.ToInt32(Fish[i]), Currdate.fldDateTime, 9, null,TarikhVariz, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    Msgtitle = "خطا در ثبت پرداخت",
                                    Er = 1
                                });
                            }
                        }
                        return Json(new { Msg = "پرداخت با موفقیت انجام شد و واریزی در سیستم ثبت گردید.", Msgtitle = "عملیات موفق", Er = 0 });
                    }
                }
                else
                {
                    return Json(new { Msg = "پرداخت انجام نشد.(خطا" + ResponseCode + ":" + ResponseDescription + ")", MsgTitle = "عملیات ناموفق", Er = 1 });
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
                Msg = ResponseDescription,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);

        }
        public XmlDocument CreateXmlFile(string FishId)
        {
            var item = servic.GetFishSaderShodeForXmlOutWithFilter("FishId", FishId, 0,IP,out Err).FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sb);
            
            writer.WriteStartDocument();
            writer.WriteStartElement("AllData");
            //foreach (var item in all)
            //{
                writer.WriteStartElement("Fish");//Start Fish
                writer.WriteAttributeString("Serial", item.SerialFish.ToString());

                writer.WriteStartElement("Amount");
                writer.WriteRaw(item.MablaghPardakhtShode.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Tax");
                writer.WriteRaw(item.Maliyat.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Complications");
                writer.WriteRaw(item.Avarez.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Status");
                writer.WriteRaw("1");
                writer.WriteEndElement();
                writer.WriteStartElement("IssueDate");
                writer.WriteRaw(item.TarikhSodoor);
                writer.WriteEndElement();
                writer.WriteStartElement("PayDate");
                writer.WriteRaw(item.TarikhPardakht);
                writer.WriteEndElement();
                writer.WriteStartElement("PayType");
                writer.WriteRaw(item.NoePardakht.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("TrackingCode");
                writer.WriteRaw(item.CodeRahgiri);
                writer.WriteEndElement();
                writer.WriteStartElement("Referencecode");
                writer.WriteRaw(item.CodeErja);
                writer.WriteEndElement();
                writer.WriteStartElement("AccountPayDate");
                writer.WriteRaw(item.TarikhVarizBeHesab);
                writer.WriteEndElement();
                writer.WriteStartElement("AccountNum");
                writer.WriteRaw(item.ShomareHesabVariz);
                writer.WriteEndElement();
                writer.WriteStartElement("BankID");
                writer.WriteRaw(item.Infi_Bank);
                writer.WriteEndElement();
                writer.WriteStartElement("BankName");
                writer.WriteRaw(item.NameBank);
                writer.WriteEndElement();
                writer.WriteStartElement("OrganName");
                writer.WriteRaw(item.OrganName);
                writer.WriteEndElement();
                writer.WriteStartElement("OrganNationalID");
                writer.WriteRaw(item.ShenaseMeliOrgan);
                writer.WriteEndElement();

                writer.WriteStartElement("Person");//Start Person
                writer.WriteStartElement("Type");
                writer.WriteRaw(item.NoeMoadi);
                writer.WriteEndElement();
                writer.WriteStartElement("Name");
                writer.WriteRaw(item.NameMoadi);
                writer.WriteEndElement();
                writer.WriteStartElement("Family");
                writer.WriteRaw(item.FamilyMoadi);
                writer.WriteEndElement();
                writer.WriteStartElement("NationalCode");
                writer.WriteRaw(item.CodeMeli);
                writer.WriteEndElement();
                writer.WriteStartElement("CompanyType");
                writer.WriteRaw(item.NoeSherkat);
                writer.WriteEndElement();
                writer.WriteStartElement("NationalID");
                writer.WriteRaw(item.ShenaseMeliSherkat);
                writer.WriteEndElement();
                writer.WriteStartElement("RegistrationNum");
                writer.WriteRaw(item.ShomareSabt);
                writer.WriteEndElement();

                writer.WriteEndElement();//End person

                writer.WriteStartElement("DaramadGroup");//Start DaramadGroup
                var d = servic.GetDaramadGroup_ParametrValuesWithFilter("fldElamAvarezId", item.ElamAvarezId.ToString(), 0,IP,out Err).ToList();
                foreach (var _item in d)
                {
                    writer.WriteStartElement("param");//Start param
                    writer.WriteStartElement("name");
                    writer.WriteRaw(_item.fldFnName);
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");
                    writer.WriteRaw(_item.fldValue);
                    writer.WriteEndElement();

                    writer.WriteEndElement();//End param
                }
                writer.WriteEndElement();//End DaramadGroup

                var CodeDaramadElamAvarezId = item.CodeDaramadElamAvarez.Split(';');

                writer.WriteStartElement("Detail");//Start Detail
                for (int i = 0; i < CodeDaramadElamAvarezId.Length - 1; i++)
                {
                    writer.WriteStartElement("CodeDaramad");//Start CodeDaramad
                    var c = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldId", CodeDaramadElamAvarezId[i], 0,IP,out Err).FirstOrDefault();
                    writer.WriteAttributeString("Code", c.fldDaramadCode);
                    writer.WriteStartElement("CodeDescription");
                    writer.WriteRaw(c.fldSharheCodeDaramad);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Amount");
                    writer.WriteRaw(c.fldTakhfifAsliValue.ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("Tax");
                    writer.WriteRaw(c.fldTakhfifMaliyatValue.ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("Complications");
                    writer.WriteRaw(c.fldTakhfifAvarezValue.ToString());
                    writer.WriteEndElement();

                    writer.WriteEndElement();//End CodeDaramad
                }
                writer.WriteEndElement();//End Detail


                writer.WriteEndElement();//End Fish
            //}
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();



            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sb.ToString());
            return xmlDocument;
        }
    }
}
