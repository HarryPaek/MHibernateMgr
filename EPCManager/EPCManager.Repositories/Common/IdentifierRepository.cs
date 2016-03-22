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
    public class IdentifierRepository : IIdentifierRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public IdentifierRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement interface methods

        public SPIdentifier Get(long id)
        {
            try
            {
                var entity = session.Get<SPIdentifier>(id);
                log.Debug(string.Format("아이디가 성공적으로 조회되었습니다. [{0}, {1}|{2}]", entity.OId, entity.Domain.Code, entity.Code));

                return entity;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이디를 조회하는 중에 오류가 발생하였습니다.(Id = [{0}])", id);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public void Add(SPIdentifier entity)
        {
            ITransaction trx = null;
            try
            {
                trx = session.BeginTransaction();
                session.Save(entity);
                trx.Commit();

                log.Debug(string.Format("아이디가 성공적으로 생성되었습니다. [{0}, {1}|{2}]", entity.OId, entity.Domain.Code, entity.Code));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이디를 생성하는 중에 오류가 발생하였습니다.(Code = [{0}|{1}])", entity.Domain.Code, entity.Code);
                log.Error(errorMsg, ex);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw new Exception(errorMsg, ex);
            }
        }

        public void Delete(SPIdentifier entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SPIdentifier entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SPIdentifier> Items
        {
            get { return GetAll().List(); }
        }

        public SPIdentifierList GetAllAsList()
        {
            return new SPIdentifierList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPIdentifier> IRepository<SPIdentifier>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPIdentifier> GetAll()
        {
            try
            {
                var entities = session.QueryOver<SPIdentifier>();
                log.Debug(string.Format("아이디 목록이 성공적으로 조회되었습니다."));

                return entities;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("아이디 목록을 조회하는 중에 오류가 발생하였습니다");
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion
    }
}
