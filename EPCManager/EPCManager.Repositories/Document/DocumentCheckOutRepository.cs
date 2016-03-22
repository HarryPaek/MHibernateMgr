using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Document;
using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Abstract.Operations;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Exceptions;
using log4net;
using System;

namespace EPCManager.Repositories.Document
{
    public class DocumentCheckOutRepository : ICheckOutRepository
    {
        private readonly ILog log;
        private readonly IDocumentRepository documentRepository;

        public DocumentCheckOutRepository(ILogManager logManager, IDocumentRepository documentRepository)
        {
            this.log = logManager.GetLog(this.GetType());
            this.documentRepository = documentRepository;
        }

        #region implement ICheckOutRepository interface methods

        /**
         * TODO: SPPeople을 SPUser로 변경 
         */
        public void CheckOut(ILockableEntity entity, SPPeople currentUser)
        {
            try
            {
                SPDocument spDocument = documentRepository.Get(entity.OId);

                if (spDocument == null)
                    throw new EPCManagerDBException(string.Format("지정하신 문서/도면이 존재하지 않습니다. (OId = [{0}])", entity.OId));

                if (spDocument.CheckoutBy == null)  // Checkout 실시
                {
                    spDocument.CheckoutBy = currentUser;
                    spDocument.CheckoutDate = DateTime.Now;
                    spDocument.ModifiedBy = currentUser;
                    spDocument.ModifiedDate = DateTime.Now;
                }
                else
                {
                    if (spDocument.CheckoutBy.OId != currentUser.OId) { // Error, 다른 사용자가 이미 Checkout 하였습니다.
                        throw new EPCManagerCheckOutCheckInException(string.Format("지정하신 문서/도면는 이미 다른 사용자가 체크아웃 하였습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.CheckoutBy.Description));
                    }
                    else {                                              //이미 해당 사용자로 CheckOut 되어 있음
                        log.Debug(string.Format("해당 문서/도면는 이미 현재사용자가 체크아웃 하였습니다. [{0}, {1} Rev.{2}, {3}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision, spDocument.CheckoutBy.Description));
                        return;
                    }
                }

                documentRepository.Update(spDocument);

                log.Debug(string.Format("문서/도면이 성공적으로 체크아웃 되었습니다. [{0}, {1} Rev.{2}, {3}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision, spDocument.CheckoutBy.Description));
            }
            catch (EPCManagerCheckOutCheckInException coiex)
            {
                log.Error(coiex.Message);

                throw coiex;
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message, emex);

                throw new EPCManagerException(string.Format("문서/도면을 체크아웃할 수 없습니다. 원인=[{0}]", emex.Message), emex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("문서/도면을 체크아웃하는 중에 오류가 발생하였습니다. (OId = [{0}])", entity.OId);
                log.Error(errorMsg, ex);

                throw new Exception(errorMsg, ex);
            }
        }

        /**
         * TODO: SPPeople을 SPUser로 변경 
         * TODO: Admin User의 경우 무조건 Check-in 가능
         */
        public void CheckIn(ILockableEntity entity, SPPeople currentUser)
        {
            try
            {
                SPDocument spDocument = documentRepository.Get(entity.OId);

                if (spDocument == null)
                    throw new EPCManagerDBException(string.Format("지정하신 문서/도면이 존재하지 않습니다. (OId = [{0}])", entity.OId));

                if (spDocument.CheckoutBy == null)  // CheckIn 필요 없음
                {
                    log.Debug(string.Format("해당 문서/도면는 체크아웃 되지 않았습니다. [{0}, {1} Rev.{2}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision));
                    return;
                }
                else
                {
                    if (spDocument.CheckoutBy.OId != currentUser.OId) {   // Error, 다른 사용자로 CheckOut된 경우에는 CheckIn할 수 없습니다.
                        throw new EPCManagerCheckOutCheckInException(string.Format("지정하신 문서/도면는 다른 사용자가 체크아웃 하였습니다. 현재 사용자로 체크인 할 수 없습니다. [{0} Rev.{1}, {2}]", spDocument.Identifier.Code, spDocument.Revision, spDocument.CheckoutBy.Description));
                    }
                    else {                                                //해당 사용자로 CheckOut 되어 있음, CheckIn 진행 
                        spDocument.CheckoutBy = null;
                        spDocument.CheckoutDate = null;
                        spDocument.ModifiedBy = currentUser;
                        spDocument.ModifiedDate = DateTime.Now;
                    }
                }

                documentRepository.Update(spDocument);

                log.Debug(string.Format("문서/도면이 성공적으로 체크인 되었습니다. [{0}, {1} Rev.{2}]", spDocument.OId, spDocument.Identifier.Code, spDocument.Revision));
            }
            catch (EPCManagerCheckOutCheckInException coiex)
            {
                log.Error(coiex.Message);

                throw coiex;
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message, emex);

                throw new EPCManagerException(string.Format("문서/도면을 체크인할 수 없습니다. 원인=[{0}]", emex.Message), emex);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("문서/도면을 체크인하는 중에 오류가 발생하였습니다. (OId = [{0}])", entity.OId);
                log.Error(errorMsg, ex);

                throw new Exception(errorMsg, ex);
            }
        }

        #endregion
    }
}
