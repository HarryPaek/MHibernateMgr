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
    internal class ItemRelationshipTester : AbstractEPCManagerTester
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
            Run(TestGetRelationshipForSpecificItem);
            Run(TestGetRelationshipForAllItems);
        }

        #region Test Methods

        private void TestGetRelationshipForSpecificItem()
        {
            ScreenOutputDecorator.PrintHeader("Get A Item with Relationship");
            var item = repository.Get(2);

            PrintItem(item);

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestGetRelationshipForAllItems()
        {
            ScreenOutputDecorator.PrintHeader("Get All Item with Relationship");
            foreach (var item in repository.Items.OrderBy(x => x.OId))
            {
                repository.Get(item.OId);
                PrintItem(item);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion

        #region Common Method

        private void PrintItem(SPItem item)
        {
            ScreenOutputDecorator.PrintHeader(string.Format("Selected Item [{0}]", item.ToReferenceString()));

            Console.WriteLine(new SPItemPresenter(item));

            ScreenOutputDecorator.PrintSubListHeader(string.Format("Relationship list in item [{0}, {1}]", item.OId, item.Relationships.Count));

            foreach (SPRelationship rel in item.Relationships)
            {
                var relationshipPresenter = new SPRelationshipPresenter(rel);
                Console.WriteLine(relationshipPresenter);
            }
            Console.WriteLine();
        }

        #endregion
    }
}
