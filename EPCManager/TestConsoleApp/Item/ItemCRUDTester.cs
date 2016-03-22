using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Common.Managers;
using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Abstract.Item;
using EPCManager.Domain.Abstract.Services;
using EPCManager.Domain.Entities;
using EPCManager.Repositories.Common;
using EPCManager.Repositories.Item;
using EPCManager.Repositories.Services;
using NHibernate;
using System;
using System.Linq;
using TestConsoleApp.Abstract;
using TestConsoleApp.Helpers;

namespace TestConsoleApp.Item
{
    internal class ItemCRUDTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private ITesterDependencyBlock testerDependencyBlock;
        private IRevisionProvider revisionProvider;
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
            revisionProvider = new DefaultRevisionProvider(logManager);

            this.testerDependencyBlock = new TesterDependencyBlock(classRepository, domainRepository, identifierRepository, peopleRepository, phaseRepository, statusRepository);
            this.repository = new ItemRepository(session, logManager, identifierRepository, statusRepository, relationshipRepository);
        }

        protected override void After()
        {
            this.repository = null;
            this.revisionProvider = null;
            this.testerDependencyBlock = null;

            NHibernateSessionManager.CloseSession();
        }

        public override void RunTest()
        {
            Run(TestGetAllItems);
            Run(TestAddItem);
            Run(TestAddDeleteItem);
            Run(TestAddUpdateItem);
            Run(TestUpdateReleasedItem);
            Run(TestDeleteRevisedItem);
        }

        #region Test Methods

        private void TestGetAllItems()
        {
            ScreenOutputDecorator.PrintHeader("Item List");

            foreach (var item in repository.Items)
            {
                var itemPresenter = new SPItemPresenter(item);
                Console.WriteLine(itemPresenter);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestAddItem()
        {
            ScreenOutputDecorator.PrintHeader("Add Item Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Item);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Item);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Item, "ITEM-TEST", string.Format("HXID-ITEM-C-{0}", commonText));

            var item = new SPItem
            {
                OId = 0,
                Revision = this.revisionProvider.GetInitialRevision(),
                RevisionSortSequence = this.revisionProvider.GetInitialRevisionSortSequence(),
                Class = cls,
                Description = string.Format("Harry Add Test Item {0}", commonText),
                Status = status,
                Identifier = id,
                Template = false,
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                repository.Add(item);
                Console.WriteLine(new SPItemPresenter(item));

                var newQueriedItem = repository.Get(item.OId);

                if (newQueriedItem == null)
                    throw new Exception(string.Format("Item Not Found. [{0}, {1} Rev.{2}]", item.OId, item.Identifier.Code, item.Revision));

                Console.WriteLine(new SPItemPresenter(newQueriedItem));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestAddUpdateItem()
        {
            ScreenOutputDecorator.PrintHeader("Add Update Item Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Item);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Item);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Item, "ITEM-TEST", string.Format("HXID-ITEM-CU-{0}", commonText));

            var item = new SPItem
            {
                OId = 0,
                Revision = this.revisionProvider.GetInitialRevision(),
                RevisionSortSequence = this.revisionProvider.GetInitialRevisionSortSequence(),
                Class = cls,
                Description = string.Format("Harry Add Update Test Item {0}", commonText),
                Status = status,
                Identifier = id,
                Template = false,
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                repository.Add(item);
                Console.WriteLine(new SPItemPresenter(item));

                var newQueriedItem = repository.Get(item.OId);

                if (newQueriedItem == null)
                    throw new Exception(string.Format("Item Not Found. [{0}, {1} Rev.{2}]", item.OId, item.Identifier.Code, item.Revision));

                Console.WriteLine(new SPItemPresenter(newQueriedItem));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to update item");
                Console.ReadLine();

                newQueriedItem.Description = string.Format("{0} Updated", newQueriedItem.Description);
                newQueriedItem.ModifiedBy = creator;
                newQueriedItem.ModifiedDate = DateTime.Now;

                repository.Update(newQueriedItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        private void TestUpdateReleasedItem()
        {
            ScreenOutputDecorator.PrintHeader("Update Released Item Test");
            var creator = testerDependencyBlock.GetTestPeople();

            try
            {
                var releasedItem = repository.Items.FirstOrDefault(d => d.Status.Name.Equals("Released", StringComparison.OrdinalIgnoreCase));

                if (releasedItem == null)
                    throw new Exception("Released Item Not Found!");

                Console.WriteLine(new SPItemPresenter(releasedItem));

                releasedItem.Description = string.Format("{0} Released Item Updated", releasedItem.Description);
                releasedItem.ModifiedBy = creator;
                releasedItem.ModifiedDate = DateTime.Now;

                repository.Update(releasedItem);
                Console.WriteLine(new SPItemPresenter(releasedItem));

                var newQueriedItem = repository.Get(releasedItem.OId);

                if (newQueriedItem == null)
                    throw new Exception(string.Format("Item Not Found. [{0}, {1} Rev.{2}]", releasedItem.OId, releasedItem.Identifier.Code, releasedItem.Revision));

                Console.WriteLine(new SPItemPresenter(newQueriedItem));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /**
         * 일반적인 Item의 경우, Item Revision과 Identifier가 동시에 삭제된다.
         */
        private void TestAddDeleteItem()
        {
            ScreenOutputDecorator.PrintHeader("Add Delete Item Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Item);
            var status = testerDependencyBlock.GetTestStatus(SPObjectTypes.Item);
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Item, "ITEM-TEST", string.Format("HXID-ITEM-CD-{0}", commonText));

            var item = new SPItem
            {
                OId = 0,
                Revision = this.revisionProvider.GetInitialRevision(),
                RevisionSortSequence = this.revisionProvider.GetInitialRevisionSortSequence(),
                Class = cls,
                Description = string.Format("Harry Add Delete Test Item {0}", commonText),
                Status = status,
                Identifier = id,
                Template = false,
                CreatedDate = DateTime.Now,
                CreatedBy = creator
            };

            try
            {
                repository.Add(item);
                Console.WriteLine(new SPItemPresenter(item));

                var newQueriedItem = repository.Get(item.OId);

                if (newQueriedItem == null)
                    throw new Exception(string.Format("Item Not Found. [{0}, {1} Rev.{2}]", item.OId, item.Identifier.Code, item.Revision));

                Console.WriteLine(new SPItemPresenter(newQueriedItem));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to delete item");
                Console.ReadLine();

                repository.Delete(newQueriedItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ScreenOutputDecorator.PrintFooter();
        }

        /**
         * Revised Item의 경우, Item Revision만 삭제되고 Identifier는 삭제되지 않아야 한다.
         */
        private void TestDeleteRevisedItem()
        {
            ScreenOutputDecorator.PrintHeader("Delete Revised Item Test");

            string commonText = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var cls = testerDependencyBlock.GetTestClass(SPObjectTypes.Item);
            var statusReleased = testerDependencyBlock.GetTestStatus(SPObjectTypes.Item, "Released");
            var statusPlanned = testerDependencyBlock.GetTestStatus(SPObjectTypes.Item, "Planned");
            var creator = testerDependencyBlock.GetTestPeople();
            var id = testerDependencyBlock.GetTestIdentifier(SPObjectTypes.Item, "ITEM-TEST", string.Format("HXID-ITEM-CD-{0}", commonText));

            var releasedItem = new SPItem
            {
                OId = 0,
                Revision = this.revisionProvider.GetInitialRevision(),
                RevisionSortSequence = this.revisionProvider.GetInitialRevisionSortSequence(),
                Class = cls,
                Description = string.Format("Harry Delete Test Revised Item {0}", commonText),
                Status = statusReleased,
                Identifier = id,
                Template = false,
                CreatedDate = DateTime.Now,
                CreatedBy = creator,
                ModifiedDate = DateTime.Now,
                ModifiedBy = creator,
                CompletedDate = DateTime.Now,
                CompletedBy = creator
            };

            var revBItem = new SPItem
            {
                OId = 0,
                Revision = this.revisionProvider.GetNextRevision(releasedItem.Revision),
                RevisionSortSequence = this.revisionProvider.GetNextRevisionSortSequence(releasedItem.RevisionSortSequence), 
                Status = statusPlanned,
                Template = false,
                CreatedDate = DateTime.Now,
                CreatedBy = creator,
            };

            try
            {
                repository.Add(releasedItem);
                Console.WriteLine(new SPItemPresenter(releasedItem));

                var newQueriedItem = repository.Get(releasedItem.OId);

                if (newQueriedItem == null)
                    throw new Exception(string.Format("Released item Not Found. [{0}, {1} Rev.{2}]", releasedItem.OId, releasedItem.Identifier.Code, releasedItem.Revision));

                Console.WriteLine(new SPItemPresenter(newQueriedItem));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to revise item");
                Console.ReadLine();

                revBItem.Class = newQueriedItem.Class;
                revBItem.Description = string.Format("{0} - Rev. B", newQueriedItem.Description);
                revBItem.Identifier = newQueriedItem.Identifier;

                repository.Add(revBItem);

                var revisedItem = repository.Get(revBItem.OId);

                if (revisedItem == null)
                    throw new Exception(string.Format("Revised item Not Found. [{0}, {1} Rev.{2}]", revBItem.OId, revBItem.Identifier.Code, revBItem.Revision));

                Console.WriteLine(new SPItemPresenter(revisedItem));

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to delete revised item");
                Console.ReadLine();

                repository.Delete(revisedItem);

                ScreenOutputDecorator.PrintHeader("Please <ENTER> to display remaining released item");
                Console.ReadLine();

                var remainingItem = repository.Get(releasedItem.OId);

                if (remainingItem == null)
                    throw new Exception(string.Format("Remaining Released item Not Found. [{0}, {1} Rev.{2}]", releasedItem.OId, releasedItem.Identifier.Code, releasedItem.Revision));

                Console.WriteLine(new SPItemPresenter(remainingItem));
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
