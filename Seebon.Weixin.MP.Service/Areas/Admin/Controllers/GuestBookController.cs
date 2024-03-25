using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.Areas.Admin.Controllers
{
    public class GuestBookController : ABaseController
    {
        GuestBookService GBLL = new GuestBookService();
        //
        // GET: /Admin/GuestBook/

        public ActionResult Index(int? p)
        {
            int pageIndex = p ?? 1;
            int pageSize = 10;
            int totalCount = 0;
            var guestbook = GBLL.GetPageList(pageIndex, pageSize, ref totalCount);
            var guestbookIPagedList = new StaticPagedList<GuestBook>(guestbook, pageIndex, pageSize, totalCount);
            return View(guestbookIPagedList);
        }

        public ActionResult GBookManger(int id)
        {
            return View(GBLL.GetModel(id));
        }

        [HttpPost]
        public string DeleteAll(string cb_id)
        {
            string IsOK = "ok";
            try
            {
                string[] scb_id = cb_id.Split(',');
                int[] acb_id = new int[scb_id.Length - 1];
                for (int i = 0; i < scb_id.Length - 1; i++)
                {
                    acb_id[i] = int.Parse(scb_id[i].ToString());
                }
                foreach (var Dadmin in GBLL.GetList(acb_id))
                {
                    GBLL.Remove(Dadmin);
                }
            }
            catch (Exception e)
            {
                IsOK = e.Message;
            }
            return IsOK;

        }

    }
}
