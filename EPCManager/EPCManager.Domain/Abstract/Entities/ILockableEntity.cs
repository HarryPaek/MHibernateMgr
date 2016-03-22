using EPCManager.Domain.Entities;
using System;

namespace EPCManager.Domain.Abstract.Entities
{
    public interface ILockableEntity : SPEntity
    {
        long OId { get; set; }
        DateTime? CheckoutDate { get; set; }
        SPPeople CheckoutBy { get; set; }
        DateTime? ModifiedDate { get; set; }
        SPPeople ModifiedBy { get; set; }
    }
}
