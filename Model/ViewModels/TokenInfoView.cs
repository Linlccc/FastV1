namespace Model.ViewModels
{
    /// <summary>
    /// Token 信息前台显示类
    /// </summary>
    public class TokenInfoView
    {
        /// <summary>
        /// token 字符串
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public double Expires_in { get; set; }

        /// <summary>
        /// token 类型
        /// </summary>
        public string Token_type { get; set; }
    }
}