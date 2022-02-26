using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model.ViewModels;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 服务
    /// </summary>
    [ApiController, CustomRoute]
    public class ServiceController : Controller
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ServiceController(ILogger<ServiceController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        public Msg<SystemInfo> GetSystemInfo()
        {
            SystemInfo systemInfo = new()
            {
                OSVersion = Environment.OSVersion.ToString(),
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                MachineName = Environment.MachineName,
                UserDomainName = Environment.UserDomainName,
                LogicalDrives = Environment.GetLogicalDrives(),
                SystemDirectory = Environment.SystemDirectory,
                RunTimes = Environment.TickCount / 1000,
                UserInteractive = Environment.UserInteractive,
                UserName = Environment.UserName,
                Version = Environment.Version.ToString(),
            };
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables()) systemInfo.EnvironmentVariables.Add(item.Key.ToString(), item.Value.ToString());

            return MsgHelper.Success(systemInfo);
        }

        /// <summary>
        /// 获取app信息
        /// </summary>
        /// <returns></returns>
        public Msg<AppInfo> GetAppInfo()
        {
            Process currentProcess = Process.GetCurrentProcess();
            AppInfo appInfo = new()
            {
                Is64BitProcess = Environment.Is64BitProcess,
                EnvironmentName = _environment.EnvironmentName,
                AppRootPath = _environment.ContentRootPath,
                WebRootPath = _environment.WebRootPath,
                FrameworkDescription = RuntimeInformation.FrameworkDescription,
                WorkingSet = $"{currentProcess.WorkingSet64 / (1024 * 1024):N2} MB",
                WorkingTime = DateTime.Now.TimeSubTract(currentProcess.StartTime),
                UseCpuTime = currentProcess.TotalProcessorTime.TotalMilliseconds + "ms",
            };
            return MsgHelper.Success(appInfo);
        }
    }
}