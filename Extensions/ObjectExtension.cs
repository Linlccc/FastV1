using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace System
{
    /// <summary>
    /// object 拓展
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 获取 object 指定值
        /// </summary>
        /// <param name="obj">object 数据对象</param>
        /// <param name="fieldName">指定字段</param>
        /// <returns></returns>
        public static object GetOValue(this object obj, string fieldName)
        {
            if (obj == null) return null;

            JObject o;
            if (obj.GetType() == typeof(JObject)) o = (JObject)obj;
            else o = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj));

            return o.GetValue(fieldName);
        }

        /// <summary>
        /// 将object 对象转换成可动态操作对象
        /// 使用 dynamic 接收转换后的对象
        /// 直接.key 获取值
        /// .key = value,设置值，或动态添加值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpandoObject(this object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            if (obj.GetType().IsAssignableFrom(typeof(JObject)))
            {
                IDictionary<string, JToken> jobjData = (IDictionary<string, JToken>)obj;
                foreach (KeyValuePair<string, JToken> item in jobjData)
                {
                    if (item.Value.GetType().IsAssignableFrom(typeof(JObject)))
                        expando.Add(item.Key, item.Value.ToExpandoObject());
                    else
                        expando.Add(item.Key, item.Value.OToString());
                }
            }
            else
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
                {
                    object value = property.GetValue(obj);
                    if (value.GetType() == typeof(object))
                        expando.Add(property.Name, value.ToExpandoObject());
                    else
                        expando.Add(property.Name, value.OToString());
                }
            }
            return (ExpandoObject)expando;
        }

        /// <summary>
        /// 当前对象是否是或继承自指定类型
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="type">类型</param>
        /// <returns>继承自指定类型返回true</returns>
        public static bool IsAssignableTo(this object obj, Type type) => obj.GetType().IsAssignableTo(type);

        #region Check Null

        /// <summary>
        /// 是空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(this object obj) => obj.OToString().Trim() == "";

        /// <summary>
        /// 是空
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool IsNull(this IEnumerable<object> objs) => objs == null || !objs.Any();

        /// <summary>
        /// 不是空
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNNull(this object obj) => !obj.IsNull();

        /// <summary>
        /// 不是空
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool IsNNull(this IEnumerable<object> objs) => !objs.IsNull();

        #endregion Check Null
    }

    /// <summary>
    /// 转换计算 拓展
    /// </summary>
    public static class Util
    {
        #region String

        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 string 类型的值，否则返回errorValue</returns>
        public static string OToString(this object obj, string errorValue = "") => obj == null ? errorValue : obj.ToString();

        #endregion String

        #region Bool

        /// <summary>
        /// object 转 bool
        /// </summary>
        /// <param name="boolStr"></param>
        /// <returns>转换失败返回false</returns>
        public static bool OToBool(this string boolStr) => bool.TryParse(boolStr, out bool result) && result;

        #endregion Bool

        #region Int

        /// <summary>
        /// 获取数字字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetNumberString(this string str)
        {
            int subscript = str.LastIndexOf('.');
            if (subscript != -1) str = str[0..subscript];
            return str;
        }

        /// <summary>
        /// object 转 int16
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 int16 类型的值，否则返回errorValue</returns>
        public static short OToInt16(this string str, short errorValue = 0) => str != null && short.TryParse(str.GetNumberString(), out short result) ? result : errorValue;

        /// <summary>
        /// object 转 uint16
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 uint16 类型的值，否则返回errorValue</returns>
        public static ushort OToUInt16(this string str, ushort errorValue = 0) => str != null && ushort.TryParse(str.GetNumberString(), out ushort result) ? result : errorValue;

        /// <summary>
        /// object 转 int
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 int 类型的值，否则返回errorValue</returns>
        public static int OToInt(this string str, int errorValue = 0) => str != null && int.TryParse(str.GetNumberString(), out int result) ? result : errorValue;

        /// <summary>
        /// object 转 long
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 long 类型的值，否则返回errorValue</returns>
        public static long OToLong(this string str, long errorValue = 0) => str != null && long.TryParse(str.GetNumberString(), out long result) ? result : errorValue;

        #endregion Int

        #region Double & Decimal

        /// <summary>
        /// object 转 Dounle
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 Double 类型的值，否则返回errorValue</returns>
        public static double OToDouble(this string str, double errorValue = 0) => str != null && double.TryParse(str, out double result) ? result : errorValue;

        /// <summary>
        /// object 转 Decimal
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 Decimal 类型的值，否则返回errorValue</returns>
        public static decimal OToDecimal(this string str, decimal errorValue = 0) => str != null && decimal.TryParse(str, out decimal result) ? result : errorValue;

        #endregion Double & Decimal

        #region DeleTime

        /// <summary>
        /// object 转 Datetime
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorValue">转换失败返回的值</param>
        /// <returns>成功转换返回 Datetime 类型的值，否则返回errorValue</returns>
        public static DateTime OToDateTime(this string str, DateTime errorValue) => str != null && DateTime.TryParse(str, out DateTime result) ? result : errorValue;

        /// <summary>
        /// object 转 Datetime
        /// </summary>
        /// <param name="str"></param>
        /// <returns>转换失败返回最小时间</returns>
        public static DateTime OToDateTime(this string str) => OToDateTime(str, DateTime.MinValue);

        #endregion DeleTime
    }
}