using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    [MetadataType(typeof(wp_ePaySocialInsur_ResultMetadata))]
    public partial class wp_ePaySocialInsur_Result
    {
        private class wp_ePaySocialInsur_ResultMetadata
        {
            [Display(Name = "年")]
            public string year { get; set; }
            [Display(Name = "月")]
            public int month { get; set; }
            [Display(Name="养老基数")]
            public Nullable<decimal> baseOld { get; set; }
            [Display(Name = "单位养老")]
            public Nullable<decimal> compOld { get; set; }
            [Display(Name = "个人养老")]
            public Nullable<decimal> persOld { get; set; }
            [Display(Name = "失业基数")]
            public Nullable<decimal> baseJob { get; set; }
            [Display(Name = "单位失业")]
            public Nullable<decimal> compJob { get; set; }
            [Display(Name = "个人失业")]
            public Nullable<decimal> persJob { get; set; }
            [Display(Name = "医疗基数")]
            public Nullable<decimal> baseMed { get; set; }
            [Display(Name = "单位医疗")]
            public Nullable<decimal> compMed { get; set; }
            [Display(Name = "个人医疗")]
            public Nullable<decimal> persMed { get; set; }
            [Display(Name = "重大疾病")]
            public Nullable<decimal> MainMed { get; set; }
            [Display(Name = "生育基数")]
            public Nullable<decimal> baseBear { get; set; }
            [Display(Name = "单位生育")]
            public Nullable<decimal> compBear { get; set; }
            [Display(Name = "工伤基数")]
            public Nullable<decimal> baseComp { get; set; }
            [Display(Name = "单位工伤")]
            public Nullable<decimal> compComp { get; set; }
            public Nullable<decimal> AGFee { get; set; }
            public string detail { get; set; }
        }
        
    }

    
}
