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

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class ParametrsController : Controller
    {
        //
        // GET: /PayRoll/Parametrs/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common_Pay.Comon_PayService Cservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Daramad.DaramadService Dservic = new WCF_Daramad.DaramadService();
        WCF_Common.CommonService ComServic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common_Pay.ClsError CErr = new WCF_Common_Pay.ClsError();
        WCF_Daramad.ClsError DErr = new WCF_Daramad.ClsError();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
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
        public ActionResult GetHesabType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = ComServic.GetHesabTypeWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrCom).ToList()
                .Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Formul(int ParamId, int TypeEstekhdam, int FormulId, int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ParamId = ParamId;
            PartialView.ViewBag.FormulId = FormulId;
            PartialView.ViewBag.TypeEstekhdam = TypeEstekhdam;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetTypeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetTypeEstekhdamWithFilter("", "", 0, IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        
        public ActionResult Save(WCF_PayRoll.OBJ_Parametrs Param,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int? TypeEstekhdamId = null; var Er = 0;
            try
            {
                if (Param.fldDesc == null)
                    Param.fldDesc = "";
                if (Param.fldTypeMablagh == false) { TypeEstekhdamId = null; }
                else { TypeEstekhdamId = Param.fldTypeEstekhdamId; }
                if (Param.fldId == 0)
                {
                    //ذخیره
                    Msg = servic.InsertParametrs(Param.fldTitle, Param.fldTypeParametr, Param.fldTypeMablagh, TypeEstekhdamId,Convert.ToBoolean( Param.fldActive),Convert.ToBoolean(Param.fldPrivate),Convert.ToByte(Param.fldHesabTypeParam),Convert.ToByte(Param.fldIsMostamar), Param.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId, IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                {
                    //ویرایش
                    Msg = servic.UpdateParametrs(Param.fldId, Param.fldTitle, Param.fldTypeParametr, Param.fldTypeMablagh, TypeEstekhdamId, Convert.ToBoolean(Param.fldActive), Convert.ToBoolean(Param.fldPrivate), Convert.ToByte(Param.fldHesabTypeParam), Convert.ToByte(Param.fldIsMostamar), Param.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);
                    MsgTitle = "ویرایش موفق";
                }
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                //var q = servic.GetPersonalInfoSelect("fldParametrsId", id.ToString(), 1).FirstOrDefault();
                //if (q != null)
                //{
                //    Msg = "آیتم مورد نظر جای دیگری استفاده شده است.";
                //    MsgTitle = "خطا";
                //}
                //else
                //{
                Msg = servic.DeleteParametrs(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);
                MsgTitle = "حذف موفق";

                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
                    Er = 1;
                }
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetParametrsDetail(Id, OrganId, IP, out Err);
            string TypeMablagh = "0";
            if (q.fldTypeMablagh)
                TypeMablagh = "1";
            string TypeParametr = "0";
            if (q.fldTypeParametr)
                TypeParametr = "1";
            string Active = "0";
            if (q.fldActive==true)
                Active = "1";
            string Private = "0";
            if (q.fldPrivate==true)
                Private = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldTypeMablagh = TypeMablagh,
                fldTypeParametr = TypeParametr,
                fldTypeEstekhdamId = q.fldTypeEstekhdamId.ToString(),
                fldDesc = q.fldDesc,
                Active=Active,
                Private=Private,
                fldHesabTypeParam=q.fldHesabTypeParam,
                fldIsMostamar=q.fldIsMostamar.ToString()
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Read(StoreRequestParameters parameters,string OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Parametrs> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Parametrs> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "fldNoeMablaghName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeMablaghName";
                            break;
                        case "fldNoeParametrName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeParametrName";
                            break;
                        case "TypeEstekhdamName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "TypeEstekhdamName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetParametrsWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                    else
                        data = servic.GetParametrsWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametrsWithFilter("", "", OrganId, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Parametrs> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadParam(StoreRequestParameters parameters, int TypeEstekhdam)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Common_Pay.OBJ_Items_Estekhdam> data = null;

            data = Cservic.GetItems_EstekhdamWithFilter("fldAnvaeEstekhdamId", TypeEstekhdam.ToString(), 0, 0, IP, out CErr).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveFormul(WCF_PayRoll.OBJ_ParameteriItemsFormul For, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                if (For.fldId == 0)
                { //ذخیره
                    Msg = servic.InsertParameteriItemsFormul(For.fldParametrId, For.fldFormul, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                { //ویرایش
                    Msg = servic.UpdateParameteriItemsFormul(For.fldId, For.fldParametrId, For.fldFormul, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);
                    MsgTitle = "ویرایش موفق";

                }
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = ComServic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out ErrCom).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult DetailFormul(int Id, int TypeEstekhdam)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetParameteriItemsFormulDetail(Id, IP, out Err);
            string CurrentText = "";
            string Formul = "";
            var s = q.fldFormul.Split(';');
            for (int i = 0; i < s.Length - 1; i++)
            {
                var f = Cservic.GetItems_EstekhdamWithFilter("fldNameEN", s[i], 0, 0, IP, out CErr).Where(l => l.fldTypeEstekhdamId == TypeEstekhdam).FirstOrDefault();

                if (f != null)
                {
                    CurrentText = CurrentText + f.fldTitle + ";";
                    Formul = Formul + f.fldTitle;
                }
                else
                {
                    CurrentText = CurrentText + s[i] + ";";
                    Formul = Formul + s[i];
                }
            }
            CurrentText = CurrentText.Replace("if(", "اگر( ").Replace("then", "آنگاه ").Replace("else", "درغیراین صورت ");
            Formul = Formul.Replace("if(", "اگر( ").Replace("then", "آنگاه ").Replace("else", "درغیراین صورت ");
            return Json(new
            {
                fldId = q.fldId,
                Formul = Formul,
                CurrentText = CurrentText,
                EnFormul = q.fldFormul
            }, JsonRequestBehavior.AllowGet);
        }
        public static double? Calculate(string Formel)
        {
            Models.FM F = new Models.FM();
            int num = 12;
            double result;
            if (F.Neu(Formel, out result) || num != 21)
                return new double?(result);
            return new double?();
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
        public ActionResult CalcFormul(string Formul,int TypeEstekhdam)
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
                return Json(new { result = "تعداد پرانتز های باز شده با تعداد پرانتز های بسته شده یکی نیست." }, JsonRequestBehavior.AllowGet);
            }


            var s = Formul.Split(';');
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (s[i] == "then" || s[i] == "else")
                    s[i] = ",";

                var f = Cservic.GetItems_EstekhdamWithFilter("fldNameEN", s[i], 0, 0, IP, out CErr).Where(l => l.fldTypeEstekhdamId == TypeEstekhdam).FirstOrDefault();
                if (f != null)
                {
                    s[i] = f.fldId.ToString();
                }
                temp = temp + s[i];
            }

            result = Calculate(Summary(temp).Replace('[', '(').Replace(']', ')')).ToString();

            //for (int i = 0; i < s.Length - 1; i++)
            //{
            //    if (s[i] == "if")
            //    {
            //        for (int j = i + 1; l < s.Length - 1; j++)
            //        {
            //            if (s[j] == "then")
            //            {
            //                for (int k = j + 1; l < s.Length - 1; k++)
            //                {
            //                    if (s[k] == "else")
            //                    {
            //                        for ( l = k + 1; l < s.Length - 1; l++)
            //                        {
            //                            els = els + s[l];
            //                            F1 = true;
            //                        }
            //                    }
            //                    if (!F1)
            //                        Then = Then + s[k];
            //                    F2 = true;
            //                }
            //            }
            //            if (!F2)
            //                If = If + s[j];
            //        }
            //    }
            //}



            return Json(new { FResult = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
