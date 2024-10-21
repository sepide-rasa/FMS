using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Automation.Controllers
{
    public class CommisionController : Controller
    {
        //
        // GET: /Automation/Commision/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        WCF_Common.CommonService servic_C = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_C = new WCF_Common.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.AhkamKari(); ;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult New(int AshkhasID, int id, string Name)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic_C.GetDate(IP, out Err_C);
            string year = q.fldTarikh.Substring(0, 4);

            PartialView.ViewBag.AshkhasID = AshkhasID;
            PartialView.ViewBag.id = id;
            PartialView.ViewBag.Name = Name;
            return PartialView;
        }
        public ActionResult Save(WCF_Automation.OBJ_Commision Commision)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Commision.fldDesc == null)
                    Commision.fldDesc = "";
                if (Commision.fldID == 0)
                {
                    //ویرایش
                    Msg = servic.InsertCommision(Commision.fldAshkhasID, Commision.fldOrganizPostEjraeeID, Commision.fldStartDate, Commision.fldEndDate,Commision.fldOrganicNumber,Commision.fldSign, Commision.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                {
                    //ویرایش
                    Msg = servic.UpdateCommision(Commision.fldID, Commision.fldAshkhasID, Commision.fldOrganizPostEjraeeID, Commision.fldStartDate, Commision.fldEndDate, Commision.fldOrganicNumber, Commision.fldSign, Commision.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType)
                {
                    Er = 1;
                    Msg = Err.ErrorMsg;
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
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCommision(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(int id)
        {
            var q = servic.GetCommisionDetail(id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var sign = "0";
            if (q.fldSign)
                sign = "1";
            return Json(new
            {
                fldID = q.fldID,
                fldAshkhasID = q.fldAshkhasID,
                fldEndDate = q.fldEndDate,
                fldO_postEjraee_Title = q.fldO_postEjraee_Title,
                fldOrganicNumber = q.fldOrganicNumber,
                fldOrganizPostEjraeeID = q.fldOrganizPostEjraeeID,
                fldStartDate = q.fldStartDate,
                sign = sign
            });
        }
        public ActionResult NodeLoadGroup(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic_C.GetOrganizationalPostsEjraeeWithFilter("","",0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_C).ToList();
                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.NodeID = item.fldId.ToString();
                    asyncNode.Icon = Ext.Net.Icon.Group;
                    asyncNode.AttributesObject = new { fldTitle = item.fldTitle};
                    nodes.Add(asyncNode);
                }
            }
            return this.Store(nodes);
        }
        public ActionResult ReadHeader(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_Ashkhas> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_Ashkhas> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldShenase_CodeMeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenase_CodeMeli";
                            break;
                        case "shomareshabt_father":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "shomareshabt_father";
                            break;
                    }
                    if (data != null)
                        data1 = servic_C.GetAshkhasWithFilter(field, searchtext, "", 100, IP, out Err_C).ToList();
                    else
                        data = servic_C.GetAshkhasWithFilter(field, searchtext, "", 100, IP, out Err_C).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic_C.GetAshkhasWithFilter("haghighi_khososi", "", "", 100, IP, out Err_C).ToList();
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

            List<WCF_Common.OBJ_Ashkhas> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReloadDetail(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Automation.OBJ_Commision> data = null;
            //data = servic.GetMasoulin_DetailList(HeaderId, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();
            data = servic.GetCommisionWithFilter("fldAshkhasID", HeaderId.ToString(), Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
