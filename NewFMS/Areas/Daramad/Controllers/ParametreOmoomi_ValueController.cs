using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Net.Security;
using System.Text.RegularExpressions;
using Aspose.Cells;
using System.Data;
using System.Web.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class ParametreOmoomi_ValueController : Controller
    {
        //
        // GET: /Daramad/ParametreOmoomi_Value/

        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();

        }
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
       

        public ActionResult New(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        
        public ActionResult GetParametreOmoomi()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetParametreOmoomiWithFilter("", "", 0,IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldNameParametreFa+"("+c.fldNameParametreEn+")" });
            return this.Store(q);
        }
        public ActionResult Save(WCF_Daramad.OBJ_ParametreOmoomi_Value ParametreOmoomi_Value)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ParametreOmoomi_Value.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertParametreOmoomi_Value(ParametreOmoomi_Value.fldParametreOmoomiId, ParametreOmoomi_Value.fldFromDate, ParametreOmoomi_Value.fldEndDate, ParametreOmoomi_Value.fldValue, ParametreOmoomi_Value.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateParametreOmoomi_Value(ParametreOmoomi_Value.fldId, ParametreOmoomi_Value.fldParametreOmoomiId, ParametreOmoomi_Value.fldFromDate, ParametreOmoomi_Value.fldEndDate, ParametreOmoomi_Value.fldValue, ParametreOmoomi_Value.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er

            }, JsonRequestBehavior.AllowGet);
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
                Msg = servic.DeleteParametreOmoomi_Value(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetParametreOmoomi_ValueDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldParametreOmoomiId = q.fldParametreOmoomiId.ToString(),
                nameParametrFa_En = q.nameParametrFa_En,
                fldValue = q.fldValue,
                fldFromDate=q.fldFromDate,
                fldEndDate=q.fldEndDate,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }


         public ActionResult Help()
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
             return PartialView;
         }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ParametreOmoomi_Value> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ParametreOmoomi_Value> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "nameParametrFa_En":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "nameParametrFa_En";
                            break;
                        case "fldValue":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldValue";
                            break;
                        case "fldFromDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFromDate";
                            break;
                        case "fldEndDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEndDate";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetParametreOmoomi_ValueWithFilter(field, searchtext,"", 100,IP, out Err).ToList();
                    else
                        data = servic.GetParametreOmoomi_ValueWithFilter(field, searchtext,"", 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametreOmoomi_ValueWithFilter("", "","", 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ParametreOmoomi_Value> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ValidateParamVal(string ParamId, string Value)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            bool Valid = true;
            var pa = servic.GetParametreOmoomiWithFilter("fldId", ParamId, 0, IP, out Err).FirstOrDefault();
            if (pa.fldFormulId != null)
            {
                var q = servic_ComPay.GetComputationFormulaWithFilter("fldId", pa.fldFormulId.ToString(), "", "", 0, 0, IP, out Err_ComPay).FirstOrDefault();

                string Formula = q.fldFormule;
                string[] ReferenceSplit = q.fldLibrary.Split(';').Reverse().Skip(1).Reverse().ToArray();

                ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
                CompilerParameters loParameters = new CompilerParameters();

                for (int i = 0; i < ReferenceSplit.Length; i++)
                {
                    if (ReferenceSplit[i] == "System.dll" || ReferenceSplit[i] == "System.Data.dll" || ReferenceSplit[i] == "System.Xml.dll" || ReferenceSplit[i] == "System.Core.dll")
                    {
                        loParameters.ReferencedAssemblies.Add(ReferenceSplit[i]);
                    }
                    else
                    {
                        var Path = Server.MapPath(@"~\Reference\" + ReferenceSplit[i]);
                        loParameters.ReferencedAssemblies.Add(Path);
                    }
                }

                loParameters.GenerateInMemory = true;
                CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Formula);

                Assembly loAssembly = loCompiled.CompiledAssembly;
                object loObject = loAssembly.CreateInstance("MyNamespace.MyClass");

                if (Value == "null" || Value == "")
                    Value = null;
                object[] loCodeParms = new object[3];
                loCodeParms[0] = Value;
                loCodeParms[1] = ParamId;

                object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                Valid = (bool)loResult;

            }
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }        
    }
}
