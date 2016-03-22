using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Exceptions;
using log4net;
using NHibernate;
using System;
using System.Linq;
using System.Collections.Generic;
using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Models;

namespace EPCManager.Repositories.Common
{
    public class DomainRepository : IDomainRepository
    {
        private readonly ISession session;
        private readonly ILog     log;
        private readonly IIdentifierRepository identifierRepository;

        public DomainRepository(ISession session, ILogManager logManager, IIdentifierRepository identifierRepository)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
            this.identifierRepository = identifierRepository;
        }

        #region implement interface methods

        public SPDomain Get(long id)
        {
            try
            {
                var entity = session.Get<SPDomain>(id);
                log.Debug(string.Format("도메인이 성공적으로 조회되었습니다. [{0}, {1}]", entity.OId, entity.Code));

                return entity;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("도메인을 조회하는 중에 오류가 발생하였습니다.(Id = [{0}])", id);
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        public void Add(SPDomain entity)
        {
            ITransaction trx = null;
            try
            {
                trx = session.BeginTransaction();
                session.Save(entity);
                trx.Commit();

                log.Debug(string.Format("도메인이 성공적으로 생성되었습니다. [{0}, {1}]", entity.OId, entity.Code));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("도메인을 생성하는 중에 오류가 발생하였습니다.(Code = [{0}])", entity.Code);
                log.Error(errorMsg, ex);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw new Exception(errorMsg, ex);
            }
        }

        public void Delete(SPDomain entity)
        {
            ITransaction trx = null;
            try
            {
                if (identifierRepository.Items != null && identifierRepository.Items.Count(x => x.Domain.OId == entity.OId) > 0)
                    throw new EPCManagerException(string.Format("이미 사용중인 도메인은 삭제할 수 없습니다. [{0}, {1}]", entity.OId, entity.Code));

                trx = session.BeginTransaction();
                session.Delete(entity);
                trx.Commit();

                log.Debug(string.Format("도메인이 성공적으로 삭제되었습니다. [{0}, {1}]", entity.OId, entity.Code));
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
                string errorMsg = string.Format("도메인을 삭제하는 중에 오류가 발생하였습니다.(Code = [{0}])", entity.Code);
                log.Error(errorMsg, ex);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw new Exception(errorMsg, ex);
            }
        }

        public void Update(SPDomain entity)
        {
            ITransaction trx = null;
            try
            {
                trx = session.BeginTransaction();
                session.Update(entity);
                trx.Commit();

                log.Debug(string.Format("도메인이 성공적으로 수정되었습니다. [{0}, {1}]", entity.OId, entity.Code));
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("도메인을 수정하는 중에 오류가 발생하였습니다.(Code = [{0}])", entity.Code);
                log.Error(errorMsg, ex);

                if (trx != null && trx.IsActive)
                    trx.Rollback();

                throw new Exception(errorMsg, ex);
            }
        }

        public IEnumerable<SPDomain> Items
        {
            get { return GetAll().List(); }
        }

        public SPDomainList GetAllAsList()
        {
            return new SPDomainList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPDomain> IRepository<SPDomain>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPDomain> GetAll()
        {
            try
            {
                var entities = session.QueryOver<SPDomain>();
                log.Debug(string.Format("도메인 목록이 성공적으로 조회되었습니다."));

                return entities;
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("도메인 목록을 조회하는 중에 오류가 발생하였습니다");
                log.Error(errorMsg, ex);
                throw new Exception(errorMsg, ex);
            }
        }

        #endregion
    }
}
