using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using NewFMS.Models;
namespace NewFMS.Areas.Daramad.Controllers
{
    public class ReplyTaghsit_TakhfifController : Controller
    {
        //
        // GET: /Daramad/ReplyTaghsit_Takhfif/

        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(int Id, string OrganId, int FiscalYearId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            var per = Permission.haveAccess(606,  Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]));/*ویرایش کارمزد*/
            var perTaghsit = Permission.haveAccess(605,Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]));/*ویرایش تقسیط_تخفیف*/
            
            var Nerkh = servic.GetTanzimateDaramadWithFilter("fldOrganId", OrganId,FiscalYearId, 1, IP,out Err).FirstOrDefault().fldNerkh;
            var m = servic_Com.GetDate(IP, out Err_Com);
            var TarikhShamsi = m.fldTarikh;
            var PTakhfif = "0";
            var PTaghsit = "0";
            var PerKarmozd = "0";
            if (per == true)
                PerKarmozd = "1";
            if (perTaghsit == true)
            {
                PTaghsit = "1";
                PTakhfif = "1";
            }
            result.ViewBag.Id = Id;
            result.ViewBag.SDate = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(m.fldDateTime));
            result.ViewBag.fldTarikh = m.fldDateTime.ToString("yyyy-MM-dd");
            result.ViewBag.TarikhShamsi = TarikhShamsi;
            result.ViewBag.Permission = PerKarmozd;
            result.ViewBag.Nerkh = Nerkh;
            result.ViewBag.perTaghsit = PTaghsit;
            result.ViewBag.perTakhfif = PTakhfif;
            return result;
        }
        public ActionResult TakhfifHa(int ElamAvarez, long MablaghAsli, double DarsadTakhfif, long SumMablaghTakhfif)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.ElamAvarez = ElamAvarez;
                PartialView.ViewBag.MablaghAsli = MablaghAsli;
                PartialView.ViewBag.DarsadTakhfif = DarsadTakhfif;
                PartialView.ViewBag.SumMablaghTakhfif = SumMablaghTakhfif;
                return PartialView;
            }
        }
        public ActionResult ReadRequest(StoreRequestParameters parameters, string elamAvarez)
        {
            try
            {


                if (Session["Username"] == null)
                    return RedirectToAction("logon", "Account", new { area = "" });
                var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

                List<WCF_Daramad.OBJ_RequestTaghsit_Takhfif> data = null;
                if (filterHeaders.Conditions.Count > 0)
                {
                    string field = "";
                    string searchtext = "";
                    List<WCF_Daramad.OBJ_RequestTaghsit_Takhfif> data1 = null;
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
                            data1 = servic.GetRequestTaghsit_TakhfifWithFilter(field, searchtext, 0, IP, out Err).ToList();
                        else
                            data = servic.GetRequestTaghsit_TakhfifWithFilter(field, searchtext, 0, IP, out Err).ToList();
                    }
                    if (data != null && data1 != null)
                        data.Intersect(data1);
                }
                else
                {
                    data = servic.GetRequestTaghsit_TakhfifWithFilter("Reply", elamAvarez, 0, IP, out Err).ToList();
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

                List<WCF_Daramad.OBJ_RequestTaghsit_Takhfif> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
                //-- end paging ------------------------------------------------------------

                return this.Store(rangeData, data.Count);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult Save(WCF_Daramad.OBJ_StatusTaghsit_Takhfif status, WCF_Daramad.OBJ_ReplyTakhfif Takhfif, WCF_Daramad.OBJ_ReplyTaghsit Taghsit, int? ReplyId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ReplyId == null)
                    ReplyId = 0;
                if (Taghsit.fldDescKarmozd==null)
                    Taghsit.fldDescKarmozd = "";
                //ذخیره
                MsgTitle = "عملیات موفق";
                if (status.fldTypeRequest == 1)//ذخیره تقسیط
                {
                    
                    Msg = servic.InsertStatusTaghsit_Takhfif(Convert.ToInt32(ReplyId), status.fldRequestId, status.fldTypeMojavez, status.fldTypeRequest, Takhfif.fldDarsad, Takhfif.fldMablagh
                    , Taghsit.fldTarikh, Taghsit.fldMablaghNaghdi, Taghsit.fldTedadAghsat, Taghsit.fldShomareMojavez, Taghsit.fldTedadMahAghsat, Taghsit.fldJarimeTakhir
                    , Convert.ToInt32(Session["OrganId"]), Convert.ToDecimal(Taghsit.fldDarsad), Taghsit.fldDescKarmozd, Taghsit.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else//ذخیره تخفیف
                {
                    Msg = servic.InsertStatusTaghsit_Takhfif(Convert.ToInt32(ReplyId), status.fldRequestId, status.fldTypeMojavez, status.fldTypeRequest, Takhfif.fldDarsad, Takhfif.fldMablagh
                    , Takhfif.fldTarikh, Taghsit.fldMablaghNaghdi, Taghsit.fldTedadAghsat, Takhfif.fldShomareMajavez, Taghsit.fldTedadMahAghsat, Taghsit.fldJarimeTakhir
                    , Convert.ToInt32(Session["OrganId"]), Convert.ToDecimal(Taghsit.fldDarsad), Taghsit.fldDescKarmozd, Takhfif.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult DeleteReply(int RequestId, int TypeRequest)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "عملیات موفق";

                if (TypeRequest == 1)// تقسیط
                {
                    var r = servic.GetReplyTaghsitWithFilter("fldRequestId", RequestId.ToString(), 0, IP, out Err).FirstOrDefault();
                    if (r != null)
                    {
                        var k = servic.SelectDataForTaghsit("ReplyTaghsitId", r.fldId.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                        if (k != null)
                        {
                            MsgTitle = "خطا";
                            Msg = "برای درخواست موردنظر اسناد تقسیط ثبت شده و پاسخ قابل حذف نمی باشد.";
                        }
                        else
                        {
                            Msg = servic.DeleteReplyTaghsit(RequestId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);

                        }
                    }
                    else
                    {
                        Msg = servic.DeleteReplyTaghsit(RequestId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    }
                }
                else//تخفیف
                {
                    Msg = servic.DeleteReplyTakhfif(RequestId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
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
        public ActionResult DetailsTakhfif(int Id)
        {
            var q = servic.GetReplyTakhfifWithFilter("fldRequestId", Id.ToString(), 1,  IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldShomareMajavez = q.fldShomareMajavez.ToString(),
                fldTarikh = q.fldTarikh,
                fldDarsad = q.fldDarsad,
                fldMablagh = q.fldMablagh,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailsTaghsit(int Id)
        {
            var q = servic.GetReplyTaghsitWithFilter("fldRequestId", Id.ToString(), 1, IP, out Err).FirstOrDefault();
            var checkKarmozd=false;
            if (q.fldDescKarmozd != "")
                checkKarmozd = true;
            return Json(new
            {
                fldId = q.fldId,
                fldShomareMojavez = q.fldShomareMojavez.ToString(),
                fldTarikh = q.fldTarikh,
                fldTedadAghsat = q.fldTedadAghsat,
                fldMablaghNaghdi = q.fldMablaghNaghdi,
                fldTedadMahAghsat = q.fldTedadMahAghsat,
                fldJarimeTakhir=q.fldJarimeTakhir,
                fldDesc = q.fldDesc,
                fldDarsad = q.fldDarsad,
                fldDescKarmozd = q.fldDescKarmozd,
                checkKarmozd = checkKarmozd
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
        public ActionResult MohasebeKarmozd(string Month, long MablaghNaghdi, long Mashmul, string ElamAvarezId,int FiscalYearId)
        {
            var ss = servic.GetTanzimateDaramadWithFilter("fldOrganId", (Session["OrganId"]).ToString(),FiscalYearId, 1, IP, out Err).FirstOrDefault();
            var h = servic.GetShomareHesabCodeDaramadDetail(ss.fldTakhirId,FiscalYearId, IP, out Err);
            var HaveFormulId = 0; var Formul = "";
                string result = "";
                string temp = "";

            if (h.fldFormulMohasebatId != null)
                HaveFormulId = 1;
            else if (h.fldFormolsaz != null)
            {
                HaveFormulId = 2;
                Formul = h.fldFormolsaz;
            }

            if (HaveFormulId == 2)
            {

                Formul = Formul.Replace("÷", "/");
                int Count1 = Formul.Count(k => k == ')');
                int Count2 = Formul.Count(k => k == '(');
                int Count3 = Formul.Count(k => k == ']');
                int Count4 = Formul.Count(k => k == '[');
                if (Count1 != Count2 || Count3 != Count4)
                {
                    return Json(new { Er = 1, MsgTitle = "خطا", Msg = "تعداد پرانتزهای باز شده با تعداد پرانتزهای بسته شده یکی نیست." }, JsonRequestBehavior.AllowGet);
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
                        if(s[i]=="taghsit")
                            s[i]=(Convert.ToInt32(Month)+1).ToString();
                        else if(s[i]=="rate")
                            s[i]=ss.fldNerkh.ToString();
                        else if (s[i] == "mablagh")
                            s[i] = (Mashmul - MablaghNaghdi).ToString();
                    }
                    temp = temp + s[i];
                }
                var sss = Summary(temp).Replace('[', '(').Replace(']', ')');
                result = Calculate(sss).ToString();
            }


            return Json(new
            {
                mablagh = Convert.ToInt64(result.Split('.')[0])
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadDataTakhfif(int ElamAvarez, decimal DarsadTakhfif, long SumMablaghTakhfif, long MablaghAsli)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var req = servic.GetRequestTaghsit_TakhfifWithFilter("Takhfif",ElamAvarez.ToString(),0,IP, out Err).FirstOrDefault();
            var repl=servic.GetReplyTakhfifWithFilter("fldRequestId",req.fldId.ToString(),0,IP,out Err).FirstOrDefault();


            List<WCF_Daramad.OBJ_EmalTakhfif> p = new List<WCF_Daramad.OBJ_EmalTakhfif>();
            var Lists = servic.GetEmalTakhfif(ElamAvarez,IP, out Err).ToList();

            long? SumTakhfif = 0;
                for (int i = 0; i < Lists.Count; i++) {
                WCF_Daramad.OBJ_EmalTakhfif l = new WCF_Daramad.OBJ_EmalTakhfif();
                l.fldID = Lists[i].fldID;
                l.AsliValue = Lists[i].AsliValue;
                l.fldSharheCodeDaramad = Lists[i].fldSharheCodeDaramad;
                l.fldDaramadCode = Lists[i].fldDaramadCode;
                l.fldTedad = Lists[i].fldTedad;
                                                       
                if (i == Lists.Count-1)
                {
                    var TakhfifAsliValue = SumMablaghTakhfif - SumTakhfif;
                    while (TakhfifAsliValue % Lists[i].fldTedad != 0)
                    {
                        TakhfifAsliValue = TakhfifAsliValue - 1;
                    }
                    Lists[i].fldTakhfifAsliValue = TakhfifAsliValue;
                }
                else if ((Lists[i].fldTakhfifAsliValue == 0))
                {
                    long mablaghT = Convert.ToInt64(Math.Floor(Convert.ToDouble(((DarsadTakhfif) / 100) * Lists[i].AsliValue)));
                    while (mablaghT % Lists[i].fldTedad != 0)
                    {
                        mablaghT = mablaghT - 1;
                    }
                    Lists[i].fldTakhfifAsliValue = mablaghT;
                }
                else if (repl != null)
                {
                    if ((Lists[i].fldTakhfifAsliValue != 0 && repl.fldDarsad != DarsadTakhfif) || (Lists[i].fldTakhfifAsliValue != 0 && repl.fldMablagh != SumMablaghTakhfif))
                    {
                        long mablaghT = Convert.ToInt64(Math.Floor(Convert.ToDouble(((DarsadTakhfif) / 100) * Lists[i].AsliValue)));
                        while (mablaghT % Lists[i].fldTedad != 0)
                        {
                            mablaghT = mablaghT - 1;
                        }
                        Lists[i].fldTakhfifAsliValue = mablaghT;
                    }
                }


                l.fldTakhfifAsliValue = Lists[i].fldTakhfifAsliValue;
                SumTakhfif = SumTakhfif + Lists[i].fldTakhfifAsliValue;

                p.Add(l);
            }
                return Json(new { data = p.ToList(), SumTakhfif = SumTakhfif }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTakhfifHa(WCF_Daramad.OBJ_StatusTaghsit_Takhfif status,List<WCF_Daramad.OBJ_EmalTakhfif> TakhfifArray, int ElamAvarez,long MablaghAsli, WCF_Daramad.OBJ_ReplyTakhfif Takhfif, int? ReplyId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                decimal darsad=Convert.ToDecimal(Takhfif.fldMablagh * 100.0 / MablaghAsli);
                Msg = servic.InsertStatusTaghsit_Takhfif(Convert.ToInt32(ReplyId), status.fldRequestId, status.fldTypeMojavez, status.fldTypeRequest, darsad, Takhfif.fldMablagh
                   , Takhfif.fldTarikh, 0,0, Takhfif.fldShomareMajavez,0, 0 , Convert.ToInt32(Session["OrganId"]), 0, "", Takhfif.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }

                foreach (var item in TakhfifArray)
                {
                    servic.UpdateCodhayeDaramadiElamAvarez_Takhfif(item.fldID, item.fldTedad, Convert.ToInt64(item.AsliValue - item.fldTakhfifAsliValue), ElamAvarez, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
