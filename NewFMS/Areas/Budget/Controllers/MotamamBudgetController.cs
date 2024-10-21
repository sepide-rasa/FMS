using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Threading.Tasks;

namespace NewFMS.Areas.Budget.Controllers
{
    public class MotamamBudgetController : Controller
    {
        // GET: Budget/MotamamBudget
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Budget.BudgetService servic = new WCF_Budget.BudgetService();
        WCF_Accounting.AccountingService servic_Acc = new WCF_Accounting.AccountingService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
        WCF_Accounting.ClsError Err_Acc = new WCF_Accounting.ClsError();
        public ActionResult Index(string containerId, short FiscalYearId)
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
            string Tarikh=servic_com.GetDate(IP, out Err_com).fldTarikh;
            result.ViewBag.Tarikh = Tarikh;
            result.ViewBag.FiscalYearId = FiscalYearId.ToString();
            return result;
        }

        //public async Task<ActionResult> ss()
        //{
        //    string Tarikh = servic_com.GetDate(IP, out Err_com).fldDateTime.ToString();
        //    Task<int>[] a = new Task<int>[2];
        //    a[0] = F1("sepide");
        //    a[1] = F2("eshghi");
        //    int s = 0;
        //    for (int i = 0; i < 2; i++)
        //    {
        //        s = s + await a[i];
        //    }
        //    string Tarikh2 = servic_com.GetDate(IP, out Err_com).fldDateTime.ToString();
        //    return Json(new
        //    {
        //        Tarikh1 =Tarikh,
        //        Tarikh2 = Tarikh2
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public async Task<int> F1(string k)
        //{
        //    await System.Threading.Tasks.Task.Delay(5000);
        //    return k.Length;
        //}
        //public async Task<int> F2(string k)
        //{
        //    await System.Threading.Tasks.Task.Delay(15000);
        //    return k.Length;
        //}
        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Acc.GetFiscalYearWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_Acc).ToList().Select(c => new { fldId = c.fldId, fldYear = c.fldYear });
            return this.Store(q);
        }

        public ActionResult Save(WCF_Budget.OBJ_Motammam Motamam)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Motamam.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertMotammam(Motamam.fldFiscalYearId, Motamam.fldTarikh, Motamam.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateMotammam(Motamam.fldId, Motamam.fldFiscalYearId, Motamam.fldTarikh, Motamam.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er

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
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteMotammam(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMotammamDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldFiscalYearId = q.fldFiscalYearId.ToString(),
                fldYear = q.fldYear,
                fldTarikh = q.fldTarikh,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Budget.OBJ_Motammam> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Budget.OBJ_Motammam> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldYear":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldYear";
                            break;
                    }
                    if (data != null)
                    {
                            data1 = servic.GetMotammamWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    }
                    else
                    {
                            data = servic.GetMotammamWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                    data = servic.GetMotammamWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
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

            List<WCF_Budget.OBJ_Motammam> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}