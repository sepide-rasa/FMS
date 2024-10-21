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
//    public class ProgramController : Controller
//    {
//        //
//        // GET: /Budget/Program/

//        WCF_Budget.BudgetService BudgetService = new WCF_Budget.BudgetService();
//        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
//        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();


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
      

//        public ActionResult GetMamoriyat()
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            var q = BudgetService.GetMamoriyatWithFilter("", "","", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
//            return this.Store(q);
//        }
//        public ActionResult Save(WCF_Budget.OBJ_Program Program)
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            string Msg = "", MsgTitle = ""; var Er = 0;
//            try
//            {
//                if (Program.fldId == 0)
//                {
//                    //ذخیره
//                    MsgTitle = "ذخیره موفق";
//                    Msg = BudgetService.InsertProgram(Program.fldMamoriyatId, Program.fldCod, Program.fldTitle, Program.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
//                }
//                else
//                {
//                    //ویرایش
//                    MsgTitle = "ویرایش موفق";
//                    Msg = BudgetService.UpdateProgram(Program.fldId,Program.fldMamoriyatId,Program.fldCod,Program.fldTitle,Program.fldDesc, Session["Username"].ToString(),(Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
//        public ActionResult Help()
//        {
//            if (Session["Username"] == null)
//                return RedirectToAction("logon", "Account", new { area = "" });
//            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
//            return PartialView;
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
//                Msg = BudgetService.DeleteProgram(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
//            var q = BudgetService.GetProgramDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
//            return Json(new
//            {
//                fldId = q.fldId,
//                fldMamoriyatId = q.fldMamoriyatId.ToString(),
//                fldCod_Mamoriyat=q.fldCod_Mamoriyat,
//                fldCod = q.fldCod,
//                fldTitle = q.fldTitle,
//                fldDesc = q.fldDesc
//            }, JsonRequestBehavior.AllowGet);
//        }

        


//        public ActionResult Read(StoreRequestParameters parameters)
//        {
//            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

//            List<WCF_Budget.OBJ_Program> data = null;
//            if (filterHeaders.Conditions.Count > 0)
//            {
//                string field = "";
//                string searchtext = "";
//                List<WCF_Budget.OBJ_Program> data1 = null;
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
//                        data1 = BudgetService.GetProgramWithFilter(field, searchtext, "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
//                    else
//                        data = BudgetService.GetProgramWithFilter(field, searchtext,"", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
//                }
//                if (data != null && data1 != null)
//                    data.Intersect(data1); 
//            }
//            else
//            {
//                data = BudgetService.GetProgramWithFilter("", "","", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

//            List<WCF_Budget.OBJ_Program> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
//            //-- end paging ------------------------------------------------------------

//            return this.Store(rangeData, data.Count);
//        }

//    }
//}
