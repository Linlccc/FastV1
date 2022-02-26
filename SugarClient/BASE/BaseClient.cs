using Attributes.SqlSugar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SqlSugar
{
    /// <summary>
    /// 仓储【同步】
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class BaseClient<TEntity> : IBaseClient<TEntity> where TEntity : class, new()
    {
        #region 构造方法，私有/内部变量
        private readonly ISqlSugarClient _sqlSugarClient;

        internal ISqlSugarClient Db
        {
            get
            {
                //如果没开多库直接返回
                if (!DBConfigInfo.IsMulti) return _sqlSugarClient;

                if (typeof(TEntity).GetCustomAttributes(typeof(ConnectAttribute), false).FirstOrDefault() is ConnectAttribute sugarConnect && !string.IsNullOrWhiteSpace(sugarConnect.Connid))
                    (_sqlSugarClient as SqlSugarClient).ChangeDatabase(sugarConnect.Connid.ToLower());
                else
                    (_sqlSugarClient as SqlSugarClient).ChangeDatabase(DBConfigInfo.DefaultDbID.ToLower());//使用默认库

                return _sqlSugarClient;
            }
        }

        public BaseClient(IUnitOfWork unitOfWork)
        {
            _sqlSugarClient = unitOfWork.GetDbClient();
        }

        #endregion 构造方法，私有/内部变量

        #region 向外放出数据操作对象
        public ISqlSugarClient SugarClient => Db;

        public IBaseClient<TDbEntity> EntityDb<TDbEntity>() where TDbEntity : class, new() => AppConfig.RootServices.GetService<IBaseClient<TDbEntity>>();

        #endregion 向外放出数据操作对象

        #region 查询

        public TEntity QueryById(dynamic id) => Db.Queryable<TEntity>().InSingle(id);

        public TEntity Single(Expression<Func<TEntity, bool>> whereExpression) => Db.Queryable<TEntity>().Single(whereExpression);

        public TEntity Max(Expression<Func<TEntity, object>> orderExpression) => Db.Queryable<TEntity>().OrderBy(orderExpression, OrderByType.Desc).First();

        public TEntity Min(Expression<Func<TEntity, object>> orderExpression) => Db.Queryable<TEntity>().OrderBy(orderExpression, OrderByType.Asc).First();

        public List<TEntity> Query() => Db.Queryable<TEntity>().ToList();

        public List<TEntity> Query(Expression<Func<TEntity, bool>> whereExpression) => Db.Queryable<TEntity>().Where(whereExpression).ToList();

        public List<TEntity> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType, int top) => Db.Queryable<TEntity>().Where(whereExpression).OrderBy(orderByExpression, orderByType).Take(top).ToList();

        public PageMsg<TEntity> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize)
        {
            int totalDataCount = 0, totalPageCount = 0;
            PageMsg<TEntity> result = new(pageIndex, pageSize);
            result.Data = Db.Queryable<TEntity>().Where(whereExpression).ToPageList(pageIndex, pageSize, ref totalDataCount, ref totalPageCount);
            result.TotalDataCount = totalDataCount;
            result.TotalPageCount = totalPageCount;
            return result;
        }

        public PageMsg<TEntity> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType = OrderByType.Asc)
        {
            int totalDataCount = 0, totalPageCount = 0;
            PageMsg<TEntity> result = new(pageIndex, pageSize);
            result.Data = Db.Queryable<TEntity>().Where(whereExpression).OrderBy(orderByExpression, orderByType).ToPageList(pageIndex, pageSize, ref totalDataCount, ref totalPageCount);
            result.TotalDataCount = totalDataCount;
            result.TotalPageCount = totalPageCount;
            return result;
        }

        public bool IsAny(Expression<Func<TEntity, bool>> whereExpression) => Db.Queryable<TEntity>().Any(whereExpression);

        public int Count(Expression<Func<TEntity, bool>> whereExpression) => Db.Queryable<TEntity>().Count(whereExpression);

        #endregion 查询

        #region 插入

        public bool Insert(TEntity insertObj) => Db.Insertable(insertObj).ExecuteCommandIdentityIntoEntity();

        public bool InsertRange(List<TEntity> insertObjs) => Db.Insertable(insertObjs).ExecuteCommandIdentityIntoEntity();

        public bool InsertRange(TEntity[] insertObjs) => Db.Insertable(insertObjs).ExecuteCommandIdentityIntoEntity();

        public int InsertReturnIdentity(TEntity insertObj) => Db.Insertable(insertObj).ExecuteReturnIdentity();

        public long InsertReturnBigIdentity(TEntity insertObj) => Db.Insertable(insertObj).ExecuteReturnBigIdentity();

        #endregion 插入

        #region 删除

        public bool Delete(Expression<Func<TEntity, bool>> whereExpression) => Db.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandHasChange();

        public bool Delete(TEntity deleteObj) => Db.Deleteable(deleteObj).ExecuteCommandHasChange();

        public bool DeleteById(dynamic id) => Db.Deleteable<TEntity>().In(id).ExecuteCommandHasChange();

        public bool DeleteByIds(dynamic[] ids) => Db.Deleteable<TEntity>().In(ids).ExecuteCommandHasChange();

        #endregion 删除

        #region 修改

        public bool Update(List<TEntity> updateObjs) => Db.Updateable(updateObjs).ExecuteCommandHasChange();

        public bool Update(params TEntity[] updateObjs) => Db.Updateable(updateObjs).ExecuteCommandHasChange();

        public bool Update(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression) => Db.Updateable(columns).Where(whereExpression).ExecuteCommandHasChange();

        public bool UpdateIgnore(TEntity updateObj, Expression<Func<TEntity, object>> ignoreColumns) => Db.Updateable(updateObj).IgnoreColumns(ignoreColumns).ExecuteCommandHasChange();

        public bool UpdateIgnore(TEntity[] updateObjs, Expression<Func<TEntity, object>> ignoreColumns) => Db.Updateable(updateObjs).IgnoreColumns(ignoreColumns).ExecuteCommandHasChange();

        public bool UpdateColumns(TEntity updateObj, Expression<Func<TEntity, object>> updateColumns) => Db.Updateable(updateObj).UpdateColumns(updateColumns).ExecuteCommandHasChange();

        public bool UpdateColumns(TEntity[] updateObjs, Expression<Func<TEntity, object>> updateColumns) => Db.Updateable(updateObjs).UpdateColumns(updateColumns).ExecuteCommandHasChange();

        public bool SetColumn(Expression<Func<TEntity, bool>> column, Expression<Func<TEntity, bool>> expression) => Db.Updateable(column).Where(expression).ExecuteCommandHasChange();

        #endregion 修改

        #region 高级保存

        public bool Storageable(params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList()).Saveable().ToStorage();
            int insertNum = storageable.AsInsertable.ExecuteCommand();//插入数量
            int upadteNum = storageable.AsUpdateable.ExecuteCommand();//更新数量
            if (insertNum + upadteNum > 0) return true;
            return false;
        }

        public bool Storageable(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList())
                .SplitInsert(insertConditions)
                .SplitUpdate(updateConditions)
                .ToStorage();
            int insertNum = storageable.AsInsertable.ExecuteCommand();//插入数量
            int upadteNum = storageable.AsUpdateable.ExecuteCommand();//更新数量
            if (insertNum + upadteNum > 0) return true;
            return false;
        }

        public int StorageableReturnNum(params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList()).Saveable().ToStorage();
            int insertNum = storageable.AsInsertable.ExecuteCommand();//插入数量
            int upadteNum = storageable.AsUpdateable.ExecuteCommand();//更新数量
            return insertNum + upadteNum;
        }

        public int StorageableReturnNum(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList())
                .SplitInsert(insertConditions)
                .SplitUpdate(updateConditions)
                .ToStorage();
            int insertNum = storageable.AsInsertable.ExecuteCommand();//插入数量
            int upadteNum = storageable.AsUpdateable.ExecuteCommand();//更新数量
            return insertNum + upadteNum;
        }

        #endregion 高级保存
    }
}