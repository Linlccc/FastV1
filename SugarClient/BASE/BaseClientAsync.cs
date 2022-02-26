using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// 仓储【异步】
    /// </summary>
    public partial class BaseClient<TEntity> : IBaseClient<TEntity> where TEntity : class, new()
    {
        #region 查询

        public async Task<TEntity> QueryByIdAsync(dynamic id) => await Db.Queryable<TEntity>().InSingleAsync(id);

        public async Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression = null) => await Db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).ToListAsync();

        public async Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, OrderByType orderByType, int top) => await Db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).Take(top).ToListAsync();

        public async Task<PageMsg<TEntity>> QueryPageAsync(Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            RefAsync<int> totalDataCount = 0;
            PageMsg<TEntity> result = new(pageIndex, pageSize);
            result.Data = await Db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(orderByExpression != null, orderByExpression, orderByType).ToPageListAsync(pageIndex, pageSize, totalDataCount);
            result.TotalDataCount = totalDataCount;
            result.TotalPageCount = (int)Math.Ceiling(totalDataCount / (decimal)pageSize);
            return result;
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> whereExpression) => await Db.Queryable<TEntity>().SingleAsync(whereExpression);

        public async Task<TEntity> MaxAsync(Expression<Func<TEntity, object>> orderExpression) => await Db.Queryable<TEntity>().OrderBy(orderExpression, OrderByType.Desc).FirstAsync();

        public async Task<TEntity> MinAsync(Expression<Func<TEntity, object>> orderExpression) => await Db.Queryable<TEntity>().OrderBy(orderExpression, OrderByType.Asc).FirstAsync();

        public async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> whereExpression = null) => await Db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).AnyAsync();

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression = null) => await Db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).CountAsync();

        public async Task<TResult> MaxFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.MaxAsync<TResult>(fieldName);
        }

        public async Task<TResult> MaxFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.MaxAsync(fieldExpression);
        }

        public async Task<TResult> MinFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.MinAsync<TResult>(fieldName);
        }

        public async Task<TResult> MinFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.MinAsync(fieldExpression);
        }

        public async Task<TResult> SumFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.SumAsync<TResult>(fieldName);
        }

        public async Task<TResult> SumFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.SumAsync(fieldExpression);
        }

        public async Task<TResult> AvgFieldAsync<TResult>(string fieldName, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.AvgAsync<TResult>(fieldName);
        }

        public async Task<TResult> AvgFieldAsync<TResult>(Expression<Func<TEntity, TResult>> fieldExpression, params Expression<Func<TEntity, bool>>[] expressions)
        {
            ISugarQueryable<TEntity> sugarQueryable = Db.Queryable<TEntity>();
            foreach (Expression<Func<TEntity, bool>> expression in expressions)
            {
                sugarQueryable.WhereIF(expression != null, expression);
            }
            return await sugarQueryable.AvgAsync(fieldExpression);
        }

        #endregion 查询

        #region 插入

        public async Task<bool> InsertAsync(TEntity insertObj) => await Db.Insertable(insertObj).ExecuteCommandIdentityIntoEntityAsync();

        public async Task<bool> InsertRangeAsync(List<TEntity> insertObjs) => await Db.Insertable(insertObjs).ExecuteCommandIdentityIntoEntityAsync();

        public async Task<bool> InsertRangeAsync(TEntity[] insertObjs) => await Db.Insertable(insertObjs).ExecuteCommandIdentityIntoEntityAsync();

        public async Task<int> InsertReturnIdentityAsync(TEntity insertObj) => await Db.Insertable(insertObj).ExecuteReturnIdentityAsync();

        public async Task<long> InsertReturnBigIdentityAsync(TEntity insertObj) => await Db.Insertable(insertObj).ExecuteReturnBigIdentityAsync();

        #endregion 插入

        #region 删除

        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression) => await Db.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandHasChangeAsync();

        public async Task<bool> DeleteAsync(TEntity deleteObj) => await Db.Deleteable(deleteObj).ExecuteCommandHasChangeAsync();

        public async Task<bool> DeleteByIdAsync(dynamic id) => await Db.Deleteable<TEntity>().In(id).ExecuteCommandHasChangeAsync();

        #endregion 删除

        #region 修改

        public async Task<bool> UpdateAsync(List<TEntity> updateObjs) => await Db.Updateable(updateObjs).ExecuteCommandHasChangeAsync();

        public async Task<bool> UpdateAsync(params TEntity[] updateObjs) => await Db.Updateable(updateObjs).ExecuteCommandHasChangeAsync();

        public async Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression) => await Db.Updateable(columns).Where(whereExpression).ExecuteCommandHasChangeAsync();

        public async Task<bool> UpdateIgnoreAsync(TEntity updateObj, Expression<Func<TEntity, object>> ignoreColumns) => await Db.Updateable(updateObj).IgnoreColumns(ignoreColumns).ExecuteCommandHasChangeAsync();

        public async Task<bool> UpdateIgnoreAsync(TEntity[] updateObjs, Expression<Func<TEntity, object>> ignoreColumns) => await Db.Updateable(updateObjs).IgnoreColumns(ignoreColumns).ExecuteCommandHasChangeAsync();

        public async Task<bool> UpdateColumnsAsync(TEntity updateObj, Expression<Func<TEntity, object>> updateColumns) => await Db.Updateable(updateObj).UpdateColumns(updateColumns).ExecuteCommandHasChangeAsync();

        public async Task<bool> UpdateColumnsAsync(TEntity[] updateObjs, Expression<Func<TEntity, object>> updateColumns) => await Db.Updateable(updateObjs).UpdateColumns(updateColumns).ExecuteCommandHasChangeAsync();

        public async Task<bool> SetColumnAsync(Expression<Func<TEntity, bool>> column, Expression<Func<TEntity, bool>> expression) => await Db.Updateable(column).Where(expression).ExecuteCommandHasChangeAsync();

        #endregion 修改

        #region 高级保存

        public async Task<bool> StorageableAsync(params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList()).Saveable().ToStorage();
            int insertNum = await storageable.AsInsertable.ExecuteCommandAsync();//插入数量
            int upadteNum = await storageable.AsUpdateable.ExecuteCommandAsync();//更新数量
            if (insertNum + upadteNum > 0) return true;
            return false;
        }

        public async Task<bool> StorageableAsync(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList())
                .SplitInsert(insertConditions)
                .SplitUpdate(updateConditions)
                .ToStorage();
            int insertNum = await storageable.AsInsertable.ExecuteCommandAsync();//插入数量
            int upadteNum = await storageable.AsUpdateable.ExecuteCommandAsync();//更新数量
            if (insertNum + upadteNum > 0) return true;
            return false;
        }

        public async Task<int> StorageableReturnNumAsync(params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList()).Saveable().ToStorage();
            int insertNum = await storageable.AsInsertable.ExecuteCommandAsync();//插入数量
            int upadteNum = await storageable.AsUpdateable.ExecuteCommandAsync();//更新数量
            return insertNum + upadteNum;
        }

        public async Task<int> StorageableReturnNumAsync(Func<StorageableInfo<TEntity>, bool> insertConditions, Func<StorageableInfo<TEntity>, bool> updateConditions, params TEntity[] entities)
        {
            StorageableResult<TEntity> storageable = Db.Storageable(entities.ToList())
                .SplitInsert(insertConditions)
                .SplitUpdate(updateConditions)
                .ToStorage();
            int insertNum = await storageable.AsInsertable.ExecuteCommandAsync();//插入数量
            int upadteNum = await storageable.AsUpdateable.ExecuteCommandAsync();//更新数量
            return insertNum + upadteNum;
        }

        #endregion 高级保存
    }
}