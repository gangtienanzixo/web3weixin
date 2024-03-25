using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
    public class SelfQueryService
    {
        /// <summary>
        /// 获取最近三个月的社保缴费情况
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public ViewSocialInfo GetLastThreeWelfare(string openid)
        {
            ViewSocialInfo curWelfare = new ViewSocialInfo();
            seebonweixinEntities db = new Model.seebonweixinEntities();
            BindUsers user =db.BindUsers.FirstOrDefault(p=>p.weixinid==openid);
            if (user != null)
            {
                
                wp_ePaySocialInsur_Result result = db.wp_ePaySocialInsur(user.idcard, DateTime.Now.AddMonths(-3).Year, DateTime.Now.AddMonths(-3).Month, DateTime.Now.Year,
                    DateTime.Now.Month).OrderByDescending(soc => soc.year).ThenByDescending(soc => soc.month).FirstOrDefault();
                if (result != null)
                {
                    curWelfare.Name = db.getNameByIdCard(user.idcard).FirstOrDefault();
                    curWelfare.Year = result.year;
                    curWelfare.Month = result.month;
                    curWelfare.PerTotal = result.persJob + result.persMed + result.persOld;
                    curWelfare.CompTotal = result.compBear + result.compComp + result.compJob + result.compMed +
                                           result.compOld;
                    return curWelfare;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取最近三个月的公积金缴费情况
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public ViewAccInfo GetLastThreeAcc(string openid)
        {
            ViewAccInfo curAcc = new ViewAccInfo();
            seebonweixinEntities db = new Model.seebonweixinEntities();
            BindUsers user = db.BindUsers.FirstOrDefault(p => p.weixinid == openid);
            if (user != null)
            {

                wp_ePayAccFund_Result result = db.wp_ePayAccFund(user.idcard, DateTime.Now.AddMonths(-3).Year, DateTime.Now.AddMonths(-3).Month, DateTime.Now.Year,
                    DateTime.Now.Month).OrderByDescending(soc => soc.year).ThenByDescending(soc => soc.month).FirstOrDefault();
                if (result != null)
                {
                    curAcc.Name = db.getNameByIdCard(user.idcard).FirstOrDefault();
                    curAcc.Year = result.year;
                    curAcc.Month = result.month;
                    curAcc.PerAmount = result.persAcc;
                    curAcc.CompAmount = result.compAcc;
                    curAcc.BaseAmount = result.baseAcc;
                    return curAcc;
                }
            }
            return null;
        }

        /// <summary>
        /// 指定时间过去一年的社保明细
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public IQueryable<wp_ePaySocialInsur_Result> GetWelfareDetail(string openid,DateTime dt)
        {
            seebonweixinEntities entities = new seebonweixinEntities();
            BindUsers user = entities.BindUsers.FirstOrDefault(p => p.weixinid == openid);
            if (user == null) return null;
            string idcard = user.idcard;
            ObjectResult<wp_ePaySocialInsur_Result> query = entities.wp_ePaySocialInsur(idcard, dt.AddMonths(-12).Year,
                dt.AddMonths(-12).Month, dt.Year, dt.Month);
            return query.AsQueryable();
        }

        /// <summary>
        /// 指定年月的社保明细
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public wp_ePaySocialInsur_Result GetwelfareObject(string openid, int? year, int? month)
        {
            seebonweixinEntities entities = new seebonweixinEntities();
            BindUsers user = entities.BindUsers.FirstOrDefault(p => p.weixinid == openid);
            if (user == null) return null;
            string idcard = user.idcard;
            return (new seebonweixinEntities()).wp_ePaySocialInsur(idcard, year, month, year, month).FirstOrDefault();
        }

        /// <summary>
        /// 指定时间过去一年的公积金明细
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public IQueryable<wp_ePayAccFund_Result> GetAccDetail(string openid, DateTime dt)
        {
            seebonweixinEntities entities = new seebonweixinEntities();
            BindUsers user = entities.BindUsers.FirstOrDefault(p => p.weixinid == openid);
            if (user == null) return null;
            return entities.wp_ePayAccFund(user.idcard, dt.AddMonths(-12).Year,
                    dt.AddMonths(-12).Month, dt.Year, dt.Month).AsQueryable();
        }

        /// <summary>
        /// 指定年月的公积金明细
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public wp_ePayAccFund_Result GetAccObject(string openid, int? year, int? month)
        {
            seebonweixinEntities entities = new seebonweixinEntities();
            BindUsers user = entities.BindUsers.FirstOrDefault(p => p.weixinid == openid);
            if (user == null) return null;
            return entities.wp_ePayAccFund(user.idcard, year, month, year, month).FirstOrDefault();
        }

    }

}
