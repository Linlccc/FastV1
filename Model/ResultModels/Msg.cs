namespace System
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class Msg<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; } = 200;

        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; } = "服务器异常";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Data { get; set; }
    }
}