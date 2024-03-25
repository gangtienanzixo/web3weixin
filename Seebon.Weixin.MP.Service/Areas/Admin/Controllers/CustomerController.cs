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
    public class CustomerController : ABaseController
    {
        CustomerService CBLL = new CustomerService();
        //
        // GET: /Admin/Customer/
        public ActionResult ClientList()
        {
            ClientService client = new ClientService();
            return View(client.OnlineClientList());
        }

        public ActionResult MyClientList()
        {
            ClientService client = new ClientService();
            AdminService admin = new AdminService();
            return View(client.MyOnlineClientList(admin.GetCurLoginUser().UserName));
        }

        public ActionResult MyMsgList(int? p)
        {
            AdminService admin = new AdminService();
            ClientService client = new ClientService();
            int pageIndex = p ?? 1;
            const int pageSize = 10;
            int totalCount = 0;
            var msglist = client.MyMsgList(admin.GetCurLoginUser().UserName,pageIndex, pageSize, ref totalCount);
            var msgListIPagedList = new StaticPagedList<DialogRecord>(msglist, pageIndex, pageSize, totalCount);
            return View(msgListIPagedList);
        }

        public ActionResult Index(int? p)
        {
            int pageIndex = p ?? 1;
            int pageSize = 15;
            int totalCount = 0;
            var PageView = CBLL.GetPageList(pageIndex, pageSize, ref totalCount);
            var IPagedList = new StaticPagedList<Customer>(PageView, pageIndex, pageSize, totalCount);
            return View(IPagedList);
        }

        public ActionResult CustomerManger(int id=0)
        {
            if (id != 0)
            {
                return View(CBLL.GetModel(id));
            }
            return View();
        }
        [HttpPost]
        public ActionResult CustomerManger(Customer Customers)
        {
            if (ModelState.IsValid)
            {
                if (Customers.id != 0)
                {
                    CBLL.Update(Customers);
                }
                else
                {
                    CBLL.AddModel(Customers);
                }
            }
            return RedirectToAction("Index");
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
                foreach (var Dadmin in CBLL.GetList(acb_id))
                {
                    CBLL.Remove(Dadmin);
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
