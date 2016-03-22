using EPCManager.Common.Logging;
using EPCManager.Domain.Abstract.Services;
using log4net;
using System;

namespace EPCManager.Repositories.Services
{
    public class DefaultRevisionProvider : IRevisionProvider
    {
        private const string finalRevision = "ZZ";
        private readonly ILog log;

        public DefaultRevisionProvider(ILogManager logManager)
        {
            this.log = logManager.GetLog(this.GetType());
        }

        #region implement IRevisionProvider interface methods

        public string GetNextRevision(string currentRevision)
        {
            if (string.IsNullOrWhiteSpace(currentRevision))
            {
                log.Error(string.Format("현재 리비전 값이 NULL이거나 오류가 있습니다. [{0}]", currentRevision));
                throw new ArgumentNullException("currentRevision");
            }

            if (currentRevision.Equals(finalRevision, StringComparison.OrdinalIgnoreCase))
            {
                log.Error(string.Format("현재 리비전 값이 사용가능한 마지막 리비전입니다. [{0}]", currentRevision));
                throw new ArgumentOutOfRangeException("currentRevision", "허용된 리비전 범위가 모두 사용되었습니다.");
            }

            return GetNextRevisionString(currentRevision.ToUpper());
        }

        public string GetInitialRevision()
        {
            return "AA";
        }

        public int GetNextRevisionSortSequence(int currentSequence)
        {
            return currentSequence++;
        }

        public int GetInitialRevisionSortSequence()
        {
            return 1;
        }

        #endregion

        #region private methods

        private string GetNextRevisionString(string currentRevision)
        {
            char[] revChars = currentRevision.ToCharArray();

            // Loop through array.
            for (int i = revChars.Length-1; i >= 0; i--)
            {
                char nextRevChar;
                bool updateNextChar = TryGetNextRevChar(revChars[i], out nextRevChar);
                revChars[i] = nextRevChar;

                if(!updateNextChar)
                    break;
            }

            return new string(revChars);
        }

        private bool TryGetNextRevChar(char current, out char next)
        {
            bool updateNextChar = false;

            if (current == 'Z')
            {
                next = 'A';
                updateNextChar = true;
            }
            else
            {
                current++;
                next = current;
            }

            return updateNextChar;
        }

        #endregion
    }
}
