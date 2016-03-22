using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Abstract.Operations;
using EPCManager.Domain.Entities;

namespace EPCManager.Repositories.Operations
{
    public class LifeCycleRepository<TEntity> : ILifeCycleRepository where TEntity : SPEntity
    {
        private readonly IRepository<TEntity> mainRepository;
        private readonly IStatusRepository statusRepository;

        public LifeCycleRepository(IRepository<TEntity> mainRepository, IStatusRepository statusRepository)
        {
            this.mainRepository = mainRepository;
            this.statusRepository = statusRepository;
        }

        public void Promote(ILifeCycleEntity entity, SPPeople currentUser)
        {
            throw new System.NotImplementedException();
        }

        public void Demote(ILifeCycleEntity entity, SPPeople currentUser)
        {
            throw new System.NotImplementedException();
        }

        public void Release(ILifeCycleEntity entity, SPPeople currentUser)
        {
            throw new System.NotImplementedException();
        }

        public void Obsolete(ILifeCycleEntity entity, SPPeople currentUser)
        {
            throw new System.NotImplementedException();
        }
    }
}
