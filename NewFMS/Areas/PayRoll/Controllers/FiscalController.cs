using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class FiscalController : Controller
    {
        //
        // GET: /PayRoll/Fiscal/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common_Pay.Comon_PayService servicComon = new WCF_Common_Pay.Comon_PayService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common_Pay.ClsError Err1 = new WCF_Common_Pay.ClsError();
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
        public ActionResult GetAnvaeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicComon.GetAnvaEstekhdamWithFilter("", "", 0, IP, out Err1).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult FiscalTitle(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.FiscalId = id;
            return PartialView;
        }
        public ActionResult CopyFiscal(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.FiscalHeaderId = id;
            return PartialView;
        }
        public ActionResult Copy(int idHeader, int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                var q = servic.GetFiscalTitleWithFilter("fldFiscalHeaderId", idHeader.ToString(), 0, IP, out Err).ToList();
                foreach (var item in q)
                {
                    servic.InsertFiscalTitle(id, item.fldItemEstekhdamId, item.fldAnvaEstekhdamId, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }

                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                    return Json(new
                    {
                        Er = Er,
                        Msg = Msg,
                        MsgTitle = MsgTitle
                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = "کپی با موفقیت انجام شد.";
                MsgTitle = "کپی";
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadItem(int Estekhdam, int FiscalId)
        {

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common_Pay.OBJ_ItemsEstekhdam_FiscalTitle> data = null;
            data = servicComon.GetItemsEstekhdam_FiscalTitleWithFilter("fldAnvaeEstekhdamId", Estekhdam.ToString(), FiscalId, 0, IP, out Err1).ToList();
            var check = servic.GetFiscalTitleWithFilter("fldAnvaEstekhdamId", Estekhdam.ToString(), 0, IP, out Err).Where(k => k.fldFiscalHeaderId == FiscalId).ToList();
            int[] checkId = new int[check.Count];

            for (int i = 0; i < check.Count; i++)
            {
                checkId[i] = check[i].fldItemEstekhdamId;
            }
            return Json(new
            {
                data = data,
                checkId = checkId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult New(int id, int State)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.State = State;
            return PartialView;
        }
        public ActionResult NewHeader(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        public ActionResult SaveFiscalTitle(List<WCF_PayRoll.OBJ_FiscalTitle> FiscalTitleVal, int fldFiscalHeaderId, string fldAnvaEstekhdamId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
             string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                servic.DeleteFiscalTitle("fldHeaderId", fldAnvaEstekhdamId, fldFiscalHeaderId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                    return Json(new
                    {
                        Er = Er,
                        Msg = Msg,
                        MsgTitle = MsgTitle
                    }, JsonRequestBehavior.AllowGet);
                }
                if (FiscalTitleVal != null)
                {
                    foreach (var item in FiscalTitleVal)
                    {
                        if (item.fldDesc == null)
                            item.fldDesc = "";
                        //if (item.fldId == 0)
                        //{ //ذخیره
                        servic.InsertFiscalTitle(item.fldFiscalHeaderId, item.fldItemEstekhdamId, item.fldAnvaEstekhdamId, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err.ErrorMsg;
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                        //}

                        //else
                        //{ //ویرایش
                        //    servic.UpdateFiscalTitle(item.fldId, item.fldFiscalHeaderId, item.fldItemEstekhdamId, item.fldAnvaEstekhdamId, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                        //    if (Err.ErrorType)
                        //    {
                        //        MsgTitle = "خطا";
                        //        Msg = Err.ErrorMsg;
                        //        Er = 1;
                        //        return Json(new
                        //        {
                        //            Er = Er,
                        //            Msg = Msg,
                        //            MsgTitle = MsgTitle
                        //        }, JsonRequestBehavior.AllowGet);
                        //    }
                        //    Msg = "ویرایش با موفقیت انجام شد.";
                        //    MsgTitle = "ویرایش موفق";
                        //}
                    }
                }
                else
                {
                    return Json(new
                    {
                        Er = Er,
                        Msg = "ذخیره با موفقیت انجام شد.",
                        MsgTitle = "ذخیره موفق"
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_PayRoll.OBJ_Fiscal_Detail Fiscal, WCF_PayRoll.OBJ_Fiscal_Header FiscalHeader)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            //System.Data.Entity.Core.Objects.ObjectParameter _id = new System.Data.Entity.Core.Objects.ObjectParameter("fldId", typeof(int));
            try
            {
                
               if (Fiscal.fldDesc == null)
                    Fiscal.fldDesc = "";
                if (Fiscal.fldId == 0)
                { //ذخیره
                    var q = servic.GetFiscal_HeaderWithFilter("fldEffectiveDate", Fiscal.fldEffectiveDate,"", 0, IP,out Err).Where(k => k.fldDateOfIssue == Fiscal.fldDateOfIssue).FirstOrDefault();
                    if (q != null)
                        servic.InsertFiscal_Detail(q.fldId, Fiscal.fldAmountFrom, Fiscal.fldAmountTo, Fiscal.fldPercentTaxOnWorkers, Fiscal.fldTaxationOfEmployees, Fiscal.fldTax, Fiscal.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    else
                    {
                        servic.InsertFiscal_Header_Detail(Fiscal.fldEffectiveDate, Fiscal.fldDateOfIssue, Fiscal.fldAmountFrom, Fiscal.fldAmountTo, Fiscal.fldPercentTaxOnWorkers, Fiscal.fldTaxationOfEmployees, Fiscal.fldTax, Fiscal.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                }
                else
                { //ویرایش
                    var q = servic.GetFiscal_HeaderWithFilter("fldEffectiveDate", Fiscal.fldEffectiveDate, "",0, IP, out Err).Where(k => k.fldDateOfIssue == Fiscal.fldDateOfIssue).FirstOrDefault();
                    if (q != null)
                        servic.UpdateFiscal_Detail(Fiscal.fldId, q.fldId, Fiscal.fldAmountFrom, Fiscal.fldAmountTo, Fiscal.fldPercentTaxOnWorkers, Fiscal.fldTaxationOfEmployees, Fiscal.fldTax, Fiscal.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    else
                    {
                        //m.sp_Pay_tblFiscal_HeaderInsert(_id, MyLib.Shamsi.Shamsi2miladiDateTime(Fiscal.fldEffectiveDate), MyLib.Shamsi.Shamsi2miladiDateTime(Fiscal.fldDateOfIssue), 1, "");
                        //m.sp_Pay_tblFiscal_DetailUpdate(Fiscal.fldId, Convert.ToInt32(_id.Value), Fiscal.fldAmountFrom, Fiscal.fldAmountTo, Fiscal.fldPercentTaxOnWorkers, Fiscal.fldTaxationOfEmployees, Fiscal.fldTax, 1, Fiscal.fldDesc);
                        var HeaderId = servic.InsertFiscal_Header(FiscalHeader.fldEffectiveDate, FiscalHeader.fldDateOfIssue, FiscalHeader.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        servic.UpdateFiscal_Detail(Fiscal.fldId, HeaderId, Fiscal.fldAmountFrom, Fiscal.fldAmountTo, Fiscal.fldPercentTaxOnWorkers, Fiscal.fldTaxationOfEmployees, Fiscal.fldTax, Fiscal.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                     if (Err.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err.ErrorMsg;
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveHeader(WCF_PayRoll.OBJ_Fiscal_Header Fiscal)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
               
                if (Fiscal.fldDesc == null)
                    Fiscal.fldDesc = "";

                { //ویرایش
                    var q = servic.GetFiscal_HeaderWithFilter("fldEffectiveDate", Fiscal.fldEffectiveDate,"", 0,IP,out Err).Where(k => k.fldDateOfIssue == Fiscal.fldDateOfIssue).FirstOrDefault();
                    if (q == null)
                    {
                        servic.UpdateFiscal_Header(Fiscal.fldId, Fiscal.fldEffectiveDate, Fiscal.fldDateOfIssue, Fiscal.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        var q2 = servic.GetFiscal_DetailWithFilter("fldFiscalHeaderId", Fiscal.fldId.ToString(), 0,IP,out Err).ToList();
                        foreach (var Item in q2)
                            servic.UpdateFiscal_Detail(Item.fldId, q.fldId, Item.fldAmountFrom, Item.fldAmountTo, Item.fldPercentTaxOnWorkers, Item.fldTaxationOfEmployees, Item.fldTax, Item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (q.fldId != Fiscal.fldId)
                            servic.DeleteFiscal_Detail(Fiscal.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteHeader(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف

                servic.DeleteFiscal_Header_Detail(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                    return Json(new
                    {
                        Er = Er,
                        Msg = Msg,
                        MsgTitle = MsgTitle
                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
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
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف

                servic.DeleteFiscal_Detail(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                    return Json(new
                    {
                        Er = Er,
                        Msg = Msg,
                        MsgTitle = MsgTitle
                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
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
        public ActionResult DetailsFiscalTitle(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = servic.GetFiscalTitleWithFilter("fldFiscalHeaderId", Id.ToString(), 0, IP, out Err).FirstOrDefault();
            var fldId = 0;
            var fldAnvaeEstekhdamId = "";
            if (q != null)
            {
                fldId = q.fldId;
                fldAnvaeEstekhdamId = q.fldAnvaEstekhdamId.ToString();
            }
            return Json(new
            {
                fldId = fldId,
                fldAnvaeEstekhdamId = fldAnvaeEstekhdamId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = servic.GetFiscal_DetailWithFilter("fldId", Id.ToString(), 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldAmountFrom = q.fldAmountFrom,
                fldAmountTo = q.fldAmountTo,
                fldDateOfIssue = q.fldDateOfIssue,
                fldEffectiveDate = q.fldEffectiveDate,
                fldFiscalHeaderId = q.fldFiscalHeaderId,
                fldPercentTaxOnWorkers = q.fldPercentTaxOnWorkers,
                fldTax = q.fldTax,
                fldTaxationOfEmployees = q.fldTaxationOfEmployees,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HeaderDetails(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = servic.GetFiscal_HeaderWithFilter("fldId", Id.ToString(), "", 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldDateOfIssue = q.fldDateOfIssue,
                fldEffectiveDate = q.fldEffectiveDate
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
          
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Fiscal_Detail> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Fiscal_Detail> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;

                        case "fldAmountFrom":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAmountFrom";
                            break;

                        case "fldAmountTo":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAmountTo";
                            break;

                        case "fldPercentTaxOnWorkers":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPercentTaxOnWorkers";
                            break;

                        case "fldTax":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTax";
                            break;

                        case "fldTaxationOfEmployees":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTaxationOfEmployees";
                            break;

                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetFiscal_DetailWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetFiscal_DetailWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetFiscal_DetailWithFilter("", "", 100, IP,out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Fiscal_Detail> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Rload(int FiscalHeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
           
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Fiscal_Detail> data = null;

            data = servic.GetFiscal_DetailWithFilter("fldFiscalHeaderId", FiscalHeaderId.ToString(), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadHeader(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
           
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Fiscal_Header> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Fiscal_Header> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;

                        case "fldEffectiveDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEffectiveDate";
                            break;

                        case "fldDateOfIssue":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDateOfIssue";
                            break;


                    }
                    if (data != null)
                        data1 = servic.GetFiscal_HeaderWithFilter(field, searchtext,"", 100, IP, out Err).ToList();
                    else
                        data = servic.GetFiscal_HeaderWithFilter(field, searchtext,"", 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetFiscal_HeaderWithFilter("", "","", 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Fiscal_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadHeaderCopy(StoreRequestParameters parameters, int FiscalHeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Fiscal_Header> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Fiscal_Header> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldEffectiveDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "ExeptEffectiveDate";
                            break;

                        case "fldDateOfIssue":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "ExeptDateOfIssue";
                            break;


                    }
                    if (data != null)
                        data1 = servic.GetFiscal_HeaderWithFilter(field, searchtext, FiscalHeaderId.ToString(), 100, IP, out Err).ToList();
                    else
                        data = servic.GetFiscal_HeaderWithFilter(field, searchtext, FiscalHeaderId.ToString(), 100, IP,out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetFiscal_HeaderWithFilter("ExeptHeaderId", "", FiscalHeaderId.ToString(), 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Fiscal_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
