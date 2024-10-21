using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace NewFMS.Areas.Contracts.Models
{
    public partial class ContractEntities
    {
        public virtual int spr_tblFactorInsert(string fldTarikh, string fldShomare, string fldShanaseMoadiyan, string fldSharhSanad, Nullable<int> fldContractId,
            Nullable<int> fldProjectId, Nullable<int> fldAshkhasId, Nullable<int> fldTankhahGroupId, Nullable<decimal> fldKasrBime, Nullable<decimal> fldKasrHosnAnjamKar, string fldShomareSabt, string fldCodeEghtesadi, string fldAddress,
            string fldTarikhVariz, string fldQrCode,System.Data.DataTable Details, Nullable<int> fldOrganId, Nullable<int> fldUserId, string fldDesc, string fldIP)
        {
            var fldTarikhParameter = fldTarikh != null ?
                new System.Data.SqlClient.SqlParameter("fldTarikh", fldTarikh) :
                new System.Data.SqlClient.SqlParameter("fldTarikh", typeof(string));

            var fldShomareParameter = fldShomare != null ?
                new System.Data.SqlClient.SqlParameter("fldShomare", fldShomare) :
                new System.Data.SqlClient.SqlParameter("fldShomare", typeof(string));

            var fldShanaseMoadiyanParameter = fldShanaseMoadiyan != null ?
                new System.Data.SqlClient.SqlParameter("fldShanaseMoadiyan", fldShanaseMoadiyan) :
                new System.Data.SqlClient.SqlParameter("fldShanaseMoadiyan", typeof(string));

            var fldSharhSanadParameter = fldSharhSanad != null ?
                new System.Data.SqlClient.SqlParameter("fldSharhSanad", fldSharhSanad) :
                new System.Data.SqlClient.SqlParameter("fldSharhSanad", typeof(string));

            var fldContractIdParameter = fldContractId.HasValue ?
               new System.Data.SqlClient.SqlParameter("fldContractId", fldContractId) :
               new System.Data.SqlClient.SqlParameter("fldContractId", DBNull.Value);

            var fldProjectIdParameter = fldProjectId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldProjectId", fldProjectId) :
                new System.Data.SqlClient.SqlParameter("fldProjectId", DBNull.Value);

            var fldAshkhasIdParameter = fldAshkhasId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldAshkhasId", fldAshkhasId) :
                new System.Data.SqlClient.SqlParameter("fldAshkhasId", DBNull.Value);

            var fldTankhahGroupIdParameter = fldTankhahGroupId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldTankhahGroupId", fldTankhahGroupId) :
                new System.Data.SqlClient.SqlParameter("fldTankhahGroupId", DBNull.Value);

            var fldKasrBimeParameter = fldKasrBime.HasValue ?
               new System.Data.SqlClient.SqlParameter("fldKasrBime", fldKasrBime) :
               new System.Data.SqlClient.SqlParameter("fldKasrBime", typeof(decimal));

            var fldKasrHosnAnjamKarParameter = fldKasrHosnAnjamKar.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldKasrHosnAnjamKar", fldKasrHosnAnjamKar) :
                new System.Data.SqlClient.SqlParameter("fldKasrHosnAnjamKar", typeof(decimal));

            var fldShomareSabtParameter = fldShomareSabt != null ?
                new System.Data.SqlClient.SqlParameter("fldShomareSabt", fldShomareSabt) :
                new System.Data.SqlClient.SqlParameter("fldShomareSabt", typeof(string));

            var fldCodeEghtesadiParameter = fldCodeEghtesadi != null ?
                new System.Data.SqlClient.SqlParameter("fldCodeEghtesadi", fldCodeEghtesadi) :
                new System.Data.SqlClient.SqlParameter("fldCodeEghtesadi", typeof(string));

            var fldAddressParameter = fldAddress != null ?
                new System.Data.SqlClient.SqlParameter("fldAddress", fldAddress) :
                new System.Data.SqlClient.SqlParameter("fldAddress", typeof(string));

            var fldTarikhVarizParameter = fldTarikhVariz != null ?
                new System.Data.SqlClient.SqlParameter("fldTarikhVariz", fldTarikhVariz) :
                new System.Data.SqlClient.SqlParameter("fldTarikhVariz", typeof(string));

            var fldQrCodeParameter = fldQrCode != null ?
                new System.Data.SqlClient.SqlParameter("fldQrCode", fldQrCode) :
                new System.Data.SqlClient.SqlParameter("fldQrCode", typeof(string));

            var fldOrganIdParameter = fldOrganId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldOrganId", fldOrganId) :
                new System.Data.SqlClient.SqlParameter("fldOrganId", typeof(int));

            var fldUserIdParameter = fldUserId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldUserId", fldUserId) :
                new System.Data.SqlClient.SqlParameter("fldUserId", typeof(int));

            var fldDescParameter = fldDesc != null ?
                new System.Data.SqlClient.SqlParameter("fldDesc", fldDesc) :
                new System.Data.SqlClient.SqlParameter("fldDesc", typeof(string));

            var fldIPParameter = fldIP != null ?
                new System.Data.SqlClient.SqlParameter("fldIP", fldIP) :
                new System.Data.SqlClient.SqlParameter("fldIP", typeof(string));

            var DetailParameter = Details.Rows.Count > 0 ?
               new System.Data.SqlClient.SqlParameter("Detail", Details) :
               new System.Data.SqlClient.SqlParameter("Detail", typeof(System.Data.DataTable));
            DetailParameter.TypeName = "Cntr.InserDetailFactor";
            DetailParameter.SqlDbType = System.Data.SqlDbType.Structured;	
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("Cntr.spr_tblFactorInsert @fldTarikh, @fldShomare, @fldShanaseMoadiyan, @fldSharhSanad,"+
                " @fldContractId, @fldProjectId, @fldAshkhasId, @fldTankhahGroupId, @fldKasrBime, @fldKasrHosnAnjamKar, @fldShomareSabt, @fldCodeEghtesadi, @fldAddress, @fldTarikhVariz, @fldQrCode, @Detail, @fldOrganId, @fldUserId, @fldDesc, @fldIP",
                fldTarikhParameter, fldShomareParameter, fldShanaseMoadiyanParameter, fldSharhSanadParameter, fldContractIdParameter, fldProjectIdParameter,fldAshkhasIdParameter,
                fldTankhahGroupIdParameter, fldKasrBimeParameter, fldKasrHosnAnjamKarParameter, fldShomareSabtParameter, fldCodeEghtesadiParameter, fldAddressParameter, fldTarikhVarizParameter, fldQrCodeParameter, DetailParameter, fldOrganIdParameter, fldUserIdParameter, fldDescParameter, fldIPParameter);
        }
        public virtual int spr_tblFactorUpdate(Nullable<int> fldId, string fldTarikh, string fldShomare, string fldShanaseMoadiyan, string fldSharhSanad,
             Nullable<int> fldProjectId, Nullable<int> fldAshkhasId, Nullable<int> fldTankhahGroupId, Nullable<decimal> fldKasrBime, Nullable<decimal> fldKasrHosnAnjamKar, string fldShomareSabt, string fldCodeEghtesadi, string fldAddress, string fldTarikhVariz, string fldQrCode,
            System.Data.DataTable Details, Nullable<int> fldOrganId, Nullable<int> fldUserId, string fldDesc, string fldIP)
        {
            var fldIdParameter = fldId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldId", fldId) :
                new System.Data.SqlClient.SqlParameter("fldId", typeof(int));

            var fldTarikhParameter = fldTarikh != null ?
                new System.Data.SqlClient.SqlParameter("fldTarikh", fldTarikh) :
                new System.Data.SqlClient.SqlParameter("fldTarikh", typeof(string));

            var fldShomareParameter = fldShomare != null ?
                new System.Data.SqlClient.SqlParameter("fldShomare", fldShomare) :
                new System.Data.SqlClient.SqlParameter("fldShomare", typeof(string));

            var fldShanaseMoadiyanParameter = fldShanaseMoadiyan != null ?
                new System.Data.SqlClient.SqlParameter("fldShanaseMoadiyan", fldShanaseMoadiyan) :
                new System.Data.SqlClient.SqlParameter("fldShanaseMoadiyan", typeof(string));

            var fldSharhSanadParameter = fldSharhSanad != null ?
                new System.Data.SqlClient.SqlParameter("fldSharhSanad", fldSharhSanad) :
                new System.Data.SqlClient.SqlParameter("fldSharhSanad", typeof(string));

            var fldProjectIdParameter = fldProjectId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldProjectId", fldProjectId) :
                new System.Data.SqlClient.SqlParameter("fldProjectId", DBNull.Value);

            var fldAshkhasIdParameter = fldAshkhasId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldAshkhasId", fldAshkhasId) :
                new System.Data.SqlClient.SqlParameter("fldAshkhasId", DBNull.Value);

            var fldTankhahGroupIdParameter = fldTankhahGroupId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldTankhahGroupId", fldTankhahGroupId) :
                new System.Data.SqlClient.SqlParameter("fldTankhahGroupId", DBNull.Value);

            var fldKasrBimeParameter = fldKasrBime.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldKasrBime", fldKasrBime) :
                new System.Data.SqlClient.SqlParameter("fldKasrBime", typeof(decimal));
    
            var fldKasrHosnAnjamKarParameter = fldKasrHosnAnjamKar.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldKasrHosnAnjamKar", fldKasrHosnAnjamKar) :
                new System.Data.SqlClient.SqlParameter("fldKasrHosnAnjamKar", typeof(decimal));

            var fldShomareSabtParameter = fldShomareSabt != null ?
                new System.Data.SqlClient.SqlParameter("fldShomareSabt", fldShomareSabt) :
                new System.Data.SqlClient.SqlParameter("fldShomareSabt", typeof(string));

            var fldCodeEghtesadiParameter = fldCodeEghtesadi != null ?
                new System.Data.SqlClient.SqlParameter("fldCodeEghtesadi", fldCodeEghtesadi) :
                new System.Data.SqlClient.SqlParameter("fldCodeEghtesadi", typeof(string));

            var fldAddressParameter = fldAddress != null ?
                new System.Data.SqlClient.SqlParameter("fldAddress", fldAddress) :
                new System.Data.SqlClient.SqlParameter("fldAddress", typeof(string));

            var fldTarikhVarizParameter = fldTarikhVariz != null ?
                new System.Data.SqlClient.SqlParameter("fldTarikhVariz", fldTarikhVariz) :
                new System.Data.SqlClient.SqlParameter("fldTarikhVariz", typeof(string));

            var fldQrCodeParameter = fldQrCode != null ?
                new System.Data.SqlClient.SqlParameter("fldQrCode", fldQrCode) :
                new System.Data.SqlClient.SqlParameter("fldQrCode", typeof(string));

            var fldOrganIdParameter = fldOrganId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldOrganId", fldOrganId) :
                new System.Data.SqlClient.SqlParameter("fldOrganId", typeof(int));

            var fldUserIdParameter = fldUserId.HasValue ?
                new System.Data.SqlClient.SqlParameter("fldUserId", fldUserId) :
                new System.Data.SqlClient.SqlParameter("fldUserId", typeof(int));

            var fldDescParameter = fldDesc != null ?
                new System.Data.SqlClient.SqlParameter("fldDesc", fldDesc) :
                new System.Data.SqlClient.SqlParameter("fldDesc", typeof(string));

            var fldIPParameter = fldIP != null ?
                new System.Data.SqlClient.SqlParameter("fldIP", fldIP) :
                new System.Data.SqlClient.SqlParameter("fldIP", typeof(string));

            var DetailParameter = Details.Rows.Count > 0 ?
               new System.Data.SqlClient.SqlParameter("Detail", Details) :
               new System.Data.SqlClient.SqlParameter("Detail", typeof(System.Data.DataTable));
            DetailParameter.TypeName = "Cntr.InserDetailFactor";
            DetailParameter.SqlDbType = System.Data.SqlDbType.Structured;

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreCommand("Cntr.spr_tblFactorUpdate @fldId, @fldTarikh, @fldShomare, @fldShanaseMoadiyan, @fldSharhSanad,"+
                " @fldProjectId, @fldAshkhasId, @fldTankhahGroupId, @fldKasrBime, @fldKasrHosnAnjamKar, @fldShomareSabt, @fldCodeEghtesadi, @fldAddress, @fldTarikhVariz, @fldQrCode, @Detail, @fldOrganId, @fldUserId, @fldDesc, @fldIP",
                fldIdParameter, fldTarikhParameter, fldShomareParameter,fldShanaseMoadiyanParameter, fldSharhSanadParameter,
                fldProjectIdParameter, fldAshkhasIdParameter, fldTankhahGroupIdParameter, fldKasrBimeParameter, fldKasrHosnAnjamKarParameter, fldShomareSabtParameter, fldCodeEghtesadiParameter, fldAddressParameter, fldTarikhVarizParameter, fldQrCodeParameter,
                DetailParameter, fldOrganIdParameter, fldUserIdParameter, fldDescParameter, fldIPParameter);
        }
    }
}