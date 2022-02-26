using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class DBConfigInfo
    {
        /// <summary>
        /// 是否是多库
        /// </summary>
        public static bool IsMulti { get; set; } = false;

        ////默认库ID
        public static string DefaultDbID { get; set; } = "";

        /// <summary>
        /// 数据库连接信息
        /// （默认库，正常库）
        /// </summary>
        public static (MutiDBOperate, List<MutiDBOperate>) MutiConnectionString { get; } = MutiInitConn();

        /// <summary>
        /// 获取数据库信息
        /// </summary>
        /// <returns></returns>
        public static (MutiDBOperate, List<MutiDBOperate>) MutiInitConn()
        {
            //得到所有开启的数据库
            List<MutiDBOperate> listDataConfig = AppConfig.GetNode<List<MutiDBOperate>>("DBConfig", "DBS").Where(m => m.Enabled).ToList();
            if (listDataConfig == null || listDataConfig.Count < 1) throw new Exception("请至少开启一个数据库");

            //默认库
            MutiDBOperate mainSimpleDB = listDataConfig.FirstOrDefault(d => d.ConnId == AppConfig.GetNode("DBConfig", "DefaultDB"));
            if (mainSimpleDB == null)
            {
                mainSimpleDB = listDataConfig.FirstOrDefault();
            }
            if (mainSimpleDB.SlaveLibraries != null)
            {
                mainSimpleDB.SlaveLibraries = mainSimpleDB.SlaveLibraries.Where(s => s.Enabled).ToList();//读库信息
                mainSimpleDB.SlaveLibraries.ForEach(s => s.DbType = mainSimpleDB.DbType);//所有的读库数据库类型和主库一样
            }
            DefaultDbID = mainSimpleDB.ConnId;

            //正常库
            List<MutiDBOperate> listdatabaseSimpleDB = listDataConfig.Where(d => d.ConnId != mainSimpleDB.ConnId).ToList();
            listdatabaseSimpleDB.ForEach(d => //分别获取读库
            {
                if (d.SlaveLibraries != null)
                {
                    d.SlaveLibraries = d.SlaveLibraries.Where(s => s.Enabled).ToList();
                    d.SlaveLibraries.ForEach(s => s.DbType = d.DbType);//所有的读库数据库类型和主库一样
                }
            });

            //如果有正常库，就是多库
            if (listdatabaseSimpleDB.Count > 0) IsMulti = true;

            return (mainSimpleDB, listdatabaseSimpleDB);
        }
    }

    /// <summary>
    /// 数据库链接配置类
    /// </summary>
    public class MutiDBOperate
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnId { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 读库命中率，值越大命中率越高
        /// </summary>
        public int HitRate { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 读库信息
        /// </summary>
        public List<MutiDBOperate> SlaveLibraries { get; set; }
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataBaseType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4
    }
}