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
    internal class IdentifierTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IIdentifierRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();
            this.repository = new IdentifierRepository(session, logManager);
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
            Run(TestGetAllAsListAndBasicSort);
        }

        #region Test Methods

        private void TestGetAllItems()
        {
            ScreenOutputDecorator.PrintHeader("Identiifer List");

            foreach (var identifier in repository.Items)
            {
                var identifierPresenter = new SPIdentifierPresenter(identifier);
                Console.WriteLine(identifierPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("Identifier List As SPIdentifierList");
            var identifierList = repository.GetAllAsList();

            foreach (var identifier in identifierList.Where(x => x.ObjectType != SPObjectTypes.Document)
                                                     .OrderBy(x => x.ObjectType)
                                                     .ThenBy(x => x.Domain.Code, StringComparer.OrdinalIgnoreCase)
                                                     .ThenByDescending(x => x.Code))
            {
                var identifierPresenter = new SPIdentifierPresenter(identifier);
                Console.WriteLine(identifierPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestGetAllAsListAndBasicSort()
        {
            ScreenOutputDecorator.PrintHeader("Identifier List As SPIdentifierList & Basic Sort By Default Key in SPIdentifier");
            var identifierList = repository.GetAllAsList();
            identifierList.Sort();

            foreach (var identifier in identifierList)
            {
                var identifierPresenter = new SPIdentifierPresenter(identifier);
                Console.WriteLine(identifierPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
