using SqlSugar;
using System;

namespace Model.BaseModels
{
    /// <summary>
    /// 任务类
    /// </summary>
    public class BaseTasks : RootEntity
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200)]
        public string Name { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务所在DLL对应的程序集名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200)]
        public string NameSpace { get; set; }

        /// <summary>
        /// 任务所在类
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200)]
        public string ClassName { get; set; }

        /// <summary>
        /// 执行传参
        /// </summary>
        public string JobParams { get; set; }

        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsStart { get; set; } = false;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int RunCounts { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 1000, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 任务执行日志
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 10000, IsNullable = true)]
        public string Log { get; set; }

        /// <summary>
        /// 触发器类型（0、simple 1、cron）
        /// </summary>
        public TriggerType TriggerType { get; set; }

        /// <summary>
        /// 任务运行时间表达式
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar", Length = 200, IsNullable = true)]
        public string Cron { get; set; }

        /// <summary>
        /// 执行间隔时间, 秒为单位
        /// </summary>
        public int IntervalSecond { get; set; }
    }

    /// <summary>
    /// 任务时间触发器类型
    /// </summary>
    public enum TriggerType
    {
        Simple,
        Cron
    }
}