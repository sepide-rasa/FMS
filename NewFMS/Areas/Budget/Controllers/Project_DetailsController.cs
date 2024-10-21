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
//    public class Project_DetailsController : Controller
//    {
//        //
//        // GET: /Budget/Project_Details/
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
       
//        public ActionResult Save(WCF_Budget.OBJ_Project_Details proj)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            string Msg = "", MsgTitle = ""; var Er = 0;
//            try
//            {
//                if (proj.fldId == 0)
//                {
//                    //ذخیره
//                    MsgTitle = "ذخیره موفق";
//                    Msg = BudgetService.InsertProject_Details(proj.fldProject_faaliyatId,proj.fldAddress,proj.fldMojri,proj.fldStratYear,proj.fldEndYear,proj.fldMeghdar,proj.fldGheymatVahed,proj.fldEtebar,proj.fldDesc,Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
//                }
//                else
//                {
//                    //ویرایش
//                    MsgTitle = "ویرایش موفق";
//                    Msg = BudgetService.UpdateProject_Details(proj.fldId,proj.fldProject_faaliyatId, proj.fldAddress, proj.fldMojri, proj.fldStratYear, proj.fldEndYear, proj.fldMeghdar, proj.fldGheymatVahed, proj.fldEtebar, proj.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
//                BudgetService.DeleteProject_Details(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
//            var q = BudgetService.GetProject_DetailsDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
//            return Json(new
//            {
//                fldId = q.fldId,
//                fldProject_faaliyatId=q.fldProject_faaliyatId,
//                fldAddress = q.fldAddress,
//                fldEndYear = q.fldEndYear,
//                fldEtebar=q.fldEtebar,
//                fldGheymatVahed=q.fldGheymatVahed,
//                fldMojri=q.fldMojri,
//                fldMeghdar=q.fldMeghdar,
//                fldStratYear=q.fldStratYear,
//                fldDesc = q.fldDesc
//            }, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult Read(StoreRequestParameters parameters)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

//            List<WCF_Budget.OBJ_Project_Details> data = null;
//            if (filterHeaders.Conditions.Count > 0)
//            {
//                string field = "";
//                string searchtext = "";
//                List<WCF_Budget.OBJ_Project_Details> data1 = null;
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
                      
//                        case "fldDesc":
//                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
//                            field = "fldDesc";
//                            break;
//                    }
//                    if (data != null)
//                        data1 = BudgetService.GetProject_DetailsWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
//                    else
//                        data = BudgetService.GetProject_DetailsWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
//                }
//                if (data != null && data1 != null)
//                    data.Intersect(data1);
//            }
//            else
//            {
//                data = BudgetService.GetProject_DetailsWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

//            List<WCF_Budget.OBJ_Project_Details> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
//            //-- end paging ------------------------------------------------------------

//            return this.Store(rangeData, data.Count);
//        }
//    }
//}
