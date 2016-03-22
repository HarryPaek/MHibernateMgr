using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Document;
using EPCManager.Domain.Abstract.Operations;
using EPCManager.Domain.Abstract.Services;
using EPCManager.Domain.Constants;
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
    internal class DocumentCopyTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private ITesterDependencyBlock testerDependencyBlock;
        private IDocumentRepository documentRepository;
        private IDocumentCopyRepository repository;

        protected override void Before()
        {
            ISession session = NHibernateSessionManager.GetCurrentSession();

            IClassRepository classRepository = new ClassRepository(session, logManager);
            IIdentifierRepository identifierRepository = new IdentifierRepository(session, logManager);
            IDomainRepository domainRepository = new DomainRepository(session, logManager, identifierRepository);
            IPeopleRepository peopleRepository = new PeopleRepository(session, logManager);
            IPhaseRepository phaseRepository = new PhaseRepository(session, logManager);
            IStatusRepository statusRepository = new StatusRepository(session, logManager);
            IRevisionProvider revisionProvider = new DefaultRevisionProvider(logManager);
            IRelationshipRepository relationshipRepository = new RelationshipRepository(session, logManager);

            this.testerDependencyBlock = new TesterDependencyBlock(classRepository, domainRepository, identifierRepository, peopleRepository, phaseRepository, statusRepository);
            this.documentRepository = new DocumentRepository(session, logManager, identifierRepository, statusRepository, relationshipRepository);
            this.repository = new DocumentCopyRepository(logManager, this.documentRepository, revisionProvider, statusRepository);
        }

        protected override void After()
        {
            this.repository = null;
            this.documentRepository = null;
            this.testerDependencyBlock = null;

            NHibernateSessionManager.CloseSession();
        }

        public override void RunTest()
        {
            Run(TestCopyDocument);
            Run(TestCopyDocumentCheckedOutByOtherUser);
            Run(TestCopyReleasedDocument);
        }

        #region Test Methods

        private void TestCopyDocument()
        {
            ScreenOutputDecorator.PrintHeader("Copy Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-CPF-{0}", commonText));
            var newId = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "CP-TEST", string.Format("HXID-CPT-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Copy Test Original Document {0}", commonText),
                Status = status,
                Phase = phase,
                Identifier = id,
                Template = 'N',
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                documentRepository.Add(document);
                Console.WriteLine(new SPDocumentPresenter(document));

                var newQueriedDoument = documentRepository.Get(document.OId);

                if (newQueriedDoument == null)
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}]", document.OId, document.Identifier.Code, document.Revision));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Copy document");
                Console.ReadLine();

                SPDocument newDocument = repository.Copy(newQueriedDoument, newId, OperationOptions.MainAttribute, creator);
                var copiedDoument = documentRepository.Get(newDocument.OId);

                if (copiedDoument == null)
                    throw new Exception(string.Format("Copied Document Not Found. [{0}, {1} Rev.{2}]", newDocument.OId, newDocument.Identifier.Code, newDocument.Revision));

                Console.WriteLine(new SPDocumentPresenter(copiedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestCopyDocumentCheckedOutByOtherUser()
        {
            ScreenOutputDecorator.PrintHeader("Promote Document Test CheckedOut by Other User");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var otherUser = testerDependencyBlock.GetTestPeople("EPCAdmin");
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-CPFCOBOU-{0}", commonText));
            var newId = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "CP-TEST", string.Format("HXID-CPTCOBOU-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Copy Test Origianl Document CheckedOut By Other User {0}", commonText),
                Status = status,
                Phase = phase,
                Identifier = id,
                Template = 'N',
                CreatedDate = DateTime.Now,
                CreatedBy = creator,
                CheckoutDate = DateTime.Now,
                CheckoutBy = creator,
            };

            try
            {
                documentRepository.Add(document);
                Console.WriteLine(new SPDocumentPresenter(document));

                var newQueriedDoument = documentRepository.Get(document.OId);

                if (newQueriedDoument == null)
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}]", document.OId, document.Identifier.Code, document.Revision));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to try Copy Document CheckedOut By other user");
                Console.ReadLine();

                SPDocument newDocument =  repository.Copy(newQueriedDoument, newId, OperationOptions.MainAttribute, otherUser);
                var copiedDoumentCOBOU = documentRepository.Get(newDocument.OId);

                if (copiedDoumentCOBOU == null)
                    throw new Exception(string.Format("Copied Document from CheckedOut By other user Not Found. [{0}, {1} Rev.{2}]", newDocument.OId, newDocument.Identifier.Code, newDocument.Revision));

                Console.WriteLine(new SPDocumentPresenter(copiedDoumentCOBOU));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestCopyReleasedDocument()
        {
            ScreenOutputDecorator.PrintHeader("Copy Released Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var creator = testerDependencyBlock.GetTestPeople();
            var newId = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "CP-TEST", string.Format("HXID-CPTREL-{0}", commonText));

            try
            {
                var releasedDocument = documentRepository.Items.FirstOrDefault(d => d.Status.Name.Equals("Released", StringComparison.OrdinalIgnoreCase));
                if (releasedDocument == null)
                    throw new Exception("Released Document Not Found!");

                Console.WriteLine(new SPDocumentPresenter(releasedDocument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to copy released document");
                Console.ReadLine();

                SPDocument newDocument =  repository.Copy(releasedDocument, newId, OperationOptions.MainAttribute, creator);
                var copiedDoumentREL = documentRepository.Get(newDocument.OId);

                if (copiedDoumentREL == null)
                    throw new Exception(string.Format("Copied Document from Released Not Found. [{0}, {1} Rev.{2}, {3}]", newDocument.OId, newDocument.Identifier.Code, newDocument.Revision, newDocument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(copiedDoumentREL));
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
