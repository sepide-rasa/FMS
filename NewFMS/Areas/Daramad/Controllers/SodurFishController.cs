using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Xml;
using System.Text;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class SodurFishController : Controller
    {
        //
        // GET: /Daramad/SodurFish/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        WCF_Accounting.AccountingService servic_Acc = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err_Acc = new WCF_Accounting.ClsError();
        public ActionResult Index(int Id, int State, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.SodoorFish();
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewData = this.ViewData;
            result.ViewBag.Id = Id;
            result.ViewBag.State = State;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }

        public ActionResult FishDetail(int ElamAvarezId, int ShomareHesabId, int ShorooShenaseGhabz)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.ElamAvarezId = ElamAvarezId;
            result.ViewBag.ShomareHesabId = ShomareHesabId;
            result.ViewBag.ShorooShenaseGhabz = ShorooShenaseGhabz;
            return result;
        }
        public ActionResult GetElamAvarez(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var q = servic.GetElamAvarezDetail(Id, (Session["OrganId"]).ToString(), IP, out Err);

                return Json(new
                {
                    fldNameShakhs = q.fldNameShakhs,
                    fldFather_Sabt = q.fldFather_Sabt,
                    fldNoeShakhs = q.fldNoeShakhs,
                    fldShenaseMeli = q.fldShenaseMeli,
                    fldAshakhasID = q.fldAshakhasID

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult CheckTakhfif(int ElameAvarezId, int fldShomareHesabId, byte fldShorooShenaseGhabz)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string MsgTitle = "";
            string Msg = "";
            byte Er = 0; bool? haveTakhfif = null;
            try
            {
                haveTakhfif = servic.CheckTakhfif(ElameAvarezId, fldShomareHesabId, fldShorooShenaseGhabz, IP, out Err);
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle,
                haveTakhfif = haveTakhfif
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadCodeDaramadAvarez(StoreRequestParameters parameters, string elamAvarez, string ShomareHesabId, string ShorooShenaseGhabz)
        {
            try
            {


                if (Session["Username"] == null)
                    return RedirectToAction("logon", "Account", new { area = "" });
                var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

                List<WCF_Daramad.OBJ_DetailSodoorFish> data = null;
                if (filterHeaders.Conditions.Count > 0)
                {
                    string field = "";
                    string searchtext = "";
                    List<WCF_Daramad.OBJ_DetailSodoorFish> data1 = null;
                    foreach (var item in filterHeaders.Conditions)
                    {
                        var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                        switch (item.FilterProperty.Name)
                        {
                            case "fldId":
                                searchtext = ConditionValue.Value.ToString();
                                field = "fldId";
                                break;
                        }
                        if (data != null)
                            data1 = servic.GetDetailSodoorFish(Convert.ToInt32(elamAvarez), Convert.ToInt32(ShomareHesabId), Convert.ToByte(ShorooShenaseGhabz), IP, out Err).ToList();
                        else
                            data = servic.GetDetailSodoorFish(Convert.ToInt32(elamAvarez), Convert.ToInt32(ShomareHesabId), Convert.ToByte(ShorooShenaseGhabz), IP, out Err).ToList();
                    }
                    if (data != null && data1 != null)
                        data.Intersect(data1);
                }
                else
                {
                    data = servic.GetDetailSodoorFish(Convert.ToInt32(elamAvarez), Convert.ToInt32(ShomareHesabId), Convert.ToByte(ShorooShenaseGhabz), IP, out Err).ToList();
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

                List<WCF_Daramad.OBJ_DetailSodoorFish> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
                //-- end paging ------------------------------------------------------------

                return this.Store(rangeData, data.Count);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult LoadAllData(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var Data = servic.GetEbtal_SodoorFish(Id, IP, out Err).ToList();
                return Json(new
                {
                    Data = Data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult Save(WCF_Daramad.OBJ_SodoorFish SodoorFish, List<WCF_Daramad.OBJ_SodoorFish_Detail> SodoorFishDetail, int State, bool? IsExternal, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; int sal = 0, mah = 0, id = 0; int PrintFish = 0;

            try
            {
                if (State == 1)
                {
                    var CodhayeDaramad = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldElamAvarezId", SodoorFish.fldElamAvarezId.ToString(), 0, IP, out Err).ToList().Where(l => l.fldShomareHesabId == SodoorFish.fldShomareHesabId && l.fldShorooshenaseGhabz == SodoorFish.fldShorooShenaseGhabz);
                    id = servic.InsertSodoorFish(SodoorFish.fldElamAvarezId, SodoorFish.fldShomareHesabId, SodoorFish.fldMablaghAvarezGerdShode,
                        SodoorFish.fldShorooShenaseGhabz, SodoorFish.fldJamKol, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in CodhayeDaramad)
                    {
                        Msg = servic.InsertSodoorFish_Detail(id, item.fldID, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    var organ = servic_Com.GetOrganizationDetail(SodoorFish.fldOrganId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com);
                    if (organ.fldCodAnformatic != null)
                    {
                        var Fish = SodoorFish.fldShorooShenaseGhabz + id.ToString();
                        sal = Fish.Length - 2;
                        if (Fish.Length > 8)
                        {
                            string s = Fish.Substring(8).PadRight(2, '0');
                            Fish = Fish.Substring(0, 8);
                            mah = Convert.ToInt32(s);
                        }
                        ghabz gh = new ghabz(Convert.ToInt32(Fish), Convert.ToInt32(organ.fldCodAnformatic), Convert.ToInt32(organ.fldCodKhedmat)
                            , Convert.ToInt64(SodoorFish.fldMablaghAvarezGerdShode), sal, mah);
                        var ShGhabz = gh.ShGhabz;
                        var ShPardakht = gh.ShPardakht;
                        var BarcodeText = gh.BarcodeText;
                        Msg = servic.UpdateShenaseGhabz_Pardakht(id, ShGhabz, ShPardakht, BarcodeText, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (id != 0)//ارسال به حسابداری
                    {
                        //var module_organ = servic_Com.GetModule_OrganWithFilter("CheckOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com)
                        //    .Where(l=>l.fldModuleId==10).FirstOrDefault();
                        //if (module_organ != null)
                        //{
                        //    servic_Acc.InsertDocumentFishDaramad(FiscalYearId, id, 10, 5, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err_Acc);
                        //    if (Err_Acc.ErrorType)
                        //    {
                        //        return Json(new
                        //        {
                        //            Er = 1,
                        //            Msg = Err_Acc.ErrorMsg,
                        //            MsgTitle = "خطا"
                        //        }, JsonRequestBehavior.AllowGet);
                        //    }
                        //}
                        //else
                        //{
                        var setting = servic.GetTanzimateDaramadWithFilter("fldOrganId_HesabRayan", organ.fldId.ToString(), FiscalYearId, 0, IP, out Err).FirstOrDefault();
                        var fishDetail = servic.GetSodoorFish_DetailWithFilter("fldfishid", id.ToString(), 0, IP, out Err).ToList();
                        if (fishDetail.Count > 0)
                        {
                            var codeDaramadi = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldId_HesabRayan", fishDetail[0].fldCodeElamAvarezId.ToString(), 1, IP, out Err).FirstOrDefault();
                            var hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", codeDaramadi.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                            if (hesab != null)
                            {
                                var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                                if (company != null)//ارسال به حسابرایان
                                {
                                    XmlDocument XDoc1 = new XmlDocument();
                                    // Create root node.
                                    XmlElement XElemRoot1 = XDoc1.CreateElement("FicheDetailByFisyear");
                                    XDoc1.AppendChild(XElemRoot1);
                                    XmlDocument XDoc2 = new XmlDocument();
                                    // Create root node.
                                    XmlElement XElemRoot2 = XDoc2.CreateElement("FicheAddAndSub");
                                    XDoc2.AppendChild(XElemRoot2);
                                    XmlDocument XDoc = new XmlDocument();
                                    // Create root node.
                                    XmlElement XElemRoot = XDoc.CreateElement("FicheDetail");
                                    XDoc.AppendChild(XElemRoot);
                                    bool discount = false;
                                    foreach (var item in fishDetail)
                                    {
                                        codeDaramadi = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldId_HesabRayan", item.fldCodeElamAvarezId.ToString(), 1, IP, out Err).FirstOrDefault();
                                        hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", codeDaramadi.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                                        if (hesab != null)
                                        {
                                            XmlElement Xsource = XDoc.CreateElement("Node");
                                            Xsource.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                            Xsource.SetAttribute("RevenueTaxCost", "0");
                                            Xsource.SetAttribute("RevenueAvarezCost", "0");
                                            Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                                            Xsource.SetAttribute("RevenueBed", "0");
                                            Xsource.SetAttribute("RevenueBes", "0");
                                            Xsource.SetAttribute("AmountSavingBand", "0");
                                            var fish1 = servic.GetSodoorFishDetail(id, IP, out Err);
                                            if (discount == false)
                                            {
                                                Xsource.SetAttribute("Discount", (fish1.fldJamKol - fish1.fldMablaghAvarezGerdShode).ToString());
                                                discount = true;
                                            }
                                            else
                                                Xsource.SetAttribute("Discount", "0");
                                            if (codeDaramadi.fldTakhfifAsliValue != null && codeDaramadi.fldTakhfifAsliValue != 0)
                                            {
                                                Xsource.SetAttribute("RevenueCost", (codeDaramadi.fldTakhfifAsliValue).ToString());
                                                //Xsource.SetAttribute("Discount", (codeDaramadi.fldAsliValue - codeDaramadi.fldTakhfifAsliValue).ToString());                                                
                                            }
                                            else
                                            {
                                                Xsource.SetAttribute("RevenueCost", (codeDaramadi.fldAsliValue).ToString());
                                            }
                                            XElemRoot.AppendChild(Xsource);

                                            if (codeDaramadi.fldAvarezValue != 0)
                                            {
                                                var code = servic.GetShomareHesabCodeDaramadWithFilter("fldId_HesabRayan", setting.fldAvarezId.ToString(), "", FiscalYearId, 1, IP, out Err).FirstOrDefault();
                                                hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", code.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                                                if (hesab != null)
                                                {
                                                    XmlElement Xsource1 = XDoc.CreateElement("Node");
                                                    Xsource1.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                                    Xsource1.SetAttribute("RevenueTaxCost", "0");
                                                    Xsource1.SetAttribute("RevenueAvarezCost", "0");
                                                    Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                                                    Xsource1.SetAttribute("RevenueBed", "0");
                                                    Xsource1.SetAttribute("RevenueBes", "0");
                                                    Xsource1.SetAttribute("AmountSavingBand", "0");
                                                    Xsource1.SetAttribute("Discount", "0");
                                                    if (codeDaramadi.fldTakhfifAvarezValue != null && codeDaramadi.fldTakhfifAvarezValue != 0)
                                                    {
                                                        Xsource1.SetAttribute("RevenueCost", codeDaramadi.fldTakhfifAvarezValue.ToString());
                                                        //Xsource1.SetAttribute("Discount", (codeDaramadi.fldAvarezValue - codeDaramadi.fldTakhfifAvarezValue).ToString());
                                                    }
                                                    else
                                                    {
                                                        Xsource1.SetAttribute("RevenueCost", codeDaramadi.fldAvarezValue.ToString());
                                                    }
                                                    XElemRoot.AppendChild(Xsource1);
                                                }
                                            }
                                            if (codeDaramadi.fldMaliyatValue != 0)
                                            {
                                                var code = servic.GetShomareHesabCodeDaramadWithFilter("fldId_HesabRayan", setting.fldMaliyatId.ToString(), "", FiscalYearId, 1, IP, out Err).FirstOrDefault();
                                                hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", code.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                                                if (hesab != null)
                                                {
                                                    XmlElement Xsource2 = XDoc.CreateElement("Node");
                                                    Xsource2.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                                    Xsource2.SetAttribute("RevenueTaxCost", "0");
                                                    Xsource2.SetAttribute("RevenueAvarezCost", "0");
                                                    Xsource2.SetAttribute("RevenueTaxAvarezCost", "0");
                                                    Xsource2.SetAttribute("RevenueBed", "0");
                                                    Xsource2.SetAttribute("RevenueBes", "0");
                                                    Xsource2.SetAttribute("AmountSavingBand", "0");
                                                    Xsource2.SetAttribute("Discount", "0");
                                                    if (codeDaramadi.fldTakhfifMaliyatValue != null && codeDaramadi.fldTakhfifMaliyatValue != 0)
                                                    {
                                                        Xsource2.SetAttribute("RevenueCost", codeDaramadi.fldTakhfifMaliyatValue.ToString());
                                                        //Xsource2.SetAttribute("Discount", (codeDaramadi.fldMaliyatValue - codeDaramadi.fldTakhfifMaliyatValue).ToString());
                                                    }
                                                    else
                                                    {
                                                        Xsource2.SetAttribute("RevenueCost", codeDaramadi.fldMaliyatValue.ToString());
                                                    }
                                                    XElemRoot.AppendChild(Xsource2);
                                                }
                                            }
                                        }
                                    }
                                    var fish = servic.GetSodoorFishDetail(id, IP, out Err);
                                    var elamavarez = servic.GetElamAvarezDetail(fish.fldElamAvarezId, organ.fldId.ToString(), IP, out Err);
                                    var hesabNum = servic_Com.GetShomareHesabeOmoomiWithFilter("fldid", SodoorFish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err_Com).FirstOrDefault();
                                    HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                                    XmlNode x = h.AccountListRevenue(Convert.ToInt32(company.fldPassService));
                                    string xx = "<hesabs>" + x.InnerXml.Replace("\"", "\'") + "</hesabs>";
                                    System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xx));
                                    System.Xml.XmlReader xr = System.Xml.XmlReader.Create(ms);
                                    string hesabNumber = "0";
                                    while (xr.Read())
                                    {
                                        if (xr.NodeType == System.Xml.XmlNodeType.Element)
                                        {
                                            if (xr.Name == "Node")
                                            {
                                                if (xr["AccountNum"].ToString() == hesabNum.fldShomareHesab)
                                                {
                                                    hesabNumber = xr["ID"].ToString();
                                                }
                                            }
                                        }
                                    }
                                    var k = h.RegisterNewFicheByAccYearCostAndDiscount(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService), id.ToString(), elamavarez.fldNameShakhs,
                                      fish.fldDate, hesabNumber, id.ToString(),
                                      "کد ملی: " + elamavarez.fldShenaseMeli + "_" + fish.fldDesc, "", "", "", "", "", 8, 1, "", "", (long)fish.fldMablaghAvarezGerdShode, 0, (fish.fldJamKol - fish.fldMablaghAvarezGerdShode)/*fish.fldMablaghTakhfif*/, XDoc, XDoc1, XDoc2);
                                    servic.UpdateSendToMali_Fish("SendToMali", true, id, IP, out Err);
                                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + k + "-" + id.ToString() + "\n");
                                }

                            }
                        }
                        //}
                    }
                    if (servic_Com.Permission(743, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err_Com))
                    {
                        Msg = "صدور فیش با موفقیت انجام شد. آیا مایل به پیش نمایش فیش هستید؟";
                        MsgTitle = "عملیات موفق";
                        PrintFish = 1;
                    }
                    else
                    {
                        Msg = "صدور فیش با موفقیت انجام شد.";
                        MsgTitle = "عملیات موفق";
                        PrintFish = 0;
                    }
                }
                else
                {
                    var c = servic.GetPardakhtFishWithFilter("fldFishId", SodoorFish.fldId.ToString(), 0, IP, out Err).FirstOrDefault();

                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (c != null && IsExternal != true)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = "فیش موردنظر پرداخت شده و امکان ابطال آن وجود ندارد.",
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (IsExternal == true)
                        {
                            var HaveAccFile = servic_Acc.GetDocumentRecord_DetailsWithFilter("FishId", SodoorFish.fldId.ToString(), 1, IP, out Err_Acc).FirstOrDefault();
                            if (HaveAccFile != null)
                            {
                                return Json(new
                                {
                                    Msg = "برای فیش مورد نظر سند ثبت شده و امکان ابطال آن وجود ندارد.",
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                            servic.DeletePardakhtFish(SodoorFish.fldId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        }
                        Msg = servic.InsertEbtal(SodoorFish.fldId, null, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                        var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                        if (company != null)//ارسال به حسابرایان
                        {
                            var k = h.VitiationSendedFiche(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService), id.ToString());
                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "cancelfish daramad: " + k + "-" + id.ToString() + "\n");
                        }
                    }
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    Msg = "ابطال فیش با موفقیت انجام شد.";
                    MsgTitle = "عملیات موفق";
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle,
                id = id,
                PrintFish = PrintFish
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendToHesabRayan( int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; 

            try 
            {
                var soo = servic.GetSodoorFishWithFilter("NotSentHesabrayan", "", 0, IP, out Err).ToList();
                foreach (var SodoorFish in soo)
                {


                    var organ = servic_Com.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com);
                    var setting = servic.GetTanzimateDaramadWithFilter("fldOrganId_HesabRayan", organ.fldId.ToString(), FiscalYearId, 0, IP, out Err).FirstOrDefault();
                    var fishDetail = servic.GetSodoorFish_DetailWithFilter("fldfishid", SodoorFish.fldId.ToString(), 0, IP, out Err).ToList();
                    if (fishDetail.Count > 0)
                    {
                        var codeDaramadi = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldId_HesabRayan", fishDetail[0].fldCodeElamAvarezId.ToString(), 1, IP, out Err).FirstOrDefault();
                        var hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", codeDaramadi.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                        if (hesab != null)
                        {
                            var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, IP, out Err).FirstOrDefault();
                            if (company != null)//ارسال به حسابرایان
                            {
                                XmlDocument XDoc1 = new XmlDocument();
                                // Create root node.
                                XmlElement XElemRoot1 = XDoc1.CreateElement("FicheDetailByFisyear");
                                XDoc1.AppendChild(XElemRoot1);
                                XmlDocument XDoc2 = new XmlDocument();
                                // Create root node.
                                XmlElement XElemRoot2 = XDoc2.CreateElement("FicheAddAndSub");
                                XDoc2.AppendChild(XElemRoot2);
                                XmlDocument XDoc = new XmlDocument();
                                // Create root node.
                                XmlElement XElemRoot = XDoc.CreateElement("FicheDetail");
                                XDoc.AppendChild(XElemRoot);
                                bool discount = false;
                                foreach (var item in fishDetail)
                                {
                                    codeDaramadi = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldId_HesabRayan", item.fldCodeElamAvarezId.ToString(), 1, IP, out Err).FirstOrDefault();
                                    hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", codeDaramadi.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                                    if (hesab != null)
                                    {
                                        XmlElement Xsource = XDoc.CreateElement("Node");
                                        Xsource.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                        Xsource.SetAttribute("RevenueTaxCost", "0");
                                        Xsource.SetAttribute("RevenueAvarezCost", "0");
                                        Xsource.SetAttribute("RevenueTaxAvarezCost", "0");
                                        Xsource.SetAttribute("RevenueBed", "0");
                                        Xsource.SetAttribute("RevenueBes", "0");
                                        Xsource.SetAttribute("AmountSavingBand", "0");
                                        var fish1 = servic.GetSodoorFishDetail(SodoorFish.fldId, IP, out Err);
                                        if (discount == false)
                                        {
                                            Xsource.SetAttribute("Discount", (fish1.fldJamKol - fish1.fldMablaghAvarezGerdShode).ToString());
                                            discount = true;
                                        }
                                        else
                                            Xsource.SetAttribute("Discount", "0");
                                        if (codeDaramadi.fldTakhfifAsliValue != null && codeDaramadi.fldTakhfifAsliValue != 0)
                                        {
                                            Xsource.SetAttribute("RevenueCost", (codeDaramadi.fldTakhfifAsliValue).ToString());
                                            //Xsource.SetAttribute("Discount", (codeDaramadi.fldAsliValue - codeDaramadi.fldTakhfifAsliValue).ToString());                                                
                                        }
                                        else
                                        {
                                            Xsource.SetAttribute("RevenueCost", (codeDaramadi.fldAsliValue).ToString());
                                        }
                                        XElemRoot.AppendChild(Xsource);

                                        if (codeDaramadi.fldAvarezValue != 0)
                                        {
                                            var code = servic.GetShomareHesabCodeDaramadWithFilter("fldId_HesabRayan", setting.fldAvarezId.ToString(), "", FiscalYearId, 1, IP, out Err).FirstOrDefault();
                                            hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", code.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                                            if (hesab != null)
                                            {
                                                XmlElement Xsource1 = XDoc.CreateElement("Node");
                                                Xsource1.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                                Xsource1.SetAttribute("RevenueTaxCost", "0");
                                                Xsource1.SetAttribute("RevenueAvarezCost", "0");
                                                Xsource1.SetAttribute("RevenueTaxAvarezCost", "0");
                                                Xsource1.SetAttribute("RevenueBed", "0");
                                                Xsource1.SetAttribute("RevenueBes", "0");
                                                Xsource1.SetAttribute("AmountSavingBand", "0");
                                                Xsource1.SetAttribute("Discount", "0");
                                                if (codeDaramadi.fldTakhfifAvarezValue != null && codeDaramadi.fldTakhfifAvarezValue != 0)
                                                {
                                                    Xsource1.SetAttribute("RevenueCost", codeDaramadi.fldTakhfifAvarezValue.ToString());
                                                    //Xsource1.SetAttribute("Discount", (codeDaramadi.fldAvarezValue - codeDaramadi.fldTakhfifAvarezValue).ToString());
                                                }
                                                else
                                                {
                                                    Xsource1.SetAttribute("RevenueCost", codeDaramadi.fldAvarezValue.ToString());
                                                }
                                                XElemRoot.AppendChild(Xsource1);
                                            }
                                        }
                                        if (codeDaramadi.fldMaliyatValue != 0)
                                        {
                                            var code = servic.GetShomareHesabCodeDaramadWithFilter("fldId_HesabRayan", setting.fldMaliyatId.ToString(), "", FiscalYearId, 1, IP, out Err).FirstOrDefault();
                                            hesab = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId_HesabRayan", code.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                                            if (hesab != null)
                                            {
                                                XmlElement Xsource2 = XDoc.CreateElement("Node");
                                                Xsource2.SetAttribute("RevenueID", hesab.fldHesabId.ToString());
                                                Xsource2.SetAttribute("RevenueTaxCost", "0");
                                                Xsource2.SetAttribute("RevenueAvarezCost", "0");
                                                Xsource2.SetAttribute("RevenueTaxAvarezCost", "0");
                                                Xsource2.SetAttribute("RevenueBed", "0");
                                                Xsource2.SetAttribute("RevenueBes", "0");
                                                Xsource2.SetAttribute("AmountSavingBand", "0");
                                                Xsource2.SetAttribute("Discount", "0");
                                                if (codeDaramadi.fldTakhfifMaliyatValue != null && codeDaramadi.fldTakhfifMaliyatValue != 0)
                                                {
                                                    Xsource2.SetAttribute("RevenueCost", codeDaramadi.fldTakhfifMaliyatValue.ToString());
                                                    //Xsource2.SetAttribute("Discount", (codeDaramadi.fldMaliyatValue - codeDaramadi.fldTakhfifMaliyatValue).ToString());
                                                }
                                                else
                                                {
                                                    Xsource2.SetAttribute("RevenueCost", codeDaramadi.fldMaliyatValue.ToString());
                                                }
                                                XElemRoot.AppendChild(Xsource2);
                                            }
                                        }
                                    }
                                }
                                var fish = servic.GetSodoorFishDetail(SodoorFish.fldId, IP, out Err);
                                var elamavarez = servic.GetElamAvarezDetail(fish.fldElamAvarezId, organ.fldId.ToString(), IP, out Err);
                                var hesabNum = servic_Com.GetShomareHesabeOmoomiWithFilter("fldid", SodoorFish.fldShomareHesabId.ToString(), "", "", 0, IP, out Err_Com).FirstOrDefault();
                                HesabRayan.ServiseToRevenueSystems h = new HesabRayan.ServiseToRevenueSystems();
                                XmlNode x = h.AccountListRevenue(Convert.ToInt32(company.fldPassService));
                                string xx = "<hesabs>" + x.InnerXml.Replace("\"", "\'") + "</hesabs>";
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xx));
                                System.Xml.XmlReader xr = System.Xml.XmlReader.Create(ms);
                                string hesabNumber = "0";
                                while (xr.Read())
                                {
                                    if (xr.NodeType == System.Xml.XmlNodeType.Element)
                                    {
                                        if (xr.Name == "Node")
                                        {
                                            if (xr["AccountNum"].ToString() == hesabNum.fldShomareHesab)
                                            {
                                                hesabNumber = xr["ID"].ToString();
                                            }
                                        }
                                    }
                                }
                                var k = h.RegisterNewFicheByAccYearCostAndDiscount(Convert.ToInt32(company.fldUserNameService), Convert.ToInt32(company.fldPassService), SodoorFish.fldId.ToString(), elamavarez.fldNameShakhs,
                                  fish.fldDate, hesabNumber, SodoorFish.fldId.ToString(),
                                  "کد ملی: " + elamavarez.fldShenaseMeli + "_" + fish.fldDesc, "", "", "", "", "", 8, 1, "", "", (long)fish.fldMablaghAvarezGerdShode, 0, (fish.fldJamKol - fish.fldMablaghAvarezGerdShode)/*fish.fldMablaghTakhfif*/, XDoc, XDoc1, XDoc2);
                                servic.UpdateSendToMali_Fish("SendToMali", true, SodoorFish.fldId, IP, out Err);
                                System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal daramad: " + k + "-" + SodoorFish.fldId.ToString() + "\n");
                            }
                        }
                    }
                }
                return Json(new
                                            {
                                                Er = 0,
                                                Msg = "تمامی فیش ها ارسال شد",
                                                MsgTitle = "عملیات موفق"
                                            }, JsonRequestBehavior.AllowGet);
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
