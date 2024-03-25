using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seebon.Weixin.MP.Service.CommonService.Model;

namespace Seebon.Weixin.MP.Service.CommonService
{
   public class GuestBookService
    {
       seebonweixinEntities db = new seebonweixinEntities();
       /// <summary>
       /// 添加留言记录
       /// </summary>
       /// <param name="GuestBook"></param>
       public void AddGB(GuestBook GuestBook)
       {
           var db = new Model.seebonweixinEntities();
           db.GuestBook.Add(GuestBook);
           db.SaveChanges();
       }

       /// <summary>
       /// 获取分页列表
       /// </summary>
       /// <returns></returns>
       public List<GuestBook> GetPageList(int pageIndex, int pageSize, ref int totalCount)
       {
           var PageView = (from p in db.GuestBook
                            orderby p.id descending
                            select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
           totalCount = db.GuestBook.Count();
           return PageView.ToList();
       }
       /// <summary>
       /// 获取一个实体
       /// </summary>
       /// <param name="id">主键ID</param>
       /// <returns></returns>
       public GuestBook GetModel(int id)
       {
           return db.GuestBook.Find(id);
       }
       /// <summary>
       /// 获取多个值
       /// </summary>
       /// <returns>数值ID</returns>
       public List<GuestBook> GetList(int[] id)
       {

           return db.GuestBook.Where(d => id.Contains(d.id)).ToList();
       }
       /// <summary>
       /// 删除实体对象
       /// </summary>
       /// <param name="Administrator"></param>
       public void Remove(GuestBook guestBook)
       {
           db.GuestBook.Remove(guestBook);
           db.SaveChanges();
       }

    }
}
