using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Entities;

namespace EPCManager.Domain.Abstract.Operations
{
    public interface ILifeCycleRepository
    {
        void Promote(ILifeCycleEntity entity, SPPeople user);
        void Demote(ILifeCycleEntity entity, SPPeople user);
        void Release(ILifeCycleEntity entity, SPPeople user);
        void Obsolete(ILifeCycleEntity entity, SPPeople user);
    }
}
