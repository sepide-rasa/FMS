using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Aspose.Cells;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class MotalebatParametriController : Controller
    {
        //
        // GET: /PayRoll/MotalebatParametri/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();
        public ActionResult MotalebatParametriSingle(string containerId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.MotalebateParametri_Personal(); 
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.OrganId = OrganId.ToString();
            return result;
        }

        public ActionResult ReadHeader(StoreRequestParameters parameters,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_PayPersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_PayPersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName_Father":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_Father";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomarePersoneli";
                            break;
                        case "fldJobeCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldJobeCode";
                            break;
                    }
                    if (data != null)
                        data1 = PayServic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                    else
                        data = PayServic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetPayPersonalInfoWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
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

            List<WCF_PayRoll.OBJ_PayPersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadMotalebateParametri(StoreRequestParameters parameters, int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_MotalebateParametri_Personal> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_MotalebateParametri_Personal> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldPersonalId_Id";
                            break;
                        case "fldParamTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_ParamTitle";
                            break;
                        case "fldNoePardakhtName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NoePardakhtName";
                            break;
                        case "fldTedad":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Tedad";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Mablagh";
                            break;
                        case "fldTarikhPardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_TarikhPardakht";
                            break;
                        case "fldMashmoleBimeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_MashmoleBimeName";
                            break;
                        case "fldMashmoleMaliytName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_MashmoleMaliytName";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_StatusName";
                            break;
                    }
                    if (data != null)
                        data1 = PayServic.GetMotalebateParametri_PersonalWithFilter(field, searchtext, PersonalId.ToString(), "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                    else
                        data = PayServic.GetMotalebateParametri_PersonalWithFilter(field, searchtext, PersonalId.ToString(), "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetMotalebateParametri_PersonalWithFilter("fldPersonalId", PersonalId.ToString(), "", "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
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

            List<WCF_PayRoll.OBJ_MotalebateParametri_Personal> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Reload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_MotalebateParametri_Personal> data = null;
            data = PayServic.GetMotalebateParametri_PersonalWithFilter("fldPersonalId", PersonalId.ToString(),"","","",Convert.ToInt32(Session["OrganId"]), 100, IP,out ErrPay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewMotalebatParametriSingle(int PersonalId, int Id, byte? DelCalc,int OrganId)
        {//باز شدن تب

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            var Date = servic.GetDate(IP, out Err);
          /*  if (Id != 0)
            {
                var type=PayServic.GetCheckKosorat_Motalebat("Motalebat", Id, IP, out ErrPay).FirstOrDefault().fldType;
                if (type == "1")
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "آیتم مورد نظر در محاسبات استفاده شده و امکان ویرایش وجود ندارد."
                    });
                    DirectResult result2 = new DirectResult();
                    return result2;
                }
            }*/
            result.ViewBag.Id = Id;
            result.ViewBag.DelCalc = DelCalc;
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.OrganId = OrganId;
            result.ViewBag.TarikhPardakht = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(Date.fldDateTime));
            return result;
        }

        public ActionResult GetMotalebatParametri(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetParametrsWithFilter("Motalebat", "", OrganId.ToString()/*(Session["OrganId"]).ToString()*/, 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldParametrName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult CheckStatusFlagEdit(short Year, byte Month, byte NobatPardakht, byte fldNoePardakht, int? fldPayPersonalId )
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0; string MonthName = "";
            string FlagMsg = "";
            try
            {

                var karkard = PayServic.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM", NobatPardakht.ToString(), Year.ToString(), Month.ToString(), 0, 0, Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPay).FirstOrDefault();
                var personal = PayServic.GetPayPersonalInfoDetail(Convert.ToInt32(fldPayPersonalId), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                var FieldName = "";
                 MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                if (karkard != null && karkard.fldFlag == true)
                {
                    Flag = 3;//بسته شدن حقوق
                }
                //}
                else
                {
                    if (fldNoePardakht == 1 && fldPayPersonalId == null && Flag != 3)
                    {
                        FieldName = "Hoghogh"; ;
                        Msg = " حقوق " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات حقوق حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 2 && fldPayPersonalId == null)
                    {
                        FieldName = "EzafeKar";
                        Msg = " اضافه کاری " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات اضافه کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 3 && fldPayPersonalId == null)
                    {
                        FieldName = "TatilKar";
                        Msg = " تعطیل کاری " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات ، محاسبات تعطیل کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 6 && fldPayPersonalId == null)
                    {
                        FieldName = "Morakhasi";
                        Msg = " مرخصی " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات مرخصی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 5 && fldPayPersonalId == null)
                    {
                        FieldName = "Mamooriyat";
                        Msg = " ماموریت " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات ماموریت حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 4 && fldPayPersonalId == null)
                    {
                        FieldName = "Eydi";
                        Msg = " عیدی " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات عیدی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }

                    var q = PayServic.CheckMohasebat(FieldName, Convert.ToInt32(fldPayPersonalId), Convert.ToByte(Month), Convert.ToInt16(Year), Convert.ToByte(NobatPardakht), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).FirstOrDefault();
                    if (q != null)
                    {
                        Flag = 4;
                    }
                    else
                    {
                        Flag = 5;
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
                MonthName = MonthName,//MyLib.Shamsi.ShamsiMonthname(Month),
                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckStatusFlag(int id)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 5;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            string FlagMsg = "";
            try
            {
                var ghatei = PayServic.GetCheckKosorat_Motalebat("MotalebatGhatei", id, IP, out ErrPay).FirstOrDefault().fldType;
                if (ghatei == "1")
                    Flag = 3;
                else
                {
                    var type = PayServic.GetCheckKosorat_Motalebat("Motalebat", id, IP, out ErrPay).FirstOrDefault().fldType;
                    if (type == "1")
                    {
                        Flag = 4;
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
              
                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id, int? fldPayPersonalId, byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                
                    if (DelCalc == 1)
                    {
                        PayServic.MohasebatDelete("AllMohasebat_Personal_Motalebat",id, Convert.ToByte(0), Convert.ToInt16(0), Convert.ToByte(0), "", "",
                            Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId/*Convert.ToInt32(Session["OrganId"])*/, out ErrPay);
                        if (ErrPay.ErrorType == true)
                        {
                            Msg = ErrPay.ErrorMsg;
                            MsgTitle = "خطا";
                            Er = 1;

                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    PayServic.DeleteMotalebateParametri_Personal(id, Session["Username"].ToString(), (Session["Password"].ToString()),
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    if (ErrPay.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrPay.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = "حذف با موفقیت انجام شد.";
                    MsgTitle = "حذف موفق";
                
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

        public ActionResult Details(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetMotalebateParametri_PersonalDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
            var q1 = PayServic.GetMohasebat_kosorat_MotalebatParamWithFilter("fldMotalebatId", Id.ToString(), 0, IP, out ErrPay);

            string NoePardakht = "0";
            if (q.fldNoePardakht)
                NoePardakht = "1";
            string Status = "0";
            if (q.fldStatus)
                Status = "1";
            /*string MashmoleBim = "0";
            if (q.fldMashmoleBime)
                MashmoleBim = "1";
            string MashmoleMaliyat = "0";
            if (q.fldMashmoleMaliyat)
                MashmoleMaliyat = "1";*/
            int Tarikh = q.fldDateDeactive;
            var Sal = Tarikh.ToString().Substring(0, 4);
            var Month = Tarikh.ToString().Substring(4);
            var use = false;
            if (q1.Count() > 0)
            {
                use = true;
            }
            return Json(new
            {
                fldId = q.fldId,
                Sal = Sal,
                Month = Month,
                fldPersonalId = q.fldPersonalId,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldParametrId = q.fldParametrId.ToString(),
                fldNoePardakht = NoePardakht,
                fldMablagh = q.fldMablagh,
                fldTedad = q.fldTedad,
                fldTarikhPardakht = q.fldTarikhPardakht,
                fldMashmoleBime = q.fldMashmoleBime,
                fldMashmoleBimeName = q.fldMashmoleBimeName,
                fldMashmoleMaliyat = q.fldMashmoleMaliyat,
                fldMashmoleMaliytName = q.fldMashmoleMaliytName,
                fldMazayaMashmool=q.fldMazayaMashmool,
                fldStatus = Status,
                use=use
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveGroup(WCF_PayRoll.OBJ_MotalebateParametri_Personal mp, string PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; var exist = "";

            try
            {
               
                //if (DelCalc == 1)
                //{
                //    PayServic.MohasebatDelete("AllMohasebat_sal", 0, mah, sal, Convert.ToByte(0), "", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out ErrPay);
                //    if (Err.ErrorType == true)
                //    {
                //        Msg = Err.ErrorMsg;
                //        MsgTitle = "خطا";
                //        Er = 1;

                //        return Json(new
                //        {
                //            Er = Er,
                //            Msg = Msg,
                //            MsgTitle = MsgTitle
                //        }, JsonRequestBehavior.AllowGet);
                //    }
                //}
               /* foreach (var item in MotalebateParametri)
                {
                    var q = PayServic.GetMotalebateParametri_PersonalWithFilter("CheckPersonal", item.fldPersonalId.ToString(), mp.fldParametrId.ToString()
                        , mp.fldMablagh.ToString(), mp.fldTarikhPardakht.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).FirstOrDefault();
                    if (q == null)
                    {
                        PayServic.InsertMotalebateParametri_Personal(item.fldPersonalId, mp.fldParametrId, mp.fldNoePardakht, mp.fldMablagh, mp.fldTedad, mp.fldTarikhPardakht
                             , mp.fldMashmoleBime, mp.fldMashmoleMaliyat, mp.fldStatus, mp.fldDateDeactive, "",
                             Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);

                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    } 
                    else
                    {
                        exist = exist + q.fldName_Father + '،';
                    }
                }*/
                exist=PayServic.InsertGroupMotalebateParametri_Personal(PersonalId, mp.fldParametrId, mp.fldNoePardakht, mp.fldMablagh, mp.fldTedad, mp.fldTarikhPardakht
                             , mp.fldMashmoleBime,mp.fldMazayaMashmool, mp.fldMashmoleMaliyat, mp.fldStatus, mp.fldDateDeactive, "",
                             Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);

                if (ErrPay.ErrorType)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = ErrPay.ErrorMsg,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = "ذخیره با موفقیت انجام شد.";
                MsgTitle = "ذخیره موفق";
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
                exist = exist,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveGroupDeactive(WCF_PayRoll.OBJ_MotalebateParametri_Personal KosorateParametri, string PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; var exist = "";

            try
            {


                PayServic.UpdateDeactiveMotalebateParametri_Personal(PersonalId, KosorateParametri.fldParametrId, KosorateParametri.fldMablagh, KosorateParametri.fldTarikhPardakht
                             , KosorateParametri.fldStatus, KosorateParametri.fldDateDeactive, "",
                             Session["Username"].ToString(), Session["Password"].ToString(),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);

                if (ErrPay.ErrorType)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = ErrPay.ErrorMsg,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }

                Msg = "عملیات با موفقیت انجام شد.";
                MsgTitle = "ذخیره موفق";
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
                exist = exist,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveSingle(WCF_PayRoll.OBJ_MotalebateParametri_Personal Motalebat, string fldYear, string fldMonth, byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                
                if (Motalebat.fldDesc == null)
                    Motalebat.fldDesc = "";
                var fldDateDeactive = "";
                var Month = "";
                if (fldMonth.Count() != 2)
                {
                    fldMonth = fldMonth.PadLeft('0', 2);
                }
                if (Motalebat.fldNoePardakht == true && Motalebat.fldTedad != 0)
                {
                    Motalebat.fldTedad = 0;
                }
                fldDateDeactive = fldYear + fldMonth;

               
                if (Motalebat.fldId == 0)
                { //ذخیره
                    //if (DelCalc == 1)
                    //{
                    //    PayServic.MohasebatDelete("AllMohasebat_Personal_sal", Motalebat.fldPersonalId, Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), Convert.ToByte(0), "", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out ErrPay);
                    //    if (ErrPay.ErrorType == true)
                    //    {
                    //        Msg = ErrPay.ErrorMsg;
                    //        MsgTitle = "خطا";
                    //        Er = 1;

                    //        return Json(new
                    //        {
                    //            Er = Er,
                    //            Msg = Msg,
                    //            MsgTitle = MsgTitle
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    PayServic.InsertMotalebateParametri_Personal(Motalebat.fldPersonalId, Motalebat.fldParametrId, Motalebat.fldNoePardakht, Motalebat.fldMablagh
                        , Motalebat.fldTedad, Motalebat.fldTarikhPardakht, Motalebat.fldMashmoleBime,Motalebat.fldMazayaMashmool,Motalebat.fldMashmoleMaliyat,
                        Motalebat.fldStatus, Convert.ToInt32(fldDateDeactive), Motalebat.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(),
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                    if (ErrPay.ErrorType)
                    {
                        Msg = ErrPay.ErrorMsg;
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                }
                else
                { //ویرایش
                    if (DelCalc == 1)
                    {
                        PayServic.MohasebatDelete("AllMohasebat_Personal_Motalebat", Motalebat.fldId, Convert.ToByte(fldMonth), Convert.ToInt16(fldYear),
                            Convert.ToByte(0), "", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId/*Convert.ToInt32(Session["OrganId"])*/, out ErrPay);
                        if (ErrPay.ErrorType == true)
                        {
                            Msg = ErrPay.ErrorMsg;
                            MsgTitle = "خطا";
                            Er = 1;

                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    PayServic.UpdateMotalebateParametri_Personal(Motalebat.fldId, Motalebat.fldPersonalId, Motalebat.fldParametrId, Motalebat.fldNoePardakht, Motalebat.fldMablagh
                        , Motalebat.fldTedad, Motalebat.fldTarikhPardakht, Motalebat.fldMashmoleBime,Motalebat.fldMazayaMashmool, Motalebat.fldMashmoleMaliyat,
                        Motalebat.fldStatus, Convert.ToInt32(fldDateDeactive), Motalebat.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(),
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveStatus(int id, bool fldStatus, string fldYear, string fldMonth,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var fldDateDeactive = "";
                if (fldMonth.Count() != 2)
                {
                    fldMonth = fldMonth.PadLeft('0', 2);
                }
                fldDateDeactive = fldYear + fldMonth;
                //ذخیره
                MsgTitle = "ذخیره موفق";
                Msg = PayServic.UpdateKosurat_Motalebat("Motalebat", fldStatus, id, Convert.ToInt32(fldDateDeactive), Session["Username"].ToString(),
                    Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);

                if (ErrPay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrPay.ErrorMsg;
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
        public ActionResult ReloadKosuratParametriGroupDeactive(string Status, string Parametr, string Mablagh, string TarikhePardakht,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.WCF_PayRoll.OBJ_MotalebateParametri_Personal> data = null;
            data = PayServic.GetMotalebateParametri_PersonalWithFilter("RealoadDeactive", Status, Parametr
                       , Mablagh, TarikhePardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out ErrPay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadMotalebateParametriGroup(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.WCF_PayRoll.OBJ_MotalebatParametriGroup> data = null;

            data = PayServic.GetMotalebatParametriGroupWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MotalebatParametriGroup(string containerId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var Date = servic.GetDate(IP, out Err);
            result.ViewBag.TarikhPardakht = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(Date.fldDateTime));
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.OrganId = OrganId.ToString();
            return result;
        }
        public ActionResult MotalebatParametriGroup_Deactive(string containerId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var Date = servic.GetDate(IP, out Err);
            result.ViewBag.TarikhPardakht = MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(Date.fldDateTime));
            result.ViewBag.OrganId = OrganId.ToString();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
    }
}
