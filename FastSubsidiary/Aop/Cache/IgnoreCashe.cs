namespace System.Collections.Generic
{
    /// <summary>
    /// 忽略缓存
    /// </summary>
    public class IgnoreCashe
    {
        /// <summary>
        /// 要忽略的服务名和参数
        /// </summary>
        public static List<(string, string)> IgnoreSreviceName_Parameter = new();

        /// <summary>
        /// 检查是否忽略指定缓存key（检查后移除该忽略）
        /// 这个后面看一下，可以移除
        /// </summary>
        /// <param name="cacheKey">缓存key</param>
        /// <returns>如果要忽略返回true，否则false</returns>
        public static bool CheckIsIgnore(string cacheKey)
        {
            (string, string) ignoreC = ("", "");//要忽略的那个信息
            bool isIgnore = false;//是否忽略

            foreach ((string, string) ignoreInfo in IgnoreSreviceName_Parameter)
            {
                if (cacheKey.Contains(ignoreInfo.Item1) && cacheKey.Contains(ignoreInfo.Item2))
                {
                    isIgnore = true;
                    ignoreC = ignoreInfo;
                    break;
                }
            }

            //如果要忽略移除该项（下次不忽略）
            if (isIgnore) IgnoreSreviceName_Parameter.Remove(ignoreC);

            return isIgnore;
        }
    }
}