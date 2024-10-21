using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Comon.Controllers
{
    public class ChartOrganEjraiiController : Controller
    {
        //
        // GET: /Comon/ChartOrganEjraii/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult IndexNew(string containerId)
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
        public ActionResult IndexNew2(string containerId)
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
        public ActionResult New(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }

        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_Common.OBJ_ChartOrganEjraee Organ)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {

                if (Organ.fldId == 0)
                {
                    //ذخیره
                    var q = servic.GetChartOrganEjraeeWithFilter("fldTitle", Organ.fldTitle, 1, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).Where(l => l.fldOrganId == Organ.fldOrganId).FirstOrDefault();
                    if (q == null)
                    {
                        MsgTitle = "ذخیره موفق";
                        Msg = servic.InsertChartOrganEjraee(Organ.fldTitle, Organ.fldPId, Organ.fldOrganId, Organ.fldNoeVahed, Organ.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        MsgTitle = "خطا";
                        Msg = "اطلاعات وارد شده تکراری است.";
                        Er = 1;
                    }
                }
                else
                {
                    //ویرایش
                    var q = servic.GetChartOrganEjraeeWithFilter("fldTitle", Organ.fldTitle, 1, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).Where(l => l.fldOrganId == Organ.fldOrganId).FirstOrDefault();
                    if (q != null && q.fldId != Organ.fldId)
                    {
                        MsgTitle = "خطا";
                        Msg = "اطلاعات وارد شده تکراری است.";
                        Er = 1;
                    }
                    else
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateChartOrganEjraee(Organ.fldId, Organ.fldTitle, Organ.fldPId, Organ.fldOrganId, Organ.fldNoeVahed, Organ.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }

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

        public ActionResult NodeLoad(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();

            if (nod == "0")
            {
                var q = servic.GetChartOrganEjraeeWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetChartOrganEjraeeWithFilter("fldPID", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
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
                var child = servic.GetChartOrganEjraeeWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();

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

        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteChartOrganEjraee(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetChartOrganEjraeeDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldOrganId = q.fldOrganId,
                fldPId = q.fldPId,
                fldNoeVahed = q.fldNoeVahed.ToString(),
                fldTitle = q.fldTitle,
                fldOrganizationName = q.fldOrganizationName,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
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
                        case "fldNoeVahedName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeVahedName";
                            break;
                        case "fldOrganizationName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrganizationName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetChartOrganEjraeeWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                    else
                        data = servic.GetChartOrganEjraeeWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetChartOrganEjraeeWithFilter("All", "", 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
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
