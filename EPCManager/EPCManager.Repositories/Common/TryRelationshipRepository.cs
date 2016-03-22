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
    public class TryRelationshipRepository : ITryRelationshipRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public TryRelationshipRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement interface methods

        public TryRelationship Get(long id)
        {
            var entity = session.Get<TryRelationship>(id);

            return entity;
        }

        public void Add(TryRelationship entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(TryRelationship entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TryRelationship entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TryRelationship> Items
        {
            get { return GetAll().List(); }
        }

        #region implemented explicitly

        IEntityList<TryRelationship> IRepository<TryRelationship>.GetAllAsList()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<TryRelationship> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion

        public IEnumerable<TryRelationship> GetAll(SPObjectTypes objectType, long entityId)
        {
            var entity = session.GetNamedQuery("TryRelationshipListLoaderWithObjectTypeAndSourceOId")
                                .SetParameter("objectType", Convert.ChangeType(objectType, objectType.GetTypeCode()))
                                .SetParameter("sourceOid", entityId).List<TryRelationship>();

            return entity;
        }

        public IEnumerable<TryRelationship> GetAll(IAssociableEntity entity)
        {
            return GetAll(entity.Identifier.ObjectType, entity.OId);
        }
    }
}
