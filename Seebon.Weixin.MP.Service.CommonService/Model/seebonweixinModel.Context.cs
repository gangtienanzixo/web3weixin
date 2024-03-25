﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class seebonweixinEntities : DbContext
    {
        public seebonweixinEntities()
            : base("name=seebonweixinEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Administrator> Administrator { get; set; }
        public DbSet<BindUsers> BindUsers { get; set; }
        public DbSet<ConsultList> ConsultList { get; set; }
        public DbSet<DialogRecord> DialogRecord { get; set; }
        public DbSet<FixedAnswer> FixedAnswer { get; set; }
        public DbSet<FocusUsers> FocusUsers { get; set; }
        public DbSet<GuestBook> GuestBook { get; set; }
        public DbSet<LogAccount> LogAccount { get; set; }
        public DbSet<LoginRecord> LoginRecord { get; set; }
        public DbSet<OnlineClient> OnlineClient { get; set; }
        public DbSet<OnlineCustomer> OnlineCustomer { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<customerlog> customerlog { get; set; }
        public DbSet<customerrecord> customerrecord { get; set; }
        public DbSet<Resem> Resem { get; set; }
    
        public virtual ObjectResult<string> getNameByIdCard(string idCardId)
        {
            var idCardIdParameter = idCardId != null ?
                new ObjectParameter("idCardId", idCardId) :
                new ObjectParameter("idCardId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("getNameByIdCard", idCardIdParameter);
        }
    
        public virtual int newseebon_Login(string username, string password)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("newseebon_Login", usernameParameter, passwordParameter);
        }
    
        public virtual ObjectResult<s_user_Login_Result> s_user_Login(string username, string password, string logIP)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            var logIPParameter = logIP != null ?
                new ObjectParameter("LogIP", logIP) :
                new ObjectParameter("LogIP", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<s_user_Login_Result>("s_user_Login", usernameParameter, passwordParameter, logIPParameter);
        }
    
        public virtual ObjectResult<wp_ePayAccFund_Result> wp_ePayAccFund(string idCardId, Nullable<int> startYear, Nullable<int> startMonth, Nullable<int> endYear, Nullable<int> endMonth)
        {
            var idCardIdParameter = idCardId != null ?
                new ObjectParameter("idCardId", idCardId) :
                new ObjectParameter("idCardId", typeof(string));
    
            var startYearParameter = startYear.HasValue ?
                new ObjectParameter("startYear", startYear) :
                new ObjectParameter("startYear", typeof(int));
    
            var startMonthParameter = startMonth.HasValue ?
                new ObjectParameter("startMonth", startMonth) :
                new ObjectParameter("startMonth", typeof(int));
    
            var endYearParameter = endYear.HasValue ?
                new ObjectParameter("endYear", endYear) :
                new ObjectParameter("endYear", typeof(int));
    
            var endMonthParameter = endMonth.HasValue ?
                new ObjectParameter("endMonth", endMonth) :
                new ObjectParameter("endMonth", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<wp_ePayAccFund_Result>("wp_ePayAccFund", idCardIdParameter, startYearParameter, startMonthParameter, endYearParameter, endMonthParameter);
        }
    
        public virtual ObjectResult<wp_ePaySocialInsur_Result> wp_ePaySocialInsur(string idCardId, Nullable<int> startYear, Nullable<int> startMonth, Nullable<int> endYear, Nullable<int> endMonth)
        {
            var idCardIdParameter = idCardId != null ?
                new ObjectParameter("idCardId", idCardId) :
                new ObjectParameter("idCardId", typeof(string));
    
            var startYearParameter = startYear.HasValue ?
                new ObjectParameter("startYear", startYear) :
                new ObjectParameter("startYear", typeof(int));
    
            var startMonthParameter = startMonth.HasValue ?
                new ObjectParameter("startMonth", startMonth) :
                new ObjectParameter("startMonth", typeof(int));
    
            var endYearParameter = endYear.HasValue ?
                new ObjectParameter("endYear", endYear) :
                new ObjectParameter("endYear", typeof(int));
    
            var endMonthParameter = endMonth.HasValue ?
                new ObjectParameter("endMonth", endMonth) :
                new ObjectParameter("endMonth", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<wp_ePaySocialInsur_Result>("wp_ePaySocialInsur", idCardIdParameter, startYearParameter, startMonthParameter, endYearParameter, endMonthParameter);
        }
    
        public virtual ObjectResult<getMyOnlineClientList_Result> getMyOnlineClientList(string connectUserName)
        {
            var connectUserNameParameter = connectUserName != null ?
                new ObjectParameter("ConnectUserName", connectUserName) :
                new ObjectParameter("ConnectUserName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getMyOnlineClientList_Result>("getMyOnlineClientList", connectUserNameParameter);
        }
    
        public virtual ObjectResult<getOnlineClientList_Result> getOnlineClientList()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<getOnlineClientList_Result>("getOnlineClientList");
        }
    }
}
