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
    internal class DomainTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IDomainRepository repository;
        private IPeopleRepository peopleRepository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();

            IIdentifierRepository identifierRepository = new IdentifierRepository(session, logManager);
            this.peopleRepository = new PeopleRepository(session, logManager);
            this.repository = new DomainRepository(session, logManager, identifierRepository);
        }

        protected override void After()
        {
            this.repository = null;
            this.peopleRepository = null;

            NHibernateSessionManager.CloseSession();
        }

        public override void RunTest()
        {
            Run(TestGetAllItems);
            Run(TestAddDomain);
            Run(TestGetAllAsList);
            Run(TestGetAllAsListAndBasicSort);
        }

        #region Test Methods

        private void TestGetAllItems()
        {
            ScreenOutputDecorator.PrintHeader("Domain List");

            foreach (var domain in repository.Items)
            {
                var domainPresenter = new SPDomainPresenter(domain);
                Console.WriteLine(domainPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsList()
        {
            ScreenOutputDecorator.PrintHeader("Domain List As SPDomainList");
            var domainList = repository.GetAllAsList();

            foreach (var domain in domainList.OrderBy(x => x.Code, StringComparer.OrdinalIgnoreCase))
            {
                var domainPresenter = new SPDomainPresenter(domain);
                Console.WriteLine(domainPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestGetAllAsListAndBasicSort()
        {
            ScreenOutputDecorator.PrintHeader("Domain List As SPDomainList & Basic Sort By Default Key in SPDomain");
            var domainList = repository.GetAllAsList();
            domainList.Sort();

            foreach (var domain in domainList)
            {
                var domainPresenter = new SPDomainPresenter(domain);
                Console.WriteLine(domainPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestAddDomain()
        {
            ScreenOutputDecorator.PrintHeader("Add Domain Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var creator = peopleRepository.Items.Where(x => x.FirstName.Equals("Harry", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (creator == null)
                creator = new SPPeople
                {
                    OId = 1  // System
                };

            var domain = new SPDomain
            {
                OId = 0,
                Code = string.Format("HXD-{0}", commonText),
                Description = string.Format("Harry Test Domain {0}", commonText),
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                repository.Add(domain);
                Console.WriteLine(new SPDomainPresenter(domain));

                var newQueriedDomain = repository.Get(domain.OId);

                if (newQueriedDomain == null)
                    throw new Exception(string.Format("Domain Not Found. [{0}][{1}]", domain.OId, domain.Code));

                Console.WriteLine(new SPDomainPresenter(newQueriedDomain));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
