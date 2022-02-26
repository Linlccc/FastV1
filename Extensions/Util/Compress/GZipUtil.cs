using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace System
{
    /// <summary>
    /// GZip 压缩工具
    /// </summary>
    public class GZipUtil
    {
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="rawString">需要压缩的字符串</param>
        /// <returns>压缩后的 Base64 字符串</returns>
        public static string Compress(string rawString)
        {
            if (rawString.IsNull()) return "";
            byte[] rawBytes = Encoding.UTF8.GetBytes(rawString);

            using MemoryStream ms = new();
            GZipStream gZips = new(ms, CompressionMode.Compress);
            gZips.Write(rawBytes, 0, rawBytes.Length);
            gZips.Close();//关闭后字节信息才在ms里
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="zipString">经过 GZip 压缩后的 Base64 字符串</param>
        /// <returns>解压后的源字符串</returns>
        public static string DeCompress(string zipString)
        {
            if (zipString.IsNull()) return "";
            byte[] zipBtyes = Convert.FromBase64String(zipString);//得到压缩后的字节数组

            using GZipStream deGZips = new(new MemoryStream(zipBtyes), CompressionMode.Decompress);
            List<byte> rawBytes = new();//存放解压后的字节数组
            byte[] bytes = new byte[1024];
            while (true)
            {
                int readLenght = deGZips.Read(bytes, 0, bytes.Length);
                if (readLenght == 0) break;
                rawBytes.AddRange(bytes);
            }
            return Encoding.UTF8.GetString(rawBytes.ToArray());
        }
    }
}