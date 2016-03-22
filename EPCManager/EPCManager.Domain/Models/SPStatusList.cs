using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPStatusList : List<SPStatus>, IEntityList<SPStatus>
    {
        public SPStatusList() : base()
        {
        }

        public SPStatusList(int capacity) : base(capacity)
        {
        }

        public SPStatusList(IEnumerable<SPStatus> collection) : base(collection)
        {
        }
    }
}
