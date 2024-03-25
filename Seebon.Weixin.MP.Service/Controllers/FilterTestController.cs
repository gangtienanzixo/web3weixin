using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seebon.Weixin.MP.Service.Controllers
{
    using Seebon.Weixin.MP.MvcExtension;

    /// <summary>
    /// 演示Seebon.Weixin.MP.MvcExtension.WeixinInternalRequestAttribute
    /// </summary>
    public class FilterTestController : Controller
    {
        [WeixinInternalRequest("访问被拒绝，请通过微信客户端访问！")]
        public ContentResult Index()
        {
            return Content("访问正常。当前地址：" + Request.Url.PathAndQuery + "<br />请点击右上角转发按钮，使用【在浏览器中打开】功能进行测试！<br />或者也可以直接在外部浏览器打开http://weixin.Seebon.com/FilterTest/进行测试。");
        }
    }
}