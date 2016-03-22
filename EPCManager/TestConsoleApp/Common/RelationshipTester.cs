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
    internal class RelationshipTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IRelationshipRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();
            this.repository = new RelationshipRepository(session, logManager);
        }

        protected override void After()
        {
            this.repository = null;
            NHibernateSessionManager.CloseSession();
        }

        public override void RunTest()
        {
            Run(TestGetANewRelationship);
            Run(TestGetAllItems);
            //Run(TestGetAllAsList);
        }

        #region Test Methods

        private void TestGetANewRelationship()
        {
            ScreenOutputDecorator.PrintHeader("Get A Relationship");
            var relationship = repository.Get(6);
            var relationshipPresenter = new SPRelationshipPresenter(relationship);

            Console.WriteLine(relationshipPresenter);

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestGetAllItems()
        {
            ScreenOutputDecorator.PrintHeader("Relationship List");
            var relationshipList = repository.GetAll(SPObjectTypes.Document, 1);

            foreach (var relationship in relationshipList)
            {
                var relationshipPresenter = new SPRelationshipPresenter(relationship);
                Console.WriteLine(relationshipPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("Relationship List As SPRelationshipTypeList & Sorted By Created Date Desc");
            var relationshipList = repository.GetAllAsList();

            foreach (var relationship in relationshipList.OrderByDescending(x => x.CreatedDate))
            {
                var relationshipPresenter = new SPRelationshipPresenter(relationship);
                Console.WriteLine(relationshipPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
