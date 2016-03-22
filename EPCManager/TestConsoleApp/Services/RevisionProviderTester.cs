using EPCManager.Common.Helpers;
using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Services;
using EPCManager.Repositories.Services;
using System;
using TestConsoleApp.Abstract;
using TestConsoleApp.Helpers;

namespace TestConsoleApp.Services
{
    internal class RevisionProviderTester : AbstractEPCManagerTester
    {
        private ILogManager logManager = LogManagerHelper.GetLogManager();
        private IRevisionProvider provider;

        protected override void Before()
        {
            this.provider = new DefaultRevisionProvider(logManager);
        }

        protected override void After()
        {
            this.provider = null;
        }

        public override void RunTest()
        {
            Run(TestAllRevisionProviderFunctions);
        }

        #region Test Methods

        private void TestAllRevisionProviderFunctions()
        {
            ScreenOutputDecorator.PrintHeader("Test All RevisionProvider Functions");

            try
            {
                Console.WriteLine(string.Format("Initail Revision = [{0}]", provider.GetInitialRevision()));

                string currentRevision = "AA";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));

                currentRevision = "A";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));

                currentRevision = "I";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));

                currentRevision = "Z";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));

                currentRevision = "AZ";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));

                currentRevision = "PP";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));

                currentRevision = "YZ";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));

                currentRevision = "ZZ";
                Console.WriteLine(string.Format("Next Revision for '{0}' = [{1}]", currentRevision, provider.GetNextRevision(currentRevision)));
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
