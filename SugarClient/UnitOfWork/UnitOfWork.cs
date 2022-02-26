using System;

namespace SqlSugar
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private int TranCount { get; set; }

        public UnitOfWork(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
            TranCount = 0;
        }

        /// <summary>
        /// 获取DB，保证唯一性
        /// </summary>
        /// <returns></returns>
        public ISqlSugarClient GetDbClient() => _sqlSugarClient;

        public void BeginTran()
        {
            lock (this)
            {
                TranCount++;
                (GetDbClient() as SqlSugarClient).BeginTran();
            }
        }

        public void CommitTran()
        {
            lock (this)
            {
                TranCount--;
                if (TranCount == 0)
                {
                    try
                    {
                        (GetDbClient() as SqlSugarClient).CommitTran();
                    }
                    catch (Exception)
                    {
                        (GetDbClient() as SqlSugarClient).RollbackTran();
                        throw;
                    }
                }
            }
        }

        public void RollbackTran()
        {
            lock (this)
            {
                TranCount--;
                (GetDbClient() as SqlSugarClient).RollbackTran();
            }
        }
    }
}