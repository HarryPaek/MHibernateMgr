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
    internal class StatusTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IStatusRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();
            this.repository = new StatusRepository(session, logManager);
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
            Run(TestGetInitialStatus);
            Run(TestGetNextStatus);
            Run(TestGetPrevisouStatus);
        }

        #region Test Methods

        private void TestGetAllItems()
        {
            ScreenOutputDecorator.PrintHeader("Status List");

            foreach (var status in repository.Items)
            {
                var statusPresenter = new SPStatusPresenter(status);
                Console.WriteLine(statusPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("Status List As SPStatusList");
            var statusList = repository.GetAllAsList();

            foreach (var status in statusList.Where(x => x.ObjectType != SPObjectTypes.Document).OrderBy(x => x.ObjectType).ThenByDescending(x => x.Ordinal))
            {
                var statusPresenter = new SPStatusPresenter(status);
                Console.WriteLine(statusPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestGetInitialStatus()
        {
            ScreenOutputDecorator.PrintHeader("Initial Status List");

            foreach (SPObjectTypes objectType in Enum.GetValues(typeof(SPObjectTypes)))
            {
                var statusPresenter = new SPStatusPresenter(repository.GetInitialStatus(objectType));
                Console.WriteLine(string.Format("\n{0}'s Initial Status = [{1}]\n", objectType, statusPresenter));
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestGetNextStatus()
        {
            ScreenOutputDecorator.PrintHeader("Test GetNextStatus()");

            foreach (SPObjectTypes objectType in Enum.GetValues(typeof(SPObjectTypes)))
            {
                SPStatus initialStatus = repository.GetInitialStatus(objectType);
                if(initialStatus != null)
                {
                    var statusPresenter = new SPStatusPresenter(repository.GetNextStatus(initialStatus));
                    Console.WriteLine(string.Format("\n{0}'s next status for [{1}]  = [{2}]\n", objectType, initialStatus.Name, statusPresenter));
                }
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestGetPrevisouStatus()
        {
            ScreenOutputDecorator.PrintHeader("Test GetPrevisouStatus()");

            foreach (SPObjectTypes objectType in Enum.GetValues(typeof(SPObjectTypes)))
            {
                SPStatus initialStatus = repository.GetInitialStatus(objectType);
                if (initialStatus != null)
                {
                    var nextstatus = repository.GetNextStatus(initialStatus);
                    if (nextstatus != null)
                    {
                        var statusPresenter = new SPStatusPresenter(repository.GetPrevisouStatus(nextstatus));
                        Console.WriteLine(string.Format("\n{0}'s previous status for [{1}]  = [{2}]\n", objectType, nextstatus.Name, statusPresenter));
                    }
                }
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
