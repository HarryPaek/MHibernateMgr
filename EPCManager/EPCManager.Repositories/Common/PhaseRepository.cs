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
    public class PhaseRepository : IPhaseRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public PhaseRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement interface methods

        public SPPhase Get(long id)
        {
            var entity = session.Get<SPPhase>(id);

            return entity;
        }

        public void Add(SPPhase entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(SPPhase entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SPPhase entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SPPhase> Items
        {
            get { return GetAll().List(); }
        }

        public SPPhaseList GetAllAsList()
        {
            return new SPPhaseList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPPhase> IRepository<SPPhase>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPPhase> GetAll()
        {
            return session.QueryOver<SPPhase>();
        }

        #endregion
    }
}
