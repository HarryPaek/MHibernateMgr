using System;

namespace EPCManager.Domain.Exceptions
{
    public class EPCManagerCheckOutCheckInException : EPCManagerException
    {
        public EPCManagerCheckOutCheckInException(string message) : base(message)
        {
        }

        public EPCManagerCheckOutCheckInException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
