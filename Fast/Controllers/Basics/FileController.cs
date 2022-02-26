using FastTool.GlobalVar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fast.Controllers.Basics
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    [ApiController, CustomRoute]
    public class FileController : Controller
    {
        /// <summary>
        /// 上传文件的临时文件夹
        /// </summary>
        public string TemporaryUpFolderPath = Path.Combine(FileWellFolderInfo.WebRootPath, FileWellFolderInfo.UpFileFolder);

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [RequestSizeLimit(1024 * 1024 * 5)]
        public async Task<Msg<string>> UpImg(IFormFile file)
        {
            string[] allowType = new string[] { "image/jpg", "image/png", "image/jpeg", "image/gif" };
            if (file == null || !allowType.Contains(file.ContentType)) return MsgHelper.Fail<string>("请上传正确的文件");

            string fileName = (await SaveFiles(file)).First().Replace(FileWellFolderInfo.WebRootPath, "");

            return MsgHelper.Success(fileName);
        }

        #region 帮助方法

        /// <summary>
        /// 保存文件集合
        /// </summary>
        /// <param name="files">要保存的文件</param>
        /// <returns>文件的名称集合</returns>
        private async Task<List<string>> SaveFiles(params IFormFile[] files)
        {
            List<string> filePaths = new();
            if (!Directory.Exists(TemporaryUpFolderPath)) Directory.CreateDirectory(TemporaryUpFolderPath);
            foreach (IFormFile file in files)
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().ToString() + FileHelper.GetPostfix(file.FileName);
                string fullName = Path.Combine(TemporaryUpFolderPath, fileName);
                using FileStream stream = new FileStream(fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                await file.CopyToAsync(stream);
                filePaths.Add(fullName);
            }
            return filePaths;
        }

        #endregion 帮助方法
    }
}