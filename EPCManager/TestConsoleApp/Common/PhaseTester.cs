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
    internal class PhaseTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IPhaseRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();
            this.repository = new PhaseRepository(session, logManager);
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
            ScreenOutputDecorator.PrintHeader("Phase List");

            foreach (var phase in repository.Items)
            {
                var phasePresenter = new SPPhasePresenter(phase);
                Console.WriteLine(phasePresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("Phase List As SPPhaseList");
            var phaseList = repository.GetAllAsList();

            foreach (var phase in phaseList.Where(x => x.ObjectType == SPObjectTypes.Document).OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
            {
                var phasePresenter = new SPPhasePresenter(phase);
                Console.WriteLine(phasePresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
