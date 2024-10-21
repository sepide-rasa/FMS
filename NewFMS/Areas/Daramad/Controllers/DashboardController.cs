using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using NewFMS.Models;
using Aspose.Cells;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Daramad/Dashboard/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService servicCom = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var curDate = servicCom.GetDate(IP, out ErrCom).fldTarikh;
            var FromDate=curDate.Substring(0, 4) + "/" + (Convert.ToInt32(curDate.Substring(5, 2)) -1).ToString().PadLeft('0',2) + "/" + curDate.Substring(8, 2);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.curDate = curDate;
            result.ViewBag.FromDate = FromDate;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult getDonutData(string AzTarikh, string TaTarikh, byte TypeDate)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetListCodeDaramad_Donati(AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]),TypeDate, IP, out Err).ToList();
            List<Models.ChartDonut> data =new List<ChartDonut>();
            foreach (var item in q)
            {
                ChartDonut Ch=new ChartDonut();
                Ch.TitleIncomeCode=item.DaramdTitleChilde.Replace("\n", String.Empty);
                Ch.ValIncomeCode=Convert.ToInt64(item.Mablagh);
                Ch.CodeIncomeCode = item.DaramadCodeTitle;
                data.Add(Ch);
            }
            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getBarData(string AzTarikh, string TaTarikh, byte TypeDate)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            int Month = Convert.ToInt32(AzTarikh.Substring(5, 2));
            int Month2 = Convert.ToInt32(TaTarikh.Substring(5, 2));
            List<NewFMS.WCF_Daramad.OBJ_RptListCodeDaramad_Day> q = new List<WCF_Daramad.OBJ_RptListCodeDaramad_Day>();
            if (Month == Month2)
            {
                q = servic.GetListCodeDaramad_Day(AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), TypeDate,IP, out Err).ToList();
            }
            else
            {
                q = servic.GetListCodeDaramad_Month(AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]),TypeDate, IP, out Err).ToList();
            }
            List<string> MD = new List<string>();
            List<long> Mablagh = new List<long>();

            foreach (var item in q)
            {
                if(Month == Month2){
                    MD.Add(item.TarikhPardakht);
                }
                else{
                    MD.Add(item.fldNameMah);
                }
                Mablagh.Add(Convert.ToInt64(item.Mablagh));
            }
            return Json(new
            {
                Mablagh = Mablagh,
                MD = MD
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getGaugeData()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetListCodeDaramad_GajeWithFilter("","1397/06/10", "1397/08/01","", Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();
            List<Models.ChartDonut> data = new List<ChartDonut>();
            foreach (var item in q)
            {
                ChartDonut Ch = new ChartDonut();
                Ch.TitleIncomeCode = item.DaramdTitleChilde.Replace("\n", String.Empty);
                Ch.ValIncomeCode = Convert.ToInt32(item.Mablagh);
                data.Add(Ch);
            }
            return Json(new
            {
                Min=q[0].MinMablagh,
                Max=q[0].MaxMablagh,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
