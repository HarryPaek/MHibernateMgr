using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Exceptions;
using log4net;
using NHibernate;
using System;
using System.Linq;
using System.Collections.Generic;
using EPCManager.Domain.Models;
using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Abstract.Item;
using EPCManager.Common.Helpers;

namespace EPCManager.Repositories.Item
{
    public class ItemRepository : IItemRepository
    {
        private const SPObjectTypes targetType = SPObjectTypes.Item;
        private readonly ISession session;
        private readonly ILog     log;
        private readonly IIdentifierRepository   identifierRepository;
        private readonly IStatusRepository       statusRepository;
        private readonly IRelationshipRepository relationshipRepository;

        public ItemRepository(ISession session, ILogManager logManager, IIdentifierRepository identifierRepository, IStatusRepository statusRepository, IRelationshipRepository relationshipRepository)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
            this.identifierRepository = identifierRepository;
            this.statusRepository = statusRepository;
            this.relationshipRepository = relationshipRepository;
        }

        #region implement IItemRepository interface methods

        public SPItem Get(long id)
        {
            try
            {
                var entity = session.Get<SPItem>(id);
                entity.Relationships.AddRange(relationshipRepository.GetAll(entity));

                log.Debug(string.Format("아이템이 성공적으로 조회되었습니다. [{0}, {1} Rev.{2}]", entity.OId, entity.Identifier.Code, entity.Revision));

                return entity;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이템을 조회하는 중에 오류가 발생하였습니다.(Id = [{0}])", id);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public void Add(SPItem entity)
        {
            ITransaction trx = null;
            try
            {
                SPIdentifier identfier = null;

                trx = session.BeginTransaction();

                if (entity.Identifier.OId > 0) {
                    identfier = identifierRepository.Get(entity.Identifier.OId);
                }

                if (identfier != null)
                    entity.Identifier = identfier;
                else
                    session.Save("SPIdentifier", entity.Identifier);
                
                session.Save(entity);

                if (trx != null && trx.IsActive)
                    trx.Commit();

                log.Debug(string.Format("아이템이 성공적으로 생성되었습니다. [{0}, {1} Rev.{2}]", entity.OId, entity.Identifier.Code, entity.Revision));
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw emex;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이템을 생성하는 중에 오류가 발생하였습니다.(Item=[{0} Rev.{1}])", entity.Identifier.Code, entity.Revision);
                log.Error(errorMsg, ex);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw new Exception(errorMsg, ex);
            }
        }

        public void Delete(SPItem entity)
        {
            ITransaction trx = null;
            try
            {
                trx = session.BeginTransaction();
                SPItem spItem = Get(entity.OId);

                if (spItem == null)
                    throw new EPCManagerDBException(string.Format("지정하신 아이템이 존재하지 않습니다. [{0} Rev.{1}]", entity.Identifier.Code, entity.Revision));

                SPStatus releasedStaus = statusRepository.GetReleaseStatus(spItem.Status.ObjectType);
                SPStatus obsoletedStaus = statusRepository.GetObsoleteStatus(spItem.Status.ObjectType);

                if (releasedStaus == null || obsoletedStaus == null)
                    throw new EPCManagerDBException("아이템의 Status를 조회하는 중에 오류가 발생하였습니다.");

                if (spItem.IsStatusAt(releasedStaus) || spItem.IsStatusAt(obsoletedStaus))
                    throw new EPCManagerException(string.Format("릴리즈되었거나 폐기된 아이템은 삭제할 수 없습니다. [{0} Rev.{1}, {2}]", entity.Identifier.Code, entity.Revision, entity.Status.Description));

                session.Delete(entity);

                SPIdentifier identfier = identifierRepository.Get(entity.Identifier.OId);

                if(identfier != null && Items.Count(x => x.Identifier.OId == identfier.OId) == 0)
                {
                    session.Delete("SPIdentifier", identfier); // 더이상 리비전이 존재하지 않는 경우 Id 삭제
                }

                if (trx != null && trx.IsActive)
                    trx.Commit();

                log.Debug(string.Format("아이템이 성공적으로 삭제되었습니다. [{0}, {1} Rev.{2}]", entity.OId, entity.Identifier.Code, entity.Revision));
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw emex;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이템을 삭제하는 중에 오류가 발생하였습니다. (Item=[{0}, {1} Rev.{2}])", entity.OId, entity.Identifier.Code, entity.Revision);
                log.Error(errorMsg, ex);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw new Exception(errorMsg, ex);
            }
        }

        public void Update(SPItem entity)
        {
            ITransaction trx = null;
            try
            {
                SPItem spItem = Get(entity.OId);

                if (spItem == null)
                    throw new EPCManagerDBException(string.Format("지정하신 아이템이 존재하지 않습니다. [{0} Rev.{1}]", entity.Identifier.Code, entity.Revision));

                SPStatus releasedStaus = statusRepository.GetReleaseStatus(spItem.Status.ObjectType);
                SPStatus obsoletedStaus = statusRepository.GetObsoleteStatus(spItem.Status.ObjectType);

                if (releasedStaus == null || obsoletedStaus == null)
                    throw new EPCManagerDBException("아이템의 Status를 조회하는 중에 오류가 발생하였습니다.");

                if (spItem.IsStatusAt(releasedStaus) || spItem.IsStatusAt(obsoletedStaus))
                    throw new EPCManagerException(string.Format("릴리즈되었거나 폐기된 아이템는 수정할 수 없습니다. [{0} Rev.{1}, {2}]", entity.Identifier.Code, entity.Revision, entity.Status.Description));

                trx = session.BeginTransaction();

                session.Update(entity);

                if (trx != null && trx.IsActive)
                    trx.Commit();

                log.Debug(string.Format("아이템이 성공적으로 수정되었습니다. [{0}, {1} Rev.{2}]", entity.OId, entity.Identifier.Code, entity.Revision));
            }
            catch (EPCManagerException emex)
            {
                log.Error(emex.Message);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw emex;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이템을 수정하는 중에 오류가 발생하였습니다. (Item=[{0}, {1} Rev.{2}])", entity.OId, entity.Identifier.Code, entity.Revision);
                log.Error(errorMsg, ex);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw new Exception(errorMsg, ex);
            }
        }

        public IEnumerable<SPItem> Items
        {
            get { return GetAll().List(); }
        }

        public SPItemList GetAllAsList()
        {
            return new SPItemList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPItem> IRepository<SPItem>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPItem> GetAll()
        {
            try
            {
                // Apply Relationship Query Filter
                IFilter filter = session.EnableFilter("fileterRelationshipQueryByObjectType");
                filter.SetParameter("objectType", Convert.ChangeType(targetType, targetType.GetTypeCode()));
                filter.Validate();

                var entities = session.QueryOver<SPItem>();
                log.Debug(string.Format("아이템 목록이 성공적으로 조회되었습니다."));

                return entities;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이템 목록을 조회하는 중에 오류가 발생하였습니다");
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion
    }
}
