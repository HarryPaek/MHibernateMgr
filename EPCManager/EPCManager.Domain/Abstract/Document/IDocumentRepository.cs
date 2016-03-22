using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Operations;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;

namespace EPCManager.Domain.Abstract.Document
{
    public interface IDocumentRepository : IRepository<SPDocument>
    {
        new SPDocumentList GetAllAsList();
    }
}
