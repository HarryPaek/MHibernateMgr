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
    internal class ClassTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IClassRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();
            this.repository = new ClassRepository(session, logManager);
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
            ScreenOutputDecorator.PrintHeader("Class List");

            foreach (var cls in repository.Items)
            {
                var classPresenter = new SPClassPresenter(cls);
                Console.WriteLine(classPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("Document Class List As SPClassList");
            var classList = repository.GetAllAsList();

            foreach (var cls in classList.Where(x => x.ObjectType == SPObjectTypes.Document).OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
            {
                var classPresenter = new SPClassPresenter(cls);
                Console.WriteLine(classPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
