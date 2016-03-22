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
    internal class DocumentLifeCycelTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private ITesterDependencyBlock testerDependencyBlock;
        private IDocumentRepository documentRepository;
        private ILifeCycleRepository repository;

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
            this.repository = new DocumentLifeCycleRepository(session, logManager, this.documentRepository, statusRepository);
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
            Run(TestPromoteDocument);
            Run(TestPromoteDocumentCheckedOutByOtherUser);
            Run(TestPromoteReleasedDocument);
            Run(TestPromoteDemoteDocument);
            Run(TestDemoteDocumentCheckedOutByOtherUser);
            Run(TestDemoteInitialStatusDocument);
            Run(TestDemoteReleasedDocument);
            Run(TestReleaseDocument);
            Run(TestObsoleteDocument);
            Run(TestObsoleteReleasedDocument);
        }

        #region Test Methods

        private void TestPromoteDocument()
        {
            ScreenOutputDecorator.PrintHeader("Promote Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-P-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Promote Test Document {0}", commonText),
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
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}, {3}]", document.OId, document.Identifier.Code, document.Revision, document.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Promote document");
                Console.ReadLine();

                repository.Promote(newQueriedDoument, creator);
                var promotedDoument = documentRepository.Get(newQueriedDoument.OId);

                if (promotedDoument == null)
                    throw new Exception(string.Format("Promoted Document Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(promotedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestPromoteDocumentCheckedOutByOtherUser()
        {
            ScreenOutputDecorator.PrintHeader("Promote Document Test CheckedOut by Other User");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var otherUser = testerDependencyBlock.GetTestPeople("EPCAdmin");
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-PCOBOU-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Promote Test Document CheckedOut By Other User {0}", commonText),
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
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}, {3}]", document.OId, document.Identifier.Code, document.Revision, document.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to try Promote Document CheckedOut By other user");
                Console.ReadLine();

                repository.Promote(newQueriedDoument, otherUser);
                var promotedDoumentCOBOU = documentRepository.Get(newQueriedDoument.OId);

                if (promotedDoumentCOBOU == null)
                    throw new Exception(string.Format("Promoted Document CheckedOut By other user Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(promotedDoumentCOBOU));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestPromoteReleasedDocument()
        {
            ScreenOutputDecorator.PrintHeader("Promote Released Document Test");

            var creator = testerDependencyBlock.GetTestPeople();

            try
            {
                var releasedDocument = documentRepository.Items.FirstOrDefault(d => d.Status.Name.Equals("Released", StringComparison.OrdinalIgnoreCase));
                if (releasedDocument == null)
                    throw new Exception("Released Document Not Found!");

                Console.WriteLine(new SPDocumentPresenter(releasedDocument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to promote released document");
                Console.ReadLine();

                repository.Promote(releasedDocument, creator);
                var promotedDoument = documentRepository.Get(releasedDocument.OId);

                if (promotedDoument == null)
                    throw new Exception(string.Format("Promoted Not Found. [{0}, {1} Rev.{2}, {3}]", releasedDocument.OId, releasedDocument.Identifier.Code, releasedDocument.Revision, releasedDocument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(promotedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestPromoteDemoteDocument()
        {
            ScreenOutputDecorator.PrintHeader("Promote Demote Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-PD-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Promote Demote Test Document {0}", commonText),
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
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}, {3}]", document.OId, document.Identifier.Code, document.Revision, document.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Promote document");
                Console.ReadLine();

                repository.Promote(newQueriedDoument, creator);
                var promotedDoument = documentRepository.Get(newQueriedDoument.OId);

                if (promotedDoument == null)
                    throw new Exception(string.Format("Promoted Document Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(promotedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Demote document");
                Console.ReadLine();

                repository.Demote(promotedDoument, creator);
                var domotedDoument = documentRepository.Get(promotedDoument.OId);

                if (domotedDoument == null)
                    throw new Exception(string.Format("Demoted Document Not Found. [{0}, {1} Rev.{2}, {3}]", promotedDoument.OId, promotedDoument.Identifier.Code, promotedDoument.Revision, promotedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(domotedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestDemoteDocumentCheckedOutByOtherUser()
        {
            ScreenOutputDecorator.PrintHeader("Demote Document Test CheckedOut by Other User");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var otherUser = testerDependencyBlock.GetTestPeople("EPCAdmin");
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-DCOBOU-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Dromote Test Document CheckedOut By Other User {0}", commonText),
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

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Promote document");
                Console.ReadLine();

                repository.Promote(newQueriedDoument, creator);
                var promotedDoument = documentRepository.Get(newQueriedDoument.OId);

                if (promotedDoument == null)
                    throw new Exception(string.Format("Promoted Document Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(promotedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to try Demote Document CheckedOut By other user");
                Console.ReadLine();

                repository.Demote(promotedDoument, otherUser);
                var demotedDoumentCOBOU = documentRepository.Get(promotedDoument.OId);

                if (demotedDoumentCOBOU == null)
                    throw new Exception(string.Format("Demoted Document CheckedOut By other user Not Found. [{0}, {1} Rev.{2}, {3}]", promotedDoument.OId, promotedDoument.Identifier.Code, promotedDoument.Revision, promotedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(demotedDoumentCOBOU));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestDemoteInitialStatusDocument()
        {
            ScreenOutputDecorator.PrintHeader("Demote Initial Status Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-DIS-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Demote Test With Initial Status Document {0}", commonText),
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
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}, {3}]", document.OId, document.Identifier.Code, document.Revision, document.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Demote document");
                Console.ReadLine();

                repository.Demote(newQueriedDoument, creator);
                var domotedDoument = documentRepository.Get(newQueriedDoument.OId);

                if (domotedDoument == null)
                    throw new Exception(string.Format("Demoted Document Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(domotedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestDemoteReleasedDocument()
        {
            ScreenOutputDecorator.PrintHeader("Demote Released Document Test");

            var creator = testerDependencyBlock.GetTestPeople();

            try
            {
                var releasedDocument = documentRepository.Items.FirstOrDefault(d => d.Status.Name.Equals("Released", StringComparison.OrdinalIgnoreCase));
                if (releasedDocument == null)
                    throw new Exception("Released Document Not Found!");

                Console.WriteLine(new SPDocumentPresenter(releasedDocument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to demote released document");
                Console.ReadLine();

                repository.Demote(releasedDocument, creator);
                var demotedDoument = documentRepository.Get(releasedDocument.OId);

                if (demotedDoument == null)
                    throw new Exception(string.Format("Demoted Not Found. [{0}, {1} Rev.{2}, {3}]", releasedDocument.OId, releasedDocument.Identifier.Code, releasedDocument.Revision, releasedDocument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(demotedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestReleaseDocument()
        {
            ScreenOutputDecorator.PrintHeader("Release Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-REL-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Release Test Document {0}", commonText),
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
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}, {3}]", document.OId, document.Identifier.Code, document.Revision, document.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Release document");
                Console.ReadLine();

                repository.Release(newQueriedDoument, creator);
                var releasedDoument = documentRepository.Get(newQueriedDoument.OId);

                if (releasedDoument == null)
                    throw new Exception(string.Format("Released Document Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(releasedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestObsoleteDocument()
        {
            ScreenOutputDecorator.PrintHeader("Obsolete Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-OBS-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Obsolete Test Document {0}", commonText),
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
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}, {3}]", document.OId, document.Identifier.Code, document.Revision, document.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Obsolete document");
                Console.ReadLine();

                repository.Obsolete(newQueriedDoument, creator);
                var obsoletedDoument = documentRepository.Get(newQueriedDoument.OId);

                if (obsoletedDoument == null)
                    throw new Exception(string.Format("Obsoleted Document Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(obsoletedDoument));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestObsoleteReleasedDocument()
        {
            ScreenOutputDecorator.PrintHeader("Obsolete Released Document Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Document);
            var phase = testerDependencyBlock.GetTestPhase(SPObjectTypes.Document);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Document);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Document, "HXD-20151224-173217", string.Format("HXID-OBSWREL-{0}", commonText));

            var document = new SPDocument
            {
                OId = 0,
                Revision = "A",
                RevisionSortSequence = 1,
                Class = cls,
                Description = string.Format("Harry Obsolete Test With Released Document {0}", commonText),
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
                    throw new Exception(string.Format("Document Not Found. [{0}, {1} Rev.{2}, {3}]", document.OId, document.Identifier.Code, document.Revision, document.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(newQueriedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Release document");
                Console.ReadLine();

                repository.Release(newQueriedDoument, creator);
                var releasedDoument = documentRepository.Get(newQueriedDoument.OId);

                if (releasedDoument == null)
                    throw new Exception(string.Format("Released Document Not Found. [{0}, {1} Rev.{2}, {3}]", newQueriedDoument.OId, newQueriedDoument.Identifier.Code, newQueriedDoument.Revision, newQueriedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(releasedDoument));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to Obsolete document");
                Console.ReadLine();

                repository.Obsolete(releasedDoument, creator);
                var obsoletedDoument = documentRepository.Get(releasedDoument.OId);

                if (obsoletedDoument == null)
                    throw new Exception(string.Format("Obsoleted Document Not Found. [{0}, {1} Rev.{2}, {3}]", releasedDoument.OId, releasedDoument.Identifier.Code, releasedDoument.Revision, releasedDoument.Status.Description));

                Console.WriteLine(new SPDocumentPresenter(obsoletedDoument));
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
