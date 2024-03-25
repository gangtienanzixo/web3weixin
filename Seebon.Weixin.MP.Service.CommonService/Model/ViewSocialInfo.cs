namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    /// <summary>
    /// 个人每月社保信息汇总
    /// </summary>
    public class ViewSocialInfo
    {
        public string Name { get; set; }
        public string Year { get; set; }
        public int Month { get; set; }
        public decimal? PerTotal { get; set; }
        public decimal? CompTotal { get; set; } 
    }
}