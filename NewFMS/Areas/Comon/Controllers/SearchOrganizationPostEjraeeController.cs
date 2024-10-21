using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.Utilities;
using Ext.Net.MVC;

namespace NewFMS.Areas.Comon.Controllers
{
    public class SearchOrganizationPostEjraeeController : Controller
    {
        //
        // GET: /Comon/SearchOrganizationPostEjraee/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult Index(int State)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }
        public ActionResult New(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_TreeOrganPost> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_TreeOrganPost> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "id":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldid";
                            break;
                        case "title":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "Namechart":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Namechart";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetTreeOrganPostEjra(field, searchtext, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
                    else
                        data = servic.GetTreeOrganPostEjra(field, searchtext, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetTreeOrganPostEjra("", "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
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

            List<WCF_Common.OBJ_TreeOrganPost> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetName(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationalPostsEjraeeDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldTitle,

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NodeLoad(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.GetOrganizationalPostsEjraeeWithFilter("", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetOrganizationalPostsEjraeeWithFilter("fldPId", item.fldId.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetOrganizationalPostsEjraeeWithFilter("fldPId", nod, 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldTitle;
                    childNode.NodeID = ch.fldId.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }

    }
}
