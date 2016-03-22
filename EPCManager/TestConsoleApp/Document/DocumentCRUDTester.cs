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
    internal class DocumentCRUDTester : AbstractEPCManagerTester
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
            Run(TestGetAllItems);
            Run(TestAddDocument);
            Run(TestAddDeleteDocument);
            Run(TestAddUpdateDocument);
            Run(TestUpdateReleasedDocument);
            Run(TestDeleteRevisedDocument);
        }

        #region Test Methods

        private void TestGetAllItems()
        {
            ScreenOutputDecorator.PrintHeader("Document List");

            foreach (var document in repository.Items)
            {
                var documentPresenter = new SPDocumentPresenter(document);
                Console.WriteLine(documentPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestAddDocument()
        {
            ScreenOutputDecorator.PrintHeader("Add Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-C-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Add Test Document {0}", commonText),
                Status = status,
                Phase = phase,
                Identifier = id,
                Template = 'N',
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                repository.Add(document);
                Console.WriteLine(new SPDocumentPresenter(document));

                var newQueriedDoument = repository.Get(document.OId);

                if (newQueriedDoument == null)
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}]", document.OId, document.Identifier.Code, document.Revision));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestAddUpdateDocument()
        {
            ScreenOutputDecorator.PrintHeader("Add Update Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-CU-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Add Update Test Document {0}", commonText),
                Status = status,
                Phase = phase,
                Identifier = id,
                Template = 'N',
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                repository.Add(document);
                Console.WriteLine(new SPDocumentPresenter(document));

                var newQueriedDoument = repository.Get(document.OId);

                if (newQueriedDoument == null)
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}]", document.OId, document.Identifier.Code, document.Revision));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to update document");
                Console.ReadLine();

                newQueriedDoument.Description = string.Format("{0} Updated", newQueriedDoument.Description);
                newQueriedDoument.ModifiedBy = creator;
                newQueriedDoument.ModifiedDate = DateTime.Now;

                repository.Update(newQueriedDoument);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestUpdateReleasedDocument()
        {
            ScreenOutputDecorator.PrintHeader("Update Released Document Test");
            var creator = testerDependencyBlock.GetTestPeople();

            try
            {
                var releasedDocument = repository.Items.FirstOrDefault(d => d.Status.Name.Equals("Released", StringComparison.OrdinalIgnoreCase));

                if (releasedDocument == null)
                    throw new Exception("Released Document Not Found!");

                Console.WriteLine(new SPDocumentPresenter(releasedDocument));

                releasedDocument.Description = string.Format("{0} Released Document Updated", releasedDocument.Description);
                releasedDocument.ModifiedBy = creator;
                releasedDocument.ModifiedDate = DateTime.Now;

                repository.Update(releasedDocument);
                Console.WriteLine(new SPDocumentPresenter(releasedDocument));

                var newQueriedDoument = repository.Get(releasedDocument.OId);

                if (newQueriedDoument == null)
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}]", releasedDocument.OId, releasedDocument.Identifier.Code, releasedDocument.Revision));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /**
         * 일반적인 Document의 경우, Document Revision과 Identifier가 동시에 삭제된다.
         */
        private void TestAddDeleteDocument()
        {
            ScreenOutputDecorator.PrintHeader("Add Delete Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-CD-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Add Delete Test Document {0}", commonText),
                Status = status,
                Phase = phase,
                Identifier = id,
                Template = 'N',
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                repository.Add(document);
                Console.WriteLine(new SPDocumentPresenter(document));

                var newQueriedDoument = repository.Get(document.OId);

                if (newQueriedDoument == null)
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}]", document.OId, document.Identifier.Code, document.Revision));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to delete document");
                Console.ReadLine();

                repository.Delete(newQueriedDoument);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /**
         * Revised Document의 경우, Document Revision만 삭제되고 Identifier는 삭제되지 않아야 한다.
         */
        private void TestDeleteRevisedDocument()
        {
            ScreenOutputDecorator.PrintHeader("Delete Revised Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var statusReleased = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document, "Released");
            var statusPlanned = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document, "Planned");
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-CD-{0}", commonText));

            var releasedDocument = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Delete Test Revised Document {0}", commonText),
                Status = statusReleased,
                Phase = phase,
                Identifier = id,
                Template = 'N',
                CreatedDate = DateTime.Now,
                CreatedBy = creator,
                ModifiedDate = DateTime.Now,
                ModifiedBy = creator,
                CompletedDate = DateTime.Now,
                CompletedBy = creator
            };

            var revBDocument = new SPDocument
            {
                OId = 0,
                Revision = "B",
                RevisionSortSequence = 2,
                Status = statusPlanned,
                Template = 'N',
                CreatedDate = DateTime.Now,
                CreatedBy = creator,
            };

            try
            {
                repository.Add(releasedDocument);
                Console.WriteLine(new SPDocumentPresenter(releasedDocument));

                var newQueriedDoument = repository.Get(releasedDocument.OId);

                if (newQueriedDoument == null)
                    throw new Exception(string.Format("Released document Not Found. [{0}, {1} Rev.{2}]", releasedDocument.OId, releasedDocument.Identifier.Code, releasedDocument.Revision));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to revise document");
                Console.ReadLine();

                revBDocument.Class = newQueriedDoument.Class;
                revBDocument.Description = string.Format("{0} - Rev. B", newQueriedDoument.Description);
                revBDocument.Phase = newQueriedDoument.Phase;
                revBDocument.Identifier = newQueriedDoument.Identifier;

                repository.Add(revBDocument);

                var revisedDoument = repository.Get(revBDocument.OId);

                if (revisedDoument == null)
                    throw new Exception(string.Format("Revised document Not Found. [{0}, {1} Rev.{2}]", revBDocument.OId, revBDocument.Identifier.Code, revBDocument.Revision));

                Console.WriteLine(new SPDocumentPresenter(revisedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to delete revised document");
                Console.ReadLine();

                repository.Delete(revisedDoument);

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to display remaining released document");
                Console.ReadLine();

                var remainingDoument = repository.Get(releasedDocument.OId);

                if (remainingDoument == null)
                    throw new Exception(string.Format("Remaining Released document Not Found. [{0}, {1} Rev.{2}]", releasedDocument.OId, releasedDocument.Identifier.Code, releasedDocument.Revision));

                Console.WriteLine(new SPDocumentPresenter(remainingDoument));
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
