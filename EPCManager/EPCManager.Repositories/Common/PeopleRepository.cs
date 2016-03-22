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
    public class PeopleRepository : IPeopleRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public PeopleRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement interface methods

        public SPPeople Get(long id)
        {
            var entity = session.Get<SPPeople>(id);

            return entity;
        }

        public void Add(SPPeople entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(SPPeople entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SPPeople entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SPPeople> Items
        {
            get { return GetAll().List(); }
        }

        public SPPeopleList GetAllAsList()
        {
            return new SPPeopleList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPPeople> IRepository<SPPeople>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPPeople> GetAll()
        {
            var items = session.QueryOver<SPPeople>();

            return items;
        }

        #endregion
    }
}
