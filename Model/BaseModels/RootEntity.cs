using SqlSugar;
using Yitter.IdGenerator;

namespace Model.BaseModels
{
    /// <summary>
    /// 根实体
    /// </summary>
    public class RootEntity
    {
        private long _id;

        /// <summary>
        /// ID【使用的雪花漂移ID】
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long Id
        {
            get
            {
                if (_id == 0) _id = YitIdHelper.NextId();
                return _id;
            }
            set => _id = value;
        }
    }
}