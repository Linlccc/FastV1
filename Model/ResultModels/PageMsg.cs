using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// 通用按页查找信息返回类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageMsg<T>
    {
        public PageMsg()
        {
        }

        public PageMsg(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        /// <summary>
        /// 当前页数【查询前赋值】
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页大小【查询前赋值】
        /// </summary>
        public int PageSize { set; get; } = 10;

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPageCount { get; set; } = 0;

        /// <summary>
        /// 数据总数
        /// </summary>
        public int TotalDataCount { get; set; } = 0;

        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> Data { get; set; }
    }
}