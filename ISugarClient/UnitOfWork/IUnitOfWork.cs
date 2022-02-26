namespace SqlSugar
{
    /// <summary>
    /// 这个一个要作用域注入
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 创建 sqlsugar client 实例
        /// </summary>
        /// <returns></returns>
        ISqlSugarClient GetDbClient();

        /// <summary>
        /// 开始事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTran();
    }
}