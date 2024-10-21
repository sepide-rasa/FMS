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
    public class SearchOrganizationByParamController : Controller
    {
        //
        // GET: /Comon/SearchOrganizationByParam/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult Index(int State,string Id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }

        public ActionResult GetName(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationDetail(Id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NodeLoad(string nod,string Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.GetOrganizationWithFilter("Param", Id, 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldName;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetOrganizationWithFilter("fldPId", item.fldId.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP,out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldName;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetOrganizationWithFilter("fldPId", nod, 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldName;
                    childNode.NodeID = ch.fldId.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }
      

    }
}
