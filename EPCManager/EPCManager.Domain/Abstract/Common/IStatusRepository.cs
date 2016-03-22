using EPCManager.Domain.Entities;

namespace EPCManager.Domain.Abstract.Common
{
    public interface IStatusRepository : IRepository<SPStatus>
    {
        SPStatus GetNextStatus(SPStatus currentStatus);
        SPStatus GetPrevisouStatus(SPStatus currentStatus);
        SPStatus GetInitialStatus(SPObjectTypes objectType);
        SPStatus GetReleaseStatus(SPObjectTypes objectType);
        SPStatus GetObsoleteStatus(SPObjectTypes objectType);

        SPStatus GetStatusByName(SPObjectTypes objectType, string statusName);
    }
}
