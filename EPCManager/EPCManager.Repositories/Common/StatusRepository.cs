using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Models;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Models;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace EPCManager.Repositories.Common
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ISession session;
        private readonly ILog     log;

        public StatusRepository(ISession session, ILogManager logManager)
        {
            this.session = session;
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement interface methods

        public SPStatus Get(long id)
        {
            var entity = session.Get<SPStatus>(id);

            return entity;
        }

        public void Add(SPStatus entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(SPStatus entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SPStatus entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SPStatus> Items
        {
            get { return GetAll().List(); }
        }

        #endregion


        #region implement interface methods for LifeCycle

        public SPStatus GetNextStatus(SPStatus currentStatus)
        {
            string obsoleteStatusName = GetObsoleteStatusName(currentStatus.ObjectType);
            string releaseStatusName = GetReleaseStatusName(currentStatus.ObjectType);

            // Released나 Obsoleted는 다음 Status가 없음, 즉 Status 변경이 불가능 함, Revise하거나 다시 생성해야하는 프로세스
            if (currentStatus.Name.Equals(releaseStatusName, StringComparison.OrdinalIgnoreCase) || currentStatus.Name.Equals(obsoleteStatusName, StringComparison.OrdinalIgnoreCase))
                return null;

            return session.QueryOver<SPStatus>().Where(s => s.ObjectType == currentStatus.ObjectType && s.Ordinal > currentStatus.Ordinal).OrderBy(s => s.Ordinal).Asc.Take(1).SingleOrDefault();
        }

        public SPStatus GetPrevisouStatus(SPStatus currentStatus)
        {
            string obsoleteStatusName = GetObsoleteStatusName(currentStatus.ObjectType);
            string releaseStatusName = GetReleaseStatusName(currentStatus.ObjectType);

            // Released나 Obsoleted는 이전 Status가 없음, 즉 Status 변경이 불가능 함, Revise하거나 다시 생성해야하는 프로세스
            if (currentStatus.Name.Equals(releaseStatusName, StringComparison.OrdinalIgnoreCase) || currentStatus.Name.Equals(obsoleteStatusName, StringComparison.OrdinalIgnoreCase))
                return null;

            return session.QueryOver<SPStatus>().Where(s => s.ObjectType == currentStatus.ObjectType && s.Ordinal < currentStatus.Ordinal).OrderBy(s => s.Ordinal).Desc.Take(1).SingleOrDefault();
        }

        public SPStatus GetInitialStatus(SPObjectTypes objectType)
        {
            return session.QueryOver<SPStatus>().Where(s => s.ObjectType == objectType).OrderBy(s=>s.Ordinal).Asc.Take(1).SingleOrDefault();
        }

        public SPStatus GetReleaseStatus(SPObjectTypes objectType)
        {
            string releaseStatusName = GetReleaseStatusName(objectType);

            if (string.IsNullOrWhiteSpace(releaseStatusName))
                return null;

            return GetStatusByName(objectType, releaseStatusName);
        }

        public SPStatus GetObsoleteStatus(SPObjectTypes objectType)
        {
            string obsoleteStatusName = GetObsoleteStatusName(objectType);

            if (string.IsNullOrWhiteSpace(obsoleteStatusName))
                return null;

            return GetStatusByName(objectType, obsoleteStatusName);
        }

        public SPStatus GetStatusByName(SPObjectTypes objectType, string statusName)
        {
            return session.QueryOver<SPStatus>().Where(s => s.ObjectType == objectType && s.Name == statusName).SingleOrDefault();
        }

        public SPStatusList GetAllAsList()
        {
            return new SPStatusList(GetAll().List());
        }

        #region implemented explicitly

        IEntityList<SPStatus> IRepository<SPStatus>.GetAllAsList()
        {
            return GetAllAsList();
        }

        #endregion

        #endregion


        #region internal methods

        internal IQueryOver<SPStatus> GetAll()
        {
            return session.QueryOver<SPStatus>();
        }

        #endregion

        #region private methods

        private string GetReleaseStatusName(SPObjectTypes objectType)
        {
            string releaseStatusName = string.Empty;

            switch (objectType)
            {
                case SPObjectTypes.Project:
                case SPObjectTypes.Work:
                case SPObjectTypes.Process:
                case SPObjectTypes.Activity:
                    releaseStatusName = "Completed";
                    break;
                case SPObjectTypes.Document:
                case SPObjectTypes.Dataset:
                case SPObjectTypes.Item:
                    releaseStatusName = "Released";
                    break;
                case SPObjectTypes.Template:
                    releaseStatusName = "Approved";
                    break;
                case SPObjectTypes.People:
                    releaseStatusName = "Enabled";
                    break;
                case SPObjectTypes.Unknown:
                case SPObjectTypes.File:
                case SPObjectTypes.Folder:
                case SPObjectTypes.Role:
                case SPObjectTypes.Organization:
                case SPObjectTypes.RelationshipType:
                case SPObjectTypes.ProjectRole:
                default:
                    releaseStatusName = string.Empty;
                    break;
            }

            return releaseStatusName;
        }

        private string GetObsoleteStatusName(SPObjectTypes objectType)
        {
            string obsoleteStatusName = string.Empty;

            switch (objectType)
            {
                case SPObjectTypes.Project:
                case SPObjectTypes.Work:
                case SPObjectTypes.Process:
                    obsoleteStatusName = "Cancelled";
                    break;
                case SPObjectTypes.Document:
                case SPObjectTypes.Dataset:
                case SPObjectTypes.Item:
                case SPObjectTypes.Template:
                    obsoleteStatusName = "Obsoleted";
                    break;
                case SPObjectTypes.People:
                    obsoleteStatusName = "Disabled";
                    break;
                case SPObjectTypes.Unknown:
                case SPObjectTypes.Activity:
                case SPObjectTypes.File:
                case SPObjectTypes.Folder:
                case SPObjectTypes.Role:
                case SPObjectTypes.Organization:
                case SPObjectTypes.RelationshipType:
                case SPObjectTypes.ProjectRole:
                default:
                    obsoleteStatusName = string.Empty;
                    break;
            }

            return obsoleteStatusName;
        }

        #endregion
    }
}
