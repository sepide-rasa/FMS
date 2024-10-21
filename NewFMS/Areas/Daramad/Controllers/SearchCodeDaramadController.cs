using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class SearchCodeDaramadController : Controller
    {
        //
        // GET: /Daramad/SearchCodeDaramad/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        WCF_Accounting.AccountingService Accservic = new WCF_Accounting.AccountingService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Accounting.ClsError AccErr = new WCF_Accounting.ClsError();
        public ActionResult Index(int State, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            return PartialView;
        }

        public ActionResult Search(int State, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            return PartialView;
        }

        public ActionResult ReadTreeDaramadSearchforSetting(StoreRequestParameters parameters, int State, int? OrganId, int FiscalYearId)
        {
            //var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data = null;
            if (State == 2 || State == 3)
            {
                //if (filterHeaders.Conditions.Count > 0)
                //{
                //    string field = "";
                //    string searchtext = "";
                //    List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data1 = null;
                //    foreach (var item in filterHeaders.Conditions)
                //    {
                //        var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                //        switch (item.FilterProperty.Name)
                //        {
                //            case "fldId":
                //                searchtext = ConditionValue.Value.ToString();
                //                field = "fldId";
                //                break;
                //            case "fldDaramadTitle":
                //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                //                field = "LastNode_fldDaramadTitle_ArzesheAfzoode";
                //                break;
                //            case "fldDaramadCode":
                //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                //                field = "LastNode_fldDaramadCode_ArzesheAfzoode";
                //                break;
                //            case "fldNameVahed":
                //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                //                field = "LastNode_fldNameVahed_ArzesheAfzoode";
                //                break;
                //            case "fldShomareHesab":
                //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                //                field = "LastNode_fldShomareHesab_ArzesheAfzoode";
                //                break;
                //            case "fldShorooshenaseGhabz":
                //                searchtext = "%" + ConditionValue.Value.ToString() + "%";
                //                field = "LastNode_fldShorooshenaseGhabz_ArzesheAfzoode";
                //                break;
                //        }
                //        if (data != null)
                //            data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 100, IP, out Err).ToList();
                //        else
                //            data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 100, IP, out Err).ToList();
                //    }
                //    if (data != null && data1 != null)
                //        data.Intersect(data1);
                //}
                //else
                //{
                data = Accservic.GetCoding_DetailsWithFilter("AllCodeForDaramad", FiscalYearId.ToString(), "", "", 0, IP, out AccErr)
                    .Select(l=>new WCF_Daramad.OBJ_ShomareHesabCodeDaramad() { 
                    fldDaramadCode=l.fldCode,
                    fldDaramadTitle=l.fldTitle,
                    fldId=l.fldId
                }).ToList();
                //}
            }
            else
            {
            //    if (filterHeaders.Conditions.Count > 0)
            //    {
            //        string field = "";
            //        string searchtext = "";
            //        List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data1 = null;
            //        foreach (var item in filterHeaders.Conditions)
            //        {
            //            var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

            //            switch (item.FilterProperty.Name)
            //            {
            //                case "fldId":
            //                    searchtext = ConditionValue.Value.ToString();
            //                    field = "fldId";
            //                    break;
            //                case "fldDaramadTitle":
            //                    searchtext = "%" + ConditionValue.Value.ToString() + "%";
            //                    field = "LastNode_fldDaramadTitle_Karmozd";
            //                    break;
            //                case "fldDaramadCode":
            //                    searchtext = "%" + ConditionValue.Value.ToString() + "%";
            //                    field = "LastNode_fldDaramadCode_Karmozd";
            //                    break;
            //                case "fldNameVahed":
            //                    searchtext = "%" + ConditionValue.Value.ToString() + "%";
            //                    field = "LastNode_fldNameVahed_Karmozd";
            //                    break;
            //                case "fldShomareHesab":
            //                    searchtext = "%" + ConditionValue.Value.ToString() + "%";
            //                    field = "LastNode_fldShomareHesab_Karmozd";
            //                    break;
            //                case "fldShorooshenaseGhabz":
            //                    searchtext = "%" + ConditionValue.Value.ToString() + "%";
            //                    field = "LastNode_fldShorooshenaseGhabz_Karmozd";
            //                    break;
            //            }
            //            if (data != null)
            //                data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 0, IP, out Err).ToList();
            //            else
            //                data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 0, IP, out Err).ToList();
            //        }
            //        if (data != null && data1 != null)
            //            data.Intersect(data1);
            //    }
            //    else
            //    {
                    data = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_Organ_Karmozd", OrganId.ToString(), "",FiscalYearId, 0, IP, out Err).ToList();
            //    }
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

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult NodeLoadGroup(string nod, int? OrganId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            //var fieldName="PId_OrganId";
            //if (OrganId == 0)
            //    fieldName = "PId";
            if (nod == "root")
            {
                nod = "0";
            }
            var q = servic.GetShomareHesabCodeDaramadWithFilter("PId", nod,OrganId.ToString(),FiscalYearId, 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldDaramadTitle;
                /*asyncNode.Expanded = true;*/
                asyncNode.AttributesObject = new
                {
                    fldDaramadCode = item.fldDaramadCode,
                    fldDaramadTitle = item.fldDaramadTitle,
                    fldMashmooleArzesheAfzoode = item.fldMashmooleArzesheAfzoode,
                    fldMashmooleKarmozd = item.fldMashmooleKarmozd,
                    fldNameVahed = item.fldNameVahed,
                    fldUnitId = item.fldUnitId,
                    fldDesc = item.fldDesc
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult ReadTreeDaramadSearch(StoreRequestParameters parameters, int? OrganId, int FiscalYearId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDaramadTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldDaramadTitle";
                            break;
                        case "fldDaramadCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldDaramadCode";
                            break;
                        case "fldNameVahed":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldNameVahed";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldShomareHesab";
                            break;
                        case "fldShorooshenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldShorooshenaseGhabz";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 100, IP, out Err).ToList();
                    else
                        data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_Organ", OrganId.ToString(), "",FiscalYearId, 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
