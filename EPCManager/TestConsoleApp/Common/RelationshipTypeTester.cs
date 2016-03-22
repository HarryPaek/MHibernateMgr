using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Entities;
using EPCManager.Repositories.Common;
using NHibernate;
using System;
using System.Linq;
using TestConsoleApp.Abstract;
using TestConsoleApp.Helpers;

namespace TestConsoleApp.Common
{
    internal class RelationshipTypeTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IRelationshipTypeRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();
            this.repository = new RelationshipTypeRepository(session, logManager);
        }

        protected override void After()
        {
            this.repository = null;
            NHibernateSessionManager.CloseSession();
        }

        public override void RunTest()
        {
            Run(TestGetAllItems);
            Run(TestGetAllAsList);
        }

        #region Test Methods

        private void TestGetAllItems()
        {
            ScreenOutputDecorator.PrintHeader("RelationshipType List");

            foreach (var relType in repository.Items)
            {
                var relTypePresenter = new SPRelationshipTypePresenter(relType);
                Console.WriteLine(relTypePresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("RelationshipType List As SPRelationshipTypeList & Sorted By Name");
            var relTypeList = repository.GetAllAsList();

            foreach (var relType in relTypeList.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
            {
                var relTypePresenter = new SPRelationshipTypePresenter(relType);
                Console.WriteLine(relTypePresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
