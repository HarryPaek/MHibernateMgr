using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPDomainList : List<SPDomain>, IEntityList<SPDomain>
    {
        public SPDomainList() : base()
        {
        }

        public SPDomainList(int capacity) : base(capacity)
        {
        }

        public SPDomainList(IEnumerable<SPDomain> collection) : base(collection)
        {
        }
    }
}
