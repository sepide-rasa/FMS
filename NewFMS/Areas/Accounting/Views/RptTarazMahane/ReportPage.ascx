
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>

<%{
      
      //string[] kk=ViewBag.Tables;
      //NewFMS.DataSet.DataSet1 dt = new NewFMS.DataSet.DataSet1();
      //NewFMS.DataSet.DataSet_Com dt_Com = new NewFMS.DataSet.DataSet_Com();
      //NewFMS.DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new NewFMS.DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
      //NewFMS.DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter Filee = new NewFMS.DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
      //NewFMS.DataSet.DataSet1TableAdapters.spr_SelectNameKarbarRptTableAdapter name = new NewFMS.DataSet.DataSet1TableAdapters.spr_SelectNameKarbarRptTableAdapter();
      //NewFMS.DataSet.DataSet1TableAdapters.spr_TarazTableAdapter taraz = new NewFMS.DataSet.DataSet1TableAdapters.spr_TarazTableAdapter();

      
      //Date.Fill(dt_Com.spr_GetDate);
      //Filee.Fill(dt_Com.spr_tblFileSelect, "fldId", ViewBag.LogoId, 0);
      //name.Fill(dt.spr_SelectNameKarbarRpt, ViewBag.UserId, ViewBag.OrganId);
   //   taraz.Fill(dt.spr_Taraz, ViewBag.AzTarikh, ViewBag.TaTarikh, Convert.ToByte(ViewBag.salMali),Convert.ToByte(ViewBag.OrganId),Convert.ToInt32(ViewBag.AzLevel),Convert.ToInt32(ViewBag.TaLevel),Convert.ToInt32(ViewBag.AzSanad),Convert.ToInt32(ViewBag.TaSanad),Convert.ToInt32(ViewBag.StartNodeID), Convert.ToByte(ViewBag.sanadtype));


      //WebReport1.Report.RegisterData(dt_Com, "dataSetAccounting");
      //WebReport1.Report.RegisterData(dt, "dataSetAccounting");
      string Path = ViewBag.Path;
      WebReport2.Report.Load(Path);

      WebReport2.ID = ViewBag.RId;


      WebReport2.Report.SetParameterValue("aztarikh", ViewBag.AzTarikh);
      WebReport2.Report.SetParameterValue("tatarikh", ViewBag.TaTarikh);
      WebReport2.Report.SetParameterValue("salmaliID", Convert.ToInt16(ViewBag.salMali));
      WebReport2.Report.SetParameterValue("organid", Convert.ToInt32(ViewBag.OrganId));
      WebReport2.Report.SetParameterValue("azLevel", Convert.ToInt32(ViewBag.AzLevel));
      WebReport2.Report.SetParameterValue("tanLevel", Convert.ToInt32(ViewBag.TaLevel));
      WebReport2.Report.SetParameterValue("azsanad", Convert.ToInt32(ViewBag.AzSanad));
      WebReport2.Report.SetParameterValue("tasanad", Convert.ToInt32(ViewBag.TaSanad));
      WebReport2.Report.SetParameterValue("StartNodeID", Convert.ToInt32(ViewBag.StartNodeID));
      WebReport2.Report.SetParameterValue("sanadtype", Convert.ToInt32(ViewBag.sanadtype));
      WebReport2.Report.SetParameterValue("Taraz", Convert.ToInt32(ViewBag.Taraz));
      WebReport2.Report.SetParameterValue("connectionstr", ViewBag.connectionstr);
      
     
  
  } %>
<div class="center-form">
<form id="Form1" runat="server" dir="ltr">
    <cc1:WebReport ID="WebReport2" runat="server"  Width="100%" Height="100%"></cc1:WebReport>

</form> </div>  

<style>
    .center-form{

    justify-content: center;
    display: flex;
    margin: 40px 50px;
    border: 3px solid #050f1c4a;
    padding: 30px;
    background-color: rgb(10 8 20 / 27%);

}
</style>

