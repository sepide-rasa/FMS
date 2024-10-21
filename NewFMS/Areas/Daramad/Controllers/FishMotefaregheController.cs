using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Xml;
using System.Net;
using System.IO;
using System.Text;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class FishMotefaregheController : Controller
    {
        //
        // GET: /Daramad/FishMotefareghe/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
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
            var tarikh = servic_Com.GetDate(IP, out Err_Com).fldTarikh;
            result.ViewBag.tarikhVariz = tarikh;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetCodeDaramad(string DaramadCode, int FiscalYearId)
        {
            string Msg = "", MsgTitle = ""; int Er = 0;
            string fldDaramadTitle = ""; int fldShomareHesabCodeDaramadId = 0;
            var q = servic.GetShomareHesabCodeDaramadWithFilter("fldDaramadCode", DaramadCode, Session["OrganId"].ToString(),FiscalYearId, 1, IP, out Err).FirstOrDefault();
            if (q != null)
            {
                fldDaramadTitle = q.fldDaramadTitle;
                fldShomareHesabCodeDaramadId = q.fldId;

            }
            else
            {
                MsgTitle = "خطا";
                Er = 1;
                Msg = "کد درآمد مورد نظر ثبت نشده است.";
            }
            return Json(new
            {
                fldDaramadTitle = fldDaramadTitle,
                fldShomareHesabCodeDaramadId = fldShomareHesabCodeDaramadId,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> ItemArray, int fldAshkhasId, int fldShomareHesabId, string TarikhVariz,string TarikhSodoor, long MablaghKoliAsliValue, string fldDesc, int FiscalYearId)
        {
            string Msg = "", MsgTitle = ""; int Er = 0, FishId = 0, IdCodeDaramadElamAvarez = 0, IdElamAvarez = 0;
            try
            {
                var darsadMalyat=servic_Com.GetMaliyatArzesheAfzoodeWithFilter("fldFromDateToEndDate", TarikhSodoor, 0, IP, out Err_Com).OrderByDescending(l => l.fldId).FirstOrDefault();
               

                IdElamAvarez = servic.InsertElamAvarez(fldAshkhasId, false, Convert.ToInt32(Session["OrganId"]), true, null, "",
                    fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    servic.DeleteKoliElamAvarez(IdElamAvarez, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                var Tanzimat = servic.GetTanzimateDaramadWithFilter("fldOrganId", Session["OrganId"].ToString(),FiscalYearId, 1, IP, out Err).FirstOrDefault();
                FishId = servic.InsertSodoorFish(IdElamAvarez, fldShomareHesabId, MablaghKoliAsliValue, Tanzimat.fldShorooshenaseGhabz, MablaghKoliAsliValue, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    servic.DeleteKoliElamAvarez(IdElamAvarez, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                Models.RasaNewFMSEntities dm = new Models.RasaNewFMSEntities();
                dm.spr_UpdateTarikhFishMotefareghe(FishId, MyLib.Shamsi.Shamsi2miladiDateTime(TarikhSodoor), Convert.ToInt32(Session["UserId"]));

                foreach (var item in ItemArray)
                {
                    var mashmool = servic.GetShomareHesabCodeDaramadWithFilter("fldId", item.fldShomareHesabCodeDaramadId.ToString(), (Session["OrganId"]).ToString(), FiscalYearId, 0, IP, out Err).FirstOrDefault();
                    decimal ss =  1;
                    if (mashmool.fldMashmooleArzesheAfzoode==true)
                    {
                        ss = ss + (darsadMalyat.fldDarsadeAvarez+ darsadMalyat.fldDarsadeMaliyat) / 100;
                         item.fldAsliValue=Convert.ToInt64(item.fldAsliValue/ss);
                         item.fldMaliyatValue = Convert.ToInt64(item.fldAsliValue* darsadMalyat.fldDarsadeMaliyat / 100);
                         item.fldAvarezValue =Convert.ToInt64( item.fldAsliValue* darsadMalyat.fldDarsadeAvarez/100);
                    }

                    IdCodeDaramadElamAvarez = servic.InsertCodhayeDaramadiElamAvarez_External(IdElamAvarez, item.fldSharheCodeDaramad, item.fldShomareHesabCodeDaramadId, item.fldAsliValue, 1, item.fldMaliyatValue,item.fldAvarezValue, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    servic.InsertSodoorFish_Detail(FishId, IdCodeDaramadElamAvarez, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                if (Err.ErrorType)
                {
                    servic.DeleteKoliElamAvarez(IdElamAvarez, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1,
                    }, JsonRequestBehavior.AllowGet);
                }
                servic.InsertPardakhtFish(FishId, MyLib.Shamsi.Shamsi2miladiDateTime(TarikhVariz), 2, null, MyLib.Shamsi.Shamsi2miladiDateTime(TarikhVariz), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                var Soo=servic.GetSodoorFishDetail(FishId, IP, out Err);
                if (Soo.fldSendToMaliFlag == false)
                {
                    var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                    if (company != null)//ارسال به حسابرایان
                    {
                        HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                        var k = h.RecovrySendedFiche(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService)
                            , FishId.ToString(), MyLib.Shamsi.Shamsi2miladiDateTime(TarikhVariz), "");
                        System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "Pardakht daramad: " + k);
                    }
                    company = servic.GetCompanyWithFilter("fldTitle", "Rasa", 0, IP, out Err).FirstOrDefault();
                    if (company != null)//ارسال به رسا
                    {
                        DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter m = new DataSet.DataSet1TableAdapters.sp_selectFishForKhazaneTableAdapter();
                        DataSet.DataSet1.sp_selectFishForKhazaneDataTable mm = new DataSet.DataSet1.sp_selectFishForKhazaneDataTable();
                        m.Fill(mm, Convert.ToInt32(Session["OrganId"]), FishId);
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
                            var fish = servic.GetSodoorFishDetail(FishId, IP, out Err);
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
                            string res = rasa.ReciveFish(company.fldUserNameService, company.fldPassService, FishId.ToString(), hesabNum.fldShomareHesab, FDetial, name, family, codeMeli, fish.fldJamKol, fish.fldMablaghAvarezGerdShode);
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
                        var payType = servic.GetNahvePardakhtWithFilter("fldid", 2.ToString(), 0, IP, out Err).FirstOrDefault();
                        var fish = servic.GetSodoorFishDetail(FishId, IP, out Err);
                        var fishDetail = servic.GetSodoorFish_DetailWithFilter("fldfishid", FishId.ToString(), 0, IP, out Err).ToList();
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
                            writer.WriteAttributeString("Serial", FishId.ToString());
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
                            writer.WriteRaw(TarikhVariz);
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
                }
                
                if (Err.ErrorType)
                {
                    servic.DeleteKoliElamAvarez(IdElamAvarez, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1,
                    }, JsonRequestBehavior.AllowGet);
                }


                Msg = "ذخیره با موفقیت انجام شد. کدشناسایی: " + IdElamAvarez;
                MsgTitle = "ذخیره موفق";
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
    }
}
