using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Constants;
using EPCManager.Domain.Entities;

namespace EPCManager.Domain.Abstract.Operations
{
    public interface ICopyRepository<TEntity> where TEntity : ICopiableEntity
    {
        TEntity Copy(TEntity baseEntity, SPIdentifier newId, OperationOptions copyOption, SPPeople user);
    }
}
