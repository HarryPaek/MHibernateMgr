using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPCManager.Repositories.Common
{
    public class RelationshipTypeRepository : IRelationshipTypeRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public RelationshipTypeRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement interface methods

        public SPRelationshipType Get(long id)
        {
            var entity = session.Get<SPRelationshipType>(id);

            return entity;
        }

        public void Add(SPRelationshipType entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(SPRelationshipType entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SPRelationshipType entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SPRelationshipType> Items
        {
            get { return GetAll().List(); }
        }

        public SPRelationshipTypeList GetAllAsList()
        {
            return new SPRelationshipTypeList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPRelationshipType> IRepository<SPRelationshipType>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPRelationshipType> GetAll()
        {
            return session.QueryOver<SPRelationshipType>();
        }

        #endregion
    }
}
