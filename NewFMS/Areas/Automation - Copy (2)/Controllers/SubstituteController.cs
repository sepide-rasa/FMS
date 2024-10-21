using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Automation.Controllers
{
    public class SubstituteController : Controller
    {
        //
        // GET: /Automation/Substitute/


        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
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
        public ActionResult GetCommision()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCommisionWithFilter("fldUserID", Session["UserId"].ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldID, fldName = c.fldName+" ("+c.fldO_postEjraee_Title+")" });
            return this.Store(q);
        }
        public ActionResult Save(WCF_Automation.OBJ_Substitute Substitute,string AzSaat,string TaSaat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                
                if (Substitute.fldID == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertSubstitute(Substitute.fldSenderComisionID, Substitute.fldReceiverComisionID, Substitute.fldStartDate, Substitute.fldEndDate, AzSaat, TaSaat, Substitute.fldIsSigner, Substitute.fldShowReceiverName, Substitute.fldDesc, IP, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), (Session["Password"].ToString()), out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateSubstitute(Substitute.fldID, Substitute.fldSenderComisionID, Substitute.fldReceiverComisionID, Substitute.fldStartDate, Substitute.fldEndDate, AzSaat, TaSaat, Substitute.fldIsSigner, Substitute.fldShowReceiverName, Substitute.fldDesc, IP, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), (Session["Password"].ToString()), out Err);

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
                servic.DeleteSubstitute(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";

                MsgTitle = "حذف موفق";
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
            var q = servic.GetSubstituteDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            string[] AzSaat=q.fldStartTime_S.Split(':');
            string[] TaSaat = q.fldEndTime_S.Split(':');
            return Json(new
            {
                AzSaat=AzSaat[0],
                AzMin = AzSaat[1],
                TaSaat = TaSaat[0],
                TaMin = TaSaat[1],
                fldID = q.fldID,
                fldEndDate = q.fldEndDate,
                fldIsSigner = q.fldIsSigner,
                fldReceiverComisionID = q.fldReceiverComisionID,
                fldReciverComisionName = q.fldReciverComisionName,
                fldSenderComisionID = q.fldSenderComisionID,
                fldSenderComisionName = q.fldSenderComisionName,
                fldShowReceiverName = q.fldShowReceiverName,
                fldStartDate = q.fldStartDate,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_Substitute> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Automation.OBJ_Substitute> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldSenderComisionName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSenderComisionName";
                            break;
                        case "fldReciverComisionName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldReciverComisionName";
                            break;
                        case "fldStartDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStartDate";
                            break;
                        case "fldEndDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEndDate";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetSubstituteWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetSubstituteWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetSubstituteWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Automation.OBJ_Substitute> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
