using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Document;
using EPCManager.Domain.Entities;
using EPCManager.Repositories.Common;
using EPCManager.Repositories.Document;
using NHibernate;
using System;
using System.Linq;
using TestConsoleApp.Abstract;
using TestConsoleApp.Helpers;

namespace TestConsoleApp.Document
{
    internal class SPDocumentListTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private ITesterDependencyBlock testerDependencyBlock;
        private IDocumentRepository repository;

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
            this.repository = new DocumentRepository(session, logManager, identifierRepository, statusRepository, relationshipRepository);
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
            Run(TestGetAllAsListAndFilterInWorkAndSortByPhaseAndModifiedDateDesc);
        }

        #region Test Methods

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndBasicSort()
        {
            ScreenOutputDecorator.PrintHeader("Document List & Basic Sort By Default Key in SPDocument");
            var documentList = repository.GetAllAsList();
            documentList.Sort();

            foreach (var document in documentList)
            {
                var documentPresenter = new SPDocumentPresenter(document);
                Console.WriteLine(documentPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndSortByCreatedDateDesc()
        {
            ScreenOutputDecorator.PrintHeader("Document List Sorted By Created Date Desc");
            var documentList = repository.GetAllAsList();

            foreach (var document in documentList.OrderByDescending(x => x.CreatedDate))
            {
                var documentPresenter = new SPDocumentPresenter(document);
                Console.WriteLine(documentPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndFilterReleased()
        {
            ScreenOutputDecorator.PrintHeader("Released Document List");
            var releasedStatus = testerDependencyBlock.StatusRepository.GetReleaseStatus(SPObjectTypes.Document);
            var documentList = repository.GetAllAsList();

            foreach (var document in documentList.Where(x => x.IsStatusAt(releasedStatus)))
            {
                var documentPresenter = new SPDocumentPresenter(document);
                Console.WriteLine(documentPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// Extended Method를 사용하기 위해서는 반드시 "using System.Linq;"해야함
        /// </summary>
        private void TestGetAllAsListAndFilterInWorkAndSortByPhaseAndModifiedDateDesc()
        {
            ScreenOutputDecorator.PrintHeader("In-Work Document List Sorted By Phase and ModifiedDate Desc");
            var inworkStatus = testerDependencyBlock.StatusRepository.GetStatusByName(SPObjectTypes.Document, "InWork");
            var documentList = repository.GetAllAsList();

            foreach (var document in documentList.Where(x => x.IsStatusAt(inworkStatus)).OrderBy(x => x.Phase.Name, StringComparer.OrdinalIgnoreCase).ThenByDescending(x => x.ModifiedDate))
            {
                var documentPresenter = new SPDocumentPresenter(document);
                Console.WriteLine(documentPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion
    }
}
