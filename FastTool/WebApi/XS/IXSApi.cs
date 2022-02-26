using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace FastTool.WebApi.XS
{
    /// <summary>
    /// 请求小说的第三方api
    /// </summary>
    //[TraceFilter(OutputTarget = OutputTarget.Console | OutputTarget.Debug)]
    public interface IXSApi : IHttpApi
    {
        /// <summary>
        /// 请求笔趣阁小说（原来我是世外高人）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("https://www.dvdspring.com/html/{id}/{pageIndex}.html")]
        Task<string> BiQuge(string id, string pageIndex);

        [HttpGet("https://www.paozw.com/biquge/{id}/{pageIndex}.html")]
        Task<string> BiQuge1(string id, string pageIndex);

        [HttpGet("https://www.zjzfcj.com/book/{id}/{pageIndex}.html")]
        Task<string> BiQuge2(string id, string pageIndex);

        
        [HttpGet("http://www.biququ.com/html/{id}/{pageIndex}.html")]
        Task<string> BiQuQu(string id, string pageIndex);
    }
}