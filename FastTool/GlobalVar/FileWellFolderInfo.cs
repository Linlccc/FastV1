using System.IO;

namespace FastTool.GlobalVar
{
    /// <summary>
    /// 文件及文件夹信息
    /// </summary>
    public static class FileWellFolderInfo
    {
        /// <summary>
        ///  wwwroot 文件夹位置
        /// </summary>
        public static string WebRootPath { get; set; }

        /// <summary>
        /// 上传文件的临时文件夹
        /// </summary>
        public static string UpFileFolder { get; } = "UpFiles";

        /// <summary>
        /// 用户头像文件夹
        /// </summary>
        public static string UserAvatarFolder { get; } = Path.Combine("Img", "UserAvatars");
    }
}