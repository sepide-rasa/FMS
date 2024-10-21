using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class TemplateCodingController : Controller
    {
        //
        // GET: /Accounting/TemplateCoding/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
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
        public ActionResult GetAccountingType()
        {
            var q = servic.GetAccountingTypeWithFilter("AccountingLevel", "", 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
            return this.Store(q);
        }
        public ActionResult GetTypeHesab()
        {
            var q = servic.GetTypeHesabWithFilter("", "", 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
            return this.Store(q);
        }
        public ActionResult GetNecessaryItems(string PCode,int TempNameId)
        {
            var q = servic.GetItemNecessaryWithFilter("fldPCode", PCode,TempNameId.ToString(), 0, IP, out Err).ToList().Select(l => new { fldNameItem = l.fldNameItem, fldId = l.fldId, 
                fldMahiyatId = l.fldMahiyatId,fldTypeHesabId = l.fldTypeHesabId,fldMahiyat_GardeshId=l.fldMahiyat_GardeshId });
            return this.Store(q);
        }
        public ActionResult GetMahiyat()
        {
            var q = servic.GetMahiyatWithFilter("", "", 0, IP, out Err).ToList().Select(l => new { fldTitle = l.fldTitle, fldId = l.fldId });
            return this.Store(q);
        }
        public ActionResult LoadTreeTemplateCoding(string nod, string TemplateId, string FilterValue)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetTemplateCodingWithFilter("fldPID", nod, TemplateId,"%" + FilterValue + "%",0, 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                var q1 = servic.GetTemplateCodingWithFilter("fldPID", item.fldId.ToString(), TemplateId,"", 0, 0, IP, out Err).ToList();
                var LevelName = servic.GetLevelsAccountingTypeDetail(item.fldLevelsAccountTypId, IP, out Err).fldName;                
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldCode+" - "+item.fldName;
                asyncNode.Expanded = false;
                asyncNode.Cls = q1.Count.ToString();
                switch (LevelName)/*سطوح انواع حسابداری*/
                {
                    case "گروه":
                        asyncNode.IconCls = "GroupIco";
                        break;
                    case "کل":
                        asyncNode.IconCls = "KolIco";
                        break;
                    case "معین":
                        asyncNode.IconCls = "MoeinIco";
                        break;
                    default:
                        asyncNode.IconCls = "TafsiliIco";
                        break;
                }
                asyncNode.AttributesObject = new
                {
                    fldCode = item.fldCode,
                    fldItemId = item.fldItemId,
                    fldName = item.fldName,
                    fldMahiyatId=item.fldMahiyatId,
                    fldMahiyat_GardeshId=item.fldMahiyat_GardeshId,
                    fldLevelsAccountTypId=item.fldLevelsAccountTypId,
                    fldName_LevelsAccountingType = item.fldName_LevelsAccountingType,
                    fldPCod=item.fldPCod,
                    fldDesc = item.fldDesc,
                    fldTypeHesabId = item.fldTypeHesabId.ToString(),
                    fldCodeBudget=item.fldCodeBudget,
                    fldAddChildNode=item.fldAddChildNode
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
        public ActionResult Save(WCF_Accounting.OBJ_TemplateName TemplateName)
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
                    Msg = servic.InsertTemplateName(TemplateName.fldName, TemplateName.fldAccountingTypeId,TemplateName.fldDesc,Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateTemplateName(TemplateName.fldId, TemplateName.fldName, TemplateName.fldAccountingTypeId, TemplateName.fldDesc,Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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

        public ActionResult SaveTemplateCoding(WCF_Accounting.OBJ_TemplateCoding TemplateCoding, int PID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (TemplateCoding.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertTemplateCoding(PID, TemplateCoding.fldItemId, TemplateCoding.fldName, TemplateCoding.fldPCod, TemplateCoding.fldMahiyatId, TemplateCoding.fldMahiyat_GardeshId,
                    TemplateCoding.fldCode, TemplateCoding.fldTempNameId, TemplateCoding.fldLevelsAccountTypId,TemplateCoding.fldTypeHesabId,TemplateCoding.fldAddChildNode,TemplateCoding.fldCodeBudget, TemplateCoding.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateTemplateCoding(TemplateCoding.fldId, TemplateCoding.fldItemId, TemplateCoding.fldName, TemplateCoding.fldMahiyatId,TemplateCoding.fldMahiyat_GardeshId, TemplateCoding.fldCode,
                    TemplateCoding.fldTempNameId, TemplateCoding.fldLevelsAccountTypId, TemplateCoding.fldTypeHesabId, TemplateCoding.fldAddChildNode, TemplateCoding.fldCodeBudget, TemplateCoding.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Msg = servic.DeleteTemplateName(Id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Msg = servic.DeleteTemplateCoding(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetTemplateNameDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldAccountingTypeId = q.fldAccountingTypeId.ToString(),
                fldName = q.fldName,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Accounting.OBJ_TemplateName> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Accounting.OBJ_TemplateName> data1 = null;
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
                        case "fldName_AccountingType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_AccountingType";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetTemplateNameWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetTemplateNameWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetTemplateNameWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Accounting.OBJ_TemplateName> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult CheckExistPCode(string Pcode, int TemplateId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetTemplateCodingWithFilter("fldCode", Pcode, TemplateId.ToString(),"",0, 0, IP, out Err).ToList();
            return Json(new
            {
                Valid=q.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckValidateCode(string Code, int AccountingTypeId, string Pcode, int Id, int TempNameId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Valid = servic.CheckValidCode(AccountingTypeId, Code, Pcode,Id,TempNameId, IP, out Err);            
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDefaultCode(string Pcode, int AccountingTypeId, int TempNameId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDefaultCode(AccountingTypeId, Pcode,TempNameId, IP, out Err).FirstOrDefault();
            return Json(new
            {
                DefaultCode = q.fldCode,
                fldAccountTypLevelId = q.fldLevelsAccountTypId,
                LevelName = q.fldName_LevelsAccountingType
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TemplateCoding(int fldTemplateId, string fldTemplateName, int fldAccountingTypeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.fldTemplateId=fldTemplateId.ToString();
            result.ViewBag.fldAccountingTypeId = fldAccountingTypeId;
            result.ViewBag.fldTemplateName = fldTemplateName;
            return result;
        }
    }
}
