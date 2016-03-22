using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPIdentifierList : List<SPIdentifier>, IEntityList<SPIdentifier>
    {
        public SPIdentifierList() : base()
        {
        }

        public SPIdentifierList(int capacity) : base(capacity)
        {
        }

        public SPIdentifierList(IEnumerable<SPIdentifier> collection) : base(collection)
        {
        }
    }
}
