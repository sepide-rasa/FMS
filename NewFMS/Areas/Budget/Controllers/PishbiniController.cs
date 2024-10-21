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
using NewFMS.Models;

namespace NewFMS.Areas.Budget
{
    public class PishbiniController : Controller
    {
        // GET: Budget/Pishbini
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Budget.BudgetService servic = new WCF_Budget.BudgetService();
        WCF_Accounting.AccountingService Accservice = new WCF_Accounting.AccountingService();

        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Accounting.ClsError Err_Acc = new WCF_Accounting.ClsError();

        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
        public ActionResult IndexDaramadi(string containerId, short Year, int NoeAmaliat)
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
            result.ViewBag.Year = Year;
            result.ViewBag.NoeAmaliat = NoeAmaliat;
            return result;
        }
        public ActionResult Tas_him(string containerId, short Year)
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
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult IndexHazineEghtesadi(string containerId, short Year, int NoeAmaliat)
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
            result.ViewBag.Year = Year;
            result.ViewBag.NoeAmaliat = NoeAmaliat;
            return result;
        }
        public ActionResult IndexbudgeProject(string containerId, short Year, int NoeAmaliat)
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
            result.ViewBag.Year = Year;
            result.ViewBag.NoeAmaliat = NoeAmaliat;
            return result;
        }
        public ActionResult GetMotamam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMotammamWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTarikh });
            return this.Store(q);
        }
        public ActionResult GetMasrafType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMasrafTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetMamooriat(int Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var HeaderId = 0;
            var Header= servic.GetCodingBudje_HeaderWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
            if (Header != null)
                HeaderId = Header.fldId;
            var q = servic.GetCodingBudje_DetailWithFilter("fldHeaderId", HeaderId.ToString(),"","", 0, IP, out Err).Where(l=>l.fldLevelId==1).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Read(StoreRequestParameters parameters, string Year, int MotamamId,int state)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_Budget.OBJ_Pishbini_Daramad> data = null;
            if(state==1)
                data = servic.SelectPishbiniBudje("Daramadi", Year, MotamamId, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();
            else
            data = servic.SelectPishbiniBudje_SalGhabl("Daramadi", Year, MotamamId, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();

                return this.Store(data, data.Count);
        }

        public ActionResult SaveTas_him(List<Models.Tas_him> Tas_him)        
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; 
            try
            {

                foreach (var item in Tas_him)
                {
                    string NoeMasraf = "";
                    if (item.fldMali)
                        NoeMasraf = NoeMasraf + "3;1,";
                    if (item.fldHazine)
                        NoeMasraf = NoeMasraf + "1;1,";
                    if (item.fldSarmaye)
                        NoeMasraf = NoeMasraf + "2;1,";

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertTas_him(item.fldCodingAcc_DetailsId, item.fldHozeMasraf +","+ item.fldHozeMamooriyat[0]+"," + NoeMasraf, Convert.ToDecimal(item.fldPercentHazine), Convert.ToDecimal(item.fldPercentTamallok), Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult SaveMosavab(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; string NoeMasraf = "";
            try
            {
                MsgTitle = "ذخیره موفق";
                Msg = servic.CopytoMosavab(Year, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult ReadTas_him(StoreRequestParameters parameters, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Budget.OBJ_CodehayeDaramad_Tas_him> data = null;
            List<Models.Tas_him> data2 = null;
            data = servic.SelectTashimDaramadCode(Year, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();
            data2=data.Select(l => new Models.Tas_him()
            {
                fldCode = l.fldCode,
                fldCodingAcc_DetailsId = l.fldCodingAcc_DetailsId,
                fldDaramadCode = l.fldDaramadCode,
                fldHazine = l.fldHazine == "0" ? false : true,
                fldSarmaye = l.fldSarmaye == "0" ? false : true,
                fldMali = l.fldMali == "0" ? false : true,
                fldHozeMamooriyat = l.fldHozeMamooriyat.Split(',').Reverse().Skip(1).ToList(),
                fldHozeMasraf =l.fldHozeMasraf == "0" ? "" : l.fldHozeMasraf,
                fldPercentHazine = l.fldPercentHazine,
                fldPercentTamallok = l.fldPercentTamallok,
                fldTitle = l.fldTitle
            }).ToList();
            return this.Store(data2, data2.Count);
        }
        public ActionResult SaveDaramadi(List<MablaghPerCoding> Mablagh_Coding/*List<WCF_Budget.OBJ_Anticipation> DaramadiVal, string Year, int MotamamId*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in Mablagh_Coding)
                {
                    MsgTitle = "ذخیره موفق";
                    System.Data.DataTable dt = new System.Data.DataTable { TableName = "Bud.tblPishbini" };
                    using (var reader = FastMember.ObjectReader.Create(item.Mablagh))
                    {
                        dt.Load(reader);
                    }
                    dt.Columns.Remove("Name");
                    dt.Select(string.Format("[BudgetTypeId] = '{0}'", 6))
                     .ToList<DataRow>()
                     .ForEach(r =>
                     {
                         r["MotammamId"] = DBNull.Value;
                     });
                    Msg = servic.InsertAnticipation(item.fldAccCodingId, null, dt, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult ReadEghtesadi(StoreRequestParameters parameters, string Year, int MotamamId, int state)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Budget.OBJ_Pishbini_Daramad> data = null;


            if (state == 1)
                data = servic.SelectPishbiniBudje("Eghtesadi", Year, MotamamId, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();
            else
                data = servic.SelectPishbiniBudje_SalGhabl("Eghtesadi", Year, MotamamId, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();

            return this.Store(data, data.Count);
        }
        public ActionResult SaveEghtesadi(List<MablaghPerCoding>Mablagh_Coding/*List<WCF_Budget.OBJ_Anticipation> EghtesadiVal, string Year, int MotamamId*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in Mablagh_Coding)
                {
                    MsgTitle = "ذخیره موفق";
                    System.Data.DataTable dt = new System.Data.DataTable { TableName = "Bud.tblPishbini" };
                    using (var reader = FastMember.ObjectReader.Create(item.Mablagh))
                    {
                        dt.Load(reader);
                    }
                    dt.Columns.Remove("Name");
                    Msg = servic.InsertAnticipation(item.fldAccCodingId, null,dt, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult KhedmatHa(short Year, int CodingAcc)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            result.ViewBag.Year = Year.ToString();
            result.ViewBag.CodingAcc = CodingAcc;
            return result;
        }
        public ActionResult ReadKhedmatHa(StoreRequestParameters parameters, int CodingAcc)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Budget.OBJ_Budje_khedmatDarsadId> data = null;
            data = servic.GetBudje_khedmatDarsadIdWithFilter("fldCodingAcc_detailId", CodingAcc.ToString(), 0, IP, out Err).ToList();           

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }
            List<WCF_Budget.OBJ_Budje_khedmatDarsadId> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveKhedmatHa(WCF_Budget.OBJ_Budje_khedmatDarsadId s)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //if (s.fldBudje_khedmatDarsadId == 0)
                //{
                //    //ذخیره
                //    MsgTitle = "ذخیره موفق";
                //    Msg = servic.InsertBudje_khedmatDarsadId(s.fldCodingAcc_detailId, s.fldCodingBudje_DetailsId,s.fldDarsad, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                //}
                //else
                //{
                //    //ویرایش
                //    MsgTitle = "ویرایش موفق";
                //    Msg = servic.UpdateBudje_khedmatDarsadId(s.fldBudje_khedmatDarsadId, s.fldCodingAcc_detailId, s.fldCodingBudje_DetailsId, s.fldDarsad, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                //}
                var q = servic.GetBudje_khedmatDarsadIdWithFilter("fldCodingAcc_detailId_Darsad", s.fldCodingAcc_detailId.ToString(), 1, IP, out Err).FirstOrDefault();
                if (q == null)
                {
                    MsgTitle = "عملیات موفق";
                    Msg = servic.InsertOnlyKhedmat(s.fldCodingAcc_detailId, s.fldCodingBudje_DetailsId, s.fldDarsad, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    if (s.fldBudje_khedmatDarsadId == 0)
                    {
                        if (q.fldDarsad + s.fldDarsad > 100)
                        {
                            Msg = "جمع درصد برای خدمتها نمی تواند از 100 بیشتر باشد.";
                            MsgTitle = "خطا";
                            Er = 1;
                        }
                        else
                        {
                            MsgTitle = "عملیات موفق";
                            Msg = servic.InsertOnlyKhedmat(s.fldCodingAcc_detailId, s.fldCodingBudje_DetailsId, s.fldDarsad, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        }
                    }
                    else{
                        var q1 = servic.GetBudje_khedmatDarsadIdDetail(s.fldBudje_khedmatDarsadId, IP, out Err);
                        if (q.fldDarsad + s.fldDarsad-q1.fldDarsad > 100)
                        {
                            Msg = "جمع درصد برای خدمتها نمی تواند از 100 بیشتر باشد.";
                            MsgTitle = "خطا";
                            Er = 1;
                        }
                        else
                        {
                            MsgTitle = "عملیات موفق";
                            Msg = servic.InsertOnlyKhedmat(s.fldCodingAcc_detailId, s.fldCodingBudje_DetailsId, s.fldDarsad, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteKhedmatHa(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteBudje_khedmatDarsadId(id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult DetailsKhedmatHa(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetBudje_khedmatDarsadIdDetail(Id, IP, out Err);
            return Json(new
            {
                fldBudje_khedmatDarsadId = q.fldBudje_khedmatDarsadId,
                fldCodingAcc_detailId = q.fldCodingAcc_detailId,
                fldCodingBudje_DetailsId = q.fldCodingBudje_DetailsId,
                fldDarsad=q.fldDarsad,
                fldTitleAcc=q.fldTitleAcc,
                fldTitleBudje=q.fldTitleBudje
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsProjeHa(int AccCodingId,int BudgetCodingId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAnticipationWithFilter("fldCodingAcc_DetailsId_CodingBudje_DetailsId", AccCodingId.ToString(), BudgetCodingId.ToString(),0, IP, out Err);
            return Json(new
            {
                AccCodingId=q[0].fldCodingAcc_DetailsId,
                AccCodingTitle=q[0].fldTitleAcc,
                Mablagh = q.Select(l => new Coding_Anticipation()
                {
                    Mablagh=l.fldMablagh,
                    Name=Enum.GetName(typeof(BudgetType),l.fldBudgetTypeId)
                }).ToList()            
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadbudgeProject(StoreRequestParameters parameters, string Year, int MotamamId, int state)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_Budget.OBJ_Pishbini_Daramad> data = null;

            if (state == 1)
                data = servic.SelectPishbiniBudje("Poroje", Year, MotamamId, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();
            else
                data = servic.SelectPishbiniBudje_SalGhabl("Poroje", Year, MotamamId, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();

            return this.Store(data, data.Count);
        }
        //public ActionResult SavebudgeProject(List<MablaghPerCoding> Mablagh_Coding/*List<WCF_Budget.OBJ_Anticipation> budgeProjectVal, List<NewFMS.Models.Coding_Anticipation> Coding_Mablagh*/)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    string Msg = "", MsgTitle = ""; var Er = 0;
        //    try
        //    {
        //        foreach (var item in Mablagh_Coding)
        //        {
        //            MsgTitle = "ذخیره موفق";
        //            System.Data.DataTable dt = new System.Data.DataTable { TableName = "Bud.tblPishbini" };
        //            using (var reader = FastMember.ObjectReader.Create(item.Mablagh))
        //            {
        //                dt.Load(reader);
        //            }
        //            dt.Columns.Remove("Name");
        //            Msg = servic.InsertAnticipation(item.fldAccCodingId, null,dt, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

        //            if (Err.ErrorType)
        //            {
        //                return Json(new
        //                {
        //                    Msg = Err.ErrorMsg,
        //                    MsgTitle = "خطا",
        //                    Er = 1
        //                }, JsonRequestBehavior.AllowGet);
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
        //        Msg = Msg,
        //        MsgTitle = MsgTitle,
        //        Er = Er
        //    }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult ProjeHa(short Year, int CodingBudge, int MotamamId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            result.ViewBag.Year = Year.ToString();
            result.ViewBag.CodingBudge = CodingBudge;
            var s=servic.GetCodingBudje_DetailWithFilter("fldId", CodingBudge.ToString(), "","", 0, IP, out Err).FirstOrDefault();
            var FieldName="Mali";
            if(s.fldEtebarTypeId==2)
                FieldName = "Sarmayeii";
            result.ViewBag.FieldName = FieldName;
            result.ViewBag.MotamamId = MotamamId;
            return result;
        }
        public ActionResult ReadProjeHa(StoreRequestParameters parameters, int ProjectId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Budget.OBJ_Pishbini_Daramad> data = null;

            data = servic.SelectProjectCoding(ProjectId,IP, out Err).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }
            List<WCF_Budget.OBJ_Pishbini_Daramad> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveProjeHa(WCF_Budget.OBJ_Anticipation P, List<Coding_Anticipation> MablaghArray)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MablaghArray=MablaghArray.Select(l => new Coding_Anticipation()
                {
                    Mablagh=l.Mablagh,
                    BudgetTypeId=(int)System.Enum.Parse(typeof(BudgetType),l.Name),
                    MotammamId = l.MotammamId
                }).ToList();
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "Bud.tblPishbini" };
                using (var reader = FastMember.ObjectReader.Create(MablaghArray))
                {
                    dt.Load(reader);
                }
                dt.Columns.Remove("Name");
                MsgTitle = "عملیات موفق";
                Msg=servic.InsertAnticipation(Convert.ToInt32(P.fldCodingAcc_DetailsId),P.fldCodingBudje_DetailsId, dt, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                //if (s.fldBudje_khedmatDarsadId == 0)
                //{
                //    //ذخیره
                //    MsgTitle = "ذخیره موفق";
                //    Msg = servic.InsertBudje_khedmatDarsadId(s.fldCodingAcc_detailId, s.fldCodingBudje_DetailsId, s.fldDarsad, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                //}
                //else
                //{
                //    //ویرایش
                //    MsgTitle = "ویرایش موفق";
                //    Msg = servic.UpdateBudje_khedmatDarsadId(s.fldBudje_khedmatDarsadId, s.fldCodingAcc_detailId, s.fldCodingBudje_DetailsId, s.fldDarsad, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                //}
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
        public ActionResult DeleteProjeHa(int AccCodingId, int BudgetCodingId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق"; 
                Msg = servic.DeleteAnticipation("Project",AccCodingId,BudgetCodingId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
    }
}