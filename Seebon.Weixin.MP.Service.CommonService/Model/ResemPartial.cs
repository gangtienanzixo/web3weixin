using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    [MetadataType(typeof(ResemMetadata))]
   public partial class Resem
    {
        private class ResemMetadata
        {
            public int Id { get; set; }

            [Required(ErrorMessage="请输入姓名")]
            public string Name { get; set; }

            [Required(ErrorMessage = "请选择性别")]
            public int Sex { get; set; }

            [DisplayName("年龄")]
            [Required(ErrorMessage = "请输入年龄")]
            public int Age { get; set; }

             [Required(ErrorMessage = "请输入毕业院校")]
            public string School { get; set; }

             [Required(ErrorMessage = "请输入专业")]
            public string Major { get; set; }

             [Required(ErrorMessage = "请输入学历")]
            public string Education { get; set; }

             [Required(ErrorMessage = "请输入联系电话")]
            public string Tel { get; set; }

             [Required(ErrorMessage = "请输入工作经验")]
            public string Experience { get; set; }

            public string Guid { get; set; }
        }
    }
}
