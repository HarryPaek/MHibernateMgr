using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Item;
using EPCManager.Domain.Entities;
using EPCManager.Repositories.Common;
using EPCManager.Repositories.Item;
using NHibernate;
using System;
using System.Linq;
using TestConsoleApp.Abstract;
using TestConsoleApp.Helpers;

namespace TestConsoleApp.Item
{
    internal class SPItemListTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private ITesterDependencyBlock testerDependencyBlock;
        private IItemRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();

            IClassRepository classRepository = new ClassRepository(session, logManager);
            IIdentifierRepository identifierRepository = new IdentifierRepository(session, logManager);
            IDomainRepository domainRepository = new DomainRepository(session, logManager, identifierRepository);
            IPeopleRepository peopleRepository = new PeopleRepository(session, logManager);
            IPhaseRepository phaseRepository = new PhaseRepository(session, logManager);
            IStatusRepository statusRepository = new StatusRepository(session, logManager);
            IRelationshipRepository relationshipRepository = new RelationshipRepository(session, logManager);

            this.testerDependencyBlock = new TesterDependencyBlock(classRepository, domainRepository, identifierRepository, peopleRepository, phaseRepository, statusRepository);
            this.repository = new ItemRepository(session, logManager, identifierRepository, statusRepository, relationshipRepository);
        }

        protected override void After()
        {
            this.repository = null;
            this.testerDependencyBlock = null;

            NHibernateSessionManager.CloseSession();
        }

        public override void RunTest()
        {
            Run(TestGetAllAsListAndBasicSort);
            Run(TestGetAllAsListAndSortByCreatedDateDesc);
            Run(TestGetAllAsListAndFilterReleased);
            Run(TestGetAllAsListAndFilterInWorkAndSortByClassAndModifiedDateDesc);
            Run(TestGetAllAsListAndFilterInitialStatusAndSortByClassAndModifiedDateDesc);
        }

        #region Test Methods

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndBasicSort()
        {
            ScreenOutputDecorator.PrintHeader("Item List & Basic Sort By Default Key in SPItem");
            var itemList = repository.GetAllAsList();
            itemList.Sort();

            foreach (var item in itemList)
            {
                var itemPresenter = new SPItemPresenter(item);
                Console.WriteLine(itemPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndSortByCreatedDateDesc()
        {
            ScreenOutputDecorator.PrintHeader("Item List Sorted By Created Date Desc");
            var itemList = repository.GetAllAsList();

            foreach (var item in itemList.OrderByDescending(x => x.CreatedDate))
            {
                var itemPresenter = new SPItemPresenter(item);
                Console.WriteLine(itemPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndFilterReleased()
        {
            ScreenOutputDecorator.PrintHeader("Released Item List");
            var releasedStatus = testerDependencyBlock.StatusRepository.GetReleaseStatus(SPObjectTypes.Item);
            var itemList = repository.GetAllAsList();

            foreach (var item in itemList.Where(x => x.IsStatusAt(releasedStatus)))
            {
                var itemPresenter = new SPItemPresenter(item);
                Console.WriteLine(itemPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndFilterInWorkAndSortByClassAndModifiedDateDesc()
        {
            ScreenOutputDecorator.PrintHeader("In-Work Item List Sorted By Class and ModifiedDate Desc");
            var inworkStatus = testerDependencyBlock.StatusRepository.GetStatusByName(SPObjectTypes.Item, "InWork");
            var itemList = repository.GetAllAsList();

            foreach (var item in itemList.Where(x => x.IsStatusAt(inworkStatus)).OrderBy(x => x.Class).ThenByDescending(x => x.ModifiedDate))
            {
                var itemPresenter = new SPItemPresenter(item);
                Console.WriteLine(itemPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndFilterInitialStatusAndSortByClassAndModifiedDateDesc()
        {
            ScreenOutputDecorator.PrintHeader("Initial Status Item List Sorted By Class and ModifiedDate Desc");
            var initialStatus = testerDependencyBlock.StatusRepository.GetInitialStatus(SPObjectTypes.Item);
            var itemList = repository.GetAllAsList();

            foreach (var item in itemList.Where(x => x.IsStatusAt(initialStatus)).OrderBy(x => x.Class).ThenByDescending(x => x.ModifiedDate))
            {
                var itemPresenter = new SPItemPresenter(item);
                Console.WriteLine(itemPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
