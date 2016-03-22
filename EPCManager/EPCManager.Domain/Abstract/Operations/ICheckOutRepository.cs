using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Entities;

namespace EPCManager.Domain.Abstract.Operations
{
    public interface ICheckOutRepository
    {
        void CheckOut(ILockableEntity entity, SPPeople user);
        void CheckIn(ILockableEntity entity, SPPeople user);
    }
}
