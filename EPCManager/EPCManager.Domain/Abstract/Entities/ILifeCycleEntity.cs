using EPCManager.Domain.Entities;
using System;

namespace EPCManager.Domain.Abstract.Entities
{
    public interface ILifeCycleEntity : SPEntity
    {
        long OId { get; set; }
        SPStatus Status { get; set; }
        DateTime? ModifiedDate { get; set; }
        SPPeople ModifiedBy { get; set; }
        DateTime? CompletedDate { get; set; }
        SPPeople CompletedBy { get; set; }

        bool IsStatusAt(SPStatus status);
    }
}
