using System.ComponentModel.DataAnnotations;
namespace Seebon.Weixin.MP.Service.CommonService.Model
{
    /// <summary>
    /// 账号信息
    /// </summary>
    public class ViewUserAccount
    {
        [Display(Name = "账号")]
        [Required(AllowEmptyStrings=false,ErrorMessage="请输入账号")]
        public string UserName { get; set; }
        [Display(Name="密码")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
        [Required]
        public string wx_tokenid { get; set; }
    }
}