using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPCManager.Domain.Abstract.Services
{
    public interface IRevisionProvider
    {
        string GetNextRevision(string currentRevision);
        string GetInitialRevision();

        int GetNextRevisionSortSequence(int currentSequence);
        int GetInitialRevisionSortSequence();
    }
}
