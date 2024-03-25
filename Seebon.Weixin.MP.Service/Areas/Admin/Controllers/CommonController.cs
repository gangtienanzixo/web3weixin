using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seebon.Weixin.MP.Common;

namespace Seebon.Weixin.MP.Service.Areas.Admin.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Admin/Common/

        [HttpPost]
        public string Upload()
        {
            string url = HttpContext.Request.Url.Host;
            UploadFileHelper files = new UploadFileHelper(".jpg,.gif,.png,.bmp,.mp3,.wma", 5, "/Content/UploadFiles/");
            var filepic = Request.Files["FilePic"];
            if (files.UpLoad(filepic))
            {
                return url + files.ShowPath;
            }
            else
            {
                return "error";
            }
        }

    }
}
