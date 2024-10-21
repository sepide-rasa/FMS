using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Weigh.Controllers
{
    public class TozinController : Controller
    {
        //
        // GET: /Weigh/Tozin/
        WCF_Weigh.WeighService servic = new WCF_Weigh.WeighService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Weigh.ClsError Err = new WCF_Weigh.ClsError();

        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
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
        public ActionResult FullSearchTozin(string AzTarikh, string TaTarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Weigh.OBJ_Tozin> data = null;
            data = servic.GetTozinWithFilter("", "","", AzTarikh, TaTarikh, 100, IP, out Err).ToList();

            return Json(new { data = data.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadTozin(StoreRequestParameters parameters, string AzTarikh, string TATarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Weigh.OBJ_Tozin> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Weigh.OBJ_Tozin> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNameBaskool":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameBaskool";
                            break;
                        case "fldMaxW":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMaxW";
                            break;
                        case "fldPlaque":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPlaque";
                            break;
                        case "fldSaat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSaat";
                            break;
                        case "fldTarikhShoroo":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhShoroo";
                            break;
                        case "fldTarikhPayan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhPayan";
                            break;
                        
                    }
                    if (data != null)
                        data1 = servic.GetTozinWithFilter(field, searchtext,"", AzTarikh, TATarikh, 100,  IP, out Err).ToList();
                    else
                        data = servic.GetTozinWithFilter(field, searchtext,"", AzTarikh, TATarikh, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetTozinWithFilter("", "","", AzTarikh, TATarikh, 100, IP, out Err).ToList();
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

            List<WCF_Weigh.OBJ_Tozin> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult MostanadatTozin(int TozinId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.TozinId = TozinId;
            return PartialView;
        }
        public ActionResult Image(int TozinId)
        {//برگرداندن عکس 

            string[] PicTozin = new string[1];
            string[] PicId = new string[1]; byte[] file = null;

            var BaskolDocument = servic.GetBaskolDocumentWithFilter("fldTozinId", TozinId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            if (BaskolDocument.Count() != 0)
            {


                PicTozin = new string[BaskolDocument.Count()];
                PicId = new string[BaskolDocument.Count()];
                for (int j = 0; j < BaskolDocument.Count(); j++)
                {
                    var pic = servicCommon.GetFileWithFilter("fldId", BaskolDocument[j].fldFileId.ToString(), 0, IP, out ErrCommon).FirstOrDefault();
                    var Icon = "";
                    PicId[j] = pic.fldId.ToString();
                    if (pic.fldImage != null)
                    {
                        Icon = Convert.ToBase64String(pic.fldImage);
                    }
                    PicTozin[j] = Icon;
                }



            }
            else
            {
                PicTozin = new string[0];
                PicId = new string[0];
            }
            return new JsonResult()
            {
                Data = new
                {
                    PicId = PicId,
                    PicTozin = PicTozin,
                },
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }
        public ActionResult ShowPicMostanadat(int FileId, string dc)
        {//برگرداندن عکس 
            var q = servicCommon.GetFileWithFilter("fldId", FileId.ToString(), 1, IP, out ErrCommon).FirstOrDefault();


            var image = Convert.ToBase64String(q.fldImage);

            return Json(new { image = image });

        }
    }
}
