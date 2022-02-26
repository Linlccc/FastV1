using Model.Models;

namespace SqlSugar
{
    /// <summary>
    /// InterfaceInfo 仓储类
    /// </summary>
    public class InterfaceInfoClient : BaseClient<InterfaceInfo>, IInterfaceInfoClient
    {
        public InterfaceInfoClient(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}