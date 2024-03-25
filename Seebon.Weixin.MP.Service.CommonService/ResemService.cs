using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seebon.Weixin.MP.AdvancedAPIs;
using Seebon.Weixin.MP.Common;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
  public class ResemService
    {
      seebonweixinEntities db = new seebonweixinEntities();

      /// <summary>
      /// 添加返回新添加的ID
      /// </summary>
      /// <param name="resem"></param>
      public int Add(Resem resem)
      {
          db.Resem.Add(resem);
          db.SaveChanges();
          return resem.Id;
      }

      /// <summary>
      /// 获取一个model
      /// </summary>
      /// <param name="id"></param>
      /// <param name="Guid"></param>
      /// <returns></returns>
      public Resem GetModel(int id,string Guid)
      {
        return  db.Resem.FirstOrDefault(p => p.Id == id && p.Guid == Guid);
      }
    }
}
