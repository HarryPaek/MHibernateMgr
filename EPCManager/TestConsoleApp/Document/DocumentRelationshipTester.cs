using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Document;
using EPCManager.Domain.Abstract.Services;
using EPCManager.Domain.Entities;
using EPCManager.Repositories.Common;
using EPCManager.Repositories.Document;
using EPCManager.Repositories.Services;
using NHibernate;
using System;
using System.Linq;
using TestConsoleApp.Abstract;
using TestConsoleApp.Helpers;

namespace TestConsoleApp.Document
{
    internal class DocumentRelationshipTester : AbstractEPCManagerTester
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
            Run(TestGetRelationshipForSpecificDocument);
            Run(TestGetRelationshipWithAdditionalGetForAllDocument);
            // Run(TestGetRelationshipForAllDocument);
        }

        #region Test Methods

        private void TestGetRelationshipForSpecificDocument()
        {
            ScreenOutputDecorator.PrintHeader("Get A Document with Relationship");
            var document = repository.Get(1);

            PrintDocument(document);

            ScreenOutputDecorator.PrintFooter();
        }

        /// <summary>
        /// 단순 리스트 조회는 Relationship을 포함하지 않음
        /// </summary>
        private void TestGetRelationshipForAllDocument()
        {
            ScreenOutputDecorator.PrintHeader("Get All Documents WITHOUT Relationship");
            foreach (var document in repository.Items.Where(x => x.OId < 6).OrderBy(x => x.OId))
            {
                PrintDocument(document);
            }

            ScreenOutputDecorator.PrintFooter();
        }


        /// <summary>
        /// 반드시 별도 Get(id)를 거쳐야 Relationship을 포함함
        /// 주의할 사항은, HN Cache로직에 의하여 별도 Update 없이도 기존 조회된 객체에 Update된다.
        /// </summary>
        private void TestGetRelationshipWithAdditionalGetForAllDocument()
        {
            ScreenOutputDecorator.PrintHeader("Get All Documents WITH Relationship");
            foreach (var document in repository.Items.Where(x => x.OId < 6).OrderBy(x => x.OId))
            {
                repository.Get(document.OId);
                PrintDocument(document);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        #endregion

        #region Common Method

        private void PrintDocument(SPDocument document)
        {
            ScreenOutputDecorator.PrintHeader(string.Format("Selected document [{0}]", document.ToReferenceString()));

            Console.WriteLine(new SPDocumentPresenter(document));

            ScreenOutputDecorator.PrintSubListHeader(string.Format("Relationship List in document [{0}, {1}]", document.OId, document.Relationships.Count));

            foreach (SPRelationship rel in document.Relationships)
            {
                var relationshipPresenter = new SPRelationshipPresenter(rel);
                Console.WriteLine(relationshipPresenter);
            }

            Console.WriteLine();
        }

        #endregion
    }
}
