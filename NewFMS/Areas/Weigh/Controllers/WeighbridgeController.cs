using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using NewFMS.Controllers;

namespace NewFMS.Areas.Weigh.Controllers
{
    public class WeighbridgeController : Controller
    {
        //
        // GET: /Weigh/Weighbridge/
        WCF_Weigh.WeighService servic = new WCF_Weigh.WeighService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Weigh.ClsError Err = new WCF_Weigh.ClsError();

        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();

        WCF_Daramad.DaramadService servicDaramad = new WCF_Daramad.DaramadService();
        WCF_Daramad.ClsError ErrDaramad = new WCF_Daramad.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId, short FiscalYearId)
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
        public ActionResult Save(WCF_Weigh.OBJ_Weighbridge Weighbridge)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Weighbridge.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertWeighbridge(Weighbridge.fldAshkhasHoghoghiId, Weighbridge.fldName, Weighbridge.fldAddress, CodeDecode.GenerateHash(Weighbridge.fldPassword), Weighbridge.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateWeighbridge(Weighbridge.fldId, Weighbridge.fldAshkhasHoghoghiId, Weighbridge.fldName, Weighbridge.fldAddress,  Weighbridge.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Msg = servic.DeleteWeighbridge(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetWeighbridgeDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldDesc = q.fldDesc,
                fldAddress = q.fldAddress,
                fldAshkhasHoghoghiId = q.fldAshkhasHoghoghiId,
                fldNameAshkhasHoghoghi = q.fldNameAshkhasHoghoghi,
                fldShenaseMelli = q.fldShenaseMelli
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Weigh.OBJ_Weighbridge> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Weigh.OBJ_Weighbridge > data1 = null;
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
                        case "fldNameAshkhasHoghoghi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameAshkhasHoghoghi";
                            break;
                        case "fldShenaseMelli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseMelli";
                            break;
                        case "fldAddress":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAddress";
                            break;
                    }
                    if (data != null)
                    {
                        data1 = servic.GetWeighbridgeWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetWeighbridgeWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetWeighbridgeWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Weigh.OBJ_Weighbridge> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Param_Value(int BaskoolId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.BaskoolId = BaskoolId;
            return PartialView;
        }
        public ActionResult ReadBaskoolParametr_Value(int BaskoolId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });


            var data = servic.SelectBaskoolParametr_Value(BaskoolId, IP,out Err).ToList();
                    return this.Store(data);
                
        }
        public ActionResult LoadDataBaskoolParametr_Value(int BaskoolId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            
                   
                    var data = servic.SelectBaskoolParametr_Value(BaskoolId, IP, out Err).ToList();

                    int[] checkId = null;

                    var check = servic.SelectBaskoolParametr_Value(BaskoolId, IP, out Err).Where(l=>l.fldParam_ValueId!=0).ToList();
                    checkId = new int[check.Count];
                    for (int i = 0; i < check.Count; i++)
                    {
                        checkId[i] = Convert.ToInt32(check[i].fldParam_ValueId);
                    }

                    return new JsonResult()
                    {
                        Data = new
                        {
                            checkId = checkId,
                            Data = data
                        },
                        MaxJsonLength = Int32.MaxValue,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                
        }
        public ActionResult SaveParametr_Value(string DetailsArray1)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                List<WCF_Weigh.OBJ_ParametrBaskoolValue> DetailsArray = JsonConvert.DeserializeObject<List<WCF_Weigh.OBJ_ParametrBaskoolValue>>(DetailsArray1);

                MsgTitle = "ذخیره موفق";
                Msg = "دخیره با موفقیت انجام شد.";
                foreach (var item in DetailsArray)
                {
                    if (item.fldId == 0)
                    {
                        servic.InsertParametrBaskoolValue(item.fldParametrBaskoolId, item.fldBaskoolId, item.fldValue, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        servic.UpdateParametrBaskoolValue(item.fldId, item.fldParametrBaskoolId, item.fldBaskoolId, item.fldValue, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
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
                Err = Er

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteParam_Value(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteParametrBaskoolValue(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult NewParametrBaskol(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.Id = id;
                return PartialView;
            }
        }
        public ActionResult SaveParametrsBaskool(WCF_Weigh.OBJ_ParametrsBaskool ParametrsBaskool)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ParametrsBaskool.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertParametrsBaskool(ParametrsBaskool.fldEnName, ParametrsBaskool.fldFaName, ParametrsBaskool.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult ChangePasBaskol(int BaskoolId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.BaskoolId = BaskoolId;
            return PartialView;
        }
        public ActionResult SaveChangePass(string fldPass, string fldNewPass, int BaskoolId)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Session["Username"] == null)
                    return RedirectToAction("logon", "Account", new { area = "" });
                var user = servic.GetWeighbridgeWithFilter("fldPassword", CodeDecode.GenerateHash(fldPass), 0, IP, out Err).FirstOrDefault();

                if (user == null)
                {
                    return Json(new
                    {
                        Er = 1,
                        MsgTitle = "خطا",
                        Msg = "رمز عبور قبلی وارد شده اشتباه است."
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    MsgTitle = "عملیات موفق";
                    Msg = servic.UpdatePassBaskool(BaskoolId, CodeDecode.GenerateHash(fldNewPass), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);


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
        public ActionResult Arze(string containerId, int BaskoolId, short FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            ViewData.Model = new Models.SaleRules();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                ViewData=ViewData,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.BaskoolId = BaskoolId;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        //public ActionResult Arze(int BaskoolId)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
        //    PartialView.ViewBag.BaskoolId = BaskoolId;
        //    return PartialView;
        //}
        public ActionResult GetOrganization()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            // var q = servic.GetModuleWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            var q = servicCommon.GetOrganizationWithFilter("Baskool", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrCommon).ToList().Select(n => new { ID = n.fldId, Name = n.fldName });
            return this.Store(q);
        }
        public ActionResult GetKala(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            // var q = servic.GetModuleWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            if (Id == 0)/*مود ذخیره*/
            {
                var Arze = servic.GetArzeWithFilter("fldOrganId", "", OrganId, 0, IP, out Err).ToList().Select(n => new { ID = n.fldKalaId, Name = n.fldNameKala });
                var kala = servicCommon.GetKalaWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrCommon).ToList().Select(n => new { ID = n.fldId, Name = n.fldName });
                var q = kala.Except(Arze);
                return this.Store(q);
            }
            else
            {
                var Arze = servic.GetArzeWithFilter("fldOrganId", "", OrganId, 0, IP, out Err).Where(l=>l.fldId!=Id).ToList()
                    .Select(n => new { ID = n.fldKalaId, Name = n.fldNameKala });
                var kala = servicCommon.GetKalaWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrCommon).ToList().Select(n => new { ID = n.fldId, Name = n.fldName });
                var q = kala.Except(Arze);
                return this.Store(q);
            }
        }
        
        //public ActionResult Vazn_Baskool(string BaskoolId)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
        //    PartialView.ViewBag.BaskoolId = BaskoolId;
        //    return PartialView;
        //}
        public ActionResult Vazn_Baskool(string containerId, string BaskoolId, short FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.BaskoolId = BaskoolId;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult GetVaznBaskol(string BaskoolId, string Pelak)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Vazn = 0;
            var P = servicCommon.GetPlaqueWithFilter("fldSerial_Plaque", Pelak, 0, IP, out ErrCommon).FirstOrDefault();
            var PelakId = 0;
            if (P != null)
            {
                PelakId = P.fldId;
            }
            
            
            //var q = servic.GetTozinWithFilter("WeighbridgeId_PlaqueId", BaskoolId, PelakId.ToString(), "", "", 0, IP, out Err).FirstOrDefault();
            //if (q != null)
            //{
            //    Vazn = q.fldMaxW;
            //}
            return Json(new { Vazn = Vazn }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCharacterPelak()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetCharacterPersianPlaqueWithFilter("", "", 0, IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }

        public ActionResult GetMasaleh(string BaskoolId,string OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetArzeWithFilter("fldBaskoolId_OrganId", BaskoolId.ToString(), Convert.ToInt32(OrganId), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldKalaId, fldName = c.fldNameKala, fldStatusForoosh = c.fldStatusForoosh, fldVaznVahed = c.fldVaznVahed });
            return this.Store(q);
        }

        public ActionResult GetNoeMasraf()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetNoeMasrafWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetLoadingPlace()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetLoadingPlaceWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult ReadParams(StoreRequestParameters parameters, int ShomareHesabCodeDaramadId,int BaskoolId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            NewFMS.Models.WeightEntities m=new Models.WeightEntities();
            List<Models.spr_SelectParamerValueFromArze> data = null;
            data = m.spr_SelectParamerValueFromArze(ShomareHesabCodeDaramadId, BaskoolId, OrganId).ToList();            

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<Models.spr_SelectParamerValueFromArze> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveSalesRules(WCF_Weigh.OBJ_Arze Header, List<WCF_Weigh.OBJ_Arze_Detail> Detail)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                Header.fldMablagh = Header.fldMablagh * Header.fldTedad;
                if (Header.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";
                    var Id = servic.InsertArze(Header.fldBaskoolId, Header.fldKalaId, Header.fldShomareHesabCodeDaramadId,Header.fldTedad,Header.fldMablagh,
                        Header.fldOrganId, Convert.ToByte(Header.fldStatusForoosh), Header.fldVaznVahed, Header.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Err = 1

                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (Detail != null)
                    {
                        foreach (var item in Detail)
                        {
                            servic.InsertArze_Detail(Id, item.fldParametrSabetCodeDaramd, item.fldValue, item.fldFlag, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }
                    }
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = "ویرایش با موفقیت انجام شد.";

                    var q = servic.GetVazn_BaskoolWithFilter("ForooshAdi", Header.fldKalaId.ToString(),16, Header.fldOrganId, 1, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        return Json(new
                        {
                            Msg = "برای کد درآمد مورد نظر فیش صادر شده است و امکان ویرایش وجود ندارد.",
                            MsgTitle = "خطا",
                            Err = 1

                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = servic.UpdateArze(Header.fldId, Header.fldBaskoolId, Header.fldKalaId, Header.fldShomareHesabCodeDaramadId,Header.fldTedad,Header.fldMablagh,
                        Header.fldOrganId, Convert.ToByte(Header.fldStatusForoosh), Header.fldVaznVahed, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Err = 1

                        }, JsonRequestBehavior.AllowGet);
                    }
                    servic.DeleteArze_Detail(Header.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Detail != null)
                    {
                        foreach (var item in Detail)
                        {
                            servic.InsertArze_Detail(Header.fldId, item.fldParametrSabetCodeDaramd, item.fldValue, item.fldFlag, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult ReadSalesRules(StoreRequestParameters parameters, int BaskoolId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                List<WCF_Weigh.OBJ_Arze> data = null;
                data = servic.GetArzeWithFilter("fldBaskoolId", BaskoolId.ToString(), 0, 0, IP, out Err).ToList();

                //-- start paging ------------------------------------------------------------
                int limit = parameters.Limit;

                if ((parameters.Start + parameters.Limit) > data.Count)
                {
                    limit = data.Count - parameters.Start;
                }

                List<WCF_Weigh.OBJ_Arze> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
                //-- end paging ------------------------------------------------------------

                return this.Store(rangeData, data.Count); 
            }
            catch (Exception x)
            {
                
                throw;
            }
           
        }
        public ActionResult ReadDetail(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Weigh.OBJ_Arze_Detail> data = null;
            data = servic.GetArze_DetailWithFilter("fldHeaderId", HeaderId.ToString(), 0, 0, IP, out Err).ToList();
            return this.Store(data);
        }
        public ActionResult DetailSalesRules(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetArzeDetail(Id,IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldKalaId = q.fldKalaId.ToString(),
                fldOrganId = q.fldOrganId.ToString(),
                fldShomareHesabCodeDaramadId = q.fldShomareHesabCodeDaramadId,
                fldDaramadCode = q.fldDaramadCode,
                fldDaramadTitle = q.fldDaramadTitle,
                fldStatusForoosh=q.fldStatusForoosh.ToString(),
                fldVaznVahed=q.fldVaznVahed
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult DetailMaghadirArze(string ArzeID)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    string Msg = "", MsgTitle = ""; int Er = 0; 

        //    string[] value = new string[1];
        //    string[] ParametreSabetId = new string[1];
        //    //string[] itemPropertyName = new string[1];
        //    try
        //    {
        //        var q = servic.GetArze_DetailWithFilter("fldHeaderId", ArzeID, 0, 0, IP, out Err).ToList();
        //        if (Err.ErrorType)
        //        {
        //            return Json(new
        //            {
        //                Msg = Err.ErrorMsg,
        //                MsgTitle = "خطا",
        //                Er = Er
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //        if (q.Count != 0)
        //        {
        //            value = new string[q.Count];
        //            ParametreSabetId = new string[q.Count];
        //            for (int i = 0; i < q.Count; i++)
        //            {
        //                value[i] = q[i].fldValue2;
        //                ParametreSabetId[i] = q[i].fldParametrSabetCodeDaramd.ToString();

        //            }
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        if (x.InnerException != null)
        //            Msg = x.InnerException.Message;
        //        else
        //            Msg = x.Message;

        //        MsgTitle = "خطا";
        //        Er = 1;
        //    }
        //    return Json(new
        //    {
        //        value = value,
        //        ParametreSabetId = ParametreSabetId,
        //        //itemPropertyName=itemPropertyName,
        //        Msg = Msg,
        //        MsgTitle = MsgTitle,
        //        Er = Er
        //    }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult DeleteSalesRules(int id,int KalaId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                var q = servic.GetVazn_BaskoolWithFilter("ForooshAdi", KalaId.ToString(), 16, OrganId, 1, IP, out Err).FirstOrDefault();
                if (q != null)
                {
                    return Json(new
                    {
                        Msg = "برای کد درآمد مورد نظر فیش صادر شده است و امکان حذف وجود ندارد.",
                        MsgTitle = "خطا",
                        Er = 1

                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = servic.DeleteArze(id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult GetChart()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetChartEjraee_LastNode(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrCommon).OrderBy(l=>l.fldTitle).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetTypeKhodro()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetTypeKhodroWithFilter("","",0, IP, out ErrCommon).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldName });
            return this.Store(q);
        }
        public ActionResult ReadTozinOnline(StoreRequestParameters parameters, string BaskoolId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Weigh.OBJ_Vazn_Baskool> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Weigh.OBJ_Vazn_Baskool> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldIsporName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldIsporName";
                            break;
                        case "fldPlaque":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPlaque";
                            break;
                        case "fldNameRanande":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameRanande";
                            break;
                        case "fldNameKala":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameKala";
                            break;
                        case "fldNameMasraf":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameMasraf";
                            break;
                        case "fldTarfHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarfHesab";
                            break;
                        case "fldTarikh_TimeTozin":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh_TimeTozin";
                            break;
                        case "fldVaznKhals":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldVaznKhals";
                            break;
                        case "fldVaznKol":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldVaznKol";
                            break;
                    }
                    if (data != null)
                    {
                        data1 = servic.GetVazn_BaskoolWithFilter(field, searchtext,16, OrganId, 100, IP, out Err).Where(l=>l.fldBaskoolId==Convert.ToInt32(BaskoolId)).ToList();
                    }
                    else
                    {
                        data = servic.GetVazn_BaskoolWithFilter(field, searchtext,16, OrganId, 100, IP, out Err).Where(l => l.fldBaskoolId == Convert.ToInt32(BaskoolId)).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetVazn_BaskoolWithFilter("fldBaskoolId", BaskoolId,16, OrganId, 100, IP, out Err).ToList();
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

            List<WCF_Weigh.OBJ_Vazn_Baskool> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveTozinOnline(WCF_Weigh.OBJ_Vazn_Baskool Vazn_Baskool, string harf, string fldPlaque2, string fldPlaque3,byte Serial,int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int TozinOnlineId = 0; int ElameAvarezId = 0; int FishId = 0;
            try
            {
                if (Vazn_Baskool.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";
                    TozinOnlineId = servic.InsertVazn_Baskool(harf, fldPlaque2, fldPlaque3, Serial, Vazn_Baskool.fldOrganId, Vazn_Baskool.fldRananadeId, Vazn_Baskool.fldNoeMasrafId, Vazn_Baskool.fldAshkhasId, Vazn_Baskool.fldChartOrganEjraeeId,
                        Vazn_Baskool.fldLoadingPlaceId,Vazn_Baskool.fldKalaId,Vazn_Baskool.fldVaznKol,Vazn_Baskool.fldRemittanceId,Vazn_Baskool.fldBaskoolId,Vazn_Baskool.fldIsPor,Vazn_Baskool.fldTypeKhodroId,Vazn_Baskool.fldTedad,Vazn_Baskool.fldTypeMohasebe, Vazn_Baskool.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                    }
                 
                    var rec=servic.GetVazn_BaskoolWithFilter("fldId", TozinOnlineId.ToString(),16, Vazn_Baskool.fldOrganId, 1, IP, out Err).FirstOrDefault();
                    if (Vazn_Baskool.fldNoeMasrafId == 2)//عادی
                    {
                    decimal? TonKilu = 0;
                        var Arze = servic.GetArzeWithFilter("fldBaskoolId_OrganId", Vazn_Baskool.fldBaskoolId.ToString(), Vazn_Baskool.fldOrganId, 0, IP, out Err)
                            .Where(l => l.fldKalaId == Convert.ToInt32(Vazn_Baskool.fldKalaId)).FirstOrDefault();

                        TonKilu = rec.fldVaznKhals;

                        int TedadForErsal = Arze.fldTedad;
                        if (Arze.fldStatusForoosh == 1)
                            TedadForErsal = Convert.ToInt32(rec.fldTedad);

                        long PriceForErsal = Arze.fldMablagh / Arze.fldTedad;
                        if (Arze.fldStatusForoosh == 1)
                            PriceForErsal = Convert.ToInt64(Arze.fldVaznVahed);

                        var ArzeDetail = servic.GetArze_DetailWithFilter("fldHeaderId", Arze.fldId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).
                            Where(l => l.fldId > 0).Select(l => new WCF_Daramad.OBJ_ParametreSabet_Value()
                            {
                                fldParametreSabetId = Convert.ToInt32(l.fldParametrSabetCodeDaramd),
                                fldValue = l.fldFlag ? (Convert.ToDecimal(l.fldValue_Arze) * rec.fldVaznKhals).ToString() : l.fldValue_Arze
                            }).ToList();
                        NewFMS.Areas.CreateFishController CF = new NewFMS.Areas.CreateFishController();
                        CF.Index(Arze.fldShomareHesabCodeDaramadId, Convert.ToInt32(Vazn_Baskool.fldAshkhasId), Vazn_Baskool.fldOrganId, ArzeDetail,TozinOnlineId,
                            PriceForErsal, TedadForErsal, TonKilu, Session["Username"].ToString(), Session["Password"].ToString(),
                            Convert.ToInt32(Session["OrganId"]),FiscalYearId, out ErrCommon);
                        if (ErrCommon.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = ErrCommon.ErrorMsg;
                            Er = 1;
                        }
                        else
                        {
                            ElameAvarezId = ErrCommon.OutputId;
                            FishId = ErrCommon.OutputId2;
                        }
                    }
                }
                else
                {
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateVazn_Baskool(Vazn_Baskool.fldId,Vazn_Baskool.fldPluqeId, Vazn_Baskool.fldOrganId, Vazn_Baskool.fldRananadeId, Vazn_Baskool.fldNoeMasrafId, Vazn_Baskool.fldAshkhasId, Vazn_Baskool.fldChartOrganEjraeeId,
                        Vazn_Baskool.fldLoadingPlaceId, Vazn_Baskool.fldKalaId, Vazn_Baskool.fldVaznKol, Vazn_Baskool.fldRemittanceId, Vazn_Baskool.fldBaskoolId, Vazn_Baskool.fldIsPor, Vazn_Baskool.fldTypeKhodroId, Vazn_Baskool.fldTedad, Vazn_Baskool.fldTypeMohasebe, Vazn_Baskool.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    TozinOnlineId = Vazn_Baskool.fldId;
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
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
                Err = Er,
                TozinOnlineId = TozinOnlineId,
                FishId=FishId,
                ElameAvarezId=ElameAvarezId,
                IsPor=Vazn_Baskool.fldIsPor
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInfoTozinOnline(string harf, string fldPlaque2, string fldPlaque3, string Serial, string BaskoolId,bool CheckPor,string HavaleId,string KalaId,int OrganId,decimal VaznKol,bool TypeMohasebe)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //string Msg = "";
            int RanandeId = 0; string TypeKhodro = ""; string NameRan = ""; string fldTarikhVaznKhali = "";
            decimal? VaznKhali = 0m; decimal? VaznKhales = 0m; decimal? fldBaghimande = 0m; decimal? fldCountHavale = 0m; bool? iskhali = false;
            int Lastrooz=0;
            if (fldPlaque2.Length == 2 && fldPlaque3.Length == 3 && Serial.Length == 2)
            {
                var q = servic.GetRannadeInPlaque("Ranande",Convert.ToByte( Serial), harf, fldPlaque2, fldPlaque3, Convert.ToInt32(BaskoolId),OrganId, IP, out Err).FirstOrDefault();
                if (q.fldRananadeId != 0)
                {
                    RanandeId = q.fldRananadeId;
                    NameRan = q.fldNameRanande;
                    TypeKhodro = q.fldTypeKhodroId.ToString();
                }
                var q1 = servic.GetRannadeInPlaque("ExistsKhali", Convert.ToByte(Serial), harf, fldPlaque2, fldPlaque3, Convert.ToInt32(BaskoolId), OrganId, IP, out Err).FirstOrDefault();
                if (q1!=null)
                {
                    iskhali = q1.IsKhali;
                }
                if (CheckPor == true)
                {
                   
                    var q3 = servic.SelectVaznKhals_VaznKhali(Convert.ToByte(Serial), harf, fldPlaque2, fldPlaque3, Convert.ToInt32(BaskoolId), OrganId, VaznKol,TypeMohasebe, IP, out Err).FirstOrDefault();
                    if (q3 != null)
                    {
                        VaznKhali = q3.fldVaznKhali;
                        VaznKhales = q3.fldVaznKhals;
                        fldTarikhVaznKhali = q3.fldTarikhVaznKhali;
                        if (q3.lastHour == false)
                            Lastrooz = 1;
                    }
                    if (HavaleId != ""&&KalaId!=null)
                    {
                        var q4 = servic.SelectMandeHavali(Convert.ToInt32(HavaleId), Convert.ToInt32(KalaId), Convert.ToInt32(BaskoolId), IP, out Err).FirstOrDefault();
                        if (q4 != null)
                        {
                            fldBaghimande = q4.fldBaghimande;
                            fldCountHavale = q4.fldCountHavale;
                        }
                    }
                }
            }
            
            return Json(new{
                RanandeId = RanandeId,
                NameRan = NameRan,
                TypeKhodro=TypeKhodro,
                //Msg = Msg,
                fldBaghimande = fldBaghimande,
                fldCountHavale = fldCountHavale,
                VaznKhali = VaznKhali,
                VaznKhales = VaznKhales,
                fldTarikhVaznKhali = fldTarikhVaznKhali,
                iskhali = iskhali,
                Lastrooz = Lastrooz
            },JsonRequestBehavior.AllowGet);
        }
        public ActionResult Print(int TozinOnlineId, int TypePrint, int? fldNoeMasrafId, int FishId, int ElameAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            servic.UpdateIsprintBaskool(TozinOnlineId, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
            result.ViewBag.TozinOnlineId = TozinOnlineId;
            result.ViewBag.TypePrint = TypePrint;
            result.ViewBag.FishId = FishId;
            result.ViewBag.ElameAvarezId = ElameAvarezId;

            result.ViewBag.fldNoeMasrafId = fldNoeMasrafId;
            return result;
        }

        public ActionResult GeneratePdfRptTozinOnline(int TozinOnlineId, int TypePrint, int? fldNoeMasrafId, int FishId, int ElameAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter logo = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptFishBaskoolTableAdapter Fish = new DataSet.DataSet1TableAdapters.spr_RptFishBaskoolTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptFishTableAdapter FishDaramad = new DataSet.DataSet1TableAdapters.spr_RptFishTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptMablaghDaramdTableAdapter CodeDaramd = new DataSet.DataSet1TableAdapters.spr_RptMablaghDaramdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblPcPosTransactionSelectTableAdapter PCPos = new DataSet.DataSet1TableAdapters.spr_tblPcPosTransactionSelectTableAdapter();

                dt.EnforceConstraints = false;
                var E = servic.GetVazn_BaskoolWithFilter("fldId", TozinOnlineId.ToString(),16, Convert.ToInt32(Session["OrganId"]),0, IP, out Err).FirstOrDefault();
                var OrganId =E.fldOrganId;
                var Organ = servicCommon.GetOrganizationDetail(OrganId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrCommon);
                logo.Fill(dt_Com.spr_tblFileSelect, "fldId", Organ.fldFileId.ToString(), 1);
                Fish.Fill(dt.spr_RptFishBaskool, TozinOnlineId);
                GetDate.Fill(dt_Com.spr_GetDate);
                var User = servicCommon.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDBDataSet");
                Report.RegisterData(dt, "rasaFMSDBDataSet");

                if (fldNoeMasrafId == 2)
                {
                    PCPos.Fill(dt.spr_tblPcPosTransactionSelect, "fldFishId", FishId.ToString(), 1);
                    var PCPosCount = dt.spr_tblPcPosTransactionSelect.Rows.Count;
                    //var ElamAvarez = servicDaramad.GetElamAvarezDetail(ElameAvarezId, Session["OrganId"].ToString(), IP, out ErrDaramad);استفاده نشده بود کامنت کردم

                    FishDaramad.Fill(dt.spr_RptFish, FishId);
                    CodeDaramd.Fill(dt.spr_RptMablaghDaramd, FishId);

                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Baskool\RptFishBaskool_A5_Daramad.frx");





                    Report.RegisterData(dt, "rasaFMSDaramad");
                    Report.RegisterData(dt_Com, "rasaFMSDaramad");
                    string tarikh = servicCommon.GetDate(IP, out ErrCommon).fldTarikh;
                    string sal = tarikh.Substring(0, 4);
                    string TaTarikh = "";
                    if (MyLib.Shamsi.Iskabise(Convert.ToInt32(sal)))
                        TaTarikh = sal + "/12/30";
                    else
                        TaTarikh = sal + "/12/29";
                    Report.SetParameterValue("AzTarikh", sal + "/01/01");
                    Report.SetParameterValue("TaTarikh", TaTarikh);
                    Report.SetParameterValue("Mohlat", "مهلت پرداخت فیش تا پایان سال " + sal + "میباشد و بعد از آن فاقد اعتبار است.");
                    Report.SetParameterValue("OrganName", Organ.fldName);
                    Report.SetParameterValue("CodeSh", ElameAvarezId);
                    Report.SetParameterValue("PCPosCount", PCPosCount);
                }

                else if (TypePrint == 1){
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Baskool\RptFishBaskool.frx");
                    Report.SetParameterValue("UserName", User.fldNameEmployee);
                }
                else{
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Baskool\RptFishBaskool_A5.frx");
                    Report.SetParameterValue("UserName", User.fldNameEmployee);
                }

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();                
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DetialTozinOnline(int TozinOnlineId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetVazn_BaskoolWithFilter("fldId", TozinOnlineId.ToString(),16, OrganId, 0, IP, out Err).FirstOrDefault();
            string[] pelakS = q.fldPlaque.Split('-');

            var q3 = pelakS[1].Substring(0, 3);
            var Harf = pelakS[1].Substring(3, 1);
            var q2 = pelakS[1].Substring(4, 2);
            var serial = pelakS[0].Trim();

            var fldTypeMohasebe = "0";
            if (q.fldTypeMohasebe==true)
                fldTypeMohasebe = "1";

            int? fldHavaleFileId = 0;
            if (q.fldRemittanceId != null)
            {
                var k=servic.GetRemittance_HeaderWithFilter("fldId",q.fldRemittanceId.ToString(), OrganId,0, IP, out Err).FirstOrDefault();
                if (k.fldFileId != null)
                    fldHavaleFileId = k.fldFileId;
            }

            return Json(new
            {
                fldPlaque3 = q3,
                Pelak_2 = q2,
                charcterPelak = Harf,
                serial=serial,
                fldId = q.fldId,
                fldAshkhasId = q.fldAshkhasId,
                fldBaghimande = q.fldBaghimande,
                fldChartOrganEjraeeId = q.fldChartOrganEjraeeId.ToString(),
                fldCountHavale = q.fldCountHavale,
                fldKalaId = q.fldKalaId.ToString(),
                fldLoadingPlaceId = q.fldLoadingPlaceId.ToString(),
                fldNoeMasrafId = q.fldNoeMasrafId.ToString(),
                fldOrganId = q.fldOrganId.ToString(),
                fldPluqeId = q.fldPluqeId,
                fldRananadeId = q.fldRananadeId,
                fldRemittanceId = q.fldRemittanceId,
                fldTarikhVaznKhali = q.fldTarikhVaznKhali,
                fldTypeKhodroId = q.fldTypeKhodroId.ToString(),
                fldVaznKhali = q.fldVaznKhali,
                fldVaznKhals = q.fldVaznKhals,
                fldVaznKol = q.fldVaznKol,
                fldIsPor = q.fldIsPor,
                fldNameRanande = q.fldNameRanande,
                fldNameAshkhas = q.fldNameAshkhas,
                fldNameHavale = q.fldNameHavale,
                fldTedad = q.fldTedad,
                fldTypeMohasebe = fldTypeMohasebe,
                fldHavaleFileId = fldHavaleFileId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAkharinRec(string BaskoolId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int? LastChart = 0;
            var q = servic.GetVazn_BaskoolWithFilter("LastChart", BaskoolId.ToString(),16,OrganId, 0, IP, out Err).FirstOrDefault();
            
            if (q != null)
            {
                LastChart = q.fldChartOrganEjraeeId;
            }


            return Json(new { LastChart = LastChart.ToString() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckVaznKhales(int TozinOnlineId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "";
            var q = servic.GetVazn_BaskoolWithFilter("fldId", TozinOnlineId.ToString(),16, 0, 0, IP, out Err).Where(l=>l.fldVaznKhals!=0).FirstOrDefault();
            if(q==null)
                Msg = "وزن خالی پلاک مورد نظر ثبت نشده لدا مجاز به چاپ نمی باشید.";
            return Json(new { Msg = Msg }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetIdSahebBar(int RananadeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int? AshkhasId = 0;
            var q = servicCommon.GetAshkhasWithFilter("fldHaghighiId", RananadeId.ToString(), "", 0, IP, out ErrCommon).FirstOrDefault();

            if (q != null)
            {
                AshkhasId = q.fldId;
            }


            return Json(new { AshkhasId = AshkhasId }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EbtalTozinOnline(int TozinOnlineId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                NewFMS.Models.WeightEntities m=new Models.WeightEntities();
                var q = m.spr_SelectIsEbtalFishDaramd(TozinOnlineId, OrganId, 16).FirstOrDefault();
                if (q != null && q.fldEbtal==false)
                {
                    return Json(new
                    {
                        Msg = "امکان ابطال رکورد مورد نظر وجود ندارد.",
                        MsgTitle = "خطا",
                        Err = 1,
                    }, JsonRequestBehavior.AllowGet);
                }
                    MsgTitle = "عملیات موفق";
                    Msg = servic.UpdateEbtalBaskool(TozinOnlineId, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
               
               
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
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTypeReport(string BaskoolId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int? Type = 0;
            var q = servic.GetTypeReportWithFilter("fldOrganId_BaskoolId", OrganId.ToString(), BaskoolId, 0, IP, out Err).FirstOrDefault();

            if (q != null)
            {
                Type = q.fldType;
            }


            return Json(new { Type = Type.ToString() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTypeReport(byte CboTypePrint, int OrganId, int BaskoolId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                //ذخیره
                MsgTitle = "عملیات موفق";
                Msg = servic.InsertTypeReport(CboTypePrint,OrganId,BaskoolId, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);


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
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveElamAvarez(int KalaId, int AshkhasId, int BaskoolId, int OrganId,int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; var HaveFormulId = 0; var EnFormul = "";
            int ShomareHesabCodeDaramadId = 0; long MablaghAsli = 0; int Tedad = 0; int IDElamAvarez = 0; string SharheCodeDaramad = ""; string MatnSharhCodeWithParamId = "";
            int IDCodeDaramadElamAvarez = 0; var HaveParamInSharh = 0;
            try
            {

                var Arze = servic.GetArzeWithFilter("fldKalaId_OrganId", KalaId.ToString(), OrganId, 0, IP, out Err).FirstOrDefault();
                if (Arze != null)
                {
                   // var detail = servic.GetArze_DetailWithFilter("fldId", Arze.fldId.ToString(), Arze.fldOrganId, 0, IP, out Err).ToList();
                    ShomareHesabCodeDaramadId = Arze.fldShomareHesabCodeDaramadId;
                    MablaghAsli = Arze.fldMablagh;
                    Tedad = Arze.fldTedad;
                    var k = servicDaramad.GetShomareHesabCodeDaramadDetail(Convert.ToInt32(Arze.fldShomareHesabCodeDaramadId),FiscalYearId, IP, out ErrDaramad);
                    if (k.fldSharhCodDaramd != null)
                    {
                        SharheCodeDaramad = k.fldSharhCodDaramd.Replace("|", "");
                        MatnSharhCodeWithParamId = SharheCodeDaramad;
                        var z = k.fldSharhCodDaramd.Split('|');
                        for (byte i = 0; i < z.Length - 1; i++)
                        {
                            var s = servic.GetArze_DetailWithFilter("fldNameParametreEn", z[i], Arze.fldOrganId, 0, IP, out Err).FirstOrDefault();
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
                        SharheCodeDaramad = k.fldDaramadTitle;
                        MatnSharhCodeWithParamId = k.fldDaramadTitle;
                    }
                    if (k.fldFormulMohasebatId != null)
                        HaveFormulId = 1;
                    else if (k.fldFormolsaz != null)
                    {
                        HaveFormulId = 2;
                        EnFormul = k.fldFormolsaz;
                    }
                    IDElamAvarez = servicDaramad.InsertElamAvarez(Convert.ToInt32(AshkhasId), false, Convert.ToInt32(OrganId), false, null, "", Arze.fldDaramadTitle, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrDaramad);
                    if (ErrDaramad.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrDaramad.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    IDCodeDaramadElamAvarez = servicDaramad.InsertCodhayeDaramadiElamAvarez(IDElamAvarez, "", Convert.ToInt32(Arze.fldShomareHesabCodeDaramadId), 0, 1, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrDaramad);
                    if (ErrDaramad.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrDaramad.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1,
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
                ElamAvarezId = IDElamAvarez,
                HaveFormulId = HaveFormulId,
                EnFormul = EnFormul,
                CodeDaramadElamAvarezId = IDCodeDaramadElamAvarez,
                ShomareHesabCodeDaramadId=ShomareHesabCodeDaramadId,
                MablaghAsli = MablaghAsli,
                Tedad = Tedad,
                SharheCodeDaramad = SharheCodeDaramad,
                MatnSharhCodeWithParamId = MatnSharhCodeWithParamId,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Mohasebe(int HaveFormulId, long Mablagh, string ShomareHesabCodeDaramadId, int ElamAvarezId, string sharheCodeDaramad, int CodeDaramadElamAvarezID, 
            int fldTedad,int FiscalYearId/*string CodeDaramadId, int AshkhasId, List<WCF_Daramad.OBJ_ParametreSabet_Value> val*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "عملیات موفق"; var Er = 0;

            if (HaveFormulId == 1)
            {
                string Valid = "";
                var pa = servicDaramad.GetShomareHesabCodeDaramadWithFilter("fldId", ShomareHesabCodeDaramadId, "", FiscalYearId,0, IP, out ErrDaramad).FirstOrDefault();
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
            Msg = servicDaramad.UpdateCodhayeDaramadiElamAvarez(CodeDaramadElamAvarezID, ElamAvarezId, sharheCodeDaramad, Convert.ToInt32(ShomareHesabCodeDaramadId), Mablagh, fldTedad, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrDaramad);



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
    }
}
