using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class IncomeCodesController : Controller
    {
        //
        // GET: /Daramad/IncomeCodes/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        WCF_Accounting.AccountingService accservic = new WCF_Accounting.AccountingService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Accounting.ClsError accErr = new WCF_Accounting.ClsError();
        WCF_Common.CommonService servicCom = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
        public ActionResult Index(string containerId,int FiscalYearId)
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
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult IndexParamFormul(string containerId, int FiscalYearId)
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
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult HelpIncomeCodes()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetUnit()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCom.GetMeasureUnitWithFilter("", "", 0, IP, out ErrCom).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameVahed });
            return this.Store(q);
        }
        //public ActionResult CheckCopy(string ShomareHesabCodeDaramadId)
        //{
        //    bool q = false;
        //    var ParamS = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId, "", 1, IP, out Err).FirstOrDefault();
        //}
        public ActionResult FormulSaz(int ShomareHesabCodeDaramadId, string Formul,/* int? FiscalYearId,*/int Id,string Tarikh)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ShomareHesabCodeDaramadId = ShomareHesabCodeDaramadId;
            PartialView.ViewBag.Formul = Formul;
            //PartialView.ViewBag.FiscalYearId = FiscalYearId;
            PartialView.ViewBag.Id = Id;
            PartialView.ViewBag.Tarikh = Tarikh;
            return PartialView;
        }
        public ActionResult ListFormul(int ShomareHesabCodeDaramadId,int? FiscalYearId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var CurrDate=servicCom.GetDate(IP,out ErrCom).fldTarikh;
            PartialView.ViewBag.ShomareHesabCodeDaramadId = ShomareHesabCodeDaramadId;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            PartialView.ViewBag.CurrDate = CurrDate;
            return PartialView;
        }
        public ActionResult HelpFormulSaz()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult DetailsFormul(int Id, int ShomareHesabCodeDaramadId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetShomareHesabCodeDaramad_Fomula("fldId", Id.ToString(), 1, Convert.ToInt32(Session["OrganId"]), IP, out Err).FirstOrDefault();
            string CurrentText = "";
            string Formul = "";
            var s = q.fldFormolsaz.Split(';');
            for (int i = 0; i < s.Length - 1; i++)
            {
                var f = servic.GetParametreSabetWithFilter("fldNameParametreEn", s[i],ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                if (f != null)
                {
                    CurrentText = CurrentText + f.fldNameParametreFa + ";";
                    Formul = Formul + f.fldNameParametreFa;
                }
                else {
                    var f1 = servic.GetParametreOmoomiWithFilter("fldNameParametreEn", s[i], 0, IP, out Err).FirstOrDefault();
                    if (f1 != null)
                    {
                        CurrentText = CurrentText + f1.fldNameParametreFa + ";";
                        Formul = Formul + f1.fldNameParametreFa;
                    }
                    else
                    {
                        CurrentText = CurrentText + s[i] + ";";
                        Formul = Formul + s[i];
                    }
                }                
            }
            CurrentText = CurrentText.Replace("if(", "اگر(").Replace("then", "آنگاه ").Replace("else", "درغیراین صورت ");
            Formul = Formul.Replace("if(", "اگر(").Replace("then", "آنگاه ").Replace("else", "درغیراین صورت ");
            return Json(new
            {
                Formul = Formul,
                CurrentText = CurrentText,
                EnFormul = q.fldFormolsaz
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveFormul(WCF_Daramad.OBJ_ShomareHesabCodeDaramad CodhayeDaramd,int Id,string Tarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Id == 0)
                {
                    Msg=servic.InsertShomareHesabCodeDaramad_Fomula(CodhayeDaramd.fldId, CodhayeDaramd.fldFormolsaz, Tarikh, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    Msg=servic.UpdateShomareHesabCodeDaramad_Fomula(Id, CodhayeDaramd.fldId, CodhayeDaramd.fldFormolsaz, Tarikh, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                //Msg = servic.UpdateFormolsazForCodhayeDarmd(CodhayeDaramd.fldId, CodhayeDaramd.fldFormolsaz, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);برای قبل که به ازای هر کد درآمد تنها یک فرمول داشتیم
                MsgTitle = "عملیات موفق";
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
        public static double? Calculate(string Formoul)
        {
            Models.FM F = new Models.FM();
            int num = 12;
            double result;
            if (F.Neu(Formoul, out result) || num != 21)
                return new double?(result);
            return new double?();
        }

        private static int CountOf(string STR, char CHAR)
        {
            int num1 = 0;
            foreach (int num2 in STR)
            {
                if (num2 == (int)CHAR)
                    ++num1;
            }
            return num1;
        }

        private static bool Removeable(string temp)
        {
            if (temp.Length < 1 || (int)temp[0] != 40 && (int)temp[0] != 91 || (int)temp[temp.Length - 1] != 41 && (int)temp[temp.Length - 1] != 93)
                return false;
            int num = 0;
            for (int index = 0; index < temp.Length; ++index)
            {
                if ((int)temp[index] == 40 || (int)temp[index] == 91)
                    ++num;
                if ((int)temp[index] == 41 || (int)temp[index] == 93)
                {
                    --num;
                    if (num == 0 && index < temp.Length - 1)
                        return false;
                    if (num == 0 && index == temp.Length - 1)
                        return true;
                }
            }
            return true;
        }

        private static void spliteStatment(string temp, out string _prefix, out string _if, out string _then, out string _else, out string _postfix)
        {
            bool flag = false;
            do
            {
                if (temp.Length > 0 && (flag = Removeable(temp)))
                    temp = temp.Substring(1, temp.Length - 2);
            }
            while (flag);
            _prefix = "";
            if (!temp.StartsWith("if("))
            {
                for (int index = 2; index < temp.Length; ++index)
                {
                    if ((int)temp[index - 2] == 105 && (int)temp[index - 1] == 102 && (int)temp[index] == 40)
                    {
                        _prefix = temp.Substring(0, index - 2);
                        if (((object)temp.Substring(index - 2)).ToString() != "")
                        {
                            temp = temp.Substring(index - 2);
                            break;
                        }
                    }
                }
            }
            do
            {
                if (temp.Length > 0 && (flag = Removeable(temp)))
                    temp = temp.Substring(1, temp.Length - 2);
            }
            while (flag);
            int num = 0;
            int index1;
            for (index1 = 2; index1 < temp.Length; ++index1)
            {
                if ((int)temp[index1] == 40 || (int)temp[index1] == 91)
                    ++num;
                if ((int)temp[index1] == 41 || (int)temp[index1] == 93)
                    --num;
                if ((int)temp[index1] == 44 && num == 0)
                    break;
            }
            _if = temp.Substring(temp.IndexOf("if(") + 3, index1 - 4);
            int index2;
            for (index2 = index1 + 1; index2 < temp.Length; index2++)
            {
                if ((int)temp[index2] == 40 || (int)temp[index2] == 91)
                    num++;
                if ((int)temp[index2] == 41 || (int)temp[index2] == 93)
                    num--;
                if ((int)temp[index2] == 44 && num == 0)
                    break;
            }
            _then = temp.Substring(temp.IndexOf(_if) + _if.Length + 2, index2 - (temp.IndexOf(_if) + _if.Length + 2));
            _postfix = "";
            if (index2 == temp.Length)
            {
                _else = "";
            }
            else
            {
                int index3;
                for (index3 = index2 + 1; index3 < temp.Length; ++index3)
                {
                    if ((int)temp[index3] == 40 || (int)temp[index3] == 91)
                        ++num;
                    else if (((int)temp[index3] == 41 || (int)temp[index3] == 93) && num != 0)
                        --num;
                    else if (((int)temp[index3] == 41 || (int)temp[index3] == 93) && num == 0)
                        break;
                }
                _else = temp.Substring(temp.IndexOf("if(" + _if + "," + _then) + _if.Length + _then.Length + 7, index3 - (temp.IndexOf("if(" + _if + "," + _then) + _if.Length + _then.Length + 7));
                _postfix = temp.Substring(index3);
            }
        }

        private static bool Condition(string _if)
        {
            if (_if.Contains("!="))
            {
                string[] strArray = _if.Split(new string[1]
        {
          "!="
        }, StringSplitOptions.None);
                double? nullable1 = Calculate(strArray[0]);
                double? nullable2 = Calculate(strArray[1]);
                return (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : (nullable1.HasValue != nullable2.HasValue ? 1 : 0)) != 0;
            }
            else if (_if.Contains("="))
            {
                if (CountOf(_if, '=') > 1)
                    throw new Exception("Error in =");
                string[] strArray = _if.Split(new string[1]
        {
          "="
        }, StringSplitOptions.None);
                double? nullable1 = Calculate(strArray[0]);
                double? nullable2 = Calculate(strArray[1]);
                return (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue == nullable2.HasValue ? 1 : 0)) != 0;
            }
            else if (_if.Contains("<"))
            {
                if (CountOf(_if, '<') > 2)
                    throw new Exception("Error in <");
                string[] strArray = _if.Split(new string[1]
        {
          "<"
        }, StringSplitOptions.None);
                if (CountOf(_if, '<') == 1)
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    return (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0;
                }
                else
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    int num;
                    if ((nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0)
                    {
                        nullable1 = Calculate(strArray[1]);
                        nullable2 = Calculate(strArray[2]);
                        num = (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) == 0 ? 1 : 0;
                    }
                    else
                        num = 1;
                    return num == 0;
                }
            }
            else
            {
                if (!_if.Contains(">"))
                    return true;
                if (CountOf(_if, '>') > 2)
                    throw new Exception("Error in >");
                string[] strArray = _if.Split(new string[1]
        {
          ">"
        }, StringSplitOptions.None);
                if (CountOf(_if, '>') == 1)
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    return (nullable1.GetValueOrDefault() <= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0;
                }
                else
                {
                    double? nullable1 = Calculate(strArray[0]);
                    double? nullable2 = Calculate(strArray[1]);
                    int num;
                    if ((nullable1.GetValueOrDefault() <= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0)
                    {
                        nullable1 = Calculate(strArray[1]);
                        nullable2 = Calculate(strArray[2]);
                        num = (nullable1.GetValueOrDefault() <= nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) == 0 ? 1 : 0;
                    }
                    else
                        num = 1;
                    return num == 0;
                }
            }
        }
        private static string Summary(string temp)
        {
            string _prefix = "";
            string _postfix = "";
            string str;
            string _else = str = temp;
            string _then = str;
            string _if = str;
            if (!temp.Trim().Contains("if"))
                return temp;
            spliteStatment(temp, out _prefix, out _if, out _then, out _else, out _postfix);
            if (Condition(Summary(_if)))
                return Summary(_prefix + _then + _postfix);
            else
                return Summary(_prefix + _else + _postfix);
        }
        public ActionResult CalcFormul(string Formul, int ShomareHesabCodeDaramadId, string ElamAvarezId, string CodeDaramadElamAvarezID)
        {
            string result = "";
            string temp = "";

            Formul=Formul.Replace("÷", "/");
            int Count1 = Formul.Count(k => k == ')');
            int Count2 = Formul.Count(k => k == '(');
            int Count3 = Formul.Count(k => k == ']');
            int Count4 = Formul.Count(k => k == '[');
            if (Count1 != Count2 || Count3 != Count4)
            {
                return Json(new { Er = 1,MsgTitle="خطا" ,Msg = "تعداد پرانتزهای باز شده با تعداد پرانتزهای بسته شده یکی نیست." }, JsonRequestBehavior.AllowGet);
            }

            var s = Formul.Split(';');
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (s[i] == "then" || s[i] == "else")
                {
                    s[i] = ",";
                }
                else
                {
                    var f = servic.GetParametreSabetWithFilter("fldNameParametreEn", s[i],ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                    if (f != null)
                    {
                        s[i] = f.fldId.ToString();
                        if (CodeDaramadElamAvarezID != "")
                        {
                            var f11 = servic.GetParametreSabet_ValueWithFilter("fldCodeDaramadElamAvarez_fldParametreSabetId", CodeDaramadElamAvarezID, f.fldId.ToString(), 0, IP, out Err).FirstOrDefault();
                            if (f11 != null)
                                s[i] = f11.fldValue.ToString();
                        }
                    }
                    else
                    {
                        var f1 = servic.GetParametreOmoomiWithFilter("fldNameParametreEn", s[i], 0, IP, out Err).FirstOrDefault();
                        if (f1 != null)
                        {
                            s[i] = "0";
                            var f11 = servic.GetParametreOmoomi_ValueWithFilter("LastDate", f1.fldId.ToString(),"", 0, IP, out Err).FirstOrDefault();
                            if (f11 != null)
                                s[i] = f11.fldValue.ToString();
                        }
                    }
                }
                temp = temp + s[i];
            }
            var sss=Summary(temp).Replace('[', '(').Replace(']', ')');
            result = Calculate(sss).ToString();
            return Json(new { Er = 0, FResult = Convert.ToInt64(result.Split('.')[0]) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadStaticParam(int ShomareHesabCodeDaramadId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Daramad.OBJ_ParametreSabet> data = null;

            data = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId.ToString(),"", 0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadGeneralParam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Daramad.OBJ_ParametreOmoomi> data = null;

            data = servic.GetParametreOmoomiWithFilter("", "", 0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadAllFormul(int fldShomareHesab_CodeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad_Fomula> data = null;
            data = servic.GetShomareHesabCodeDaramad_Fomula("fldShomareHesab_CodeId", fldShomareHesab_CodeId.ToString(), 0, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }  
        public ActionResult Save(WCF_Daramad.OBJ_CodhayeDaramd CodhayeDaramd,int fldPID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (CodhayeDaramd.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertCodhayeDaramd(fldPID,CodhayeDaramd.fldDaramadCode, CodhayeDaramd.fldDaramadTitle, CodhayeDaramd.fldMashmooleArzesheAfzoode, CodhayeDaramd.fldMashmooleKarmozd,CodhayeDaramd.fldAmuzeshParvaresh,
                        CodhayeDaramd.fldUnitId, CodhayeDaramd.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCodhayeDaramd(CodhayeDaramd.fldId, CodhayeDaramd.fldDaramadCode, CodhayeDaramd.fldDaramadTitle, CodhayeDaramd.fldMashmooleArzesheAfzoode, CodhayeDaramd.fldMashmooleKarmozd, CodhayeDaramd.fldAmuzeshParvaresh,
                        CodhayeDaramd.fldUnitId, CodhayeDaramd.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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

        public ActionResult Delete(int Id, int Pid, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; var q =false;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCodhayeDaramd(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            finally
            {
                q = servic.GetCodhayeDaramdWithFilter("PId", Pid.ToString(), FiscalYearId, 0, 0, IP, out Err).Any();
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er,
                Child=Convert.ToByte(q)
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckChildFlag(int Id, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var AccountingChild= servic.GetCodhayeDaramdWithFilter("PId", Id.ToString(), FiscalYearId, 0, 0, IP, out Err).Where(l => l.fldFlag == 1).Any();
            return Json(new
            {
                AccountingChild = AccountingChild
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertFromAccounting(int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string MsgeTitle = "عملیات موفق"; byte Er = 0;
            var FiscalYear=accservic.GetFiscalYearDetail(FiscalYearId,Convert.ToInt32(Session["OrganId"]), IP,out accErr);
            var Msg=servic.InsertFromAccounting(FiscalYear.fldYear, Session["Username"].ToString(), Session["Password"].ToString(), 
                Convert.ToInt32(Session["OrganId"]), IP, out Err);
            if (Err.ErrorType)
            {
                MsgeTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg=Msg,
                MsgeTitle = MsgeTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NodeLoadGroup(string nod,int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "root")
            {
                nod = "0";
            }
            var q = servic.GetCodhayeDaramdWithFilter("PId", nod, FiscalYearId,0, 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldDaramadTitle;
                /*asyncNode.Expanded = true;*/
                asyncNode.AttributesObject = new { fldDaramadCode = item.fldDaramadCode, fldDaramadTitle = item.fldDaramadTitle,
                    fldMashmooleArzesheAfzoode = item.fldMashmooleArzesheAfzoode,fldMashmooleKarmozd=item.fldMashmooleKarmozd,
                                            fldAmuzeshParvaresh=item.fldAmuzeshParvaresh,
                                                   fldNameVahed = item.fldNameVahed,
                                                   fldUnitId = item.fldUnitId,
                                                   fldDesc = item.fldDesc,
                                                   fldFlag=item.fldFlag,
                                                   lastNod=item.lastNod
                    //,fldFormolsaz=item.fldFormolsaz,fldFormulKoliId=item.fldFormulKoliId,fldFormulMohasebatId=item.fldFormulMohasebatId
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }

        public ActionResult MoveNodes(int childId, int parentId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servic.UpdateDaramadId(childId, parentId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Er = 1;
                    Msg = Err.ErrorMsg;
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
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCom.GetOrganizationWithFilter("fldTreeOrgan", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out ErrCom).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult NodeLoadGroupParamFormul(string nod, int? OrganId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "root")
            {
                nod = "0";
            }
            var q = servic.GetShomareHesabCodeDaramadWithFilter("PId", nod, OrganId.ToString(),FiscalYearId, 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldDaramadTitle;
                /*asyncNode.Expanded = true;*/
                asyncNode.AttributesObject = new
                {
                    fldDaramadCode = item.fldDaramadCode,
                    fldDaramadTitle = item.fldDaramadTitle,
                    fldMashmooleArzesheAfzoode = item.fldMashmooleArzesheAfzoode,
                    fldMashmooleKarmozd = item.fldMashmooleKarmozd,
                    fldAmuzeshParvaresh=item.fldAmuzeshParvaresh,
                    fldNameVahed = item.fldNameVahed,
                    fldUnitId = item.fldUnitId,
                    fldDesc = item.fldDesc,
                    fldFormolsaz = item.fldFormolsaz,
                    fldFormulKoliId = item.fldFormulKoliId,
                    fldFormulMohasebatId = item.fldFormulMohasebatId,
                    fldShorooshenaseGhabz=item.fldShorooshenaseGhabz,
                    fldShomareHesab=item.fldShomareHesab
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult ReadDaramadParamFormul(StoreRequestParameters parameters, int FiscalYearId/*,int? OrganId*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDaramadTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldDaramadTitle";
                            break;
                        case "fldDaramadCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldDaramadCode";
                            break;
                        case "fldNameVahed":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldNameVahed";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldShomareHesab";
                            break;
                        case "fldShorooshenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldShorooshenaseGhabz";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, (Session["OrganId"]).ToString(),FiscalYearId, 0, IP, out Err).ToList();
                    else
                        data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, (Session["OrganId"]).ToString(),FiscalYearId, 0, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_Organ", (Session["OrganId"]).ToString(), "",FiscalYearId, 0, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
