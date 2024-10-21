using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using NewFMS.WCF_Common;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace NewFMS.Controllers
{
    public class WritingFormulaController : Controller
    {
        //
        // GET: /WritingFormula/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_Common_Pay.Comon_PayService servic_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Pay = new WCF_Common_Pay.ClsError();

        WCF_Daramad.DaramadService servic_Daramad = new WCF_Daramad.DaramadService();
        WCF_Daramad.ClsError Err_Daramad = new WCF_Daramad.ClsError();

        WCF_Archive.ArchiveService servic_Archive = new WCF_Archive.ArchiveService();
        WCF_Archive.ClsError Err_Archive = new WCF_Archive.ClsError();
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Admin", new { area = "faces" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult IndexTab(string containerId,string State)
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
            result.ViewBag.State = State;
            return result;
        }
        public ActionResult New(int id, int OrganId)
        {//باز شدن تب

           
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = id;
            result.ViewBag.OrganId = OrganId.ToString();
            return result;
        }

        public ActionResult Formula(int? TableId,string ID,string FieldName,string FormulId, string OrganId, string RankingType, string OrganName, string AzTarikh,
            int? FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Admin", new { area = "faces" });
            /*FieldName=""   ID:0 حکم  */

            if (FormulId == "0" && ID != "0")
            {
                if (FieldName == "FormulOmoomi")
                {
                    var k = servic_Daramad.GetParametreOmoomiDetail(Convert.ToInt32(ID), IP, out Err_Daramad);
                    if (k.fldFormulId != null)
                        FormulId = k.fldFormulId.ToString();
                }
                else if (FieldName == "FormulSabet")
                {
                    var k = servic_Daramad.GetParametreSabetDetail(Convert.ToInt32(ID), IP, out Err_Daramad);
                    if (k.fldFormulId != null)
                        FormulId = k.fldFormulId.ToString();
                }
                else if (FieldName == "formulMohasebat")
                {
                    var k = servic_Daramad.GetShomareHesabCodeDaramad_Fomula("fldShomareHesab_CodeId", ID, 0, Convert.ToInt32(OrganId)/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Daramad).Where(l => l.fldTarikhEjra == AzTarikh).FirstOrDefault();
                    if (k!=null) 
                        if(k.fldFormulMohasebatId != null)
                        FormulId = k.fldFormulMohasebatId.ToString();
                }
                else if (FieldName == "FormulKoli")
                {
                    var k = servic_Daramad.GetShomareHesabCodeDaramadDetail(Convert.ToInt32(ID), Convert.ToInt32(FiscalYearId), IP, out Err_Daramad);
                    if (k.fldFormulKoliId != null)
                        FormulId = k.fldFormulKoliId.ToString();
                }
                else if (FieldName == "FormulDarmadGroup")
                {
                    var k = servic_Daramad.GetDaramadGroup_ParametrDetail(Convert.ToInt32(ID), IP, out Err_Daramad);
                    if (k.fldFormuleId != null)
                        FormulId = k.fldFormuleId.ToString();
                }

            }
            ViewBag.ID = ID;
            ViewBag.FieldName = FieldName;
            ViewBag.FormulId = FormulId;
            ViewBag.OrganId = OrganId;
            ViewBag.OrganName = OrganName;
            ViewBag.RankingType = RankingType;
            ViewBag.AzTarikh = AzTarikh;
            ViewBag.TableId = TableId;
            return View();
        }

        public ActionResult Details(int Id,int OrganId)
        {
            var q = servic_Pay.GetComputationFormulaDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            var Type = "0";
            if (q.fldType == true)
                Type = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldAzTarikh=q.fldAzTarikh,
                fldOrganId = q.fldOrganId.ToString(),
                NameOrgan = q.NameOrgan,
                fldType = Type,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Read(StoreRequestParameters parameters, int State,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Admin", new { area = "faces" });
            var t = true;
            if (State == 1)
                t = false;
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common_Pay.OBJ_ComputationFormula> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common_Pay.OBJ_ComputationFormula> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                    }
                    if (data != null)
                        data1 = servic_Pay.GetComputationFormulaWithFilter("", "", "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err_Pay).Where(l => l.fldType == t).ToList();
                    else
                        data = servic_Pay.GetComputationFormulaWithFilter("", "", "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err_Pay).Where(l => l.fldType == t).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic_Pay.GetComputationFormulaWithFilter("", "", "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err_Pay).Where(l => l.fldType == t).ToList();
            }

            //var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            ////FilterConditions fc = parameters.GridFilters;

            ////-- start filtering ------------------------------------------------------------
            //if (fc != null)
            //{
            //    foreach (var condition in fc.Conditions)
            //    {
            //        string field = condition.FilterProperty.Name;
            //        var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

            //        data.RemoveAll(
            //            item =>
            //            {
            //                object oValue = item.GetType().GetProperty(field).GetValue(item, null);
            //                return !oValue.ToString().Contains(value.ToString());
            //            }
            //        );
            //    }
            //}
            ////-- end filtering ------------------------------------------------------------

            ////-- start paging ------------------------------------------------------------
            //int limit = parameters.Limit;

            //if ((parameters.Start + parameters.Limit) > data.Count)
            //{
            //    limit = data.Count - parameters.Start;
            //}

            //List<RaiTrainClientWS.OBJ_CompanyPersonal> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            ////-- end paging ------------------------------------------------------------

            return this.Store(data);
        }

        public ActionResult LoadData(string FieldName, int FormulId,int OrganId)
        {
            //if (FormulId == 0 || FormulId == null)
            //{
            //    return RedirectToAction("Login", "Admin", new { area = "faces" });
            //}
            string Msg = ""; string Code = ""; string Reference = ""; var kk = "";
            try
            {
                var s = servic_Daramad.GetTanzimateDaramadWithFilter("fldOrganId_Check", OrganId.ToString()/*(Session["OrganId"]).ToString()*/, 0, 0, IP, out Err_Daramad).FirstOrDefault();
                if (s != null)
                {
                    var ff=s.fldFormul.Split('|');
                    if(ff.Length>1)
                    kk = "\nint CodeDaramadElamAvarezId=Convert.ToInt32(Parameters[0]);\n\nstring commandText = " + ff[0] + ";\n\nstring commandTextPublic = " + ff[1] + ";\n\nNewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();\nvar tblParam = service.GetData(commandText).Tables[0];\nvar tblPublicParam = service.GetData(commandTextPublic).Tables[0];\nif (tblParam.Rows.Count > 0)\n{\n\n/*   دسترسی به پارامترها  Convert.ToInt32(tblParam.Rows[0]['EN-PARAM-NAME'])*/\n}";
                    else
                        kk = "\nint CodeDaramadElamAvarezId=Convert.ToInt32(Parameters[0]);\n\nstring commandText = " + s.fldFormul + ";\n\nNewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();\nvar tblParam = service.GetData(commandText).Tables[0];\nif (tblParam.Rows.Count > 0)\n{\n\n/*   دسترسی به پارامترها  Convert.ToInt32(tblParam.Rows[0]['EN-PARAM-NAME'])*/\n}";
                    // "\nint ElamAvarezId=Convert.ToInt32(Parameters[0]);\n\nstring commandText = "DECLARE @cols AS NVARCHAR(MAX), @query  AS NVARCHAR(MAX) SELECT @cols = STUFF((SELECT   ',' +  Drd.tblParametreSabet.fldNameParametreEn FROM   Drd.tblParametreSabet INNER JOIN Drd.tblParametreSabet_Value ON Drd.tblParametreSabet.fldId = Drd.tblParametreSabet_Value.fldParametreSabetId WHERE fldElamAvarezId=  " + ElamAvarezId + "FOR XML PATH(''), TYPE ).value('.', 'NVARCHAR(MAX)') ,1,1,'') SELECT @query =  'SELECT * FROM (SELECT   Drd.tblParametreSabet.fldNameParametreEn, Drd.tblParametreSabet_Value.fldValue FROM         Drd.tblParametreSabet INNER JOIN Drd.tblParametreSabet_Value ON Drd.tblParametreSabet.fldId = Drd.tblParametreSabet_Value.fldParametreSabetId WHERE fldElamAvarezId= " + ElamAvarezId + ")X PIVOT  ( min(fldValue) for [fldNameParametreEn] in (' + @cols + ') ) P' EXEC SP_EXECUTESQL @query ";\n\nNewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();\nvar tblParam = service.GetData(commandText).Tables[0];\nif (tblParam.Rows.Count > 0)\n{\n\n/*   دسترسی به پارامترها  Convert.ToInt32(tblParam.Rows[0]['k50'])*/\n}";
                }
                    var Defult = "";
                if (FormulId != 0)
                {
                    var organ = 0;
                    if (FieldName == "")//حکم
                        organ = OrganId/*Convert.ToInt32(Session["OrganId"])*/;
                    //var q = servic_Pay.GetComputationFormulaWithFilter("fldFormule_fldLibrary", OrganId, RankingType,"", 1, Session["Username"].ToString(), Session["Password"].ToString(), out Err_Pay).FirstOrDefault();
                    var q = servic_Pay.GetComputationFormulaDetail(FormulId, organ, IP, out Err_Pay);
                    if (Err_Pay.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err_Pay.ErrorMsg,
                            MsgTitle = "Error1",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (q != null)
                    {
                        if (q.fldFormule != "" && q.fldFormule != null)
                        {
                            Code = q.fldFormule;
                            Reference = q.fldLibrary;
                            Defult = "";
                        }
                        else if (FieldName == "formulMohasebat")
                            Defult = kk;
                    }
                    else if (FieldName == "formulMohasebat")
                        Defult = kk;
                }
                else if (FieldName == "formulMohasebat")
                    Defult = kk;
                return Json(new
                {
                    Code = Code,
                    Defult = Defult,
                    Reference = Reference,
                    Er = 0
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                return Json(new
                {
                    MsgTitle = "Error",
                    Msg = Msg+x.StackTrace,
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                string savePath = Server.MapPath(@"~\Reference\" + file.FileName);
                if (Path.GetExtension(file.FileName).ToLower() == ".dll")
                {
                    if (System.IO.File.Exists(savePath) == false)
                    {
                        file.SaveAs(savePath);
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "Error",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "Error",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }

        public ActionResult Compile(string Code, string Ref)
        {
            Code = Code.Replace("&gt;", ">").Replace("&lt;", "<");
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            var RefArray = Ref.Split(';');

            ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
            CompilerParameters loParameters = new CompilerParameters();

            // *** Start by adding any referenced assemblies

            for (int i = 0; i < RefArray.Length - 1; i++)
            {
                if (RefArray[i] == "System.dll" || RefArray[i] == "System.Data.dll" || RefArray[i] == "System.Xml.dll" || RefArray[i] == "System.Core.dll")
                {
                    loParameters.ReferencedAssemblies.Add(RefArray[i]);
                }
                else
                {
                    var Path = Server.MapPath(@"~\Reference\" + RefArray[i]);
                    loParameters.ReferencedAssemblies.Add(Path);
                }
            }
            // *** Load the resulting assembly into memory
            loParameters.GenerateInMemory = true;

            // *** Now compile the whole thing
            CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Code);

            if (loCompiled.Errors.HasErrors)
            {
                // *** Create Error String
                Er = 1;
                MsgTitle = "Error";
                Msg = loCompiled.Errors.Count.ToString() + " Errors:";
                for (int x = 0; x < loCompiled.Errors.Count; x++)
                    Msg = Msg + "\r\nLine: " + loCompiled.Errors[x].Line.ToString() + " - " +
                        loCompiled.Errors[x].ErrorText;
            }
            else
            {
                MsgTitle = "Success";
                Msg = "Compiled Successfully";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(int IDD,string FieldName,string Code, string Ref, int? OrganId, int? RankingTpe, string Id,string AzTarikh)
        {
            string Msg = ""; string MsgTitle = ""; byte Er = 0; string Idd = "0";
            try
            {
                bool? BRankingTpe = null;
                if (RankingTpe != null)
                    BRankingTpe = Convert.ToBoolean(RankingTpe);

            Code = Code.Replace("&gt;", ">").Replace("&lt;", "<");
          
            var RefArray = Ref.Split(';');

            ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
            CompilerParameters loParameters = new CompilerParameters();

            // *** Start by adding any referenced assemblies
            for (int i = 0; i < RefArray.Length - 1; i++)
            {
                if (RefArray[i] == "System.dll" || RefArray[i] == "System.Data.dll" || RefArray[i] == "System.Xml.dll" || RefArray[i] == "System.Core.dll")
                {
                    loParameters.ReferencedAssemblies.Add(RefArray[i]);
                }
                else
                {
                    var Path = Server.MapPath(@"~\Reference\" + RefArray[i]);
                    loParameters.ReferencedAssemblies.Add(Path);
                }
            }

            // *** Load the resulting assembly into memory
            loParameters.GenerateInMemory = true;

            // *** Now compile the whole thing
            CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Code);

            if (loCompiled.Errors.HasErrors)
            {
                // *** Create Error String
                Er = 1;
                MsgTitle = "Error";
                Msg = loCompiled.Errors.Count.ToString() + " Errors:";
                for (int x = 0; x < loCompiled.Errors.Count; x++)
                    Msg = Msg + "\r\nLine: " + loCompiled.Errors[x].Line.ToString() + " - " +
                        loCompiled.Errors[x].ErrorText;
            }
            else
            {
                if (Id == "0")
                {
                    Idd = servic_Pay.InsertComputationFormula(BRankingTpe, /*CodeDecode.stringcode*/(Code), OrganId, Ref, AzTarikh, IDD, FieldName, "", Session["Username"].ToString(), Session["Password"].ToString(),Convert.ToInt32(OrganId)/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                    Msg = "Saved Successfully";
                    MsgTitle = "Success";
                    if (Err_Pay.ErrorType)
                    {
                        Msg = Err_Pay.ErrorMsg;
                        MsgTitle = "Error";
                        Er = 1;
                    }
                 }
                else
                {
                    Msg = servic_Pay.UpdateComputationFormula(FieldName, Convert.ToInt32(Id), BRankingTpe, /*CodeDecode.stringcode*/(Code), OrganId, Ref, AzTarikh, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(OrganId)/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                    //Idd = Convert.ToInt32(Id);
                    Idd = Id;
                    Msg = "Updated Successfully";
                    MsgTitle = "Success";
                    if (Err_Pay.ErrorType)
                    {
                        Msg = Err_Pay.ErrorMsg;
                        MsgTitle = "Error";
                        Er = 1;
                    }
                }
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                Idd = Idd
            }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveFormul(WCF_Common_Pay.OBJ_ComputationFormula  Formul)
        {
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            try
            {

           
             string Idd = "0"; string type="0" ,Code="",Ref="";
                if(Formul.fldType==true)
                    type="1";
                if (Formul.fldId == 0)
                {
                    var q = servic_Pay.GetComputationFormulaWithFilter("fldOrgan_fldType", Formul.fldOrganId.ToString(), type, "",Convert.ToInt32(Formul.fldOrganId)/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err_Pay).FirstOrDefault();
                    if (q != null)
                    {
                        Code = q.fldFormule;
                        Ref = q.fldLibrary;
                    }
                    Idd = servic_Pay.InsertComputationFormula(Formul.fldType, /*CodeDecode.stringcode*/(Code), Formul.fldOrganId, Ref, Formul.fldAzTarikh, 0, "", "",
                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Formul.fldOrganId)/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                    if (Err_Pay.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err_Pay.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var q = servic_Pay.GetComputationFormulaDetail(Formul.fldId, Convert.ToInt32(Formul.fldOrganId)/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                    if (q != null)
                    {
                        Code = q.fldFormule;
                        Ref = q.fldLibrary;
                    }
                    Msg = servic_Pay.UpdateComputationFormula("", Formul.fldId, Formul.fldType, /*CodeDecode.stringcode*/(Code), Formul.fldOrganId, Ref, Formul.fldAzTarikh, "",
                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Formul.fldOrganId)/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);

                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                    if (Err_Pay.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err_Pay.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
           
            }
            catch (Exception x)
            {

                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id,int OrganId)
        {
            string Msg = "", MsgTitle = "";

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic_Pay.DeleteComputationFormula(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                if (Err_Pay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err_Pay.ErrorMsg;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
