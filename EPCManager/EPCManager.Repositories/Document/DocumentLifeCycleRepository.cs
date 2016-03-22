using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Document;
using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Abstract.Operations;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Exceptions;
using log4net;
using NHibernate;
using System;

namespace EPCManager.Repositories.Document
{
    public class DocumentLifeCycleRepository : ILifeCycleRepository
    {
        private readonly ISession session;
        private readonly ILog log;
        private readonly IDocumentRepository documentRepository;
        private readonly IStatusRepository statusRepository;

        public DocumentLifeCycleRepository(ISession session, ILogManager logManager, IDocumentRepository documentRepository, IStatusRepository statusRepository)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
            this.documentRepository = documentRepository;
            this.statusRepository = statusRepository;
        }

        #region implement ILifeCycleRepository interface methods

        /**
         * TODO: SPPeople을 SPUser로 변경 
         */
        public void Promote(ILifeCycleEntity entity, SPPeople user)
        {
            try
            {
                SPDocument spDocument = ChangeLifeCycleStatus(user, entity, LifeCycleOperations.Promote);

                log.Debug(string.Format("문서/도면이 성공적으로 Promote 되었습니다. [{0}, {1} Rev.{2}, {3}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));
            }
            catch (EPCManagerLifeCycleException coiex)
            {
                log.Error(coiex.Message);
                throw coiex;
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message, emex);
                throw new EPCManagerException(string.Format("문서/도면을 Promote할 수 없습니다. 원인=[{0}]", emex.Message), emex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("문서/도면을 Promote하는 중에 오류가 발생하였습니다. (OId = [{0}])", entity.OId);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public void Demote(ILifeCycleEntity entity, SPPeople user)
        {
            try
            {
                SPDocument spDocument = ChangeLifeCycleStatus(user, entity, LifeCycleOperations.Demote);

                log.Debug(string.Format("문서/도면이 성공적으로 Demote 되었습니다. [{0}, {1} Rev.{2}, {3}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));
            }
            catch (EPCManagerLifeCycleException coiex)
            {
                log.Error(coiex.Message);
                throw coiex;
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message, emex);
                throw new EPCManagerException(string.Format("문서/도면을 Demote할 수 없습니다. 원인=[{0}]", emex.Message), emex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("문서/도면을 Demote하는 중에 오류가 발생하였습니다. (OId = [{0}])", entity.OId);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public void Release(ILifeCycleEntity entity, SPPeople user)
        {
            try
            {
                SPDocument spDocument = ChangeLifeCycleStatus(user, entity, LifeCycleOperations.Release);

                log.Debug(string.Format("문서/도면이 성공적으로 릴리즈 되었습니다. [{0}, {1} Rev.{2}, {3}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));
            }
            catch (EPCManagerLifeCycleException coiex)
            {
                log.Error(coiex.Message);
                throw coiex;
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message, emex);
                throw new EPCManagerException(string.Format("문서/도면을 릴리즈할 수 없습니다. 원인=[{0}]", emex.Message), emex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("문서/도면을 릴리즈하는 중에 오류가 발생하였습니다. (OId = [{0}])", entity.OId);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public void Obsolete(ILifeCycleEntity entity, SPPeople user)
        {
            try
            {
                SPDocument spDocument = ChangeLifeCycleStatus(user, entity, LifeCycleOperations.Obsolete);

                log.Debug(string.Format("문서/도면이 성공적으로 폐기 되었습니다. [{0}, {1} Rev.{2}, {3}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));
            }
            catch (EPCManagerLifeCycleException coiex)
            {
                log.Error(coiex.Message);
                throw coiex;
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message, emex);
                throw new EPCManagerException(string.Format("문서/도면을 폐기할 수 없습니다. 원인=[{0}]", emex.Message), emex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("문서/도면을 폐기하는 중에 오류가 발생하였습니다. (OId = [{0}])", entity.OId);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion


        #region private methods

        private SPDocument ChangeLifeCycleStatus(SPPeople user, ILifeCycleEntity entity, LifeCycleOperations operation)
        {
            SPDocument spDocument = documentRepository.Get(entity.OId);

            if (spDocument == null)
                throw new EPCManagerDBException(string.Format("지정하신 문서/도면이 존재하지 않습니다. (OId = [{0}])", entity.OId));

            if (spDocument.CheckoutBy != null && spDocument.CheckoutBy.OId != user.OId) // Error, 다른 사용자가 이미 Checkout 하였습니다.
                throw new EPCManagerLifeCycleException(string.Format("지정하신 문서/도면는, 다른 사용자가 체크아웃하여, 라이프사이클 상태를 변경할 수 없습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.CheckoutBy.Description));

            SPStatus releasedStatus = GetToReleaseStatus(spDocument.Status.ObjectType);
            SPStatus obsoletedStatus = GetToObsoleteStatus(spDocument.Status.ObjectType);

            if (releasedStatus == null || obsoletedStatus == null)
                throw new EPCManagerDBException("문서/도면의 Status를 조회하는 중에 오류가 발생하였습니다.");

            SPStatus newStatus = null;

            switch (operation)
            {
                case LifeCycleOperations.Promote:
                    newStatus = TryGetToPromoteStatus(spDocument, releasedStatus, obsoletedStatus);
                    break;

                case LifeCycleOperations.Demote:
                    newStatus = TryGetToDemoteStatus(spDocument, releasedStatus, obsoletedStatus);
                    break;

                case LifeCycleOperations.Release:
                    newStatus = TryGetReleasedStatus(spDocument, releasedStatus, obsoletedStatus);
                    break;

                case LifeCycleOperations.Obsolete:
                    newStatus = TryGetObsoletedStatus(spDocument, releasedStatus, obsoletedStatus);
                    break;
            }

            if (newStatus == null)
                throw new EPCManagerDBException(string.Format("문서/도면의 {0} Status가 존재하지 않습니다.", operation));

            // Status Change 실시
            spDocument.Status = newStatus;
            spDocument.ModifiedBy = user;
            spDocument.ModifiedDate = DateTime.Now;

            if (newStatus.Equals(releasedStatus) || newStatus.Equals(obsoletedStatus))  // Released Or Obsoleted
            {
                spDocument.CompletedBy = user;
                spDocument.CompletedDate = DateTime.Now;
            }

            Update(spDocument);

            return spDocument;
        }

        private SPStatus TryGetToPromoteStatus(SPDocument spDocument, SPStatus releasedStatus, SPStatus obsoletedStatus)
        {
            if (spDocument.IsStatusAt(releasedStatus) || spDocument.IsStatusAt(obsoletedStatus))
                throw new EPCManagerLifeCycleException(string.Format("릴리즈되었거나 폐기된 문서/도면는 라이프사이클 상태를 변경할 수 없습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));

            return GetToPromoteStatus(spDocument.Status);
        }

        private SPStatus TryGetToDemoteStatus(SPDocument spDocument, SPStatus releasedStatus, SPStatus obsoletedStatus)
        {
            if (spDocument.IsStatusAt(releasedStatus) || spDocument.IsStatusAt(obsoletedStatus))
                throw new EPCManagerLifeCycleException(string.Format("릴리즈되었거나 폐기된 문서/도면는 라이프사이클 상태를 변경할 수 없습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));

            SPStatus initialStatus = GetInitialStatus(spDocument.Status.ObjectType);
            if (initialStatus == null)
                throw new EPCManagerDBException("문서/도면의 Status를 조회하는 중에 오류가 발생하였습니다.");

            if (spDocument.IsStatusAt(initialStatus))
                throw new EPCManagerLifeCycleException(string.Format("지정하신 문서/도면는, 현재 최초 단계에 있어, Demote할 수 없습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));

            return GetToDemoteStatus(spDocument.Status);
        }

        private SPStatus TryGetReleasedStatus(SPDocument spDocument, SPStatus releasedStatus, SPStatus obsoletedStatus)
        {
            if (spDocument.IsStatusAt(releasedStatus))
                throw new EPCManagerLifeCycleException(string.Format("지정하신 문서/도면는 이미 릴리즈 상태에 있습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));

            if (spDocument.IsStatusAt(obsoletedStatus))
                throw new EPCManagerLifeCycleException(string.Format("릴리즈되었거나 폐기된 문서/도면는 라이프사이클 상태를 변경할 수 없습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));

            return releasedStatus;
        }

        private SPStatus TryGetObsoletedStatus(SPDocument spDocument, SPStatus releasedStatus, SPStatus obsoletedStatus)
        {
            if (spDocument.IsStatusAt(obsoletedStatus))
                throw new EPCManagerLifeCycleException(string.Format("지정하신 문서/도면는 이미 폐기 상태에 있습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.Status.Description));

            return obsoletedStatus;
        }
        
        private SPStatus GetToPromoteStatus(SPStatus currentStatus)
        {
            return statusRepository.GetNextStatus(currentStatus);
        }

        private SPStatus GetToDemoteStatus(SPStatus currentStatus)
        {
            return statusRepository.GetPrevisouStatus(currentStatus);
        }

        private SPStatus GetInitialStatus(SPObjectTypes objectType)
        {
            return statusRepository.GetInitialStatus(objectType);
        }

        private SPStatus GetToReleaseStatus(SPObjectTypes objectType)
        {
            return statusRepository.GetReleaseStatus(objectType);
        }

        private SPStatus GetToObsoleteStatus(SPObjectTypes objectType)
        {
            return statusRepository.GetObsoleteStatus(objectType);
        }

        /// <summary>
        /// Update for lifecycle change
        /// - 일반 Update로직과 달라서 별도로 구현함
        /// </summary>
        /// <param name="document">document to update</param>
        private void Update(SPDocument document)
        {
            ITransaction trx = null;
            try
            {
                trx = session.BeginTransaction();
                session.Update(document);

                if (trx != null && trx.IsActive)
                    trx.Commit();
            }
            catch (Exception)
            {
                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw;
            }
        }

        #endregion


        private enum LifeCycleOperations
        {
            Promote,
            Demote,
            Release,
            Obsolete
        }
    }
}
