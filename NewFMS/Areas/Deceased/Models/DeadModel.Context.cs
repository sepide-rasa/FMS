﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewFMS.Areas.Deceased.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class RasaNewFMSEntities1 : DbContext
    {
        public RasaNewFMSEntities1()
            : base("name=RasaNewFMSEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual int spr_tblGhabrInfoDelete(Nullable<int> fldId, Nullable<int> fldUserId)
        {
            var fldIdParameter = fldId.HasValue ?
                new ObjectParameter("fldId", fldId) :
                new ObjectParameter("fldId", typeof(int));
    
            var fldUserIdParameter = fldUserId.HasValue ?
                new ObjectParameter("fldUserId", fldUserId) :
                new ObjectParameter("fldUserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spr_tblGhabrInfoDelete", fldIdParameter, fldUserIdParameter);
        }
    
        public virtual int spr_tblGhabrInfoInsert(string fldName, string fldFamily, string fldNameFather, string fldBDate, string fldDeathDate, Nullable<int> fldObjectId, Nullable<int> fldUserId, Nullable<int> fldOrganId)
        {
            var fldNameParameter = fldName != null ?
                new ObjectParameter("fldName", fldName) :
                new ObjectParameter("fldName", typeof(string));
    
            var fldFamilyParameter = fldFamily != null ?
                new ObjectParameter("fldFamily", fldFamily) :
                new ObjectParameter("fldFamily", typeof(string));
    
            var fldNameFatherParameter = fldNameFather != null ?
                new ObjectParameter("fldNameFather", fldNameFather) :
                new ObjectParameter("fldNameFather", typeof(string));
    
            var fldBDateParameter = fldBDate != null ?
                new ObjectParameter("fldBDate", fldBDate) :
                new ObjectParameter("fldBDate", typeof(string));
    
            var fldDeathDateParameter = fldDeathDate != null ?
                new ObjectParameter("fldDeathDate", fldDeathDate) :
                new ObjectParameter("fldDeathDate", typeof(string));
    
            var fldObjectIdParameter = fldObjectId.HasValue ?
                new ObjectParameter("fldObjectId", fldObjectId) :
                new ObjectParameter("fldObjectId", typeof(int));
    
            var fldUserIdParameter = fldUserId.HasValue ?
                new ObjectParameter("fldUserId", fldUserId) :
                new ObjectParameter("fldUserId", typeof(int));
    
            var fldOrganIdParameter = fldOrganId.HasValue ?
                new ObjectParameter("fldOrganId", fldOrganId) :
                new ObjectParameter("fldOrganId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spr_tblGhabrInfoInsert", fldNameParameter, fldFamilyParameter, fldNameFatherParameter, fldBDateParameter, fldDeathDateParameter, fldObjectIdParameter, fldUserIdParameter, fldOrganIdParameter);
        }
    
        public virtual ObjectResult<spr_tblGhabrInfoSelect> spr_tblGhabrInfoSelect(string fieldname, string value, Nullable<int> h)
        {
            var fieldnameParameter = fieldname != null ?
                new ObjectParameter("fieldname", fieldname) :
                new ObjectParameter("fieldname", typeof(string));
    
            var valueParameter = value != null ?
                new ObjectParameter("value", value) :
                new ObjectParameter("value", typeof(string));
    
            var hParameter = h.HasValue ?
                new ObjectParameter("h", h) :
                new ObjectParameter("h", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spr_tblGhabrInfoSelect>("spr_tblGhabrInfoSelect", fieldnameParameter, valueParameter, hParameter);
        }
    
        public virtual int spr_tblGhabrInfoUpdate(Nullable<int> fldId, string fldName, string fldFamily, string fldNameFather, string fldBDate, string fldDeathDate, Nullable<int> fldUserId, Nullable<int> fldOrganId)
        {
            var fldIdParameter = fldId.HasValue ?
                new ObjectParameter("fldId", fldId) :
                new ObjectParameter("fldId", typeof(int));
    
            var fldNameParameter = fldName != null ?
                new ObjectParameter("fldName", fldName) :
                new ObjectParameter("fldName", typeof(string));
    
            var fldFamilyParameter = fldFamily != null ?
                new ObjectParameter("fldFamily", fldFamily) :
                new ObjectParameter("fldFamily", typeof(string));
    
            var fldNameFatherParameter = fldNameFather != null ?
                new ObjectParameter("fldNameFather", fldNameFather) :
                new ObjectParameter("fldNameFather", typeof(string));
    
            var fldBDateParameter = fldBDate != null ?
                new ObjectParameter("fldBDate", fldBDate) :
                new ObjectParameter("fldBDate", typeof(string));
    
            var fldDeathDateParameter = fldDeathDate != null ?
                new ObjectParameter("fldDeathDate", fldDeathDate) :
                new ObjectParameter("fldDeathDate", typeof(string));
    
            var fldUserIdParameter = fldUserId.HasValue ?
                new ObjectParameter("fldUserId", fldUserId) :
                new ObjectParameter("fldUserId", typeof(int));
    
            var fldOrganIdParameter = fldOrganId.HasValue ?
                new ObjectParameter("fldOrganId", fldOrganId) :
                new ObjectParameter("fldOrganId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spr_tblGhabrInfoUpdate", fldIdParameter, fldNameParameter, fldFamilyParameter, fldNameFatherParameter, fldBDateParameter, fldDeathDateParameter, fldUserIdParameter, fldOrganIdParameter);
        }
    }
}