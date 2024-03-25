using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    [MetadataType(typeof(wp_ePayAccFund_ResultMetadata))]
    public partial class wp_ePayAccFund_Result
    {
        private class wp_ePayAccFund_ResultMetadata
        {
            [Display(Name = "年")]
            public int year { get; set; }
            [Display(Name = "月")]
            public int month { get; set; }
            [Display(Name="缴费基数")]
            public decimal baseAcc { get; set; }
            [Display(Name = "公司缴费")]
            public decimal compAcc { get; set; }
            [Display(Name = "个人缴费")]
            public decimal persAcc { get; set; }
        }
    }
}
