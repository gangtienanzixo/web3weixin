using System.Web;
using System.Web.Mvc;

namespace Seebon.Weixin.MP.Service
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}