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
    public class KarkardMahaneGroupController : Controller
    {
        //
        // GET: /PayRoll/KarkardMahaneGroup/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();

        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var result = new Ext.Net.MVC.PartialViewResult();
            var q = Cservic.GetDate( IP, out CErr);
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            return result;
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult IndexKhAs()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var result = new Ext.Net.MVC.PartialViewResult();
            var q = Cservic.GetDate( IP, out CErr);
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            return result;
        }
        public ActionResult HelpKarkardMahaneKhas()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetChartOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSpecialPermissionWithFilter("fldUserSelectId", Session["UserId"].ToString(), 0, IP, out Err).Where(l => l.fldOpertionId == 1).ToList().Select(c => new { fldId = c.fldChartOrganId, fldName = c.fldTitleChart });
            return this.Store(q);
        }
        public ActionResult IndexGroup(string containerId, string FieldName, string CostCenter_ChartId, short Sal, byte Mah, byte NobatPardakht,bool Flag,byte DelCalc,int OrganId)
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
            result.ViewBag.FieldName = FieldName;
            result.ViewBag.CostCenter_ChartId = CostCenter_ChartId;
            result.ViewBag.Sal = Sal;
            result.ViewBag.Mah = Mah;
            result.ViewBag.DelCalc = DelCalc;
            result.ViewBag.NobatPardakht = NobatPardakht;
            result.ViewBag.OrganId = OrganId.ToString();


            if (Flag == true)
            {
                servic.UpdateFlag("Mohasebat_Sal", Sal, Mah, NobatPardakht,0,OrganId/* Convert.ToInt32(Session["OrganId"])*/, Convert.ToByte(3), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                //if (Err_Pay.ErrorType)
                //{
                //    MsgTitle = "خطا";
                //    Msg = Err_Pay.ErrorMsg;
                //    Er = 1;
                //    return Json(new
                //    {
                //        Er = Er,
                //        Msg = Msg,
                //        MsgTitle = MsgTitle
                //    }, JsonRequestBehavior.AllowGet);
                //}
            }
            return result;
        }
        public ActionResult GetCostCenter(string ID,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCostCenterWithFilter("", "", OrganId.ToString()/*(Session["OrganId"]).ToString()*/, 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Save(List<WCF_PayRoll.OBJ_KarKardeMahane> KarkardVal, byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (DelCalc == 1)
                {
                    servic.MohasebatDelete("AllMohasebat_sal", 0, Convert.ToByte(KarkardVal[0].fldMah), Convert.ToInt16(KarkardVal[0].fldYear), 
                        Convert.ToByte(KarkardVal[0].fldNobatePardakht), "", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP,
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, out Err);
                    if (Err.ErrorType == true)
                    {
                        Msg = Err.ErrorMsg;
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
                foreach (var item in KarkardVal)
                {
                    if (item.fldDesc == null)
                        item.fldDesc = "";

                    //ذخیره
                    if (item.fldId == 0)
                    {
                        var q = servic.GetKarKardeMahaneWithFilter("CheckSave", item.fldPersonalId.ToString(), item.fldYear.ToString(), item.fldMah.ToString(),
                            item.fldNobatePardakht, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "اطلاعات کارکرد ماهانه شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "اخطار";
                            Er = 1;
                        }
                        else
                        {
                            servic.InsertKarKardeMahane(item.fldPersonalId, item.fldYear, item.fldMah, item.fldKarkard, item.fldGheybat, item.fldNobateKari, 
                                item.fldEzafeKari,item.fldTatileKari,item.fldMamoriatBaBeitote,item.fldMamoriatBedoneBeitote,item.fldMosaedeh,item.fldNobatePardakht,
                                item.fldFlag, item.fldGhati, item.fldBa10, item.fldBa20, item.fldBa30, item.fldBe10, item.fldBe20, item.fldBe30,item.fldShift
                                ,Convert.ToBoolean(item.fldMoavaghe),item.fldAzTarikhMoavaghe,item.fldTaTarikhMoavaghe, item.fldDesc, Session["Username"].ToString(),
                                (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                            Msg = "ذخیره با موفقیت انجام شد.";
                            MsgTitle = "ذخیره موفق";
                        }
                    }
                    else
                    {
                        //ویرایش
                        var q = servic.GetKarKardeMahaneWithFilter("CheckEdit", item.fldPersonalId.ToString(), item.fldYear.ToString(),item.fldMah.ToString(),
                            item.fldNobatePardakht, item.fldId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "اطلاعات کارکرد ماهانه شخص مورد نظر تکراری است.";
                            MsgTitle = "اخطار";
                            Er = 1;
                        }
                        else
                        {
                            servic.UpdateKarKardeMahane(item.fldId, item.fldPersonalId, item.fldYear, item.fldMah, item.fldKarkard, item.fldGheybat, item.fldNobateKari,
                                item.fldEzafeKari, item.fldTatileKari, item.fldMamoriatBaBeitote, item.fldMamoriatBedoneBeitote, item.fldMosaedeh, item.fldNobatePardakht,
                                item.fldFlag, item.fldGhati, item.fldBa10, item.fldBa20, item.fldBa30, item.fldBe10, item.fldBe20, item.fldBe30, item.fldShift, 
                                Convert.ToBoolean(item.fldMoavaghe), item.fldAzTarikhMoavaghe, item.fldTaTarikhMoavaghe, item.fldDesc, Session["Username"].ToString(),
                                (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                            Msg = "ویرایش با موفقیت انجام شد.";
                            MsgTitle = "ویرایش موفق";
                        }
                    }
                    if (Err.ErrorType)
                    {
                        Msg = Err.ErrorMsg;
                        MsgTitle = "خطا";
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(List<WCF_PayRoll.OBJ_KarKardeMahane> KarkardGroupVal, short? fldYear, byte? fldMah, byte? fldNobatePardakht, byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (DelCalc == 1)
                {
                    servic.MohasebatDelete("AllMohasebat_sal", 0, Convert.ToByte(fldMah), Convert.ToInt16(fldYear), Convert.ToByte(fldNobatePardakht), "", "",
                        Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId/*Convert.ToInt32(Session["OrganId"])*/, out Err);
                    if (Err.ErrorType == true)
                    {
                        Msg = Err.ErrorMsg;
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
                //حذف
                foreach (var item in KarkardGroupVal)
                {
                    servic.DeleteKarKardeMahane(item.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er=1
                        }, JsonRequestBehavior.AllowGet);
                    }
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
        public ActionResult ReloadKarkardMahaneGroup(string FieldName, string CostCenter_ChartId, short Sal, byte Mah, byte NobatPardakht,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_KarKardeMahane> data = null;

            data = servic.GetKarKardeMahaneGroupWithFilter(FieldName, CostCenter_ChartId, Sal, Mah, NobatPardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult NodeLoad(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = Cservic.GetChartOrganEjraeeWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out CErr).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = Cservic.GetChartOrganEjraeeWithFilter("fldPID", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out CErr).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = Cservic.GetChartOrganEjraeeWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out CErr).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldTitle;
                    childNode.NodeID = ch.fldId.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }
        public ActionResult CheckDay(int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetDate( IP, out CErr);
            var y = MyLib.Shamsi.Miladi2ShamsiString(q.fldDateTime);
            //var year = y.Substring(0, 4);
            //var month = Convert.ToInt32(y.Substring(5, 2));

            var year = Convert.ToInt32(NewFMS.Models.CurrentDate.Year);
            var month = Convert.ToInt32(NewFMS.Models.CurrentDate.Month);

            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            var k = servic.GetPayPersonalInfoDetail(PersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("",k.fldPrs_PersonalInfoId,"", IP, out Err_Com_Pay).FirstOrDefault();
            var AnvaeEstekhdamId = f.fldAnvaEstekhdamId;

            var z = servic_Com_Pay.GetAnvaEstekhdamDetail(AnvaeEstekhdamId, IP, out Err_Com_Pay);

            var TypeEstekhdam = z.fldNoeEstekhdamId;//نوع 1=کارگر نوع 2=کارمند رسمی 3=کارمند پیمانی 4= شهردار
            var isKabise = MyLib.Shamsi.Iskabise(Convert.ToInt32(year));

            var days = 30;
            if (TypeEstekhdam == 1 && month <= 6)
            {
                days = 31;
            }
            else if (TypeEstekhdam == 1 && month > 6 && month < 12)
            {
                days = 30;
            }
            else if (TypeEstekhdam == 1 && month == 12 && isKabise == true)
            {
                days = 30;
            }
            else if (TypeEstekhdam == 1 && month == 12 && isKabise == false)
            {
                days = 29;
            }

            return Json(new
            {
                days = days
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
