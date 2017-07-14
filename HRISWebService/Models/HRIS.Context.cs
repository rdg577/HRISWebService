﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRISWebService.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class HRISEntities : DbContext
    {
        public HRISEntities()
            : base("name=HRISEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tjustifyApp> tjustifyApps { get; set; }
        public virtual DbSet<tapp212Image> tapp212Image { get; set; }
        public virtual DbSet<tappEmployee> tappEmployees { get; set; }
        public virtual DbSet<tAttDailyLog> tAttDailyLogs { get; set; }
        public virtual DbSet<tAttDTR> tAttDTRs { get; set; }
        public virtual DbSet<vPtlosApp> vPtlosApps { get; set; }
        public virtual DbSet<tptlosApp> tptlosApps { get; set; }
        public virtual DbSet<tpassSlipApp> tpassSlipApps { get; set; }
        public virtual DbSet<vpassSlipApp> vpassSlipApps { get; set; }
        public virtual DbSet<vJustifyApp> vJustifyApps { get; set; }
        public virtual DbSet<tappDFlexible> tappDFlexibles { get; set; }
        public virtual DbSet<tappDFlexiblesLog> tappDFlexiblesLogs { get; set; }
        public virtual DbSet<tappSalarySched> tappSalaryScheds { get; set; }
        public virtual DbSet<tappSalarySchem> tappSalarySchems { get; set; }
        public virtual DbSet<tappEmpStatu> tappEmpStatus { get; set; }
        public virtual DbSet<tappOffice> tappOffices { get; set; }
        public virtual DbSet<tappPosition> tappPositions { get; set; }
        public virtual DbSet<tappPositionSub> tappPositionSubs { get; set; }
    
        public virtual ObjectResult<string> DtrAction(string dtrID, string txtperiod, Nullable<int> period, Nullable<int> action, string actionEIC, string remarks)
        {
            var dtrIDParameter = dtrID != null ?
                new ObjectParameter("DtrID", dtrID) :
                new ObjectParameter("DtrID", typeof(string));
    
            var txtperiodParameter = txtperiod != null ?
                new ObjectParameter("txtperiod", txtperiod) :
                new ObjectParameter("txtperiod", typeof(string));
    
            var periodParameter = period.HasValue ?
                new ObjectParameter("Period", period) :
                new ObjectParameter("Period", typeof(int));
    
            var actionParameter = action.HasValue ?
                new ObjectParameter("action", action) :
                new ObjectParameter("action", typeof(int));
    
            var actionEICParameter = actionEIC != null ?
                new ObjectParameter("actionEIC", actionEIC) :
                new ObjectParameter("actionEIC", typeof(string));
    
            var remarksParameter = remarks != null ?
                new ObjectParameter("remarks", remarks) :
                new ObjectParameter("remarks", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("DtrAction", dtrIDParameter, txtperiodParameter, periodParameter, actionParameter, actionEICParameter, remarksParameter);
        }
    
        public virtual ObjectResult<string> JustifyAction(string eIC, Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, Nullable<int> action, string actionEIC, string ctrlNo, string remarks, Nullable<int> period)
        {
            var eICParameter = eIC != null ?
                new ObjectParameter("EIC", eIC) :
                new ObjectParameter("EIC", typeof(string));
    
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var actionParameter = action.HasValue ?
                new ObjectParameter("action", action) :
                new ObjectParameter("action", typeof(int));
    
            var actionEICParameter = actionEIC != null ?
                new ObjectParameter("actionEIC", actionEIC) :
                new ObjectParameter("actionEIC", typeof(string));
    
            var ctrlNoParameter = ctrlNo != null ?
                new ObjectParameter("CtrlNo", ctrlNo) :
                new ObjectParameter("CtrlNo", typeof(string));
    
            var remarksParameter = remarks != null ?
                new ObjectParameter("remarks", remarks) :
                new ObjectParameter("remarks", typeof(string));
    
            var periodParameter = period.HasValue ?
                new ObjectParameter("period", period) :
                new ObjectParameter("period", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("JustifyAction", eICParameter, startDateParameter, endDateParameter, actionParameter, actionEICParameter, ctrlNoParameter, remarksParameter, periodParameter);
        }
    }
}
