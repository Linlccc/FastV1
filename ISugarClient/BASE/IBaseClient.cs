using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlSugar
{
    /// <summary>
    /// 仓储接口【同步】
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial interface IBaseClient<TEntity> where TEntity : class, new()
    {
        #region 向外放出数据操作对象

        /// <summary>
        /// 获取数据连接对象
        /// </summary>
        ISqlSugarClient SugarClient { get; }

        /// <summary>
        /// 获取指定实体数据操作对象,和注入的不在同一范围
        /// </summary>
        /// <typeparam name="TDbEntity">实体</typeparam>
        /// <returns></returns>
        IBaseClient<TDbEntity> EntityDb<TDbEntity>() where TDbEntity : class, new();

        #endregion 向外放出数据操作对象

        #region 查询

        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>一或零条数据</returns>
        TEntity QueryById(dynamic id);

        /// <summary>
        /// 根据条件获取一条数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>一或零条数据</returns>
        TEntity Single(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 按指定条件获取最大的一条纪录
        /// </summary>
        /// <param name="orderExpression">条件</param>
        /// <returns>一或零条数据</returns>
        TEntity Max(Expression<Func<TEntity, object>> orderExpression);

        /// <summary>
        /// 按指定条件获取最小的一条纪录
        /// </summary>
        /// <param name="orderExpression">条件</param>
        /// <returns>一或零条数据</returns>
        TEntity Min(Expression<Func<TEntity, object>> orderExpression);

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <returns>所有数据</returns>
        List<TEntity> Query();

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>符合条件的数据</returns>
        List<TEntity> Query(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 获取指定条件，指定排序方式的n条数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="orderByExpression">排序条件</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="top">前几条</param>
        /// <returns></returns>
        List<TEntity> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType, int top);

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">一页数据量</param>
        /// <returns>当前页详细信息</returns>
        PageMsg<TEntity> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize);

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">一页数据量</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序方式</param>
        /// <returns>当前页详细信息</returns>
        PageMsg<TEntity> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType = OrderByType.Asc);

        /// <summary>
        /// 查询指定条件是否有数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>有数据返回true，否则false</returns>
        bool IsAny(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 查询指定条件数据条数
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>数据条数</returns>
        int Count(Expression<Func<TEntity, bool>> whereExpression);

        #endregion 查询

        #region 插入

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="insertObj">数据</param>
        /// <returns>是否成功</returns>
        bool Insert(TEntity insertObj);

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="insertObj">数据</param>
        /// <returns>自增id</returns>
        int InsertReturnIdentity(TEntity insertObj);

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="insertObj">数据</param>
        /// <returns>自增id</returns>
        long InsertReturnBigIdentity(TEntity insertObj);

        /// <summary>
        /// 插入一个集合数据
        /// </summary>
        /// <param name="insertObjs">数据集合</param>
        /// <returns>是否成功</returns>
        bool InsertRange(List<TEntity> insertObjs);

        /// <summary>
        /// 插入一个数组数据
        /// </summary>
        /// <param name="insertObjs">数据数组</param>
        /// <returns>是否成功</returns>
        bool InsertRange(TEntity[] insertObjs);

        #endregion 插入

        #region 删除

        /// <summary>
        /// 根据id删除数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        bool DeleteById(dynamic id);

        /// <summary>
        /// 根据id集合删除数据
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns>是否成功</returns>
        bool DeleteByIds(dynamic[] ids);

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>是否成功</returns>
        bool Delete(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 删除指定数据
        /// </summary>
        /// <param name="deleteObj">数据</param>
        /// <returns>是否成功</returns>
        bool Delete(TEntity deleteObj);

        #endregion 删除

        #region 更新

        /// <summary>
        /// 根据数据集合
        /// </summary>
        /// <param name="updateObjs">数据集合</param>
        /// <returns>是否成功</returns>
        bool Update(List<TEntity> updateObjs);

        /// <summary>
        /// 根据数据数组
        /// </summary>
        /// <param name="updateObjs">数据数组</param>
        /// <returns>是否成功</returns>
        bool Update(params TEntity[] updateObjs);

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <param name="columns">要更新的数据</param>
        /// <param name="whereExpression">更新条件</param>
        /// <returns>是否成功</returns>
        bool Update(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 更新数据，忽略指定列
        /// </summary>
        /// <param name="updateObj">数据</param>
        /// <param name="ignoreColumns">忽略的列</param>
        /// <returns>是否成功</returns>
        bool UpdateIgnore(TEntity updateObj, Expression<Func<TEntity, object>> ignoreColumns);

        /// <summary>
        /// 更新数据数组，忽略指定列
        /// </summary>
        /// <param name="updateObjs">数据数组</param>
        /// <param name="ignoreColumns">忽略的列</param>
        /// <returns>是否成功</returns>
        bool UpdateIgnore(TEntity[] updateObjs, Expression<Func<TEntity, object>> ignoreColumns);

        /// <summary>
        /// 只更新指定列
        /// </summary>
        /// <param name="updateObj">数据</param>
        /// <param name="updateColumns">要更新的列</param>
        /// <returns>是否成功</returns>
        bool UpdateColumns(TEntity updateObj, Expression<Func<TEntity, object>> updateColumns);

        /// <summary>
        /// 只更新指定列
        /// </summary>
        /// <param name="updateObjs">数据数组</param>
        /// <param name="updateColumns">要更新的列</param>
        /// <returns>是否成功</returns>
        bool UpdateColumns(TEntity[] updateObjs, Expression<Func<TEntity, object>> updateColumns);

        /// <summary>
        /// 更新指定的一列
        /// </summary>
        /// <param name="column">要更新的一列</param>
        /// <param name="expression">更新条件</param>
        /// <returns>是否成功</returns>
        bool SetColumn(Expression<Func<TEntity, bool>> column, Expression<Func<TEntity, bool>> expression);

        #endregion 更新

        #region 高级保存

        /// <summary>
        /// 更新或者插入
        /// 根据主键判断，如果数据库存在更新，不存在插入
        /// </summary>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入是否成功</returns>
        bool Storageable(params TEntity[] entities);

        /// <summary>
        /// 更新或者插入
        /// 根据你自己的条件更新或插入
        /// </summary>
        /// <param name="insertConditions">更新条件</param>
        /// <param name="updateConditions">插入条件</param>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入是否成功</returns>
        bool Storageable(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities);

        /// <summary>
        /// 更新或者插入
        /// 根据主键判断，如果数据库存在更新，不存在插入
        /// </summary>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入的条数</returns>
        int StorageableReturnNum(params TEntity[] entities);

        /// <summary>
        /// 更新或者插入
        /// 根据你自己的条件更新或插入
        /// </summary>
        /// <param name="insertConditions">更新条件</param>
        /// <param name="updateConditions">插入条件</param>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入的条数</returns>
        int StorageableReturnNum(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities);

        #endregion 高级保存
    }
}