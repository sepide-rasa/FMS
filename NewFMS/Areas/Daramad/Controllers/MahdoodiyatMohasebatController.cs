using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class MahdoodiyatMohasebatController : Controller
    {
        //
        // GET: /Daramad/MahdoodiyatMohasebat/

        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        public ActionResult Index(string containerId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.FiscalYearId = FiscalYearId;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult New(int id, int FiscalYearId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            return PartialView;
        }

        public ActionResult Save(WCF_Daramad.OBJ_MahdoodiyatMohasebat MahdoodiyatM, string IdUser, string IdAshkhas, string IdShomareHesabDaramad)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int Id = 0;
            try
            {
                List<string> Id_User = new List<string>();
                Id_User = IdUser.Split(';').Reverse().Skip(1).Reverse().ToList();

                List<string> Id_Ashkhas = new List<string>();
                Id_Ashkhas = IdAshkhas.Split(';').Reverse().Skip(1).Reverse().ToList();

                List<string> Id_ShomareHesabDaramad = new List<string>();
                Id_ShomareHesabDaramad = IdShomareHesabDaramad.Split(';').Reverse().Skip(1).Reverse().ToList();
                if (MahdoodiyatM.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد";
                    Id = servic.InsertMahdoodiyatMohasebat(MahdoodiyatM.fldTitle,MahdoodiyatM.fldAzTarikh,MahdoodiyatM.fldTatarikh,MahdoodiyatM.fldNoeKarbar,MahdoodiyatM.fldNoeCodeDaramad,MahdoodiyatM.fldNoeAshkhas,MahdoodiyatM.fldStatus, MahdoodiyatM.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in Id_User)
                    {
                        Msg = servic.InsertMohdoodiyatMohasebat_User(Id, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                    foreach (var item in Id_Ashkhas)
                    {
                        Msg = servic.InsertMahdoodiyatMohasebat_Ashkhas(Id, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                    foreach (var item in Id_ShomareHesabDaramad)
                    {
                        Msg = servic.InsertMahdoodiyatMohasebat_ShomareHesabDaramad(Id, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateMahdoodiyatMohasebat(MahdoodiyatM.fldId, MahdoodiyatM.fldTitle, MahdoodiyatM.fldAzTarikh, MahdoodiyatM.fldTatarikh, MahdoodiyatM.fldNoeKarbar, MahdoodiyatM.fldNoeCodeDaramad, MahdoodiyatM.fldNoeAshkhas, MahdoodiyatM.fldStatus, MahdoodiyatM.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = servic.DeleteMohdoodiyatMohasebat_User(MahdoodiyatM.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    foreach (var item in Id_User)
                    {
                        Msg = servic.InsertMohdoodiyatMohasebat_User(MahdoodiyatM.fldId, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                    Msg = servic.DeleteMahdoodiyatMohasebat_Ashkhas(MahdoodiyatM.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    foreach (var item in Id_Ashkhas)
                    {
                        Msg = servic.InsertMahdoodiyatMohasebat_Ashkhas(MahdoodiyatM.fldId, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                    Msg = servic.DeleteMahdoodiyatMohasebat_ShomareHesabDaramad(MahdoodiyatM.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    foreach (var item in Id_ShomareHesabDaramad)
                    {
                        Msg = servic.InsertMahdoodiyatMohasebat_ShomareHesabDaramad(MahdoodiyatM.fldId, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Msg = servic.DeleteMahdoodiyatMohasebat(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string IdUsers = "";
            string NameUser = "";

            string IdAshkhas = "";
            string NameAshkhas = "";

            string IdShomareHesabDaramad = "";
            string CodeDaramad = "";

            var q = servic.GetMahdoodiyatMohasebatDetail(Id, IP, out Err);
            string NoAshkhas = "0";
            if (q.fldNoeAshkhas)
                NoAshkhas = "1";
            string NoKarbar = "0";
            if (q.fldNoeKarbar)
                NoKarbar = "1";
            string NoeCodeDaramad = "0";
            if (q.fldNoeCodeDaramad)
                NoeCodeDaramad = "1";
            string Status = "0";
            if (q.fldStatus)
                Status = "1";
            var q1 = servic.GetMohdoodiyatMohasebat_UserWithFilter("fldMahdoodiyatMohasebatId", Id.ToString(), 0, IP, out Err).ToList();

            foreach (var item in q1)
            {
                IdUsers = IdUsers + item.fldIdUser + ";";
                NameUser = NameUser + item.fldNameEmployee + ",";
            }
            var q2 = servic.GetMahdoodiyatMohasebat_AshkhasWithFilter("fldMahdoodiyatMohasebatId", Id.ToString(), 0, IP, out Err).ToList();
            foreach (var item in q2)
            {
                IdAshkhas = IdAshkhas + item.fldAshkhasId + ";";
                NameAshkhas = NameAshkhas + item.fldName + ",";
            }
            var q3 = servic.GetMahdoodiyatMohasebat_ShomareHesabDaramadWithFilter("fldMahdoodiyatMohasebatId", Id.ToString(), 0, IP, out Err).ToList();
            foreach (var item in q3)
            {
                IdShomareHesabDaramad = IdShomareHesabDaramad + item.fldShomarehesabDarmadId + ";";
                CodeDaramad = CodeDaramad + item.fldDaramadCode + ",";
            }
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldAzTarikh=q.fldAzTarikh,
                fldTatarikh = q.fldTatarikh,
                IdUsers = IdUsers,
                NameUser = NameUser,
                IdAshkhas = IdAshkhas,
                NameAshkhas = NameAshkhas,
                IdShomareHesabDaramad = IdShomareHesabDaramad,
                CodeDaramad = CodeDaramad,
                NoAshkhas = NoAshkhas,
                NoKarbar = NoKarbar,
                NoeCodeDaramad = NoeCodeDaramad,
                Status = Status
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول استان و امکان سرچ کردن بر اساس   
        /// </summary>

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_MahdoodiyatMohasebat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_MahdoodiyatMohasebat> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNoeKarbarName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeKarbarName";
                            break;
                        case "fldNoeCodeDaramadName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeCodeDaramadName";
                            break;
                        case "fldNoeAshkhasName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeAshkhasName";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStatusName";
                            break;
                        case "fldAzTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAzTarikh";
                            break;
                        case "fldTatarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTatarikh";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMahdoodiyatMohasebatWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetMahdoodiyatMohasebatWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMahdoodiyatMohasebatWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_MahdoodiyatMohasebat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
