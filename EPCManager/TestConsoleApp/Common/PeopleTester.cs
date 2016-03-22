using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Repositories.Common;
using NHibernate;
using System;
using System.Linq;
using TestConsoleApp.Abstract;
using TestConsoleApp.Helpers;

namespace TestConsoleApp.Common
{
    internal class PeopleTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IPeopleRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();
            this.repository = new PeopleRepository(session, logManager);
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
            ScreenOutputDecorator.PrintHeader("People List");

            foreach (var people in repository.Items)
            {
                var peoplePresenter = new SPPeoplePresenter(people);
                Console.WriteLine(peoplePresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("People List As SPPeopleList");
            var peopleList = repository.GetAllAsList();

            foreach (var people in peopleList.OrderBy(x => x.FirstName, StringComparer.CurrentCultureIgnoreCase).ThenByDescending(x => x.Identifier.Code))
            {
                var peoplePresenter = new SPPeoplePresenter(people);
                Console.WriteLine(peoplePresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
