using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using System.Dynamic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
namespace NewFMS.Areas.Budget.Controllers
{
    public class AnticipationController : Controller
    {
        // GET: Budget/Anticipation
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Budget.BudgetService service = new WCF_Budget.BudgetService();
        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
        public ActionResult Index(string containerId, short Year)
        {
            var commandText = " EXEC BUD.SelectPishbini_Daramad @Year";
            DataTable dt = new DataTable();

            
            //dt.Load(dr);
            //using (var reader = com.ExecuteReader())
            //{
            //    var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            //    foreach (var val2 in names)
            //    {
            //        dt.Columns.Add(val2);
            //    }
            //    foreach (IDataRecord record in reader as IEnumerable)
            //    {
            //        var expando = new ExpandoObject() as IDictionary<string, object>;
            //        foreach (var name in names)
            //            //expando[name] = record[name];
            //        dt.Rows.Add(record[name]);
                 
            //        //yield return expando;
            //    }

                
            //    //foreach (dynamic val in datasource)
            //    //{
            //    //    IDictionary<string, object> items = val;
                    
            //    //}
            //    //ViewBag.datasource = dt; 

            //}
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            //result.ViewBag.datasource = dt;
            return result;
            //var data = dr.Cast<dynamic>();
            //return View();
        }
        public ActionResult getData(short? Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataTable dt = new DataTable();
            //var dns = new List<IDictionary<string, object>>();
            List<IDictionary<string, object>> dns = new List<IDictionary<string, object>>();
            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings["RasaNewFMSConnectionString"].ConnectionString;
                string commandText = " EXEC BUD.SelectPishbini_Daramad @Year";
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand com = new SqlCommand(commandText, con);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@Year", Year);

                using (var reader = com.ExecuteReader())
                {
                    var names = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                    //foreach (var val2 in names)
                    //{
                    //    dt.Columns.Add(val2);
                    //}
                    foreach (IDataRecord record in reader as IEnumerable)
                    {
                        var expando =/* new ExpandoObject() as */new Dictionary<string, object>();

                        foreach (var name in names)
                            //expando[name] = record[name];
                            expando.Add(name, record[name]);
                        dns.Add(expando);
                        //dt.Rows.Add(record[name]);

                        //yield return expando;
                    }
                }
                //var expando =/* new ExpandoObject() as */new Dictionary<string, object>();
                //expando.Add("عنوان", "test");
                //var expando1 =/* new ExpandoObject() as */new Dictionary<string, object>();
                //expando1.Add("عنوان", "test1");
                //dns.Add(expando);
                //dns.Add(expando1);

                //var dr = com.ExecuteReader();
                //dt.Load(dr);
                //list = dt.AsEnumerable().ToList();
                //JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);  
            }
            catch (Exception)
            {
                
                throw;
            }

            return this.Store(dns);
        }
        public ActionResult getDynamicCol(byte State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var Cols = service.GetBudgetTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err)
                .Select(l => new { fldTitle = l.fldTitle, fldType=l.fldId }).ToList();
            return Json(new
            {
                Cols = Cols
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Read(StoreRequestParameters parameters, short Year)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });

        //    var commandText = " EXEC prs_RptCountCharkhe_Depo @charkhe,@Nahi,@Movazaf,@AzTarikh,@TaTarikh";
        //    DataTable dt=new DataTable();
            
        //    string ConnectionString = ConfigurationManager.ConnectionStrings["AutomationEntities"].ConnectionString;
        //    SqlConnection con = new SqlConnection(ConnectionString);
        //    con.Open();
        //    SqlCommand com = new SqlCommand( commandText, con);
        //    com.Parameters.Clear();
        //    com.Parameters.AddWithValue("@Nahi", 1);
        //    com.Parameters.AddWithValue("@Movazaf", 0);
        //    var dr=com.ExecuteReader();
        //    var data=dr.Cast<IEnumerable<dynamic>>().ToList();
            
        //    //var IncomAnticipate = new ExpandoObject() as IDictionary<string, Object>;
        //    //var IncomAnticipate = new ExpandoObject() as IDictionary<string, Object>;

        //    //foreach (var item in dt)
        //    //{
        //    //    dt.columns
        //    //    IncomAnticipate.Add();
        //    //}

        //    //List<dynamic> data = null;
            
        //    //    data = servic.GetDocumentRecord_HeaderWithFilter("", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 100, IP, out Err).ToList();

        //    ////-- start filtering ------------------------------------------------------------
        //    //if (fc != null)
        //    //{
        //    //    foreach (var condition in fc.Conditions)
        //    //    {
        //    //        string field = condition.FilterProperty.Name;
        //    //        var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

        //    //        data.RemoveAll(
        //    //            item =>
        //    //            {
        //    //                object oValue = item.GetType().GetProperty(field).GetValue(item, null);
        //    //                return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
        //    //            }
        //    //        );
        //    //    }
        //    //}
        //    ////-- end filtering ------------------------------------------------------------

        //    ////-- start paging ------------------------------------------------------------
        //    //int limit = parameters.Limit;

        //    //if ((parameters.Start + parameters.Limit) > data.Count)
        //    //{
        //    //    limit = data.Count - parameters.Start;
        //    //}

        //    //List<WCF_Accounting.OBJ_DocumentRecord_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
        //    ////-- end paging ------------------------------------------------------------

        //    //return this.Store(rangeData, data.Count);
        //}
    }
}