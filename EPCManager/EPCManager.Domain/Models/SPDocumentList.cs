using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using System.Collections.Generic;

namespace EPCManager.Domain.Models
{
    public class SPDocumentList : List<SPDocument>, IEntityList<SPDocument>
    {
        public SPDocumentList() : base()
        {
        }

        public SPDocumentList(int capacity) : base(capacity)
        {
        }

        public SPDocumentList(IEnumerable<SPDocument> collection) : base(collection)
        {
        }
    }
}
