using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Comon.Controllers
{
    public class SearchChartOrganController : Controller
    {
        //
        // GET: /Comon/SearchChartOrgan/

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

        public ActionResult GetName(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetChartOrganDetail(Id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldTitle,
                fldOrganId=q.fldOrganId

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NodeLoad(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.GetChartOrganWithFilter("Chart_Organ", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetChartOrganWithFilter("fldPID_Organ", item.fldId.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList();
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
                var child = servic.GetChartOrganWithFilter("fldPId", nod, 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();

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
