using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class MoteghayerhayAhkamController : Controller
    {
        //
        // GET: /Personeli/MoteghayerhayAhkam/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
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
            var q = Cservic.GetDate(IP, out CErr);
            var y = MyLib.Shamsi.Miladi2ShamsiString(q.fldDateTime);
            var year = y.Substring(0, 4);
            result.ViewBag.Year = year;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult Save(WCF_Personeli.OBJ_MoteghayerhaAhkam Ahkam)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Ahkam.fldDesc == null)
                    Ahkam.fldDesc = "";
                if (Ahkam.fldId == 0)
                {
                    //ذخیره
                    servic.InsertMoteghayerhaAhkam(Ahkam.fldYear, Ahkam.fldType, Ahkam.fldHagheOlad, Ahkam.fldHagheAeleMandi, Ahkam.fldKharoBar
                        , Ahkam.fldMaskan, Ahkam.fldKharoBarMojarad, Ahkam.fldHadaghalDaryafti, Ahkam.fldHaghBon, Ahkam.fldHadaghalTadil, Ahkam.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                }
                else
                {
                    //ویرایش
                    servic.UpdateMoteghayerhaAhkam(Ahkam.fldId, Ahkam.fldYear, Ahkam.fldType, Ahkam.fldHagheOlad, Ahkam.fldHagheAeleMandi, Ahkam.fldKharoBar
                       , Ahkam.fldMaskan, Ahkam.fldKharoBarMojarad, Ahkam.fldHadaghalDaryafti, Ahkam.fldHaghBon, Ahkam.fldHadaghalTadil, Ahkam.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                servic.DeleteMoteghayerhaAhkam(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMoteghayerhaAhkamDetail(Id, IP, out Err);
            string Type = "0";
            if (q.fldType)
                Type = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldHaghBon = q.fldHaghBon,
                fldHadaghalDaryafti = q.fldHadaghalDaryafti,
                fldHagheAeleMandi = q.fldHagheAeleMandi,
                fldHadaghalTadil=q.fldHadaghalTadil,
                fldHagheOlad = q.fldHagheOlad,
                fldKharoBar = q.fldKharoBar,
                fldKharoBarMojarad = q.fldKharoBarMojarad,
                fldMaskan = q.fldMaskan,
                fldType = Type.ToString(),
                fldYear = q.fldYear.ToString(),
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_MoteghayerhaAhkam> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_MoteghayerhaAhkam> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeName";
                            break;
                        case "fldYear":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldYear";
                            break;
                        case "fldHagheOlad":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldHagheOlad";
                            break;
                        case "fldKharoBar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldKharoBar";
                            break;
                        case "fldKharoBarMojarad":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldKharoBarMojarad";
                            break;
                        case "fldMaskan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMaskan";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetMoteghayerhaAhkamFilter(field, searchtext, 100, IP,out Err).ToList();
                    else
                        data = servic.GetMoteghayerhaAhkamFilter(field, searchtext, 100, IP,  out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMoteghayerhaAhkamFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_MoteghayerhaAhkam> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
