using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Document;
using EPCManager.Domain.Abstract.Operations;
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
    internal class DocumentCheckOutCheckInTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private ITesterDependencyBlock testerDependencyBlock;
        private IDocumentRepository documentRepository;
        private ICheckOutRepository repository;

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
            this.documentRepository = new DocumentRepository(session, logManager, identifierRepository, statusRepository, relationshipRepository);
            this.repository = new DocumentCheckOutRepository(logManager, this.documentRepository);
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
            Run(TestCheckOutDocument);
            Run(TestCheckOutDocumentAgainWithOtherUser);
            Run(TestCheckOutReleasedDocument);
            Run(TestCheckOutCheckInDocument);
            Run(TestCheckOutCheckInDocumentWithOtherUser);
        }

        #region Test Methods

        private void TestCheckOutDocument()
        {
            ScreenOutputDecorator.PrintHeader("CheckOut Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-CO-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry CheckOut Test Document {0}", commonText),
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

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to checkout document");
                Console.ReadLine();

                repository.CheckOut(newQueriedDoument, creator);
                var checkoutDoument = documentRepository.Get(newQueriedDoument.OId);

                if (checkoutDoument == null)
                    throw new Exception(string.Format("CheckOut Document Not Found. [{0}, {1} Rev.{2}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkoutDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestCheckOutDocumentAgainWithOtherUser()
        {
            ScreenOutputDecorator.PrintHeader("CheckOut Document with other user Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var otherUser = testerDependencyBlock.GetTestPeople("EPCAdmin");
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-COWOU-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry CheckOut Test Document with Other User {0}", commonText),
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

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to checkout document");
                Console.ReadLine();

                repository.CheckOut(newQueriedDoument, creator);
                var checkoutDoument = documentRepository.Get(newQueriedDoument.OId);

                if (checkoutDoument == null)
                    throw new Exception(string.Format("CheckOut Document Not Found. [{0}, {1} Rev.{2}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkoutDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to try checkout document with other user");
                Console.ReadLine();

                repository.CheckOut(checkoutDoument, otherUser);
                var checkoutDoumentWOU = documentRepository.Get(checkoutDoument.OId);

                if (checkoutDoumentWOU == null)
                    throw new Exception(string.Format("CheckOut Document with other user Not Found. [{0}, {1} Rev.{2}]", checkoutDoument.OId, checkoutDoument.Identifier.Code, checkoutDoument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkoutDoumentWOU));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestCheckOutReleasedDocument()
        {
            ScreenOutputDecorator.PrintHeader("CheckOut Released Document Test");

            var creator = testerDependencyBlock.GetTestPeople();

            try
            {
                var releasedDocument = documentRepository.Items.FirstOrDefault(d => d.Status.Name.Equals("Released", StringComparison.OrdinalIgnoreCase));
                if (releasedDocument == null)
                    throw new Exception("Released Document Not Found!");

                Console.WriteLine(new SPDocumentPresenter(releasedDocument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to checkout released document");
                Console.ReadLine();

                repository.CheckOut(releasedDocument, creator);
                var checkoutDoument = documentRepository.Get(releasedDocument.OId);

                if (checkoutDoument == null)
                    throw new Exception(string.Format("CheckOut Document Not Found. [{0}, {1} Rev.{2}]", releasedDocument.OId, releasedDocument.Identifier.Code, releasedDocument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkoutDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestCheckOutCheckInDocument()
        {
            ScreenOutputDecorator.PrintHeader("CheckOut CheckIn Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-COI-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry CheckOut CheckIn Test Document {0}", commonText),
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

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to checkout document");
                Console.ReadLine();

                repository.CheckOut(newQueriedDoument, creator);
                var checkoutDoument = documentRepository.Get(newQueriedDoument.OId);

                if (checkoutDoument == null)
                    throw new Exception(string.Format("Checkout Document Not Found. [{0}, {1} Rev.{2}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkoutDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to checkin document");
                Console.ReadLine();

                repository.CheckIn(checkoutDoument, creator);
                var checkinDoument = documentRepository.Get(checkoutDoument.OId);

                if (checkinDoument == null)
                    throw new Exception(string.Format("Checkin Document Not Found. [{0}, {1} Rev.{2}]", checkoutDoument.OId, checkoutDoument.Identifier.Code, checkoutDoument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkinDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestCheckOutCheckInDocumentWithOtherUser()
        {
            ScreenOutputDecorator.PrintHeader("CheckOut CheckIn Document with other user Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var otherUser = testerDependencyBlock.GetTestPeople("EPCAdmin");
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-COIWOU-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry CheckOut CheckIn Test Document with Other User {0}", commonText),
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

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to checkout document");
                Console.ReadLine();

                repository.CheckOut(newQueriedDoument, creator);
                var checkoutDoument = documentRepository.Get(newQueriedDoument.OId);

                if (checkoutDoument == null)
                    throw new Exception(string.Format("Checkout Document Not Found. [{0}, {1} Rev.{2}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkoutDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to checkin document with Other User");
                Console.ReadLine();

                repository.CheckIn(checkoutDoument, otherUser);
                var checkinDoument = documentRepository.Get(checkoutDoument.OId);

                if (checkinDoument == null)
                    throw new Exception(string.Format("Checkin Document Not Found. [{0}, {1} Rev.{2}]", checkoutDoument.OId, checkoutDoument.Identifier.Code, checkoutDoument.Revision));

                Console.WriteLine(new SPDocumentPresenter(checkinDoument));
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
