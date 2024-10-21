using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;

namespace NewFMS.Areas.Budget.Controllers
{ 
    public class TemplateCodingController : Controller
    {
        // GET: Budget/TemplateCoding
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Budget.BudgetService servic = new WCF_Budget.BudgetService();
        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
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
        //public ActionResult GetAccountingType()
        //{
        //    var q = servic.GetAccountingTypeWithFilter("AccountingLevel", "", 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
        //    return this.Store(q);
        //}
        public ActionResult LoadTreeTemplateCoding(string nod, string TemplateId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetTemplatCodingWithFilter("fldPID", nod, TemplateId, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                //var LevelName = servic.GetCodingLevelDetail(item.fldCodingLevelId,Convert.ToInt32(Session["OrganId"]), IP, out Err).fldName;
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldCode + " - " + item.fldName ;
                asyncNode.Expanded = true;
                switch (item.fldNameCodingLevel)/*سطوح سرفصل کدینگ بودجه*/
                {
                    case "مأموریت":
                        asyncNode.IconCls = "GroupIco";
                        break;
                    case "برنامه":
                        asyncNode.IconCls = "KolIco";
                        break;
                }
                asyncNode.AttributesObject = new
                {
                    fldCode = item.fldCode,
                    fldName = item.fldName,
                    fldCodingLevelId = item.fldCodingLevelId,
                    fldNameCodingLevel = item.fldNameCodingLevel,
                    fldPCod = item.fldPCod,
                    fldDesc = item.fldDesc,
                    fldLevelId=item.fldLevelId
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_Budget.OBJ_TemplatName TemplateName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (TemplateName.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertTemplatName(TemplateName.fldName,TemplateName.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateTemplatName(TemplateName.fldId, TemplateName.fldName, TemplateName.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveTemplateCoding(WCF_Budget.OBJ_TemplatCoding TemplateCoding, int PID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (TemplateCoding.fldLevelId > 2)
                {
                    return Json(new
                    {
                        Msg = "امکان تعریف بیش از دو سطح مجاز نمی باشد.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (TemplateCoding.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertTemplatCoding(PID, TemplateCoding.fldName, TemplateCoding.fldPCod, TemplateCoding.fldTemplatNameId,
                    TemplateCoding.fldCode, TemplateCoding.fldCodingLevelId, TemplateCoding.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateTemplatCoding(TemplateCoding.fldId, TemplateCoding.fldName, TemplateCoding.fldTemplatNameId, TemplateCoding.fldCode,
                    TemplateCoding.fldCodingLevelId, TemplateCoding.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteTemplatName(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult DeleteNode(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteTemplatCoding(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetTemplatNameDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Budget.OBJ_TemplatName> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Budget.OBJ_TemplatName> data1 = null;
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
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetTemplatNameWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetTemplatNameWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetTemplatNameWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Budget.OBJ_TemplatName> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult CheckExistPCode(string Pcode, int TemplateId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetTemplatCodingWithFilter("fldCode", Pcode, TemplateId.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            return Json(new
            {
                Valid = q.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckValidateCode(string Code, string Pcode, int Id, int TempNameId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Valid = servic.CheckValidCodeBudje(Code, Pcode, Id, TempNameId, IP, out Err);
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDefaultCode(string Pcode, int TempNameId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDefaultCodeBudje(Pcode, TempNameId, IP, out Err).FirstOrDefault();
            return Json(new
            {
                DefaultCode = q.fldCode,
                LevelId = q.LevelId,
                LevelName = q.fldLevelName
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TemplateCoding(int fldTemplateId, string fldTemplateName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.fldTemplateId = fldTemplateId;
            result.ViewBag.fldTemplateName = fldTemplateName;
            return result;
        }
    }
}