namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    /// <summary>
    /// 个人每月公积金信息汇总
    /// </summary>
    public class ViewAccInfo
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? PerAmount { get; set; }
        public decimal? CompAmount { get; set; } 
    }
}