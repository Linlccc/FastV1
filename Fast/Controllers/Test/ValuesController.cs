using FastTool.SuperObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model.BaseModels;
using Model.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fast.Controllers.Test
{
    [ApiController, CustomRoute(ApiGroup.Test)]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly IFeaturesClient _featuresClient;

        public ValuesController(ILogger<ValuesController> logger, IUnitOfWork unitOfWork, ISqlSugarClient sqlSugarClient, IFeaturesClient featuresClient)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _sqlSugarClient = sqlSugarClient;
            _featuresClient = featuresClient;
        }

        [HttpPost]
        public void T4(object a0)
        {
            string v1 = a0.GetOValue("a").ToString();

            dynamic c1 = a0.SoToExpandoObject();

            var vv1 = c1.a;
            var vv2 = c1.b.c.aa;
            c1.b.d = "aa";
            c1.v.d = "aa";
            c1.name = "asdasd";
            var vv3 = c1["age"];

            object a1 = new
            {
                a = "asdasd",
                b = new { c = "ss" }
            };

            int? d1 = 5;
            if (d1 is not null)
                d1 = 6;
            var d11 = d1 switch
            {
                > 2 and < 10 => 5,
                _ => throw new Exception()
            };

            string d2 = null;
            if (d2 is null)
                d2 = "";

            UserRole userRole = new UserRole()
            {
                RoleId = 1,
                UserId = 1
            };

            object u1 = userRole switch
            {
                { RoleId: > 0, UserId: > 0 } => 1,
                UserRole { RoleId: > 10 } => 2,
                RootEntity r => r.GetType(),
                _ => 0
            };

            var c2 = a1.SoToExpandoObject();

            string v2 = a1.GetOValue("a").ToString();
        }

        [HttpPost]
        public string T5(string a)
        {
            //_testDb2.Insert(new DBTestModel() { });

            string data = "111111111111";

            ////Request.EnableBuffering();可以实现多次读取Body
            ////Request.Body.Position = 0;
            //StreamReader sr = new StreamReader(Request.Body);
            //data = sr.ReadToEnd();

            ////再次读取 依然可以成功读到
            ////Request.Body.Seek(0, SeekOrigin.Begin);
            //StreamReader sr2 = new StreamReader(Request.Body);
            //data += sr2.ReadToEnd();

            //foreach (var item in Request.Headers)
            //{
            //    data += $"{item.Key}:{item.Value}{Environment.NewLine}";
            //}

            //var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            //if (string.IsNullOrEmpty(ip))
            //{
            //    //ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            //    //ip = Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
            //    data += Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            //}

            return data;
        }

        [HttpGet]
        public async Task<string> T6()
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            var t1 = new Task(() =>
           {
               Parallel.For(0, 10, i =>
               {
                   Console.WriteLine("1\t" + i + "\t" + Thread.CurrentThread.ManagedThreadId);
                   Thread.Sleep(1000);
               });
           }).GetWorkTimeAsync();
            var t2 = new Task(() =>
            {
                Parallel.For(0, 10, i =>
                {
                    Console.WriteLine("2\t" + i + "\t" + Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1000);
                });
            }).GetWorkTimeAsync();

            var sw5 = await new Task<int>(() =>
            {
                Parallel.For(0, 10, i =>
                {
                    Console.WriteLine("2\t" + i + "\t" + Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1000);
                });
                return 1;
            }).GetWorkTimeAsync();

            var sw4 = await Task.Run(() =>
            {
                long a = 0;
                var b = new Random();
                Parallel.For(0, 10, i =>
                {
                    a += b.Next(234234234, 234234299);
                    Console.WriteLine("3\t" + i + "\t" + Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1000);
                });
                return a;
            }).GetWorkTimeAsync();

            Stopwatch sw2 = (await t1).Item1;
            Stopwatch sw3 = (await t2).Item1;

            sw.Stop();
            long a1 = sw.ElapsedMilliseconds;
            long a2 = sw2.ElapsedMilliseconds;
            long a3 = sw3.ElapsedMilliseconds;
            long a4 = sw4.Item1.ElapsedMilliseconds;
            long a5 = sw5.Item1.ElapsedMilliseconds;

            return $"{a1}{Environment.NewLine}{a2}{Environment.NewLine}{a3}{Environment.NewLine}{a4}{Environment.NewLine}{a5}{Environment.NewLine}{sw4.Item2}";
        }

        [HttpGet]
        public object T8<T>(Expression<Func<T, object>> func) where T : new()
        {
            var a = func;
            return a;
        }

        [HttpGet]
        public void T9()
        {
            string a1 = "";
            string a2 = "      ";
            object a3 = null;
            User a4 = null;
            List<object> a6 = null;
            List<User> a7 = null;
            object a5 = new object();
            List<object> a8 = new List<object>();
            List<User> a9 = new List<User>();

            bool v1 = a1.IsNull();
            bool v2 = a2.IsNull();
            bool v3 = a3.IsNull();
            bool v4 = a4.IsNull();
            bool v5 = a5.IsNull();
            bool v6 = a6.IsNull();
            bool v7 = a7.IsNull();
            bool v8 = a8.IsNull();
            bool v9 = a9.IsNull();
        }

        [HttpGet]
        public async Task T10(long id = 0)
        {
            await _featuresClient.RecursiveDeleteById(id);
        }

        [HttpGet]
        public string T11()
        {
            return AppConfig.GetNode("DBConfig", "UpdateDb");
        }

        [HttpGet]
        public void Ex()
        {
            throw new Exception("测试报错");
        }

        public void SuperObjectTest()
        {
            dynamic superO = new SuperObject();

            superO.aa = new { cc = "cc" };
            superO["bb"] = "22";
            var a = superO.GetDynamicMemberNames();
            string v1 = superO.ToString();
            //superO["aa"]["cc"] = "hhh";

            var v2 = superO["aa"];
            var v3 = superO.aa;
            var v4 = superO.aa.cc;

            superO.aa.cc = "55";
            superO["aa"]["cc"] = new { bb = "66" };
            superO.aa.cc.bb = "77";
            superO["aa"]["cc"]["bb"] = "88";
            superO["aa"]["cc"].bb = "hhh";
            v1 = superO.ToString();

            User user = new User()
            {
                AdminSignalrConnectionId = "1",
                Avatar = "asd",
                CreateTime = DateTime.Now,
                Enabled = false,
                ErrorCount = 5,
                Id = 4,
                LastLoginTime = DateTime.Now,
                LoginName = "asd",
                LoginPWD = "gsdf",
                NickName = "asgdfg",
                PlaintextPwd = "gjfrt",
                Remark = "wrwe",
                UpdateTime = DateTime.Now
            };
            dynamic vva = SuperObject.Parse(user);
            var vv1 = vva["AdminSignalrConnectionId"];
            vva["AdminSignalrConnectionId"] = "5555";
            vv1 = vva["AdminSignalrConnectionId"];
            vva["Avatar"] = new
            {
                Enabled = false,
                ErrorCount = 5,
                Id = 4,
            };
            var vv2 = vva.Avatar;
            var vv3 = vva.ToString();
        }
    }
}