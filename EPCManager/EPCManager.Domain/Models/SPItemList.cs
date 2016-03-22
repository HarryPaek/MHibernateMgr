using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPItemList : List<SPItem>, IEntityList<SPItem>
    {
        public SPItemList() : base()
        {
        }

        public SPItemList(int capacity) : base(capacity)
        {
        }

        public SPItemList(IEnumerable<SPItem> collection) : base(collection)
        {
        }
    }
}
