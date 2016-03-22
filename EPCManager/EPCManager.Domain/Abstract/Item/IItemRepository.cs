using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Operations;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;

namespace EPCManager.Domain.Abstract.Item
{
    public interface IItemRepository : IRepository<SPItem>
    {
        new SPItemList GetAllAsList();
    }
}
