using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Entities;
using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;

namespace EPCManager.Repositories.Common
{
    public class RelationshipRepository : IRelationshipRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public RelationshipRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement IRelationshipRepository methods

        public IEnumerable<SPRelationship> GetAll(SPObjectTypes objectType, long entityId)
        {
            try
            {
                var entities = session.GetNamedQuery("RelationshipListLoaderWithObjectTypeAndSourceOId")
                                      .SetParameter("objectType", Convert.ChangeType(objectType, objectType.GetTypeCode()))
                                      .SetParameter("sourceOid", entityId).List<SPRelationship>();

                log.Debug(string.Format("연관 관계 목록이 성공적으로 조회되었습니다.[{0}, {1}]", objectType, entityId));

                return entities;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("연관 관계 목록을 조회하는 중에 오류가 발생하였습니다.[{0}, {1}]", objectType, entityId);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public IEnumerable<SPRelationship> GetAll(IAssociableEntity entity)
        {
            return GetAll(entity.Identifier.ObjectType, entity.OId);
        }

        #endregion

        #region implement interface methods

        public SPRelationship Get(long id)
        {
            try
            {
                var entity = session.Get<SPRelationship>(id);

                log.Debug(string.Format("연관 관계가 성공적으로 조회되었습니다.[{0}, [{1}]<->[{2}]]", entity.OId, entity.LeftObject.OId, entity.RightObject.OId));

                return entity;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("연관 관계를 조회하는 중에 오류가 발생하였습니다.(Id = [{0}])", id);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public void Add(SPRelationship entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(SPRelationship entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SPRelationship entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SPRelationship> Items
        {
            get { return GetAll().List(); }
        }

        public SPRelationshipList GetAllAsList()
        {
            throw new NotImplementedException();
        }

        #region implemented explicitly

        IEntityList<SPRelationship> IRepository<SPRelationship>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPRelationship> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
