//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Ext;
//using Ext.Net.MVC;
//using Ext.Net.Utilities;
//using Ext.Net;


//namespace NewFMS.Areas.Budget.Controllers
//{
//    public class MamoriyatController : Controller
//    {
//        //
//        // GET: /Budget/Mamoriyat/
//        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
//        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
//        WCF_Budget.BudgetService BudgetService = new WCF_Budget.BudgetService();
//        public ActionResult Index(string containerId)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            var result = new Ext.Net.MVC.PartialViewResult
//            {
//                WrapByScriptTag = true,
//                ContainerId = containerId,
//                RenderMode = RenderMode.AddTo
//            };

//            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
//            return result;
//        }
//        public ActionResult Save(WCF_Budget.OBJ_Mamoriyat Mamoriyat)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            string Msg = "", MsgTitle = ""; var Er = 0;
//            try
//            {
//                if (Mamoriyat.fldId == 0)
//                {
//                    //ذخیره
//                    MsgTitle = "ذخیره موفق";
//                    Msg = BudgetService.InsertMamoriyat(Mamoriyat.fldCod, Mamoriyat.fldTitle, Mamoriyat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
//                }
//                else
//                {
//                    //ویرایش
//                    MsgTitle = "ویرایش موفق";
//                    Msg = BudgetService.UpdateMamoriyat(Mamoriyat.fldId, Mamoriyat.fldCod, Mamoriyat.fldTitle, Mamoriyat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

//                }
//                if (Err.ErrorType)
//                {
//                    MsgTitle = "خطا";
//                    Msg = Err.ErrorMsg;
//                    Er = 1;
//                }
//            }
//            catch (Exception x)
//            {
//                if (x.InnerException != null)
//                    Msg = x.InnerException.Message;
//                else
//                    Msg = x.Message;

//                MsgTitle = "خطا";
//                Er = 1;
//            }
//            return Json(new
//            {
//                Msg = Msg,
//                MsgTitle = MsgTitle,
//                Err = Er
//            }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult Delete(int id)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            string Msg = "", MsgTitle = ""; int Er = 0;

//            try
//            {
//                //حذف
//                MsgTitle = "حذف موفق";
//                BudgetService.DeleteMamoriyat(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
//                Msg = "حذف با موفقیت انجام شد.";

//                MsgTitle = "حذف موفق";
//                if (Err.ErrorType)
//                {
//                    MsgTitle = "خطا";
//                    Msg = Err.ErrorMsg;
//                    Er = 1;
//                }

//            }
//            catch (Exception x)
//            {
//                if (x.InnerException != null)
//                    Msg = x.InnerException.Message;
//                else
//                    Msg = x.Message;

//                MsgTitle = "خطا";
//                Er = 1;
//            }
//            return Json(new
//            {
//                Msg = Msg,
//                MsgTitle = MsgTitle,
//                Er = Er
//            }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult Details(int Id)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            var q = BudgetService.GetMamoriyatDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
//            return Json(new
//            {
//                fldId = q.fldId,
//                fldCod = q.fldCod,
//                fldTitle=q.fldTitle,
//                fldDesc = q.fldDesc
//            }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult Read(StoreRequestParameters parameters)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

//            List<WCF_Budget.OBJ_Mamoriyat> data = null;
//            if (filterHeaders.Conditions.Count > 0)
//            {
//                string field = "";
//                string searchtext = "";
//                List<WCF_Budget.OBJ_Mamoriyat> data1 = null;
//                foreach (var item in filterHeaders.Conditions)
//                {
//                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

//                    switch (item.FilterProperty.Name)
//                    {
//                        case "fldId":
//                            searchtext = ConditionValue.Value.ToString();
//                            field = "fldId";
//                            break;
//                        case "fldCod":
//                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
//                            field = "fldCod";
//                            break;
//                        case "fldTitle":
//                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
//                            field = "fldTitle";
//                            break;
//                        case "fldDesc":
//                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
//                            field = "fldDesc";
//                            break;
//                    }
//                    if (data != null)
//                        data1 = BudgetService.GetMamoriyatWithFilter(field, searchtext,"", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
//                    else
//                        data = BudgetService.GetMamoriyatWithFilter(field, searchtext,"", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
//                }
//                if (data != null && data1 != null)
//                    data.Intersect(data1);
//            }
//            else
//            {
//                data = BudgetService.GetMamoriyatWithFilter("", "","", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
//            }

//            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

//            //FilterConditions fc = parameters.GridFilters;

//            //-- start filtering ------------------------------------------------------------
//            if (fc != null)
//            {
//                foreach (var condition in fc.Conditions)
//                {
//                    string field = condition.FilterProperty.Name;
//                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

//                    data.RemoveAll(
//                        item =>
//                        {
//                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
//                            return !oValue.ToString().Contains(value.ToString());
//                        }
//                    );
//                }
//            }
//            //-- end filtering ------------------------------------------------------------

//            //-- start paging ------------------------------------------------------------
//            int limit = parameters.Limit;

//            if ((parameters.Start + parameters.Limit) > data.Count)
//            {
//                limit = data.Count - parameters.Start;
//            }

//            List<WCF_Budget.OBJ_Mamoriyat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
//            //-- end paging ------------------------------------------------------------

//            return this.Store(rangeData, data.Count);
//        }
//    }
//}
