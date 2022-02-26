using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// 仓储接口【异步】
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial interface IBaseClient<TEntity> where TEntity : class, new()
    {
        #region 查询

        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>一或零条数据</returns>
        Task<TEntity> QueryByIdAsync(dynamic id);

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>符合条件的数据</returns>
        Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression = null);

        /// <summary>
        /// 获取指定条件，指定排序方式的n条数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <param name="orderByExpression">排序条件</param>
        /// <param name="orderByType">排序方式</param>
        /// <param name="top">前几条</param>
        /// <returns></returns>
        Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType, int top);

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">一页数据量</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序方式</param>
        /// <returns>当前页详细信息</returns>
        Task<PageMsg<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);

        /// <summary>
        /// 根据条件获取一条数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>一或零条数据</returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 按指定条件获取最大的一条纪录
        /// </summary>
        /// <param name="orderExpression">条件</param>
        /// <returns>一或零条数据</returns>
        Task<TEntity> MaxAsync(Expression<Func<TEntity, object>> orderExpression);

        /// <summary>
        /// 按指定条件获取最大的一条纪录
        /// </summary>
        /// <param name="orderExpression">条件</param>
        /// <returns>一或零条数据</returns>
        Task<TEntity> MinAsync(Expression<Func<TEntity, object>> orderExpression);

        /// <summary>
        /// 查询指定条件是否有数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>有数据返回true，否则false</returns>
        Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> whereExpression = null);

        /// <summary>
        /// 查询指定条件数据条数
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>数据条数</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression = null);

        /// <summary>
        /// 获取最大字段
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldName">获取字段名</param>
        /// <param name="expressions">条件</param>
        /// <returns>该字段最大的值</returns>
        Task<TResult> MaxFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions);

        /// <summary>
        /// 获取最大字段
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldExpression">获取字段</param>
        /// <param name="expressions">条件</param>
        /// <returns>该字段最大的值</returns>
        Task<TResult> MaxFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions);

        /// <summary>
        /// 获取最小字段
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldName">获取字段名</param>
        /// <param name="expressions">条件</param>
        /// <returns>该字段最小的值</returns>
        Task<TResult> MinFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions);

        /// <summary>
        /// 获取最小字段
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldExpression">获取字段</param>
        /// <param name="expressions">条件</param>
        /// <returns>该字段最小的值</returns>
        Task<TResult> MinFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions);

        /// <summary>
        /// 字段求和
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldName">获取字段名</param>
        /// <param name="expressions">条件</param>
        /// <returns>字段和</returns>
        Task<TResult> SumFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions);

        /// <summary>
        /// 字段求和
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldExpression">获取字段</param>
        /// <param name="expressions">条件</param>
        /// <returns>字段和</returns>
        Task<TResult> SumFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions);

        /// <summary>
        /// 字段平均值
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldName">获取字段名</param>
        /// <param name="expressions">条件</param>
        /// <returns>字段平均值</returns>
        Task<TResult> AvgFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions);

        /// <summary>
        /// 字段平均值
        /// </summary>
        /// <typeparam name="TResult">获取值类型</typeparam>
        /// <param name="fieldExpression">获取字段</param>
        /// <param name="expressions">条件</param>
        /// <returns>字段平均值</returns>
        Task<TResult> AvgFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions);

        #endregion 查询

        #region 插入

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="insertObj">数据</param>
        /// <returns>是否成功</returns>
        Task<bool> InsertAsync(TEntity insertObj);

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="insertObj">数据</param>
        /// <returns>自增id</returns>
        Task<int> InsertReturnIdentityAsync(TEntity insertObj);

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="insertObj">数据</param>
        /// <returns>自增id</returns>
        Task<long> InsertReturnBigIdentityAsync(TEntity insertObj);

        /// <summary>
        /// 插入一个集合数据
        /// </summary>
        /// <param name="insertObjs">数据集合</param>
        /// <returns>是否成功</returns>
        Task<bool> InsertRangeAsync(List<TEntity> insertObjs);

        /// <summary>
        /// 插入一个数组数据
        /// </summary>
        /// <param name="insertObjs">数据数组</param>
        /// <returns>是否成功</returns>
        Task<bool> InsertRangeAsync(TEntity[] insertObjs);

        #endregion 插入

        #region 删除

        /// <summary>
        /// 根据id删除数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteByIdAsync(dynamic id);

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="whereExpression">条件</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 删除指定数据
        /// </summary>
        /// <param name="deleteObj">数据</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(TEntity deleteObj);

        #endregion 删除

        #region 更新

        /// <summary>
        /// 根据数据集合
        /// </summary>
        /// <param name="updateObjs">数据集合</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(List<TEntity> updateObjs);

        /// <summary>
        /// 根据数据数组
        /// </summary>
        /// <param name="updateObjs">数据数组</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(params TEntity[] updateObjs);

        /// <summary>
        /// 根据条件更新数据
        /// </summary>
        /// <param name="columns">要更新的数据</param>
        /// <param name="whereExpression">更新条件</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 更新数据，忽略指定列
        /// </summary>
        /// <param name="updateObj">数据</param>
        /// <param name="ignoreColumns">忽略的列</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateIgnoreAsync(TEntity updateObj, Expression<Func<TEntity, object>> ignoreColumns);

        /// <summary>
        /// 更新数据数组，忽略指定列
        /// </summary>
        /// <param name="updateObjs">数据数组</param>
        /// <param name="ignoreColumns">忽略的列</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateIgnoreAsync(TEntity[] updateObjs, Expression<Func<TEntity, object>> ignoreColumns);

        /// <summary>
        /// 只更新指定列
        /// </summary>
        /// <param name="updateObj">数据</param>
        /// <param name="updateColumns">要更新的列</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateColumnsAsync(TEntity updateObj, Expression<Func<TEntity, object>> updateColumns);

        /// <summary>
        /// 只更新指定列
        /// </summary>
        /// <param name="updateObjs">数据数组</param>
        /// <param name="updateColumns">要更新的列</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateColumnsAsync(TEntity[] updateObjs, Expression<Func<TEntity, object>> updateColumns);

        /// <summary>
        /// 更新指定的一列
        /// </summary>
        /// <param name="column">要更新的一列</param>
        /// <param name="expression">更新条件</param>
        /// <returns>是否成功</returns>
        Task<bool> SetColumnAsync(Expression<Func<TEntity, bool>> column, Expression<Func<TEntity, bool>> expression);

        #endregion 更新

        #region 高级保存

        /// <summary>
        /// 更新或者插入
        /// 根据主键判断，如果数据库存在更新，不存在插入
        /// </summary>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入是否成功</returns>
        Task<bool> StorageableAsync(params TEntity[] entities);

        /// <summary>
        /// 更新或者插入
        /// 根据你自己的条件更新或插入
        /// </summary>
        /// <param name="insertConditions">更新条件</param>
        /// <param name="updateConditions">插入条件</param>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入是否成功</returns>
        Task<bool> StorageableAsync(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities);

        /// <summary>
        /// 更新或者插入
        /// 根据主键判断，如果数据库存在更新，不存在插入
        /// </summary>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入的条数</returns>
        Task<int> StorageableReturnNumAsync(params TEntity[] entities);

        /// <summary>
        /// 更新或者插入
        /// 根据你自己的条件更新或插入
        /// </summary>
        /// <param name="insertConditions">更新条件</param>
        /// <param name="updateConditions">插入条件</param>
        /// <param name="entities">要更新或者插入的数据</param>
        /// <returns>更新或者插入的条数</returns>
        Task<int> StorageableReturnNumAsync(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities);

        #endregion 高级保存
    }
}