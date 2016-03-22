using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;

namespace EPCManager.Domain.Abstract.Common
{
    public interface IDomainRepository : IRepository<SPDomain>
    {
        new SPDomainList GetAllAsList();
    }
}
