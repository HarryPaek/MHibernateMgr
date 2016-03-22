using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;

namespace EPCManager.Domain.Abstract.Common
{
    public interface IIdentifierRepository : IRepository<SPIdentifier>
    {
        new SPIdentifierList GetAllAsList();
    }
}
