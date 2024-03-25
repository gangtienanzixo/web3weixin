using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    [MetadataType(typeof(GuestBookMetadata))]
    public partial class GuestBook
    {
        private class GuestBookMetadata
        {
            public int id { get; set; }

            [Required(ErrorMessage = "请选择联系方式类型")]
            public string TelType { get; set; }

            [Required(ErrorMessage = "请输入联系方式")]
            public string Tel { get; set; }

            [Required(ErrorMessage = "请输入留言内容")]
            public string Content { get; set; }
        }
    }
}
