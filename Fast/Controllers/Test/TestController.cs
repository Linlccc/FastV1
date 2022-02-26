using FastTool.WebApi.XS;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fast.Controllers.Test
{
    [ApiController, CustomRoute(ApiGroup.Test)]
    public class TestController : Controller
    {
        private readonly IXSApi _xSApi;

        public TestController(IXSApi xSApi)
        {
            _xSApi = xSApi;
        }

        public string T1([FromQuery, FromForm] string v1, [FromForm] string v2)
        {
            return v1 + v2;
        }

        public string T2([FromBody] string v1, string v2)
        {
            return v1 + v2;
        }

        public string T3(User user, string v1, string v2)
        {
            return v1 + v2;
        }


        #region BiQuQu

        /// <summary>
        /// 获取笔趣趣小说
        /// </summary>
        /// <param name="id">小说id</param>
        /// <param name="o">初始页</param>
        /// <param name="s">开始页</param>
        /// <param name="e">结束页</param>
        /// <returns></returns>
        public async Task<string> GetBQQXS([Required] string id, int o, [Required] int s, int e)
        {
            if (e < s) e = s + 9;
            s += o;
            e += o;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = s; i <= e; i++)
            {
                stringBuilder.AppendLine(await GetOnePageBQQXs(id, i));
            }

            string body = stringBuilder.ToString();

            return body;
        }

        private async Task<string> GetOnePageBQQXs(string id, int pageIndex)
        {
            string[] reMoves =
            {
            };

            string xs = await _xSApi.BiQuQu(id, pageIndex.ToString());

            MatchCollection titleMatchs = xs.Matches("<h1>.*</h1>");
            string title = titleMatchs[0].Value.Replace("<h1>", "").Replace("</h1>", "");

            string body = xs[xs.IndexOf("<p>")..xs.IndexOf("<script>chaptererror();</script>")];
            body = body.Replace("<p>", Environment.NewLine).Replace("</p>", "");

            foreach (string item in reMoves)
            {
                body = body.Replace(item, "");
            }

            string next = "0";
            long nextNum = 0;

            if (title.IsNNull())
            {
                //下一章
                next = xs.Matches("<a id=\"pager_next\" href=\"/html/[0-9]+/[0-9_]+.html\" target=\"_top\" class=\"next\">下一章</a>")[0].Value.Matches("[0-9]+\\.html")[0].Value.Replace(".html", "");
                //当前章
                string current = title.Matches("第(.|\\s)*章")[0].Value.Replace("第", "").Replace("章", "").Replace("两", "二");
                nextNum = int.Parse(current) + 1;
            }
            else return "";

            return $"{title}{Environment.NewLine}{body}{Environment.NewLine}{title}{Environment.NewLine}{long.Parse(next) - nextNum} + {nextNum} = {next}{Environment.NewLine}--------------------------";
        }

        #endregion BiQuQu

        #region BiQuge2

        /// <summary>
        /// 获取小说
        /// </summary>
        /// <param name="id">小说id</param>
        /// <param name="o">初始页</param>
        /// <param name="s">开始页</param>
        /// <param name="e">结束页</param>
        /// <returns></returns>
        public async Task<string> GetB2XS([Required] string id, int o, [Required] int s, int e)
        {
            if (e < s) e = s + 9;
            s += o;
            e += o;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = s; i <= e; i++)
            {
                stringBuilder.AppendLine(await GetOnePageB2Xs(id, i));
            }

            string body = stringBuilder.ToString();

            return body;
        }

        private async Task<string> GetOnePageB2Xs(string id, int pageIndex)
        {
            string xs = (await _xSApi.BiQuge2(id, pageIndex.ToString())).Replace("章节错误,点此举报(免注册)</a>,举报后维护人员会在两分钟内校正章节内容,请耐心等待,并刷新页面。</div>", "");
            string xs_2 = (await _xSApi.BiQuge2(id, pageIndex.ToString() + "_2")).Replace("章节错误,点此举报(免注册)</a>,举报后维护人员会在两分钟内校正章节内容,请耐心等待,并刷新页面。</div>", "");

            MatchCollection titleMatchs = xs.Matches("<h1 class=\"title\">.*</h1>");
            string title = titleMatchs[0].Value.Replace("<h1 class=\"title\">", "").Replace("</h1>", "");
            MatchCollection titleMatchs_2 = xs.Matches("<h1 class=\"title\">.*</h1>");
            string title_2 = titleMatchs_2[0].Value.Replace("<h1 class=\"title\">", "").Replace("</h1>", "");

            MatchCollection bodyMatchs = xs.Matches("<div class=\"content\" id=\"content\"(.|\\s)*?</div>");
            string body = Regex.Replace(bodyMatchs[0].Value, @"<(.|\\s)*?>", Environment.NewLine);
            MatchCollection bodyMatchs_2 = xs_2.Matches("<div class=\"content\" id=\"content\"(.|\\s)*?</div>");
            string body_2 = Regex.Replace(bodyMatchs_2[0].Value, @"<(.|\\s)*?>", Environment.NewLine);
            if (body[0..100] != body_2[0..100]) body += body_2;
            body = "  " + body.Replace("美女视频免费看", "").Replace("&nbsp;", "").Replace("免费看片APP", "").Replace(" ", "");

            string next = "0";
            long nextNum = 0;

            if (title.IsNNull())
            {
                //下一章
                if (title_2.IsNNull())
                {
                    MatchCollection nextMatchs = xs_2.Matches("<a href=\"/book/[0-9]+/[0-9_]+.html\">下一章</a>")[0].Value.Matches("[0-9]+\\.html");
                    if (nextMatchs.Count != 0) next = nextMatchs[0].Value.Replace(".html", "");
                }
                else next = xs.Matches("<a href=\"/book/[0-9]+/[0-9_]+.html\">下一章</a>")[0].Value.Matches("[0-9]+\\.html")[0].Value.Replace(".html", "");
                //当前章
                string current = title.Matches("第(.|\\s)*章")[0].Value.Replace("第", "").Replace("章", "").Replace("两", "二");
                nextNum = ParseCnToInt(current) + 1;
            }
            else return "";

            return $"{title}{Environment.NewLine}{body}{title}{Environment.NewLine}{long.Parse(next) - nextNum} + {nextNum} = {next}{Environment.NewLine}--------------------------";
        }

        #endregion BiQuge2

        #region BiQuge1

        /// <summary>
        /// 获取小说
        /// </summary>
        /// <param name="id">小说id</param>
        /// <param name="o">初始页</param>
        /// <param name="s">开始页</param>
        /// <param name="e">结束页</param>
        /// <returns></returns>
        public async Task<string> GetB1XS([Required] string id, int o, [Required] int s, int e)
        {
            if (e < s) e = s + 9;
            s += o;
            e += o;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = s; i <= e; i++)
            {
                stringBuilder.AppendLine(await GetOnePageB1Xs(id, i));
            }

            string body = stringBuilder.ToString();

            return body;
        }

        private async Task<string> GetOnePageB1Xs(string id, int pageIndex)
        {
            string xs = await _xSApi.BiQuge1(id, pageIndex.ToString());
            string xs_2 = await _xSApi.BiQuge1(id, pageIndex.ToString() + "_2");

            MatchCollection titleMatchs = xs.Matches("<h1>.*</h1>");
            string title = titleMatchs[0].Value.Replace("<h1>", "").Replace("</h1>", "");
            MatchCollection titleMatchs_2 = xs.Matches("<h1>.*</h1>");
            string title_2 = titleMatchs_2[0].Value.Replace("<h1>", "").Replace("</h1>", "");

            MatchCollection bodyMatchs = xs.Matches("<article id=\"content\"(.|\\s)*?</article>");
            string body = Regex.Replace(bodyMatchs[0].Value, @"<(.|\\s)*?>", Environment.NewLine);
            MatchCollection bodyMatchs_2 = xs_2.Matches("<article id=\"content\"(.|\\s)*?</article>");
            string body_2 = Regex.Replace(bodyMatchs_2[0].Value, @"<(.|\\s)*?>", Environment.NewLine);
            if (body[0..20] != body_2[0..20]) body += body_2;

            string next = "0";
            long nextNum = 0;

            if (title.IsNNull())
            {
                //下一章
                if (title_2.IsNNull())
                {
                    MatchCollection nextMatchs = xs_2.Matches("<a id=\"next\"(.|\\s)*?</a>")[0].Value.Matches("[0-9]+\\.html");
                    if (nextMatchs.Count != 0) next = nextMatchs[0].Value.Replace(".html", "");
                }
                else next = xs.Matches("<a id=\"next\"(.|\\s)*?</a>")[0].Value.Matches("[0-9]+\\.html")[0].Value.Replace(".html", "");
                //当前章
                string current = title.Matches("第(.|\\s)*章")[0].Value.Replace("第", "").Replace("章", "");
                nextNum = ParseCnToInt(current) + 1;
            }
            else return "";

            return $"{title}{Environment.NewLine}{body}{title}{Environment.NewLine}{long.Parse(next) - nextNum} + {nextNum} = {next}{Environment.NewLine}--------------------------";
        }

        #endregion BiQuge1

        #region BiQuge

        /// <summary>
        /// 获取小说
        /// </summary>
        /// <param name="id">小说id</param>
        /// <param name="o">初始页</param>
        /// <param name="s">开始页</param>
        /// <param name="e">结束页</param>
        /// <returns></returns>
        public async Task<string> GetXS([Required] string id, int o, [Required] int s, int e)
        {
            if (e < s) e = s + 9;
            s += o;
            e += o;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = s; i <= e; i++)
            {
                stringBuilder.AppendLine(await GetOnePageXs(id, i));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 获取一页小说
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private async Task<string> GetOnePageXs(string id, int pageIndex)
        {
            string xs = await _xSApi.BiQuge(id, pageIndex.ToString());

            MatchCollection titleMatchs = xs.Matches("<h1>.*</h1>");
            string title = titleMatchs[0].Value.Replace("<h1>", "").Replace("</h1>", "");

            MatchCollection bodyMatchs = xs.Matches("<div id=\"content\">(.|\\s)*?</div>");
            string body = bodyMatchs[0].Value.Replace("<div id=\"content\">", "").Replace("</div>", "")
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("<br>", Environment.NewLine)
                .Replace("&nbsp;", " ")
                .Replace("<script>chaptererror();</script>", "");

            if (body.LastIndexOf("【作者有话说】") != -1) body = body.Remove(body.LastIndexOf("【作者有话说】"));

            return title + Environment.NewLine + body;
        }

        #endregion BiQuge

        #region 中文数字转阿拉伯数字

        /// <summary>
        /// 转换数字
        /// </summary>
        protected static long CharToNumber(char c)
        {
            switch (c)
            {
                case '一': return 1;
                case '二': return 2;
                case '三': return 3;
                case '四': return 4;
                case '五': return 5;
                case '六': return 6;
                case '七': return 7;
                case '八': return 8;
                case '九': return 9;
                case '零': return 0;
                default: return -1;
            }
        }

        /// <summary>
        /// 转换单位
        /// </summary>
        protected static long CharToUnit(char c)
        {
            switch (c)
            {
                case '十': return 10;
                case '百': return 100;
                case '千': return 1000;
                case '万': return 10000;
                case '亿': return 100000000;
                default: return 1;
            }
        }

        /// <summary>
        /// 将中文数字转换阿拉伯数字
        /// </summary>
        /// <param name="cnum">汉字数字</param>
        /// <returns>长整型阿拉伯数字</returns>
        public static long ParseCnToInt(string cnum)
        {
            cnum = Regex.Replace(cnum, "\\s+", "");
            long firstUnit = 1;//一级单位
            long secondUnit = 1;//二级单位
            long result = 0;//结果
            for (var i = cnum.Length - 1; i > -1; --i)//从低到高位依次处理
            {
                var tmpUnit = CharToUnit(cnum[i]);//临时单位变量
                if (tmpUnit > firstUnit)//判断此位是数字还是单位
                {
                    firstUnit = tmpUnit;//是的话就赋值,以备下次循环使用
                    secondUnit = 1;
                    if (i == 0)//处理如果是"十","十一"这样的开头的
                    {
                        result += firstUnit * secondUnit;
                    }
                    continue;//结束本次循环
                }
                if (tmpUnit > secondUnit)
                {
                    secondUnit = tmpUnit;
                    continue;
                }
                result += firstUnit * secondUnit * CharToNumber(cnum[i]);//如果是数字,则和单位想乘然后存到结果里
            }
            return result;
        }

        #endregion 中文数字转阿拉伯数字
    }
}