using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace System
{
    public static class MsgHelper
    {
        #region Msg

        /// <summary>
        /// 成功
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static Msg<T> Success<T>(T data, string msg = "成功") => new()
        {
            Data = data,
            Message = msg,
            Code = StatusCodes.Status200OK,
            Success = true
        };

        /// <summary>
        /// 失败
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="msg">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static Msg<T> Fail<T>(string msg = "失败,请刷新后重试", T data = default, int code = StatusCodes.Status400BadRequest) => new()
        {
            Message = msg,
            Data = data,
            Code = code,
            Success = false
        };

        #endregion Msg

        #region Page

        /// <summary>
        /// 使用当前页数等数据和结果组合成新数据
        /// </summary>
        /// <typeparam name="T">旧数据结果类型</typeparam>
        /// <typeparam name="TR">新数据结果类型</typeparam>
        /// <param name="pageModel">旧数据</param>
        /// <param name="result">结果</param>
        /// <returns></returns>
        public static PageMsg<TR> GetNewPageModel<T, TR>(this PageMsg<T> pageModel, List<TR> result)
        {
            return new PageMsg<TR>()
            {
                PageIndex = pageModel.PageIndex,
                TotalPageCount = pageModel.TotalPageCount,
                TotalDataCount = pageModel.TotalDataCount,
                PageSize = pageModel.PageSize,
                Data = result
            };
        }

        #endregion Page
    }
}