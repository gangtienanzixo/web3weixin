using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    /// <summary>
    /// 在线客户显示的信息
    /// </summary>
    public class ViewClient
    {
        [Display(Name="微信Id")]
        public string OpenId { get; set; }
        public string Name { get; set; }
        [Display(Name="微信昵称")]
        public string NickName { get; set; }
        [Display(Name="请求对话时间")]
        public DateTime OnlineTime { get; set; }
        [Display(Name="请求客服id")]
        public string ConnectUserName { get; set; }
    }
}
