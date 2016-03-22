using System;

namespace EPCManager.Domain.Exceptions
{
    public class EPCManagerException : Exception
    {
        public EPCManagerException(string message) : base(message)
        {
        }

        public EPCManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
