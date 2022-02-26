using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FastTool.SuperObject
{
    public class SuperObject : DynamicObject
    {
        public SuperObject()
        {
            XmlElement = new XElement("root", CreateTypeAttr());
        }

        public SuperObject(XElement xElement, NodeType jsonType)
        {
            XmlElement = xElement;
            _type = jsonType;
        }

        #region 内部字段

        /// <summary>
        /// 数据类型
        /// </summary>
        private readonly NodeType _type = NodeType.@object;

        /// <summary>
        /// 要转成字符串的类型
        /// </summary>
        private static readonly Type[] ToBeConvertStringTypes = new[] { typeof(DateTimeOffset) };

        #endregion 内部字段

        public XElement XmlElement { get; private set; }

        public bool IsObject => _type == NodeType.@object;
        public bool IsArray => _type == NodeType.array;

        /// <summary>
        /// 是否包含某个键
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsHav(string name) => IsObject && XmlElement.Element(name).IsNNull();

        /// <summary>
        /// 数组是否包含指定索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsHav(int index) => IsArray && XmlElement.Elements().ElementAtOrDefault(index).IsNNull();

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Delete(string name)
        {
            XElement element = XmlElement.Element(name);
            if (element.IsNull()) return false;
            element.Remove();
            return true;
        }

        /// <summary>
        /// 删除指定索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Delete(int index)
        {
            XElement element = XmlElement.Elements().ElementAtOrDefault(index);
            if (element.IsNull()) return false;
            element.Remove();
            return true;
        }

        #region 重写动态类型方法

        /// <summary>
        /// 获取成员名称
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetDynamicMemberNames() => IsObject ? XmlElement.Elements().Select(e => e.Name.LocalName) : XmlElement.Elements().Select((e, i) => i.ToString());

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return IsObject ? GetValue(XmlElement.Element((string)indexes[0]), out result) : GetValue(XmlElement.Elements().ElementAtOrDefault((int)indexes[0]), out result);
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return IsObject ? GetValue(XmlElement.Element(binder.Name), out result) : GetValue(XmlElement.Elements().ElementAtOrDefault(int.Parse(binder.Name)), out result);
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return IsObject ? SetMember((string)indexes[0], value) : SetIndex((int)indexes[0], value);
        }

        /// <summary>
        /// 设置成员
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return IsObject ? SetMember(binder.Name, value) : SetIndex(int.Parse(binder.Name), value);
        }

        /// <summary>
        /// 对象转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            foreach (XElement element in XmlElement.Descendants().Where(x => x.Attribute("type").Value == "null"))
            {
                element.Remove();
            }
            return CreateJsonString(new XStreamingElement("root", CreateTypeAttr(_type), XmlElement.Elements()));
        }

        #endregion 重写动态类型方法

        #region 内部方法

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SetIndex(int index, object value)
        {
            NodeType jsonType = GetNodeType(value);
            XElement e = XmlElement.Elements().ElementAtOrDefault(index);
            if (e == null) XmlElement.Add(new XElement("item", CreateTypeAttr(GetNodeType(value)), CreateNodeValue(value)));
            else
            {
                e.Attribute("type").Value = jsonType.ToString();
                e.ReplaceNodes(CreateNodeValue(value));
            }
            return true;
        }

        /// <summary>
        /// 设置成员
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SetMember(string name, object value)
        {
            NodeType jsonType = GetNodeType(value);

            if (value is SuperObject so)
            {
                if (so.IsObject) value = value.SoToExpandoObject();
                else if (so.IsArray)
                {
                    List<object> list = new();
                    foreach (var item in (dynamic)so)
                    {
                        list.Add(item is SuperObject ? item.ToExpandoObject() : item);
                    }
                    value = list;
                }
            }

            XElement e = XmlElement.Element(name);
            if (e == null) XmlElement.Add(new XElement(name, CreateTypeAttr(jsonType), CreateNodeValue(value)));
            else
            {
                e.Attribute("type").Value = jsonType.ToString();
                e.ReplaceNodes(CreateNodeValue(value));
            }
            return true;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool GetValue(XElement element, out object result)
        {
            if (element == null)
            {
                result = null;
                return false;
            }

            result = ToValue(element);
            return true;
        }

        #endregion 内部方法

        #region 内部静态方法

        /// <summary>
        /// 创建类型属性
        /// </summary>
        /// <param name="jsonType"></param>
        /// <returns></returns>
        private static XAttribute CreateTypeAttr(NodeType jsonType = NodeType.@object) => new("type", jsonType.ToString());

        /// <summary>
        /// 创建节点值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static object CreateNodeValue(object obj)
        {
            return GetNodeType(obj) switch
            {
                NodeType.@object => CreateXObject(obj),
                NodeType.array => (obj as IEnumerable).Cast<object>().Select(o => new XStreamingElement("item", CreateTypeAttr(GetNodeType(o)), CreateNodeValue(o))),
                NodeType.@string or NodeType.number => obj,
                NodeType.boolean => obj.ToString().ToLower(),
                _ => null
            };

            //对象
            static IEnumerable<XStreamingElement> CreateXObject(object obj)
            {
                if (obj is ExpandoObject expobj) return expobj.Select(o => new XStreamingElement(o.Key, CreateTypeAttr(GetNodeType(o)), CreateNodeValue(o)));
                return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new { p.Name, Value = p.GetValue(obj, null) })
                    .Select(o => new XStreamingElement(o.Name, CreateTypeAttr(GetNodeType(o.Value)), CreateNodeValue(o.Value)));
            }
        }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static NodeType GetNodeType(object obj)
        {
            if (obj == null) return NodeType.@null;

            Type objType = obj.GetType();

            if (ToBeConvertStringTypes.Contains(objType)) return NodeType.@string;

            if (obj is ExpandoObject) return NodeType.@object;

            return Type.GetTypeCode(objType) switch
            {
                TypeCode.Object => obj is IEnumerable ? NodeType.array : NodeType.@object,
                TypeCode.String or TypeCode.Char or TypeCode.DateTime => NodeType.@string,
                TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal or TypeCode.SByte or TypeCode.Byte => NodeType.number,
                TypeCode.Boolean => NodeType.boolean,
                _ => NodeType.@null
            };
        }

        /// <summary>
        /// XElement 转动态类型
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static dynamic ToValue(XElement element)
        {
            NodeType type = (NodeType)Enum.Parse(typeof(NodeType), element.Attribute("type").Value);
            return type switch
            {
                NodeType.@object or NodeType.array => new SuperObject(element, type),
                NodeType.@string => (string)element,
                NodeType.number => (double)element,
                NodeType.boolean => (bool)element,
                _ => null
            };
        }

        /// <summary>
        /// 创建 Json 字符串
        /// </summary>
        /// <param name="xStreaming"></param>
        /// <returns></returns>
        private static string CreateJsonString(XStreamingElement xStreaming)
        {
            using MemoryStream memoryStream = new();
            using XmlDictionaryWriter xmlDictionaryWriter = JsonReaderWriterFactory.CreateJsonWriter(memoryStream, Encoding.Unicode);
            xStreaming.WriteTo(xmlDictionaryWriter);
            xmlDictionaryWriter.Flush();
            return Encoding.Unicode.GetString(memoryStream.ToArray());
        }

        #endregion 内部静态方法

        #region 公开静态

        /// <summary>
        /// 处理已有类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static dynamic Parse(object obj)
        {
            string json = CreateJsonString(new XStreamingElement("root", CreateTypeAttr(GetNodeType(obj)), CreateNodeValue(obj)));
            using XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.Unicode.GetBytes(json), XmlDictionaryReaderQuotas.Max);
            return ToValue(XElement.Load(reader));
        }

        #endregion 公开静态

        public enum NodeType
        {
            @null,
            @object,
            array,
            @string,
            number,
            boolean
        }
    }

    public static class ExpandoObjectExtensions
    {
        public static ExpandoObject SoToExpandoObject(this object obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));

            if (obj is SuperObject so && so.IsObject)
            {
                dynamic expando1 = new ExpandoObject();
                IDictionary<string, object> dic = (IDictionary<string, object>)expando1;
                foreach (KeyValuePair<string, object> item in (dynamic)so)
                {
                    dic.Add(item.Key, item.Value is SuperObject ? item.Value.SoToExpandoObject() : item.Value);
                }
                return expando1;
            }

            if (obj is not ExpandoObject expando)
            {
                expando = new ExpandoObject();
                IDictionary<string, object> dic = expando;
                IDictionary<string, object> dictionary = obj.ToDictionary();
                foreach (KeyValuePair<string, object> item in dictionary)
                {
                    dic.Add(item);
                }
            }
            return expando;
        }
    }

    public static class DictionaryExtensions
    {
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            _ = obj ?? throw new ArgumentNullException(nameof(obj));
            if (obj is IDictionary<string, object> dictionary) return dictionary;

            if (obj is SuperObject so && so.IsObject)
            {
                IDictionary<string, object> dic = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> item in (dynamic)so)
                {
                    dic.Add(item.Key, item.Value is SuperObject ? item.Value.ToDictionary() : item.Value);
                }
                return dic;
            }

            PropertyInfo[] properties = obj.GetType().GetProperties();
            FieldInfo[] fields = obj.GetType().GetFields();
            IEnumerable<MemberInfo> members = properties.Cast<MemberInfo>().Concat(fields.Cast<MemberInfo>());
            return members.ToDictionary(m => m.Name, m => GetValue(obj, m));
        }

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private static object GetValue(object obj, MemberInfo member)
        {
            if (member is PropertyInfo property) return property.GetValue(obj, null);
            if (member is FieldInfo field) return field.GetValue(obj);

            throw new ArgumentException("传递的成员既不是PropertyInfo，也不是FieldInfo");
        }
    }
}