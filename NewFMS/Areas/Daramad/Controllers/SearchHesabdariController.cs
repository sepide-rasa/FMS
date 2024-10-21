using Ext.Net;
using Ext.Net.Utilities;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class SearchHesabdariController : Controller
    {
        //
        // GET: /Daramad/SearchHesabdari/

        public ActionResult Index()
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();            
            return PartialView;
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.HesabdariCode> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                
            }
            else
            {
                List<Models.HesabdariCode> code = new List<Models.HesabdariCode>();
                WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
                WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
                var company = servic.GetCompanyWithFilter("fldTitle", "hesabrayan", 0, "", out Err).FirstOrDefault();
                if (company != null)
                {
                    HesabRayan.ServiseToRevenueSystems m = new HesabRayan.ServiseToRevenueSystems();
                    XmlNode x = m.RevenueCodeList();
                    string xx = "<hesabs>" + x.InnerXml.Replace("\"", "\'") + "</hesabs>";
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xx));
                    System.Xml.XmlReader xr = System.Xml.XmlReader.Create(ms);

                    while (xr.Read())
                    {
                        if (xr.NodeType == System.Xml.XmlNodeType.Element)
                            if (xr.Name == "Node")
                            {
                                code.Add(new Models.HesabdariCode
                                {
                                    fldId = Convert.ToInt32(xr["ID"]),
                                    fldCodeDaramad = xr["RevenueCode"].ToString(),
                                    fldDescDaramad = xr["RevenueName"].ToString()
                                });
                            }
                    }
                }
                company = servic.GetCompanyWithFilter("fldTitle", "Rasa", 0, "", out Err).FirstOrDefault();
                if (company != null)
                {
                    RasaKhazane.KhazaneService r = new RasaKhazane.KhazaneService();
                    var codeing = r.LoadCodeHesab(company.fldUserNameService,company.fldPassService);
                    foreach (var item in codeing)
                    {
                        code.Add(new Models.HesabdariCode
                        {
                            fldId = Convert.ToInt32(item.fldSafaslMoinId),
                            fldCodeDaramad = item.fldSafaslMoinId.ToString(),
                            fldDescDaramad =item.fldOnvanHesab.ToString()
                        });
                    }
                }
                data = code.ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<Models.HesabdariCode> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
