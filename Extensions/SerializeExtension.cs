using Newtonsoft.Json;
using System.Text;

namespace System
{
    /// <summary>
    /// 序列化 拓展
    /// </summary>
    public static class SerializeExtension
    {
        /// <summary>
        /// 将对象序列化成 字符串
        /// </summary>
        /// <param name="item"></param>
        /// <returns>字节数组</returns>
        public static string Serialize(this object obj) => JsonConvert.SerializeObject(obj);

        /// <summary>
        /// 将对象序列化成 字节数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>字节数组</returns>
        public static byte[] SerializeToByte(this object obj) => Encoding.UTF8.GetBytes(obj.Serialize());

        /// <summary>
        /// 将字符串反序列化成对象
        /// </summary>
        /// <typeparam name="TEntity">指定对象类型</typeparam>
        /// <param name="bytes">字节数组</param>
        /// <returns>对象值</returns>
        public static TEntity DeSerialize<TEntity>(this string value) => value.IsNull() ? default : JsonConvert.DeserializeObject<TEntity>(value);

        /// <summary>
        /// 将字节数组反序列化成对象
        /// </summary>
        /// <typeparam name="TEntity">指定对象类型</typeparam>
        /// <param name="bytes">字节数组</param>
        /// <returns>对象值</returns>
        public static TEntity DeSerializeFromByte<TEntity>(this byte[] bytes) => bytes == null || bytes.Length == 0 ? default : Encoding.UTF8.GetString(bytes).DeSerialize<TEntity>();
    }
}