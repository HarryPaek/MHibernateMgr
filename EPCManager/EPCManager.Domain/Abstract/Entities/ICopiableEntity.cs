using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPCManager.Domain.Abstract.Entities
{
    public interface ICopiableEntity : SPEntity
    {
        long OId { get; set; }
        SPIdentifier Identifier { get; set; }

        ICopiableEntity Clone();
    }
}
