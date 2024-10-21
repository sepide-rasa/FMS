using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Aspose.Cells;
using NewFMS.Models;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class MoteghayerhaHoghoghiController : Controller
    {
        //
        // GET: /PayRoll/MoteghayerhaHoghoghi/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();

        WCF_Common_Pay.Comon_PayService Common_Payservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId)
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
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ReadHeader(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_MoteghayerhayeHoghoghi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_MoteghayerhayeHoghoghi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;

                        case "fldTarikhEjra":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhEjra";
                            break;

                        case "fldTarikhSodur":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhSodur";
                            break;

                        case "fldAnvaeEstekhdamTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAnvaeEstekhdamTitle";
                            break;
                        case "fldEstekhdamType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEstekhdamType";
                            break;

                        case "fldTypeBimeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeBimeName";
                            break;

                    }
                    if (data != null)
                        data1 = PayServic.GetMoteghayerhayeHoghoghiWithFilter(field, searchtext, 100, IP,out ErrPay).ToList();
                    else
                        data = PayServic.GetMoteghayerhayeHoghoghiWithFilter(field, searchtext, 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetMoteghayerhayeHoghoghiWithFilter("", "", 100, IP, out ErrPay).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<WCF_PayRoll.OBJ_MoteghayerhayeHoghoghi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult New(int Id)
        {//باز شدن تب

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = Id;
            return result;
        }

        public ActionResult MoteghayerhaHoghoghiCopy(int Id)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = Id;
            return result;
        }

        public ActionResult GetAnvaeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetAnvaEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle, fldNoeEstekhdamId=c.fldNoeEstekhdamId });
            return this.Store(q);
        }

        public ActionResult GetTypeBime()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetTypeBimeWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }

        public ActionResult ReloadItems(int EstekhdamId, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_ItemsEstekhdam_MoteghayerHoghoghi> data = null;
            List<int> CheckID = new List<int>();

           
            data = PayServic.GetItemsEstekhdam_MoteghayerHoghoghiWithFilter("fldAnvaEstekhdamId", EstekhdamId.ToString(),HeaderId, 0, IP, out ErrPay).ToList();
            var Checked = PayServic.GetMoteghayerhayeHoghoghi_DetailWithFilter("fldMoteghayerhayeHoghoghiId", HeaderId.ToString(), 0, IP, out ErrPay).ToList();
            List<MotagheyerHoghoghi> list = new List<MotagheyerHoghoghi>(Checked.Count());
          
            foreach (var item in Checked)
            {
                CheckID.Add(item.fldItemEstekhdamId);
                list.Add(new MotagheyerHoghoghi() { fldItemEstekhdamId = item.fldItemEstekhdamId, fldMazayaMashmool = Convert.ToBoolean(item.fldMazayaMashmool) });
               
               
            }
            return Json(new { data = data, CheckID = CheckID,list=list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetMoteghayerhayeHoghoghiWithFilter("fldId", Id.ToString(), 1, IP, out ErrPay).FirstOrDefault();
            var typeEstekhamId = Common_Payservic.GetAnvaEstekhdamWithFilter("fldId", q.fldAnvaeEstekhdamId.ToString(), 1, IP, out ErrC_P).FirstOrDefault().fldNoeEstekhdamId;
            return Json(new
            {
                fldId = q.fldId,
                fldTarikhEjra = q.fldTarikhEjra,
                fldTarikhSodur = q.fldTarikhSodur,
                fldEstekhdamTypeId = q.fldAnvaeEstekhdamId.ToString(),
                fldTypeBimeId = q.fldTypeBimeId.ToString(),
                fldDarsadBimePersonal = q.fldDarsadBimePersonal,
                fldDarsadbimeKarfarma = q.fldDarsadbimeKarfarma,
                fldDarsadBimeBikari = q.fldDarsadBimeBikari,
                fldDarsadBimeMashagheleZiyanAvar = q.fldDarsadBimeMashagheleZiyanAvar,
                fldDarsadBimeJanbazan = q.fldDarsadBimeJanbazan,
                fldHaghDarmanTahteTakallof = q.fldHaghDarmanTahteTakaffol,
                fldHaghDarmanKarFarma = q.fldHaghDarmanKarFarma,
                fldHaghDarmanMazad = q.fldHaghDarmanMazad,
                fldHaghDarmanKarmand = q.fldHaghDarmanKarmand,
                fldHaghDarmanDolat = q.fldHaghDarmanDolat,
                fldMaxHaghDarman = q.fldMaxHaghDarman,
                fldZaribHoghoghiSal = q.fldZaribHoghoghiSal,
                fldHoghogh = q.fldHoghogh,
                fldFoghShoghl = q.fldFoghShoghl,
                fldFoghVizhe = q.fldFoghVizhe,
                fldTafavotTatbigh = q.fldTafavotTatbigh,
                fldHaghJazb = q.fldHaghJazb,
                fldTadil = q.fldTadil,
                fldBarJastegi = q.fldBarJastegi,
                fldSanavat = q.fldSanavat,
                fldFoghTalash = q.fldFoghTalash,
                fldZaribEzafeKar = q.fldZaribEzafeKar,
                fldSaatKari = q.fldSaatKari,
                fldDesc = q.fldDesc,
                typeEstekhamId = typeEstekhamId.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WCF_PayRoll.OBJ_MoteghayerhayeHoghoghi MoteghayerhayeHoghoghi, string ItemEstekhdamIds, List<Models.MotagheyerHoghoghi> items)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                if (MoteghayerhayeHoghoghi.fldDesc == null)
                    MoteghayerhayeHoghoghi.fldDesc = "";
                if (MoteghayerhayeHoghoghi.fldId == 0)
                { //ذخیره
                    PayServic.InsertMoteghayerhayeHoghoghi(MoteghayerhayeHoghoghi.fldTarikhEjra, MoteghayerhayeHoghoghi.fldTarikhSodur, MoteghayerhayeHoghoghi.fldAnvaeEstekhdamId, MoteghayerhayeHoghoghi.fldTypeBimeId
                        , MoteghayerhayeHoghoghi.fldZaribEzafeKar, MoteghayerhayeHoghoghi.fldSaatKari, MoteghayerhayeHoghoghi.fldDarsadBimePersonal, MoteghayerhayeHoghoghi.fldDarsadbimeKarfarma, MoteghayerhayeHoghoghi.fldDarsadBimeBikari, MoteghayerhayeHoghoghi.fldDarsadBimeJanbazan,
                        MoteghayerhayeHoghoghi.fldHaghDarmanKarmand, MoteghayerhayeHoghoghi.fldHaghDarmanKarFarma, MoteghayerhayeHoghoghi.fldHaghDarmanDolat, MoteghayerhayeHoghoghi.fldHaghDarmanMazad, MoteghayerhayeHoghoghi.fldHaghDarmanTahteTakaffol,
                        MoteghayerhayeHoghoghi.fldDarsadBimeMashagheleZiyanAvar, MoteghayerhayeHoghoghi.fldMaxHaghDarman, MoteghayerhayeHoghoghi.fldZaribHoghoghiSal, MoteghayerhayeHoghoghi.fldHoghogh, MoteghayerhayeHoghoghi.fldFoghShoghl, MoteghayerhayeHoghoghi.fldTafavotTatbigh,
                        MoteghayerhayeHoghoghi.fldFoghVizhe, MoteghayerhayeHoghoghi.fldHaghJazb, MoteghayerhayeHoghoghi.fldTadil, MoteghayerhayeHoghoghi.fldBarJastegi, MoteghayerhayeHoghoghi.fldSanavat,ItemEstekhdamIds,MoteghayerhayeHoghoghi.fldFoghTalash, MoteghayerhayeHoghoghi.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                    if (ErrPay.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrPay.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                else
                { //ویرایش
                    PayServic.UpdateMoteghayerhayeHoghoghi(MoteghayerhayeHoghoghi.fldId, MoteghayerhayeHoghoghi.fldTarikhEjra, MoteghayerhayeHoghoghi.fldTarikhSodur,
                        MoteghayerhayeHoghoghi.fldAnvaeEstekhdamId, MoteghayerhayeHoghoghi.fldTypeBimeId, MoteghayerhayeHoghoghi.fldZaribEzafeKar, MoteghayerhayeHoghoghi.fldSaatKari, MoteghayerhayeHoghoghi.fldDarsadBimePersonal, MoteghayerhayeHoghoghi.fldDarsadbimeKarfarma,
                        MoteghayerhayeHoghoghi.fldDarsadBimeBikari, MoteghayerhayeHoghoghi.fldDarsadBimeJanbazan, MoteghayerhayeHoghoghi.fldHaghDarmanKarmand, MoteghayerhayeHoghoghi.fldHaghDarmanKarFarma, MoteghayerhayeHoghoghi.fldHaghDarmanDolat,
                        MoteghayerhayeHoghoghi.fldHaghDarmanMazad, MoteghayerhayeHoghoghi.fldHaghDarmanTahteTakaffol, MoteghayerhayeHoghoghi.fldDarsadBimeMashagheleZiyanAvar, MoteghayerhayeHoghoghi.fldMaxHaghDarman, MoteghayerhayeHoghoghi.fldZaribHoghoghiSal,
                        MoteghayerhayeHoghoghi.fldHoghogh, MoteghayerhayeHoghoghi.fldFoghShoghl, MoteghayerhayeHoghoghi.fldTafavotTatbigh, MoteghayerhayeHoghoghi.fldFoghVizhe, MoteghayerhayeHoghoghi.fldHaghJazb, MoteghayerhayeHoghoghi.fldTadil,
                        MoteghayerhayeHoghoghi.fldBarJastegi, MoteghayerhayeHoghoghi.fldSanavat, MoteghayerhayeHoghoghi.fldFoghTalash, MoteghayerhayeHoghoghi.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                    if (ErrPay.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrPay.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    PayServic.DeleteMoteghayerhayeHoghoghi_Detail("HeaderId", MoteghayerhayeHoghoghi.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                    if (ErrPay.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrPay.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    //if (ItemEstekhdamIds != "")
                    //{
                        foreach (var _item in items /*ItemEstekhdamIds.Split(';')*/)
                        {
                            PayServic.InsertMoteghayerhayeHoghoghi_Detail(MoteghayerhayeHoghoghi.fldId, Convert.ToInt32(_item.fldItemEstekhdamId), _item.fldMazayaMashmool, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                            if (ErrPay.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = ErrPay.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    //}
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
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
                Er=Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(int HeaderId, string DateSodor, string DateEjar)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                Msg=PayServic.CopyMoteghayerhayHoghoghi(DateEjar, DateSodor, HeaderId, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrPay);
                MsgTitle = "عملیات موفق";
                if (ErrPay.ErrorType)
                {
                    Er = 1;
                    Msg = ErrPay.ErrorMsg;
                    MsgTitle = "خطا";
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
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                //حذف
                PayServic.DeleteMoteghayerhayeHoghoghi(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
                if (ErrPay.ErrorType)
                {
                    Msg = ErrPay.ErrorMsg;
                    MsgTitle = "خطا";
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
                Er=Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
