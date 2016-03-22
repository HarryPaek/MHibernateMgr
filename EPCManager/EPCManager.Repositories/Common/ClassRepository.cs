using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;

namespace EPCManager.Repositories.Common
{
    public class ClassRepository : IClassRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public ClassRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement interface methods

        public SPClass Get(long id)
        {
            var entity = session.Get<SPClass>(id);

            return entity;
        }

        public void Add(SPClass entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(SPClass entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SPClass entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SPClass> Items
        {
            get { return GetAll().List(); }
        }

        public SPClassList GetAllAsList()
        {
            return new SPClassList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPClass> IRepository<SPClass>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPClass> GetAll()
        {
            return session.QueryOver<SPClass>();
        }

        #endregion
    }
}
