using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using NewFMS.Models;
using System.IO;
using System;
using System.Linq;

namespace NewFMS.Areas.Deceased.Controllers
{
    public class CemeteryController : Controller
    {
        //
        // GET: /Deceased/Cemetery/
        WCF_Deceased.DeceasedService service = new WCF_Deceased.DeceasedService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();

        WCF_Common.CommonService service_com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Cemetery();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData=ViewData
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult New(int Id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            var q=service_com.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com);
            result.ViewBag.Id = Id;
            result.ViewBag.StateId = q.fldStateId.ToString();
            result.ViewBag.CityId = q.fldCityId.ToString();
            return result;
        }
        public ActionResult CreateSprite(int Row,int Col,string Align)
        {
            if (Align == "Horizontal")
            {
                var x = 5;
                var y = 5;
                var y1 = 0;
                for (var i = 0; i < Row; i++)
                {
                    for (var j = 0; j < Col; j++)
                    {
                        y1 = y;
                        for (var k = 0; k < 1; k++)//پیشفرض 1 طبقه باشه قبلا 3 بود
                        {
                            Sprite sprite = new Sprite
                            {
                                SpriteID = "SpriteR" + i+"C" + j+"F" + k,
                                Type = SpriteType.Rect,
                                Width = 30,
                                Height = 17,
                                Fill = "#a6a6a6",
                                X = x,
                                Y = y,
                                Group = new[] { "rectanglesR" + i+"C"+ j },
                                Text = ""
                            };
                            sprite.Listeners.Click.Handler = "AddFloor(this," + "'rectanglesR" + i +"C"+ j + "')";
                            sprite.Listeners.MouseDown.Handler = "RemoveFloor(e,this,'rectanglesR" + i+"C" + j + "')";
                            this.GetCmp<DrawComponent>("Draw1").Add(sprite);
                            sprite.Show(true);
                            y = y + 18;
                        }
                        y = y1;
                        x = x + 40;
                    }
                    x = 5;
                    y = y + 70;
                }
                var count = this.GetCmp<DrawComponent>("Draw1").Items.Count;
                this.GetCmp<DrawComponent>("Draw1").SetSurfaceSize(Col * 40, (Row * 70)+50);
                return this.Direct();
            }
            else
            {
                var x = 5;
                var x1 = 0;
                var y = 5;
                for (var i = 0; i < Row; i++)
                {
                    for (var j = 0; j < Col; j++)
                    {
                        x1 = x;
                        for (var k = 0; k < 1; k++)//پیشفرض 1 طبقه باشه قبلا 3 بود
                        {
                            Sprite sprite = new Sprite
                            {
                                SpriteID = "SpriteR" + i + "C" + j + "F"  +k,
                                Type = SpriteType.Rect,
                                Width = 30,
                                Height = 17,
                                Fill = "#a6a6a6",
                                X = x,
                                Y = y,
                                Group = new[] { "rectanglesR" + i + "C" + j },
                                Text = ""
                            };
                            sprite.Listeners.Click.Handler = "AddFloor(this," + "'rectanglesR" + i+"C" + j + "')";
                            sprite.Listeners.MouseDown.Handler = "RemoveFloor(e,this,'rectanglesR" + i+"C" + j + "')";
                            this.GetCmp<DrawComponent>("Draw1").Add(sprite);
                            sprite.Show(true);
                            y = y + 18;
                        }
                        x = x1;
                        y = y + 17;
                    }
                    y = 5;
                    x = x + 40;
                }
                var count = this.GetCmp<DrawComponent>("Draw1").Items.Count;
                this.GetCmp<DrawComponent>("Draw1").SetSurfaceSize(Row * 40, (Col *70)+50 );
                return this.Direct();
            }
        }
        public ActionResult ChangeSprite(byte Floor, int x, int y, string groupName,string text,byte co_Rez)
        {
            var height = 17;
            if (Floor == 2)
            {
                height = 26;
            }
            else if (Floor == 1)
            {
                height = 53;
            }
            for (var k = 0; k < Floor; k++)
            {
                var color="#a6a6a6";
                if (co_Rez != 0 && Floor-co_Rez < k+1)
                {
                    co_Rez =Convert.ToByte(co_Rez-1);
                    color = "#ffa64d";
                }
                Sprite sprite = new Sprite
                {
                    //Group = new[] { "rectanglesR" + i + "C" + j },
                    //"SpriteR" + i + "C" + j + "F" + +k,
                    SpriteID = "SpriteR" + groupName.Substring(11, (groupName.Length-1) - groupName.IndexOf("C"))+"C" + groupName.Substring(groupName.IndexOf("C")) + "F"+k,
                    Type = SpriteType.Rect,
                    Width = 30,
                    Height = height,
                    Fill =color,
                    X = x,
                    Y = y,
                    Group = new[] { groupName },
                    Text=text
                };
                sprite.Listeners.Click.Handler = "AddFloor(this,'" + groupName + "')";
                sprite.Listeners.MouseDown.Handler = "RemoveFloor(e,this,'" + groupName + "')";
                sprite.Listeners.Render.Handler = "showInf(this,'" + groupName + "')";
                this.GetCmp<DrawComponent>("Draw1").Add(sprite);
                sprite.Show(true);
                y = y + height + 1;
            }
            return this.Direct();
        }
        public ActionResult GetState()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service_com.GetStateWithFilter("", "", 0, IP, out Err_com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetCity(int StateId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service_com.GetCityWithFilter("fldStateId", StateId.ToString(), 0, IP, out Err_com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetLatLng(int CityId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service_com.GetCityDetail(CityId,IP,out Err_com);
            return Json(new
            {
                fldLatitude = q.fldLatitude,
                fldLongitude = q.fldLongitude
         
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service.GetVadiSalamDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldStateId = q.fldStateId.ToString(),
                fldTitle = q.fldName,
                fldAddress=q.fldAddress,
                fldCityId=q.fldCityId.ToString(),
                fldLatitude = q.fldLatitude,
                fldLongitude=q.fldLongitude,
                fldDesc=q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = service.DeleteVadiSalam(Id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Save(WCF_Deceased.OBJ_VadiSalam Cemetery)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Cemetery.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = service.InsertVadiSalam(Cemetery.fldName, Cemetery.fldStateId, Cemetery.fldCityId,Cemetery.fldAddress,Cemetery.fldLatitude,Cemetery.fldLongitude,Cemetery.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = service.UpdateVadiSalam(Cemetery.fldId, Cemetery.fldName, Cemetery.fldStateId, Cemetery.fldCityId, Cemetery.fldAddress, Cemetery.fldLatitude, Cemetery.fldLongitude, Cemetery.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        /// <summary>
        /// باز شدن فرم جهت ذخیره اطلاعات جدید و ویرایش اطلاعات موجود
        /// </summary>
        /// <param name="id">کد جدول پرسنل</param>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public ActionResult NewGhete(int VadiId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.VadiId = VadiId;
            return result;
        }
        public ActionResult SaveGhete(WCF_Deceased.OBJ_Ghete Ghete, string StrFloors, bool? IsUsed)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Ghete.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = service.InsertGhete(Ghete.fldVadiSalamId,Ghete.fldNameGhete,Convert.ToInt32(Ghete.CountRadif),Convert.ToInt32(Ghete.CountShomare),StrFloors, Ghete.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    if (IsUsed == false)
                    {
                        Msg = service.UpdateGheteKhali(Ghete.fldId, Ghete.fldVadiSalamId, Ghete.fldNameGhete, Convert.ToInt32(Ghete.CountRadif), Convert.ToInt32(Ghete.CountShomare), StrFloors, Ghete.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        Msg = service.UpdateTedadTabaghat(Ghete.fldId, StrFloors, Ghete.fldNameGhete, Ghete.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult DeleteGhete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = service.DeleteAllTablesOnGhete(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult DetailsGhete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service.GetGheteDetail(Id,Convert.ToInt32(Session["OrganId"]),IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldNameGhete = q.fldNameGhete,
                fldDesc = q.fldDesc,
                fldRawCount=q.CountRadif,
                fldColCount=q.CountShomare
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInfoGhabr(int shomareId,byte count,byte Index)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Sh_tabaghe = 0;
            if (Index == 0)
            {
                Sh_tabaghe = count;
            }
            if (Index == 1)
            {
                Sh_tabaghe = count - 1;
            }
            else if (Index == 2)
            {
                Sh_tabaghe = 1;
            }
            var q = service.SelectInfoEmployeeInGhabrAmanat(shomareId,Convert.ToByte(Sh_tabaghe), IP, out Err).FirstOrDefault();
            if (q != null)
            {
                return Json(new
                {
                    fldName_Family = q.fldName_Family,
                    fldFatherName = q.fldFatherName,
                    fldSh_Shenasname = q.fldSh_Shenasname,
                    fldMeli_Moshakhase = q.fldMeli_Moshakhase,
                    fldReason = q.fldReason,
                    fldTarikhFot = q.fldTarikhFot,
                    fldNameMahal = q.fldNameMahal,
                    fldTarikhRezerv = q.fldTarikhRezerv,
                    fldShomareTabaghe = q.fldShomareTabaghe,
                    fldTarikhDafn = q.fldTarikhDafn,
                    IsNew = 0
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    IsNew = 1
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult createSavedSprit(int Id,string Align)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = service.SelectGheteDetail(Id, IP, out Err).FirstOrDefault();
            int Row = Convert.ToInt32(q.CountRadif); 
            int Col = Convert.ToInt32(q.CountShomare);
            string strFloors = q.Ghete;
            var FloorArray=strFloors.Split('|').ToList();
            if (Align == "Horizontal")
            {
                var x = 5;
                var y = 5;
                var y1 = 0;
                for (var i = 0; i < Row; i++)
                {
                    for (var j = 0; j < Col; j++)
                    {
                        y1 = y;
                        var Item = FloorArray.Where(l => l.Split(',')[0] == "R"+(i + 1).ToString() +"C"+ (j + 1).ToString()).FirstOrDefault();
                        var ItemSplited = Item.Split(',');

                        var height = 17;
                        if (ItemSplited[2] == "2")
                        {
                            height = 26;
                        }
                        else if (ItemSplited[2] == "1")
                        {
                            height = 53;
                        }
                        var color = "";
                        for (var k = 0; k < Convert.ToInt32(ItemSplited[2]); k++)
                        {
                            if (Convert.ToInt32(ItemSplited[5]) > 0)
                            {
                                color = "#a6a6a6";
                                ItemSplited[5] = (Convert.ToInt32(ItemSplited[5]) - 1).ToString();
                            }
                            else if (Convert.ToInt32(ItemSplited[4]) > 0)
                            {
                                color = "#ffa64d";
                                ItemSplited[4] = (Convert.ToInt32(ItemSplited[4]) - 1).ToString();
                            }
                            else
                            {
                                color = "#ff4d4d";
                                ItemSplited[3] = (Convert.ToInt32(ItemSplited[3]) - 1).ToString();
                            }
                            Sprite sprite = new Sprite
                            {
                                SpriteID = "SpriteR" + i + "C" + j + "F" +k,
                                Type = SpriteType.Rect,
                                Width = 30,
                                Height = height,
                                Fill = color,
                                X = x,
                                Y = y,
                                Group = new[] { "rectanglesR" + i+"C" + j },
                                Text = ItemSplited[1] + ";" + k
                            };
                            sprite.Listeners.Click.Handler = "AddFloor(this," + "'rectanglesR" + i+"C" + j + "')";
                            sprite.Listeners.MouseDown.Handler = "RemoveFloor(e,this,'rectanglesR" + i +"C"+ j + "')";
                            sprite.Listeners.Render.Handler = "showInf(this," + "'rectanglesR" + i +"C"+ j + "')";
                            this.GetCmp<DrawComponent>("Draw1").Add(sprite);
                            sprite.Show(true);
                            y = y + height + 1;
                        }
                        y = y1;
                        x = x + 40;
                    }
                    x = 5;
                    y = y + 70;
                }
                var count = this.GetCmp<DrawComponent>("Draw1").Items.Count;
                this.GetCmp<DrawComponent>("Draw1").SetSurfaceSize(Col * 40, (Row * 70)+50);
                return this.Direct();
            }
            else
            {
                var x = 5;
                var x1 = 0;
                var y = 5;
                for (var i = 0; i < Row; i++)
                {
                    for (var j = 0; j < Col; j++)
                    {
                        x1 = x;
                        var Item = FloorArray.Where(l => l.Split(',')[0] == "R"+(i + 1).ToString() + "C"+(j + 1).ToString()).FirstOrDefault();
                        var ItemSplited = Item.Split(',');

                        var height = 17;
                        if (ItemSplited[2] == "2")
                        {
                            height = 26;
                        }
                        else if (ItemSplited[2] == "1")
                        {
                            height = 53;
                        }
                        var color = "";
                        for (var k = 0; k < Convert.ToInt32(ItemSplited[2]); k++)
                        {
                            if (Convert.ToInt32(ItemSplited[5]) > 0)
                            {
                                color = "#a6a6a6";
                                ItemSplited[5] = (Convert.ToInt32(ItemSplited[5]) - 1).ToString();
                            }
                            else if (Convert.ToInt32(ItemSplited[4]) > 0)
                            {
                                color = "#ffa64d";
                                ItemSplited[4] = (Convert.ToInt32(ItemSplited[4]) - 1).ToString();
                            }
                            else
                            {
                                color = "#ff4d4d";
                                ItemSplited[3] = (Convert.ToInt32(ItemSplited[3]) - 1).ToString();
                            }
                            Sprite sprite = new Sprite
                            {
                                SpriteID = "SpriteR" + i + "C" + j + "F" + k,
                                Type = SpriteType.Rect,
                                Width = 30,
                                Height = height,
                                Fill = color,
                                X = x,
                                Y = y,
                                Group = new[] { "rectanglesR" + i+"C" + j },
                                Text = ItemSplited[1] + ";" + k
                            };
                            sprite.Listeners.Click.Handler = "AddFloor(this," + "'rectanglesR" + i+"C" + j + "')";
                            sprite.Listeners.MouseDown.Handler = "RemoveFloor(e,this,'rectanglesR" + i+"C" + j + "')";
                            sprite.Listeners.Render.Handler = "showInf(this," + "'rectanglesR" + i+"C" + j + "')";
                            this.GetCmp<DrawComponent>("Draw1").Add(sprite);
                            sprite.Show(true);
                            y = y + height + 1;
                        }
                        x = x1;
                        y = y + 17;
                    }
                    y = 5;
                    x = x + 40;
                }
                var count = this.GetCmp<DrawComponent>("Draw1").Items.Count;
                this.GetCmp<DrawComponent>("Draw1").SetSurfaceSize(Row * 40, (Col * 70)+50);
                return this.Direct();
            }
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Deceased.OBJ_VadiSalam> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Deceased.OBJ_VadiSalam> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldNameState":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameState";
                            break;
                        case "fldNameCity":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameCity";
                            break;
                        case "fldAddress":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAddress";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = service.GetVadiSalamWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = service.GetVadiSalamWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]),100,IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service.GetVadiSalamWithFilter("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Deceased.OBJ_VadiSalam> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadPieces(StoreRequestParameters parameters, int CemeteryId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Deceased.OBJ_Ghete> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Deceased.OBJ_Ghete> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldNameGhete":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameGhete";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = service.GetGheteWithFilter(field, searchtext,0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldVadiSalamId == CemeteryId).Take(100).ToList();
                    else
                        data = service.GetGheteWithFilter(field, searchtext,0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(k => k.fldVadiSalamId == CemeteryId).Take(100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service.GetGheteWithFilter("fldVadiSalamId", CemeteryId.ToString(),0,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Deceased.OBJ_Ghete> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
