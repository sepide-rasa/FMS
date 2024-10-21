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
    public class SecretariatController : Controller
    {
        //
        // GET: /Automation/Secretariat/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        WCF_Common.CommonService servic_C = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_C = new WCF_Common.ClsError();

        public ActionResult Index(string containerId)
        {
            if (Session["UserName"] == null)
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
        public ActionResult Save(List<WCF_Automation.OBJ_Secretariat_OrganizationUnit> Secretariat, int ChartEjraeeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                /*if (c.Permission(22, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                {*/
                var saveData = servic.GetSecretariat_OrganizationUnitWithFilter("fldSecretariatID", ChartEjraeeId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                if (Err.ErrorType == true)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er=1
                    }, JsonRequestBehavior.AllowGet);
                }
                string[] SaveOrganizationUnitID = new string[saveData.Count];
                int[] SaveTableId = new int[saveData.Count];
                string[] NewOrganizationUnitID = new string[Secretariat.Count];

                for (int j = 0; j < saveData.Count; j++)
                {
                    SaveOrganizationUnitID[j] = saveData[j].fldOrganizationUnitID.ToString();
                    SaveTableId[j] = saveData[j].fldID;
                }
                for (int i = 0; i < Secretariat.Count; i++)
                {
                    NewOrganizationUnitID[i] = Secretariat[i].fldOrganizationUnitID.ToString();
                }
                var DeleteRow = SaveOrganizationUnitID.Except(NewOrganizationUnitID).ToArray();
                var InsertRow = NewOrganizationUnitID.Except(SaveOrganizationUnitID).ToArray();
                var UpdatedRow = NewOrganizationUnitID.Intersect(SaveOrganizationUnitID).ToArray();
                if (DeleteRow.Length != 0)
                    {
                        foreach (var item in DeleteRow)
                        {
                            var Index = SaveOrganizationUnitID.ToList().IndexOf(item);
                            servic.DeleteSecretariat_OrganizationUnit("fldId", saveData[Index].fldID, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                            if (Err.ErrorType == true)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    
                    

                }

                if (InsertRow.Length != 0)
                {
                    foreach (var item in InsertRow)
                    {
                        var Index = Secretariat.FindIndex(l => l.fldOrganizationUnitID == Convert.ToInt32(item));
                        Msg = servic.InsertSecretariat_OrganizationUnit(ChartEjraeeId, Secretariat[Index].fldOrganizationUnitID, "", Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        if (Err.ErrorType == true)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Er=1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                if (UpdatedRow.Length != 0)
                {
                 
                    foreach (var item in UpdatedRow)
                    {
                        var Index = Secretariat.FindIndex(l => l.fldOrganizationUnitID == Convert.ToInt32(item));
                        var index2 = Array.FindIndex(SaveOrganizationUnitID, l => l == item);
                        Secretariat[Index].fldID = SaveTableId[index2];
                        Msg = servic.UpdateSecretariat_OrganizationUnit(SaveTableId[index2], ChartEjraeeId, Secretariat[Index].fldOrganizationUnitID, "", Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        if (Err.ErrorType == true)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                MsgTitle = "ذخیره موفق";
                Msg = "ذخیره با موفقیت انجام شد.";
                if (Err.ErrorType == true)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                //}
                //else
                //{
                //    return null;
                //}
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
        public ActionResult New(int ChartOrganEjraeeId, string Name)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ChartOrganEjraeeId = ChartOrganEjraeeId;
            PartialView.ViewBag.Name = Name;
            return PartialView;
        }
        public ActionResult NodeLoad(string node, string SecretariatID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var child = servic_C.GetChartOrganEjraeeWithFilter("fldPID_Auto", node, 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_C).ToList();

            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldTitle;
                childNode.NodeID = ch.fldId.ToString();
                childNode.Icon = Ext.Net.Icon.Building;
                childNode.Checked = false;
                var f = servic.GetSecretariat_OrganizationUnitWithFilter("fldSecretariatID", SecretariatID.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                if (f.Count() != 0)
                {
                    foreach (var _item in f)
                    {
                        if (_item.fldOrganizationUnitID == ch.fldId)
                        {
                            childNode.Checked = true;
                        }
                    }
                }
                nodes.Add(childNode);
            }
            return this.Store(nodes);
        }
        
        
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_ChartOrganEjraee> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_ChartOrganEjraee> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                    }
                    if (data != null)
                        data1 = servic_C.GetChartOrganEjraeeWithFilter(field, searchtext, 100,Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_C).Where(l=>l.fldNoeVahed==2).ToList();
                    else
                        data = servic_C.GetChartOrganEjraeeWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_C).Where(l => l.fldNoeVahed == 2).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic_C.GetChartOrganEjraeeWithFilter("fldNoeVahed_Auto", "2", 100, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_C).ToList();
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

            List<WCF_Common.OBJ_ChartOrganEjraee> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
