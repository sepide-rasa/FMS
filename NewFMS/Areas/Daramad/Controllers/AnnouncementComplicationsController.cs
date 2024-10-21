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
    public class AnnouncementComplicationsController : Controller
    {
        //
        // GET: /Daramad/AnnouncementComplications/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(string containerId, int Id, int Info, int FiscalYearId)
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
            var tarikh = servic_Com.GetDate(IP, out Err_Com).fldTarikh;
            result.ViewBag.Id = Id;
            result.ViewBag.tarikhNow = tarikh;
            result.ViewBag.Info = Info;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult MainIndex(string containerId, int FiscalYearId)
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
            var setting = servic.GetTanzimateDaramadWithFilter("fldOrganId", Session["OrganId"].ToString(),FiscalYearId, 0, IP, out Err).ToList();
            result.ViewBag.HaveSetting = setting.Count.ToString();
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
            //return null;//برای امیریه
        }

        public ActionResult History(int ElameAvarezId)
        {
            if (Session["Username"] == null || Session["OrganId"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });             
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ElameAvarezId = ElameAvarezId;
            
            return PartialView;
        }
        public ActionResult ReadHistory(int ElameAvarezId)
        {
            if (Session["Username"] == null || Session["OrganId"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Daramad.OBJ_ElamAvarezLog> data = null;
            data = servic.GetElamAvarezLog(ElameAvarezId, IP, out Err).ToList();
            return this.Store(data);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("fldTreeOrgan", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetDaramadGroup()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDaramadGroupWithFilter("InElamAvarez", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetParams(int Id, int CodeDaramadElamAvarezID, int AshkhasId,int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string JenseKhasiyatItems = ""; string NameKhasiyat_FaItems = ""; string NameKhasiyat_EnItems = ""; string ItemPropertiesId = ""; string SharheCodeDaramad = ""; //string NoeKhasiyat = "";
            string MatnSharhCodeWithParamId = "";
            var HaveParamInSharh = 0;
            var HaveFormul = 1;

            var c = servic.GetShomareHesabCodeDaramadDetail(Id,FiscalYearId, IP, out Err);

            //مالیات بر ارزش افزوده
            if (c.fldMashmooleArzesheAfzoode)
            {
                var da = servic_Com.GetDate(IP, out Err_Com);
                var haveMaliat = servic_Com.GetMaliyatArzesheAfzoodeWithFilter("fldFromDateToEndDate", da.fldTarikh, 0, IP, out Err_Com).FirstOrDefault();
                if (haveMaliat == null)
                {
                    return Json(new
                    {
                        Msg = "مالیات بر ارزش افزوده در این تاریخ ثبت نشده است.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            // مقادیر پارامترهای ثابت
            var q1 = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", Id.ToString(), "", 0, IP, out Err).Where(l => l.fldNoe == false && l.fldTypeParametr == false/*&&l.fldVaziyat==true*/).ToList();
            foreach (var item in q1)
            {
                var q2 = servic.GetParametreSabet_NerkhWithFilter("fldTarikh_ParametreSabetId", item.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                if (q2 == null)
                {
                    return Json(new
                    {
                        Msg = "مقادیر مربوط به پارامترهای ثابت این کد درآمد، تعریف نشده است.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
            }


            if (c.fldFormulMohasebatId == null && c.fldFormolsaz == null)
                HaveFormul = 0;
            //پارامترهای محاسباتی متغیر
            var q = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", Id.ToString(), "", 0, IP, out Err).Where(l => l.fldNoe == false && l.fldTypeParametr == true/*&&l.fldVaziyat==true*/).ToList();
            foreach (var item in q)
            {
                if (item.fldVaziyat)
                {
                    JenseKhasiyatItems = JenseKhasiyatItems + item.fldNoeField + ";";
                    NameKhasiyat_FaItems = NameKhasiyat_FaItems + item.fldNameParametreFa + ";";
                    NameKhasiyat_EnItems = NameKhasiyat_EnItems + item.fldNameParametreEn + ";";
                    ItemPropertiesId = ItemPropertiesId + item.fldId + ";";
                }
            }
            var tedad = 1;
            long Mablagh = 0;
            var Tarikh = servic_Com.GetDate(IP, out Err_Com).fldTarikh;
            if (CodeDaramadElamAvarezID != 0)//ویرایش
            {
                var h = servic.GetCodhayeDaramadiElamAvarez(CodeDaramadElamAvarezID, IP, out Err);
                tedad = h.fldTedad;
                //SharheCodeDaramad = h.fldSharheCodeDaramad;
                //MatnSharhCodeWithParamId = h.fldSharheCodeDaramad;
                Mablagh = h.fldAsliValue / h.fldTedad;
                Tarikh = MyLib.Shamsi.Miladi2ShamsiString(h.fldDate);
            }
            //else
            //{

            //}
            var mahdud = 0;
            var a = servic.GetMahdoodiyatMohasebatWithFilter("fldTarikh", Tarikh, 0, IP, out Err).ToList();
            foreach (var item in a)
            {
                var Karbar = item.fldNoeKarbar;
                if (item.fldNoeKarbar == false)
                {
                    var a2 = servic.GetMohdoodiyatMohasebat_UserWithFilter("fldMahdoodiyatMohasebatId", item.fldId.ToString(), 0, IP, out Err).Where(l => l.fldIdUser == Convert.ToInt32(Session["UserId"])).FirstOrDefault();
                    if (a2 != null)
                        Karbar = true;
                }
                var Ashkhas = item.fldNoeAshkhas;
                if (item.fldNoeKarbar == false)
                {
                    var a2 = servic.GetMahdoodiyatMohasebat_AshkhasWithFilter("fldMahdoodiyatMohasebatId", item.fldId.ToString(), 0, IP, out Err).Where(l => l.fldAshkhasId == AshkhasId).FirstOrDefault();
                    if (a2 != null)
                        Ashkhas = true;
                }
                var CodeDaramad = item.fldNoeCodeDaramad;
                if (item.fldNoeKarbar == false)
                {
                    var a2 = servic.GetMahdoodiyatMohasebat_ShomareHesabDaramadWithFilter("fldMahdoodiyatMohasebatId", item.fldId.ToString(), 0, IP, out Err).Where(l => l.fldShomarehesabDarmadId == Id).FirstOrDefault();
                    if (a2 != null)
                        CodeDaramad = true;
                }

                if (Karbar && Ashkhas && CodeDaramad)
                {
                    mahdud = 1;
                    break;
                }
            }

            if (mahdud == 1)
            {
                SharheCodeDaramad = c.fldDaramadTitle;
            }
            else
            {
                if (c.fldSharhCodDaramd != null)
                {
                    SharheCodeDaramad = c.fldSharhCodDaramd.Replace("|", "");
                    MatnSharhCodeWithParamId = SharheCodeDaramad;
                    var z = c.fldSharhCodDaramd.Split('|');
                    for (byte i = 0; i < z.Length - 1; i++)
                    {
                        var s = servic.GetParametreSabetWithFilter("fldNameParametreEn", z[i], Id.ToString(),0, IP, out Err).FirstOrDefault();
                        if (s != null)
                        {
                            HaveParamInSharh = 1;
                            SharheCodeDaramad = SharheCodeDaramad.Replace(z[i], "<" + s.fldNameParametreFa + ">");
                            MatnSharhCodeWithParamId = MatnSharhCodeWithParamId.Replace(z[i], "<" + s.fldId + ">");
                            //break;
                        }
                        if (Err.ErrorType)
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    SharheCodeDaramad = c.fldDaramadTitle;
                    MatnSharhCodeWithParamId = c.fldDaramadTitle;
                }
            }

            return Json(new
            {
                Er = 0,
                JenseKhasiyatItems = JenseKhasiyatItems,
                NameKhasiyat_FaItems = NameKhasiyat_FaItems,
                NameKhasiyat_EnItems = NameKhasiyat_EnItems,
                ItemPropertiesId = ItemPropertiesId,
                NameVahed = c.fldNameVahed,
                tedad = tedad,
                MablaghAsli = Mablagh,
                SharheCodeDaramad = SharheCodeDaramad,
                MatnSharhCodeWithParamId = MatnSharhCodeWithParamId,
                HaveParamInSharh = HaveParamInSharh,
                HaveFormul = HaveFormul,
                IsMahdud = mahdud
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult combodata(string Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetParametreSabetWithFilter("fldId", Id, "", 0, IP, out Err).FirstOrDefault();
            var q2 = servic.GetComboBoxValueWithFilter("fldComboBoxId", q.fldComboBaxId.ToString(), "", 0, IP, out Err).ToList().Select(c => new { value = c.fldValue, text = c.fldTitle });
            return this.Store(q2);
        }
        public ActionResult combodataDaramadGroup(string Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDaramadGroup_ParametrWithFilter("fldId", Id, 0, IP, out Err).FirstOrDefault();
            var q2 = servic.GetComboBoxValueWithFilter("fldComboBoxId", q.fldComboBoxId.ToString(), "", 0, IP, out Err).ToList().Select(c => new { value = c.fldValue, text = c.fldTitle });
            return this.Store(q2);
        }
        public ActionResult ValidateOneParam(string ShomareHesabCodeDaramadId, string ParamId, string Value)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            bool Valid = true;
            var pa = servic.GetParametreSabetWithFilter("fldId", ParamId, "", 0, IP, out Err).FirstOrDefault();
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
                loCodeParms[2] = ShomareHesabCodeDaramadId;

                object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                Valid = (bool)loResult;

            }
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ValidateAllParams(string ShomareHesabCodeDaramadId, List<WCF_Daramad.OBJ_ParametreSabet_Value> val, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Valid = "";
            var pa = servic.GetShomareHesabCodeDaramadWithFilter("fldId", ShomareHesabCodeDaramadId, "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            if (pa.fldFormulKoliId != null)
            {
                var q = servic_ComPay.GetComputationFormulaWithFilter("fldId", pa.fldFormulKoliId.ToString(), "", "", 0, 0, IP, out Err_ComPay).FirstOrDefault();

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

                object[] loCodeParms = new object[val.Count+1];
                loCodeParms[0] = val.Count;
                foreach (var item in val)
                {
                    var i = 1;
                    var ss = servic.GetParametreSabetWithFilter("fldId", item.fldParametreSabetId.ToString(),"", 0, IP, out Err).FirstOrDefault();
                    loCodeParms[i] =ss.fldNameParametreEn+"|"+ item.fldValue.ToString();
                    i++;
                }

                
                object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                Valid = (string)loResult;
            }
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckHaveFormulId(string ShomareHesabCodeDaramadId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int ElamAvarezId = 0; var HaveFormulId = 0;

            List<WCF_Daramad.OBJ_ParametreSabet_Value> groups = new List<WCF_Daramad.OBJ_ParametreSabet_Value>();
            WCF_Daramad.OBJ_ParametreSabet_Value V = new WCF_Daramad.OBJ_ParametreSabet_Value();
            try
            {

                var k = servic.GetShomareHesabCodeDaramadDetail(Convert.ToInt32(ShomareHesabCodeDaramadId),FiscalYearId, IP, out Err);
                if (k.fldFormulMohasebatId != null)
                    HaveFormulId = 1;
                else if (k.fldFormolsaz != null)
                    HaveFormulId = 2;

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
                Er = Er,
                HaveFormulId = HaveFormulId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveMaghadir(string CodeDaramadElamAvarezID, string ShomareHesabCodeDaramadId, string AshkhasId, string OrganId, string ElamAvarezId, string Desc,
            List<WCF_Daramad.OBJ_ParametreSabet_Value> val, List<WCF_Daramad.OBJ_DaramadGroup_ParametrValues> Paramval, string MatnSharhCodeWithParamId, int? DaramadGroupId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; var HaveFormulId = 0; var EnFormul = "";
            int IDElamAvarez = 0;
            int IDCodeDaramadElamAvarez = 0;
            try
            {
                if (ElamAvarezId != "")
                    IDElamAvarez = Convert.ToInt32(ElamAvarezId);
                var k = servic.GetShomareHesabCodeDaramadDetail(Convert.ToInt32(ShomareHesabCodeDaramadId),FiscalYearId, IP, out Err);
                if (k.fldFormulMohasebatId != null)
                    HaveFormulId = 1;
                else if (k.fldFormolsaz != null)
                {
                    HaveFormulId = 2;
                    EnFormul = k.fldFormolsaz;
                }
                if (DaramadGroupId == 0)
                    DaramadGroupId = null;

                //if (Param.fldId == 0)
                //{
                //ذخیره
                MsgTitle = "ذخیره موفق";
                if (IDElamAvarez == 0)
                {
                    IDElamAvarez = servic.InsertElamAvarez(Convert.ToInt32(AshkhasId),false, Convert.ToInt32(OrganId), false, DaramadGroupId, "", Desc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
                            ElamAvarezId = ElamAvarezId
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (Paramval != null)
                    {
                        foreach (var item in Paramval)
                        {
                            servic.InsertDaramadGroup_ParametrValues(IDElamAvarez, item.fldParametrGroupDaramadId, item.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1,
                                    ElamAvarezId = ElamAvarezId
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    servic.UpdateElamAvarez(IDElamAvarez, Convert.ToInt32(AshkhasId), false, Convert.ToInt32(OrganId), DaramadGroupId, "", Desc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
                            ElamAvarezId = ElamAvarezId
                        }, JsonRequestBehavior.AllowGet);
                    }
                    servic.DeleteDaramadGroup_ParametrValues(IDElamAvarez, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Paramval != null)
                    {
                        foreach (var item in Paramval)
                        {
                            servic.InsertDaramadGroup_ParametrValues(IDElamAvarez, item.fldParametrGroupDaramadId, item.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1,
                                    ElamAvarezId = ElamAvarezId
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                if (Convert.ToInt32(CodeDaramadElamAvarezID) == 0)
                {
                    IDCodeDaramadElamAvarez = servic.InsertCodhayeDaramadiElamAvarez(IDElamAvarez, "", Convert.ToInt32(ShomareHesabCodeDaramadId), 0, 1, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
                            ElamAvarezId = ElamAvarezId
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    IDCodeDaramadElamAvarez = Convert.ToInt32(CodeDaramadElamAvarezID);
                }
                //servic.DeleteParametreSabet_Value(IDElamAvarez, Convert.ToInt32(ShomareHesabCodeDaramadId), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                servic.DeleteParametreSabet_Value(IDCodeDaramadElamAvarez,0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1,
                        ElamAvarezId = ElamAvarezId
                    }, JsonRequestBehavior.AllowGet);
                }
                if (val != null)
                {
                    foreach (var item in val)
                    {
                        servic.InsertParametreSabet_Value(IDElamAvarez, item.fldValue, item.fldParametreSabetId, IDCodeDaramadElamAvarez, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                        MatnSharhCodeWithParamId = MatnSharhCodeWithParamId.Replace("<" + item.fldParametreSabetId + ">", item.fldValue);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1,
                                ElamAvarezId = ElamAvarezId
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                //ذخیره مقادیر پارامترهای ثابت
                var q1 = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId, "", 0, IP, out Err).Where(l => l.fldNoe == false && l.fldTypeParametr == false).ToList();
                foreach (var item in q1)
                {
                    var q2 = servic.GetParametreSabet_NerkhWithFilter("fldTarikh_ParametreSabetId", item.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                    servic.InsertParametreSabet_Value(IDElamAvarez, q2.fldValue, item.fldId, IDCodeDaramadElamAvarez, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                }
                //}
                //else
                //{
                //    MsgTitle = "ویرایش موفق";
                //    Msg = servic.UpdateParametreSabet(Param.fldId, Param.fldCodhayeDaramadId, Param.fldNameParametreFa, Param.fldNameParametreEn, Param.fldNoe, Param.fldNoeField,
                //        Param.fldVaziyat, Param.fldComboBaxId, Param.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                //}
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
                Er = Er,
                ElamAvarezId = IDElamAvarez,
                HaveFormulId = HaveFormulId,
                EnFormul = EnFormul,
                CodeDaramadElamAvarezId = IDCodeDaramadElamAvarez,
                GenerateMatnSharhCode = MatnSharhCodeWithParamId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Mohasebe(int HaveFormulId, long Mablagh, string ShomareHesabCodeDaramadId, int ElamAvarezId, string sharheCodeDaramad,
            int CodeDaramadElamAvarezID, int fldTedad, int FiscalYearId/*string CodeDaramadId, int AshkhasId, List<WCF_Daramad.OBJ_ParametreSabet_Value> val*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "عملیات موفق"; var Er = 0;

            if (HaveFormulId == 1)
            {
                string Valid = "";
                var pa = servic.GetShomareHesabCodeDaramadWithFilter("fldId", ShomareHesabCodeDaramadId, "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                var q = servic_ComPay.GetComputationFormulaWithFilter("fldId", pa.fldFormulMohasebatId.ToString(), "", "", 0, 0, IP, out Err_ComPay).FirstOrDefault();
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
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

                object[] loCodeParms = new object[1];
                loCodeParms[0] = CodeDaramadElamAvarezID;
                //loCodeParms[1] = ElamAvarezId;
                //loCodeParms[2] = fldTedad;
                /*object[] loCodeParms = new object[val.Count+1];
                loCodeParms[0] = CodeDaramadId;
                int h = 0;
                foreach (var item in val)
                {
                    h++;
                    loCodeParms[h] = item.fldValue;
                }*/

                object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                /* System.Data.DataSet dt = (System.Data.DataSet)loResult;
                 dt.DataSetName = "dataSet1";
                 DataSet.DataSet1 d = new DataSet.DataSet1();

                 NewFMS.DataSet.DataSet1.tblCodhayeDaramadiElamAvarezDataTable calc = new DataSet.DataSet1.tblCodhayeDaramadiElamAvarezDataTable();
                 calc = (NewFMS.DataSet.DataSet1.tblCodhayeDaramadiElamAvarezDataTable)dt.Tables["tblCodhayeDaramadiElamAvarez"];*/

                Mablagh = (long)loResult;/*Convert.ToInt32(calc.Rows[0]["fldAsliValue"]);*/
            }

            /*  var da=servic_Com.GetDate(Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com);
              var haveMaliat = servic_Com.GetMaliyatArzesheAfzoodeWithFilter("fldFromDateToEndDate", da.fldTarikh, 0, IP, out Err_Com).FirstOrDefault();
              if (haveMaliat == null)
              {
                  return Json(new
                  {
                      Msg = "مالیات بر ارزش افزوده در این تاریخ ثبت نشده است.",
                      MsgTitle = "خطا",
                      Er = 1
                  }, JsonRequestBehavior.AllowGet);
              }*/

            /*if (CodeDaramadElamAvarezID == 0)
                Msg=servic.InsertCodhayeDaramadiElamAvarez(ElamAvarezId, sharheCodeDaramad, Convert.ToInt32(ShomareHesabCodeDaramadId), Mablagh, fldTedad, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
            else*/
            Msg = servic.UpdateCodhayeDaramadiElamAvarez(CodeDaramadElamAvarezID, ElamAvarezId, sharheCodeDaramad, Convert.ToInt32(ShomareHesabCodeDaramadId), Mablagh, fldTedad, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);



            if (Err.ErrorType)
            {
                Msg = Err.ErrorMsg;
                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
                /* fldCodhayeDaramdId=calc.Rows[0]["fldCodhayeDaramdId"].ToString(),
                 fldAsliValue=calc.Rows[0]["fldAsliValue"].ToString(),
                 fldAvarezValue=calc.Rows[0]["fldAvarezValue"].ToString(),
                 fldMaliyatValue=calc.Rows[0]["fldMaliyatValue"].ToString()*/
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelCode(int id, string ElamAvarezID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                var z = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldElamAvarezId", ElamAvarezID, 0, IP, out Err).ToList();
                if (z.Count == 1)
                {
                    Msg = servic.DeleteWithElamAvarezId(Convert.ToInt32(ElamAvarezID), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    Er = 3;
                }
                Msg = servic.DeleteCodhayeDaramadiElamAvarez(id, "fldId", 0, 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ReadCodeDaramadAvarez(StoreRequestParameters parameters, string elamAvarez)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetCodhayeDaramadiElamAvarezWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetCodhayeDaramadiElamAvarezWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldElamAvarezId", elamAvarez, 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadElamAvarez(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ElamAvarez> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ElamAvarez> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "Noe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Noe";
                            break;
                        case "fldNameShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameShakhs";
                            break;
                        case "fldShenaseMeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseMeli";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldMablaghKoli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghKoli";
                            break;
                        case "fldMablaghTakhfif":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghTakhfif";
                            break;
                        case "fldMablaghGHabelPardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghGHabelPardakht";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetElamAvarezWithFilter(field, searchtext, (Session["OrganId"]).ToString(), 100, IP, out Err).ToList();
                    else
                        data = servic.GetElamAvarezWithFilter(field, searchtext, (Session["OrganId"]).ToString(), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetElamAvarezWithFilter("", "", (Session["OrganId"]).ToString(), 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ElamAvarez> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult DetailAvarez(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetElamAvarezDetail(Id, (Session["OrganId"]).ToString(), IP, out Err);
            var DaramadGroup = "0";
            if (q.fldDaramadGroupId != null)
                DaramadGroup = q.fldDaramadGroupId.ToString();
            return Json(new
            {
                fldId = q.fldId,
                fldNoeShakhs = q.fldNoeShakhs,
                fldShenaseMeli = q.fldShenaseMeli,
                fldNameShakhs = q.fldNameShakhs,
                fldAshakhasID = q.fldAshakhasID,
                fldFather_Sabt = q.fldFather_Sabt,
                fldOrganId = q.fldOrganId.ToString(),
                fldDesc = q.fldDesc,
                fldDaramadGroupId = DaramadGroup
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailCodhayeDaramadiElamAvarez(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCodhayeDaramadiElamAvarez(Id, IP, out Err);
            return Json(new
            {
                fldID = q.fldID,
                fldShomareHesabCodeDaramadId = q.fldShomareHesabCodeDaramadId,
                codeDaramadTitle = q.codeDaramadTitle,
                fldDaramadTitle = q.fldDaramadTitle,
                fldDaramadCode = q.fldDaramadCode
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChecKHaveFish(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.CheckLastIdForElamAvarez("CheckElamAvarez", Id, IP, out Err).FirstOrDefault();
            var HaveFish = 0;
            if (q.fldType)
                HaveFish = 1;
            return Json(new
            {
                HaveFish = HaveFish
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelElamAvarez(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteWithElamAvarezId(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult DetailProperties(string Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0; var Tedad = 0;

            string[] value = new string[1];
            string[] ParametreSabetId = new string[1];
            //string[] itemPropertyName = new string[1];
            try
            {
                var h = servic.GetCodhayeDaramadiElamAvarez(Convert.ToInt32(Id), IP, out Err);
                Tedad = h.fldTedad;
                var q = servic.GetParametreSabet_ValueWithFilter("fldCodeDaramadElamAvarezId", Id, "", 0, IP, out Err).ToList();
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = Er
                    }, JsonRequestBehavior.AllowGet);
                }
                if (q.Count != 0)
                {
                    value = new string[q.Count];
                    ParametreSabetId = new string[q.Count];
                    //itemPropertyName = new string[q.Count];
                    for (int i = 0; i < q.Count; i++)
                    {
                        value[i] = q[i].fldValue;
                        //if (q[i].fldIsCombo == "1")
                        //    value[i] = q[i].fldIsComboName;
                        ParametreSabetId[i] = q[i].fldParametreSabetId.ToString();
                        //itemPropertyName[i] = q[i].fldNameKhasiyat_Fa;

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
                Er = 1;
            }
            return Json(new
            {
                value = value,
                ParametreSabetId = ParametreSabetId,
                fldTedad = Tedad,
                //itemPropertyName=itemPropertyName,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SharheCodeDaramadWin(int ShomareHesabeCodeDaramad)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ReloadSharheCodeDaramadParam(int ShomareHesabeCodeDaramad, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.Models.ParamGrid> Griddata = new List<NewFMS.Models.ParamGrid>();

            var h = servic.GetShomareHesabCodeDaramadDetail(ShomareHesabeCodeDaramad,FiscalYearId, IP, out Err);
            var z = h.fldSharhCodDaramd.Split('|');
            for (byte i = 0; i < z.Length - 1; i++)
            {
                Models.ParamGrid V = new Models.ParamGrid();
                var s = servic.GetParametreSabetWithFilter("fldNameParametreEn", z[i], ShomareHesabeCodeDaramad.ToString(), 0, IP, out Err).FirstOrDefault();
                if (s != null)
                {
                    V.ParamId = s.fldId;
                    V.ParamEn = s.fldNameParametreEn;
                    V.ParamName = s.fldNameParametreFa;
                    V.Value = "";
                    Griddata.Add(V);
                }
                if (Err.ErrorType)
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Griddata = Griddata, SharhCodDaramd = h.fldSharhCodDaramd }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveDesc(WCF_Daramad.OBJ_ElamAvarez avarez,List<WCF_Daramad.OBJ_DaramadGroup_ParametrValues> Paramval)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = ""; var Er = 0; var ElamAvarezID = 0;
            try
            {
                if (avarez.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    ElamAvarezID = servic.InsertElamAvarez(avarez.fldAshakhasID, false, avarez.fldOrganId, false, avarez.fldDaramadGroupId, "", avarez.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Paramval != null)
                    {
                        foreach (var item in Paramval)
                        {
                            servic.InsertDaramadGroup_ParametrValues(ElamAvarezID, item.fldParametrGroupDaramadId, item.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {//ویرایش
                    ElamAvarezID = avarez.fldId;
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateElamAvarez(avarez.fldId, avarez.fldAshakhasID, false, avarez.fldOrganId, avarez.fldDaramadGroupId, "", avarez.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    servic.DeleteDaramadGroup_ParametrValues(ElamAvarezID, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Paramval != null)
                    {
                        foreach (var item in Paramval)
                        {
                            servic.InsertDaramadGroup_ParametrValues(ElamAvarezID, item.fldParametrGroupDaramadId, item.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
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
                Err = Er,
                ElamAvarezID = ElamAvarezID
            }, JsonRequestBehavior.AllowGet);
        }
        private string where(string Shenase_Code,string AzTarikh, string TaTarikh, string Fish, string Taghsit, string Takhfif, string kol)
        {
            try
            {
                string s = "";
                if (AzTarikh != "" && AzTarikh != null)
                {
                    //if (s != "")
                    //    s = s + " and ";
                    s = s + " and cast(fldDate as date)>='" + MyLib.Shamsi.Shamsi2miladiString(AzTarikh) + "'";
                }
                if (TaTarikh != "" && TaTarikh != null)
                {
                    //if (s != "")
                    //    s = s + " and ";
                    s = s + " and cast(fldDate as date)<='" + MyLib.Shamsi.Shamsi2miladiString(TaTarikh) + "'";
                }
                if (Fish != "11")
                {
                    //if (s != "")
                    //    s = s + " and ";
                    s = s + " and fldStatusFish=" + Fish;
                }
                if (Taghsit != "11")
                {
                    //if (s != "")
                    //    s = s + " and ";
                    s = s + " and fldStatusTaghsit=" + Taghsit;
                }
                if (Takhfif != "11")
                {
                    //if (s != "")
                    //    s = s + " and ";
                    s = s + " and fldStatusTakhfif=" + Takhfif;
                }
                if (kol != "11")
                {
                    //if (s != "")
                    //    s = s + " and ";
                    s = s + " and fldStatusKoli=" + kol;
                }
                if (Shenase_Code != "")
                {
                    var sh = Shenase_Code.Split('(');
                    if (sh[0] == "سایر")
                    {
                        //if (s != "")
                        //    s = s + " and ";
                        s = s + " and fldNameShakhs like N'" + sh[1].Substring(0, sh[1].Length - 1)+"'";
                    }
                    else
                    {
                        //if (s != "")
                        //    s = s + " and ";
                        s = s + " and fldShenaseMeli like '" + Shenase_Code + "'";
                    }
                }
                //if (s != "")
                //    s = " WHERE " + s;

                return s;
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    return x.InnerException.Message;
                return "";
            }
        }
        public ActionResult FullSearch(string Shenase_Code, string AzTarikh, string TaTarikh, string Fish, string Taghsit, string Takhfif, string kol)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string commandText = "SELECT fldId, fldAshakhasID, fldType, fldUserId, fldDesc, fldDate ,Noe,fldNameShakhs,fldShenaseMeli " +
         " ,fldNoeShakhs,fldTarikh,fldFather_Sabt,fldOrganId,fldReplyTaghsitId,fldMablaghKoli+fldMablaghTakhfif as fldMablaghKoli,fldMablaghTakhfif,ISNULL((fldMablaghKoli/*-fldMablaghTakhfif*/),0) AS fldMablaghGHabelPardakht " +
        ",fldStatusFish,fldStatusTakhfif,fldStatusTaghsit,fldStatusKoli, " +
        "CASE WHEN fldStatusFish=N'0' then N'فیشی صادر نشده'  WHEN fldStatusFish=N'1' then N'کلیه فیش های صادرشده'  WHEN fldStatusFish=N'2' then N'فیش صادر شده'  WHEN fldStatusFish=N'3' then N'ابطال فیش' WHEN fldStatusFish=N'4' then N'ابطال کلیه فیش ها' end as fldStatusFishName, " +
        "CASE WHEN fldStatusTakhfif=N'0' then N'درخواست تخفیف صادر نشده'  WHEN fldStatusTakhfif=N'1' THEN N'موافقت شده' WHEN fldStatusTakhfif=N'2' THEN N'عدم موافقت' WHEN fldStatusTakhfif=N'3' THEN N'ابطال شده' WHEN fldStatusTakhfif=N'4' THEN N'درخواست تخفیف' end as fldStatusTakhfifName , " +
        "CASE WHEN fldStatusTaghsit=N'0' then N'درخواست تقسیط صادرنشده' WHEN fldStatusTaghsit=N'1' then N'موافقت شده' WHEN fldStatusTaghsit=N'2' then N'عدم موافقت' WHEN fldStatusTaghsit=N'3' then N'ابطال شده' WHEN fldStatusTaghsit=N'4' then N'درخواست تقسیط' end as  fldStatusTaghsitName, " +
        "CASE WHEN fldStatusKoli='1' THEN N'تسویه نقدی'  WHEN fldStatusKoli='2' THEN N'تسویه تقسیط' ELSE N'تسویه نشده' END AS fldStatusKoliName ,SharhDesc,ISNULL(fldIsExternal,0) AS fldIsExternal " +
        " FROM(SELECT fldId, fldAshakhasID, fldType, fldUserId, fldDesc, fldDate, CASE WHEN fldIsExternal = 0   THEN N'داخلی' WHEN fldIsExternal = 1 THEN N'متفرقه' else '' END AS Noe,  " +
                 " Com.fn_NameAshkhasHaghighi_Hoghoghi(fldAshakhasID) AS fldNameShakhs, Com.fn_ShenaseMelliAshkhas(fldAshakhasID) AS fldShenaseMeli,  " +
           " CASE WHEN (SELECT COUNT(*) FROM Drd.tblSodoorFish WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)=(SELECT COUNT(fldFishId) FROM Drd.tblEbtal WHERE fldFishId IN (SELECT fldId FROM Drd.tblSodoorFish WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)) THEN N'4' " +
            "WHEN EXISTS (SELECT * FROM Drd.tblSodoorFish WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldid IN (SELECT fldFishId FROM Drd.tblEbtal WHERE fldFishId=Drd.tblSodoorFish.fldId)) THEN N'3' " +
        "	WHEN (SELECT COUNT(*) FROM Drd.tblCodhayeDaramadiElamAvarez WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)=(SELECT COUNT(id) FROM ( SELECT MAX(fldId) AS id FROM Drd.tblSodoorFish_Detail WHERE fldFishId IN (SELECT fldId FROM Drd.tblSodoorFish WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId) GROUP BY fldCodeElamAvarezId)t) THEN N'1' " +
        "	WHEN EXISTS(SELECT * FROM Drd.tblSodoorFish_Detail WHERE fldCodeElamAvarezId IN (SELECT fldid FROM Drd.tblCodhayeDaramadiElamAvarez WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)) THEN N'2' " +
        "	ELSE N'0'END AS fldStatusFish " +
        "	,CASE WHEN EXISTS (SELECT * FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)AND fldTypeRequest=2 AND fldTypeMojavez=1) THEN N'1' " +
        "	WHEN EXISTS (SELECT * FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)AND fldTypeRequest=2 AND fldTypeMojavez=2) THEN N'2' " +
        "	WHEN EXISTS (SELECT * FROM Drd.tblRequestTaghsit_Takhfif WHERE fldid IN (SELECT fldRequestTaghsit_TakhfifId FROM Drd.tblEbtal WHERE fldRequestTaghsit_TakhfifId=Drd.tblRequestTaghsit_Takhfif.fldId) AND fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldRequestType=2) THEN N'3' " +
        "	WHEN EXISTS (SELECT * FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldRequestType=2) THEN N'4' " +
        "	ELSE N'0' END AS fldStatusTakhfif " +
        "	,CASE WHEN EXISTS (SELECT * FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)AND fldTypeRequest=1 AND fldTypeMojavez=1) THEN N'1' " +
        "	WHEN EXISTS (SELECT * FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)AND fldTypeRequest=1 AND fldTypeMojavez=2) THEN N'2' " +
        "	WHEN EXISTS (SELECT * FROM Drd.tblRequestTaghsit_Takhfif WHERE fldid IN (SELECT fldRequestTaghsit_TakhfifId FROM Drd.tblEbtal WHERE fldRequestTaghsit_TakhfifId=Drd.tblRequestTaghsit_Takhfif.fldId) AND fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldRequestType=1) THEN N'3' " +
        "	WHEN EXISTS (SELECT * FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldRequestType=1) THEN N'4' " +
        "	ELSE N'0' END AS fldStatusTaghsit, " +
        "	CASE WHEN ((SELECT  sum(cast(cast(fldAsliValue as bigint)*cast(fldtedad as bigint)+cast(fldMaliyatValue as bigint)+cast(fldAvarezValue as bigint)as bigint)) FROM Drd.tblCodhayeDaramadiElamAvarez WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId) " +
        "	=(SELECT isnull(SUM(cast(fldMablaghSanad as bigint)),0) FROM Drd.tblCheck WHERE fldStatus=2 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldid NOT IN (SELECT fldRequestTaghsit_TakhfifId FROM Drd.tblEbtal WHERE fldRequestTaghsit_TakhfifId=Drd.tblRequestTaghsit_Takhfif.fldId))))) " +
        "	+(SELECT isnull(SUM(cast(fldMablaghSanad as bigint)),0) FROM Drd.tblSafte WHERE fldStatus=2 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldid NOT IN (SELECT fldRequestTaghsit_TakhfifId FROM Drd.tblEbtal WHERE fldRequestTaghsit_TakhfifId=Drd.tblRequestTaghsit_Takhfif.fldId))))) " +
        "	+(SELECT isnull(SUM(cast(fldMablaghSanad as bigint)),0) FROM Drd.tblBarat WHERE fldStatus=2 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldid NOT IN (SELECT fldRequestTaghsit_TakhfifId FROM Drd.tblEbtal WHERE fldRequestTaghsit_TakhfifId=Drd.tblRequestTaghsit_Takhfif.fldId))))) " +
        "	+(SELECT isnull(SUM(cast(fldMablagh as bigint)),0) FROM Drd.tblNaghdi_Talab WHERE fldType=0 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldid NOT IN (SELECT fldRequestTaghsit_TakhfifId FROM Drd.tblEbtal WHERE fldRequestTaghsit_TakhfifId=Drd.tblRequestTaghsit_Takhfif.fldId))))) " +
        "	)OR ((SELECT (SUM((fldAsliValue+fldMaliyatValue+fldAvarezValue)*fldTedad) /(SELECT fldMablaghGerdKardan FROM Drd.tblTanzimateDaramad WHERE tblTanzimateDaramad.fldOrganId=Drd.tblElamAvarez.fldOrganId and fldMablaghGerdKardan<>0))*(SELECT fldMablaghGerdKardan FROM Drd.tblTanzimateDaramad WHERE tblTanzimateDaramad.fldOrganId=Drd.tblElamAvarez.fldOrganId) FROM Drd.tblCodhayeDaramadiElamAvarez WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId) " +
        "	=(SELECT SUM(fldMablaghAvarezGerdShode) FROM Drd.tblSodoorFish WHERE fldid IN (SELECT fldFishId FROM Drd.tblSodoorFish_Detail WHERE fldFishId=Drd.tblSodoorFish.fldId)AND fldElamAvarezId=Drd.tblElamAvarez.fldId AND fldid NOT IN (SELECT fldFishId FROM Drd.tblEbtal WHERE fldFishId=Drd.tblSodoorFish.fldId)) ) THEN N'1' " +
        "	WHEN (SELECT  sum(cast(cast(fldAsliValue as bigint)*cast(fldtedad as bigint)+cast(fldMaliyatValue as bigint)+cast(fldAvarezValue as bigint)as bigint)) FROM Drd.tblCodhayeDaramadiElamAvarez WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId) " +
        "	=(SELECT isnull(SUM(cast(fldMablaghSanad as bigint)),0) FROM Drd.tblCheck WHERE fldStatus<>5 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)))) " +
        "	+(SELECT isnull(SUM(cast(fldMablaghSanad as bigint)),0) FROM Drd.tblSafte WHERE fldStatus<>5 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)))) " +
        "	+(SELECT isnull(SUM(cast(fldMablaghSanad as bigint)),0) FROM Drd.tblBarat WHERE fldStatus<>5 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)))) " +
        "	+(SELECT isnull(SUM(cast(fldMablagh as bigint)),0) FROM Drd.tblNaghdi_Talab WHERE fldType=0 AND fldReplyTaghsitId IN (SELECT fldId FROM Drd.tblReplyTaghsit WHERE fldStatusId IN (SELECT  fldid FROM Drd.tblStatusTaghsit_Takhfif WHERE fldRequestId IN (SELECT fldId FROM Drd.tblRequestTaghsit_Takhfif WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId)))) " +
        "	THEN N'2' ELSE '0' END AS fldStatusKoli , " +
            "         (SELECT CASE WHEN fldHaghighiId IS NOT NULL THEN '0' WHEN fldHoghoghiId IS NOT NULL THEN '1' END AS Expr1 " +
              "         FROM      Com.tblAshkhas " +
             "          WHERE   (fldId = Drd.tblElamAvarez.fldAshakhasID)) AS fldNoeShakhs, Com.MiladiTOShamsi(fldDate) AS fldTarikh, " +
               "    isnull(   (SELECT CASE WHEN fldHaghighiId IS NOT NULL THEN(SELECT fldFatherName FROM  Com.tblEmployee_Detail WHERE   fldEmployeeId = fldHaghighiId) WHEN fldHoghoghiId IS NOT NULL THEN " +
                "      (SELECT fldShomareSabt FROM      Com.tblAshkhaseHoghoghi WHERE   fldid = fldHoghoghiId) END AS Expr1 FROM Com.tblAshkhas AS tblAshkhas_1 WHERE   (fldId = Drd.tblElamAvarez.fldAshakhasID)),'') AS fldFather_Sabt, fldOrganId " +
                "      ,isnull((select drd.tblReplyTaghsit.fldid from drd.tblReplyTaghsit where drd.tblReplyTaghsit.fldStatusId=( "+
            " select drd.tblStatusTaghsit_Takhfif.fldid from drd.tblStatusTaghsit_Takhfif where drd.tblStatusTaghsit_Takhfif.fldRequestId=( "+
            " select top(1)drd.tblRequestTaghsit_Takhfif.fldId from drd.tblRequestTaghsit_Takhfif where drd.tblRequestTaghsit_Takhfif.fldElamAvarezId=Drd.tblElamAvarez.fldid and drd.tblRequestTaghsit_Takhfif.fldId "+
            " not in(select drd.tblEbtal.fldRequestTaghsit_TakhfifId from drd.tblEbtal "+
            " where drd.tblEbtal.fldRequestTaghsit_TakhfifId=drd.tblRequestTaghsit_Takhfif.fldid) order by drd.tblRequestTaghsit_Takhfif.fldid desc "+
            " ) and tblStatusTaghsit_Takhfif.fldTypeMojavez=1 and tblStatusTaghsit_Takhfif.fldTypeRequest=1 "+
            " )),0) fldReplyTaghsitId" +
                ",isnull((SELECT isnull(SUM(fldTakhfifMaliyatValue+fldSumAsli+fldTakhfifAvarezValue),SUM(fldMaliyatValue+fldSumAsli+fldAvarezValue)) FROM Drd.tblCodhayeDaramadiElamAvarez WHERE fldElamAvarezId=Drd.tblElamAvarez.fldId),0) fldMablaghKoli, " +
                "		ISNULL(cast(drd.Fn_MablaghTakhfif('MablaghTakhfif_ElamAvarez',fldId) AS bigint),CAST(0 AS BIGINT)) AS fldMablaghTakhfif" +
             " ,(SELECT com.fn_stringDecode(fldName) FROM Com.tblOrganization WHERE fldid=Drd.tblElamAvarez.fldOrganId)AS fldNameOrgan " +
            " ,drd.fn_SharheCode_ElamAvarez(fldid) AS SharhDesc,fldIsExternal " +
        "FROM     Drd.tblElamAvarez)t " +
        " where fldOrganId= " +(Session["OrganId"]).ToString()+
                     where(Shenase_Code, AzTarikh, TaTarikh, Fish, Taghsit, Takhfif, kol) + " ORDER BY fldid DESC";

            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var avarez = service.GetData(commandText).Tables[0];

            List<WCF_Daramad.OBJ_ElamAvarez> p = new List<WCF_Daramad.OBJ_ElamAvarez>();
            for (int i = 0; i < avarez.Rows.Count; i++)
            {
                WCF_Daramad.OBJ_ElamAvarez l = new WCF_Daramad.OBJ_ElamAvarez();
                l.fldId = (int)(avarez.Rows[i]["fldId"]);
                l.fldAshakhasID = (int)(avarez.Rows[i]["fldAshakhasID"]);
                l.fldFather_Sabt = (string)(avarez.Rows[i]["fldFather_Sabt"]);
                l.fldMablaghGHabelPardakht = (long)(avarez.Rows[i]["fldMablaghGHabelPardakht"]);
                l.fldMablaghKoli = (long)(avarez.Rows[i]["fldMablaghKoli"]);
                l.fldMablaghTakhfif = (long)(avarez.Rows[i]["fldMablaghTakhfif"]);
                l.fldNameShakhs = (string)(avarez.Rows[i]["fldNameShakhs"]);
                l.fldNoeShakhs = (string)(avarez.Rows[i]["fldNoeShakhs"]);
                l.fldOrganId = (int)(avarez.Rows[i]["fldOrganId"]);
                l.fldReplyTaghsitId = (int)(avarez.Rows[i]["fldReplyTaghsitId"]);
                l.fldShenaseMeli = (string)(avarez.Rows[i]["fldShenaseMeli"]);
                l.fldStatusFish = (string)(avarez.Rows[i]["fldStatusFish"]);
                l.fldStatusFishName = (string)(avarez.Rows[i]["fldStatusFishName"]);
                l.fldStatusKoli = (string)(avarez.Rows[i]["fldStatusKoli"]);
                l.fldStatusKoliName = (string)(avarez.Rows[i]["fldStatusKoliName"]);
                l.fldStatusTaghsit = (string)(avarez.Rows[i]["fldStatusTaghsit"]);
                l.fldStatusTaghsitName = (string)(avarez.Rows[i]["fldStatusTaghsitName"]);
                l.fldStatusTakhfif = (string)(avarez.Rows[i]["fldStatusTakhfif"]);
                l.fldStatusTakhfifName = (string)(avarez.Rows[i]["fldStatusTakhfifName"]);
                l.fldTarikh = (string)(avarez.Rows[i]["fldTarikh"]);
                l.fldType = (bool)(avarez.Rows[i]["fldType"]);
                l.SharhDesc = (string)(avarez.Rows[i]["SharhDesc"]);
                l.fldDesc = (string)(avarez.Rows[i]["fldDesc"]);
                l.fldIsExternal = (bool)(avarez.Rows[i]["fldIsExternal"]);
                l.Noe = (string)(avarez.Rows[i]["Noe"]);
                p.Add(l);
            }

            return Json(p.ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckCodeDaramad(string CodeDaramad, string OrganId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; string TitleCodeDaramad = ""; var fldId = 0;
            try
            {

                var k = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_fldDaramadCode", CodeDaramad, OrganId,FiscalYearId, 0, IP, out Err).FirstOrDefault();
                if (k == null)
                {
                    MsgTitle = "خطا";
                    Msg = "کد درآمد وارد شده نامعتبر است.";
                    Er = 2;
                }
                else
                {
                    TitleCodeDaramad = k.fldDaramadTitle;
                    fldId = k.fldId;
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
                Err = Er,
                TitleCodeDaramad = TitleCodeDaramad,
                fldId = fldId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WinShowFormul(int ShomareHesabCodeDaramadId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ShomareHesabCodeDaramadId = ShomareHesabCodeDaramadId;
            return PartialView;
        }
        public ActionResult LoadFormulForWin(int ShomareHesabCodeDaramadId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string MatnFromul = "";
            var typeF = 0;
            var q = servic.GetShomareHesabCodeDaramadDetail(ShomareHesabCodeDaramadId,FiscalYearId, IP, out Err);
            if (q.fldFormolsaz != null)
            {
                typeF = 1;
                var s = q.fldFormolsaz.Split(';');
                for (int i = 0; i < s.Length - 1; i++)
                {
                    var f = servic.GetParametreSabetWithFilter("fldNameParametreEn", s[i], ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                    if (f != null)
                    {
                        MatnFromul = MatnFromul + f.fldNameParametreFa;
                    }
                    else
                    {
                        var f1 = servic.GetParametreOmoomiWithFilter("fldNameParametreEn", s[i], 0, IP, out Err).FirstOrDefault();
                        if (f1 != null)
                        {
                            MatnFromul = MatnFromul + f1.fldNameParametreFa;
                        }
                        else
                        {
                            MatnFromul = MatnFromul + s[i];
                        }
                    }
                }
                MatnFromul = MatnFromul.Replace("if(", "اگر(").Replace("then", "آنگاه ").Replace("else", "درغیراین صورت ");
            }
            else if (q.fldFormulMohasebatId != null)
            {
                typeF = 2;
                var q2 = servic_ComPay.GetComputationFormulaDetail(Convert.ToInt32(q.fldFormulMohasebatId), 0, IP, out Err_ComPay);
                MatnFromul = q2.fldFormule;
            }
            MatnFromul = MatnFromul.Replace('{', '[');
            MatnFromul = MatnFromul.Replace('}', ']');
            return Json(new
            {
                Formul = MatnFromul,
                typeF = typeF
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckCode_ShenaseMelli(string Code_ShenaseMelli, string Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //Type=0 حقیقی
            string Msg = "", MsgTitle = ""; var Er = 0; string fldShenase_CodeMeli = ""; var fldId = 0;
            string FName = ""; string shomareshabt_father = "";
            try
            {

                var k = servic_Com.GetAshkhasWithFilter("fldShenase_CodeMeli", Code_ShenaseMelli, Type, 0, IP, out Err_Com).FirstOrDefault();
                if (k == null)
                {
                    MsgTitle = "خطا";
                    Msg = "کد/شناسه ملی وارد شده، در سیستم موجود نمی باشد. آیا مایل به ذخیره آن هستید؟";
                    Er = 2;
                }
                else
                {
                    fldShenase_CodeMeli = k.fldShenase_CodeMeli;
                    fldId = k.fldId;
                    FName = k.Name;
                    shomareshabt_father = k.shomareshabt_father;
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
                Err = Er,
                fldShenase_CodeMeli = fldShenase_CodeMeli,
                fldId = fldId,
                Name = FName,
                shomareshabt_father = shomareshabt_father
            }, JsonRequestBehavior.AllowGet);
        }
        public int Formulator(int ElamAvarezId)
        {
            string commandText = "DECLARE @cols AS NVARCHAR(MAX), " +
            "@query  AS NVARCHAR(MAX) " +
            "SELECT @cols = STUFF((SELECT   ',' +  Drd.tblParametreSabet.fldNameParametreEn " +
            "FROM         Drd.tblParametreSabet INNER JOIN " +
                                     "Drd.tblParametreSabet_Value ON Drd.tblParametreSabet.fldId = Drd.tblParametreSabet_Value.fldParametreSabetId " +
                                    "WHERE fldElamAvarezId=  " + ElamAvarezId +
                        "FOR XML PATH(''), TYPE " +
                        ").value('.', 'NVARCHAR(MAX)')  " +
                    ",1,1,'') " +
            "SELECT @query =  " +
            "'SELECT * FROM " +
            "(SELECT   Drd.tblParametreSabet.fldNameParametreEn, Drd.tblParametreSabet_Value.fldValue " +
            "FROM         Drd.tblParametreSabet INNER JOIN " +
                                     "Drd.tblParametreSabet_Value ON Drd.tblParametreSabet.fldId = Drd.tblParametreSabet_Value.fldParametreSabetId " +
                                    "WHERE fldElamAvarezId= " + ElamAvarezId + ")X " +
            "PIVOT  " +
            "( " +
                "min(fldValue) " +
                "for [fldNameParametreEn] in (' + @cols + ') " +
            ") P' " +
            "EXEC SP_EXECUTESQL @query ";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();

            var tblParam = service.GetData(commandText).Tables[0];
            var MM = 0.0;

            var m1 = 0; var m2 = 0; var m3 = 0.0; var m4 = 0; var litr = 0; var mablagh = 0; var Kilumetr = 0; var m1030 = 0; var m3050 = 0; var m50 = 0;
            if (tblParam.Rows.Count > 0)
            {
                Kilumetr = Convert.ToInt32(tblParam.Rows[0]["k50"]);
                mablagh = Convert.ToInt32(tblParam.Rows[0]["mablagh"]);
                litr = Convert.ToInt32(tblParam.Rows[0]["litr"]);
                m1030 = Convert.ToInt32(tblParam.Rows[0]["m1030"]);
                m3050 = Convert.ToInt32(tblParam.Rows[0]["m3050"]);
                m50 = Convert.ToInt32(tblParam.Rows[0]["m50"]);

                if ((Kilumetr - 10) < 0)
                    m1 = Kilumetr * 10;
                else
                {
                    Kilumetr = Kilumetr - 10;
                    m1 = 10 * 10;

                    if ((Kilumetr - 20) < 0)
                        m2 = Kilumetr * m1030;
                    else
                    {
                        Kilumetr = Kilumetr - 20;
                        m2 = 20 * m1030;

                        if ((Kilumetr - 20) < 0)
                            m3 = Kilumetr * m3050 / 2.0;
                        else
                        {
                            Kilumetr = Kilumetr - 20;
                            m3 = 20 * m3050 / 2.0;

                            if (Kilumetr != 0)
                                m4 = Kilumetr * m50;
                        }
                    }
                }

                MM = (m1 + m2 + m3 + m4 + mablagh) * litr;
            }
            return Convert.ToInt32(MM);
        }
        public ActionResult MohasebeBaMahdudiat(string CodeDaramadElamAvarezID, string ShomareHesabCodeDaramadId, string AshkhasId, string OrganId, string ElamAvarezId, string Desc, string sharheCodeDaramad, int fldTedad, long Mablagh, int? DaramadGroupId,List<WCF_Daramad.OBJ_DaramadGroup_ParametrValues> Paramval)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "اعلام عوارض با موفقیت ذخیره شد.", MsgTitle = "عملیات موفق"; var Er = 0; 
            int IDElamAvarez = 0;
            int IDCodeDaramadElamAvarez = 0;
            try
            {
                if (ElamAvarezId != "")
                    IDElamAvarez = Convert.ToInt32(ElamAvarezId);
                if (DaramadGroupId == 0)
                    DaramadGroupId = null;
                //if (Param.fldId == 0)
                //{
                //ذخیره 
                if (IDElamAvarez == 0)
                {
                    IDElamAvarez = servic.InsertElamAvarez(Convert.ToInt32(AshkhasId), false, Convert.ToInt32(OrganId), false, DaramadGroupId, "", Desc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
                            ElamAvarezId = ElamAvarezId
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (Paramval != null)
                    {
                        foreach (var item in Paramval)
                        {
                            servic.InsertDaramadGroup_ParametrValues(IDElamAvarez, item.fldParametrGroupDaramadId, item.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1,
                                    ElamAvarezId = ElamAvarezId
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    servic.UpdateElamAvarez(IDElamAvarez, Convert.ToInt32(AshkhasId), false, Convert.ToInt32(OrganId), DaramadGroupId, "", Desc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
                            ElamAvarezId = ElamAvarezId
                        }, JsonRequestBehavior.AllowGet);
                    }
                    servic.DeleteDaramadGroup_ParametrValues(IDElamAvarez, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Paramval != null)
                    {
                        foreach (var item in Paramval)
                        {
                            servic.InsertDaramadGroup_ParametrValues(IDElamAvarez, item.fldParametrGroupDaramadId, item.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1,
                                    ElamAvarezId = ElamAvarezId
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                if (Convert.ToInt32(CodeDaramadElamAvarezID) == 0)
                {
                    servic.InsertCodhayeDaramadiElamAvarez_External(IDElamAvarez, sharheCodeDaramad, Convert.ToInt32(ShomareHesabCodeDaramadId), Mablagh, fldTedad,0,0, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
                            ElamAvarezId = ElamAvarezId
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    servic.UpdateCodhayeDaramadiElamAvarez_External(Convert.ToInt32(CodeDaramadElamAvarezID), IDElamAvarez, sharheCodeDaramad, Convert.ToInt32(ShomareHesabCodeDaramadId), Mablagh, 0,0,fldTedad, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                     if (Err.ErrorType)
                     {
                         return Json(new
                         {
                             Msg = Err.ErrorMsg,
                             MsgTitle = "خطا",
                             Er = 1,
                             ElamAvarezId = ElamAvarezId
                         }, JsonRequestBehavior.AllowGet);
                     }
                     servic.DeleteParametreSabet_Value(Convert.ToInt32(CodeDaramadElamAvarezID), 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                     if (Err.ErrorType)
                     {
                         return Json(new
                         {
                             Msg = Err.ErrorMsg,
                             MsgTitle = "خطا",
                             Er = 1,
                             ElamAvarezId = ElamAvarezId
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
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                IDElamAvarez = IDElamAvarez
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetParamsDaramadGroup(int DaramadGroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string NameKhasiyat_FaItems = ""; string NameKhasiyat_EnItems = ""; string ItemPropertiesId = ""; string JenseKhasiyatItems = "";
           


            var q = servic.GetDaramadGroup_ParametrWithFilter("fldDaramadGroupId", DaramadGroupId.ToString(),  0, IP, out Err).Where(l => l.fldStatus ==  true).ToList();
            if (Err.ErrorType)
            {
                return Json(new
                {
                    Msg = Err.ErrorMsg,
                    MsgTitle = "خطا",
                    Er = 1
                }, JsonRequestBehavior.AllowGet);
            }
            foreach (var item in q)
            {
                    NameKhasiyat_FaItems = NameKhasiyat_FaItems + item.fldFnName + ";";
                    NameKhasiyat_EnItems = NameKhasiyat_EnItems + item.fldEnName + ";";
                    ItemPropertiesId = ItemPropertiesId + item.fldId + ";";
                    JenseKhasiyatItems = JenseKhasiyatItems + item.fldNoeField + ";";
            }
           

            return Json(new
            {
                Er = 0,
                NameKhasiyat_FaItems = NameKhasiyat_FaItems,
                NameKhasiyat_EnItems = NameKhasiyat_EnItems,
                ItemPropertiesId = ItemPropertiesId,
                JenseKhasiyatItems = JenseKhasiyatItems
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ParamsDaramadGroupValue(string ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0; 

            string[] value = new string[1];
            string[] ParametrId = new string[1];
            //string[] itemPropertyName = new string[1];
            try
            {
                var q = servic.GetDaramadGroup_ParametrValuesWithFilter("fldElamAvarezId", ElamAvarezId,  0, IP, out Err).ToList();
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = Er
                    }, JsonRequestBehavior.AllowGet);
                }
                if (q.Count != 0)
                {
                    value = new string[q.Count];
                    ParametrId = new string[q.Count];
                    //itemPropertyName = new string[q.Count];
                    for (int i = 0; i < q.Count; i++)
                    {
                        value[i] = q[i].fldValue;
                        //if (q[i].fldIsCombo == "1")
                        //    value[i] = q[i].fldIsComboName;
                        ParametrId[i] = q[i].fldParametrGroupDaramadId.ToString();
                        //itemPropertyName[i] = q[i].fldNameKhasiyat_Fa;

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
                Er = 1;
            }
            return Json(new
            {
                value = value,
                ParametrId = ParametrId,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ValidateDaramadGroupParam(string DaramadGroupId, string ParamId, string Value)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            bool Valid = true;
            var pa = servic.GetDaramadGroup_ParametrWithFilter("fldId", ParamId, 0, IP, out Err).FirstOrDefault();
            if (pa.fldFormuleId != null)
            {
                var q = servic_ComPay.GetComputationFormulaWithFilter("fldId", pa.fldFormuleId.ToString(), "", "", 0, 0, IP, out Err_ComPay).FirstOrDefault();

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
                loCodeParms[2] = DaramadGroupId;

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
