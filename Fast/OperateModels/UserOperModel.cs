using System.ComponentModel.DataAnnotations;

namespace OperateModels
{
    /// <summary>
    /// 登录用户
    /// </summary>
    public class LoginInfo
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string Name { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public class EditPass
    {
        [Required(ErrorMessage = "旧密码不能为空")]
        public string OldPass { get; set; }

        [Required(ErrorMessage = "新密码不能为空")]
        public string NewPass { get; set; }
    }
}