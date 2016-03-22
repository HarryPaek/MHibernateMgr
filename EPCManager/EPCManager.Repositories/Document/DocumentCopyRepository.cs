using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Document;
using EPCManager.Domain.Abstract.Services;
using EPCManager.Domain.Constants;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Exceptions;
using log4net;
using System;

namespace EPCManager.Repositories.Document
{
    public class DocumentCopyRepository : IDocumentCopyRepository
    {
        private readonly ILog     log;
        private readonly IDocumentRepository documentRepository;
        private readonly IRevisionProvider   revisionProvider;
        private readonly IStatusRepository   statusRepository;

        public DocumentCopyRepository(ILogManager logManager, IDocumentRepository documentRepository, IRevisionProvider revisionProvider, IStatusRepository statusRepository)
        {
            this.log = logManager.GetLog(this.GetType());
            this.documentRepository = documentRepository;
            this.revisionProvider = revisionProvider;
            this.statusRepository = statusRepository;
        }

        public SPDocument Copy(SPDocument baseEntity, SPIdentifier newId, OperationOptions copyOption, SPPeople user)
        {
            try
            {
                SPDocument spBaseDocument = documentRepository.Get(baseEntity.OId);

                if (spBaseDocument == null)
                    throw new EPCManagerDBException(string.Format("지정하신 문서/도면이 존재하지 않습니다. (OId = [{0}])", baseEntity.OId));

                SPStatus initialStatus = statusRepository.GetInitialStatus(spBaseDocument.Status.ObjectType);
                if (initialStatus == null)
                    throw new EPCManagerDBException("문서/도면의 Status를 조회하는 중에 오류가 발생하였습니다.");

                SPDocument newDocument = spBaseDocument.Clone();

                //Set Initial values for creation
                newDocument.OId = 0;
                newDocument.Revision = revisionProvider.GetInitialRevision();
                newDocument.RevisionSortSequence = revisionProvider.GetInitialRevisionSortSequence();
                newDocument.Status = initialStatus;
                newDocument.Identifier = newId;
                newDocument.CheckoutDate = null;
                newDocument.CheckoutBy = null;
                newDocument.CreatedDate = DateTime.Now;
                newDocument.CreatedBy = user;
                newDocument.ModifiedDate = null;
                newDocument.ModifiedBy = null;
                newDocument.CompletedDate = null;
                newDocument.CompletedBy = null;

                documentRepository.Add(newDocument);

                log.Debug(string.Format("문서/도면이 성공적으로 복사 되었습니다. [{0}, {1} Rev.{2}]", newDocument.OId, newDocument.Identifier.Code, newDocument.Revision));

                return newDocument;
            }
            catch (EPCManagerCopyException coex)
            {
                log.Error(coex.Message);

                throw coex;
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message, emex);

                throw new EPCManagerException(string.Format("문서/도면을 복사할 수 없습니다. 원인=[{0}]", emex.Message), emex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("문서/도면을 복사하는 중에 오류가 발생하였습니다. (OId = [{0}])", baseEntity.OId);
                log.Error(errorMsg, ex);

                throw new Exception(errorMsg, ex);
            }
        }
    }
}
