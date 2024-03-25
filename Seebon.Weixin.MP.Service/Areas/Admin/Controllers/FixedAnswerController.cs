using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Seebon.Weixin.MP.Common;
using Seebon.Weixin.MP.Service.CommonService;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.Areas.Admin.Controllers
{
    public class FixedAnswerController : ABaseController
    {
        FixedAnswerService FBLL = new FixedAnswerService();
        //
        // GET: /Admin/GuestBook/

        public ActionResult Index(int? p)
        {
            int pageIndex = p ?? 1;
            int pageSize = 15;
            int totalCount = 0;
            var PageView = FBLL.GetPageList(pageIndex, pageSize, ref totalCount);
            var IPagedList = new StaticPagedList<FixedAnswer>(PageView, pageIndex, pageSize, totalCount);
            return View(IPagedList);
        }

        public ActionResult FAnswerManger(int id=0)
        {
            List<SelectListItem> msgtype = new List<SelectListItem>();
            msgtype.Add(new SelectListItem { Text = "文本", Value = "text" });
            msgtype.Add(new SelectListItem { Text = "音乐", Value = "music" });
            msgtype.Add(new SelectListItem { Text = "图文", Value = "news" });
            msgtype.Add(new SelectListItem { Text = "图片", Value = "image" });
            msgtype.Add(new SelectListItem { Text = "语音", Value = "voice" });
            msgtype.Add(new SelectListItem { Text = "视频", Value = "video" });
            ViewData["SMsgType"] = msgtype;
            if (id != 0)
            {
                var fAModel=FBLL.GetModel(id);
                ViewData["MsgType"] = fAModel.MsgType;
                return View(fAModel);
            }
            else
            {
                return View();
            }
            //return View();
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
                foreach (var Dadmin in FBLL.GetList(acb_id))
                {
                    FBLL.Remove(Dadmin);
                }
            }
            catch (Exception e)
            {
                IsOK = e.Message;
            }
            return IsOK;

        }

        [HttpPost]
        public ActionResult FAnswerManger(FixedAnswer fixedAnswer)
        {
            if (ModelState.IsValid)
            {
                if (fixedAnswer.id != 0)
                {
                    FBLL.Update(fixedAnswer);
                }
                else
                {
                    FBLL.AddModel(fixedAnswer);
                }
            }
            return RedirectToAction("Index");
        }


    }
}
