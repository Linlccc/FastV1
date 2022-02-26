using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// String 拓展
    /// </summary>
    public static class StringExtension
    {
        #region Md5

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>MD5 加密字符</returns>
        public static string MD5Encrypt16(this string source) => BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(source)), 4, 8).Replace("-", "");

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>MD5 加密字符</returns>
        public static string MD5Encrypt32(this string source) => BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(source))).Replace("-", "");

        /// <summary>
        /// 64位MD5加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>MD5 加密字符</returns>
        public static string MD5Encrypt64(this string source) => Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(source)));

        #endregion Md5

        #region Regex

        /// <summary>
        /// 是否匹配正则表达式
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this string str, string pattern) => new Regex(pattern).IsMatch(str);

        /// <summary>
        /// 和正则表达式匹配的项
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static MatchCollection Matches(this string str, string pattern) => new Regex(pattern).Matches(str);

        #endregion Regex

        /// <summary>
        /// 获取文本长度，区分全角半角
        /// 全角算两个字符
        /// </summary>
        /// <returns></returns>
        public static int FullHalfLength(this string text)
        {
            return Regex.Replace(text, "[^\x00-\xff]", "**").Length;
            //可使用以下方法，不过要看在不同编码中字节数
            //return Encoding.Default.GetByteCount(text);
        }

        /// <summary>
        /// 获取中文文本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetChineseText(this string text)=> Regex.Replace(text, "[\x00-\xff]", "");

        /// <summary>
        /// 切割骆驼命名式字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitCamelCase(this string str)
        {
            if (str.IsNull() || str.Length == 1) return new string[] { str };
            return Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})").Where(u => u.Length > 0).ToArray();
        }
    }
}