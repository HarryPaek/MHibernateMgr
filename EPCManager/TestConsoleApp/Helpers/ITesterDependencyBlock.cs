using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    public interface ITesterDependencyBlock
    {
        IClassRepository ClassRepository { get; }
        IDomainRepository DomainRepository { get; }
        IIdentifierRepository IdentifierRepository { get; }
        IPeopleRepository PeopleRepository { get; }
        IPhaseRepository PhaseRepository { get; }
        IStatusRepository StatusRepository { get; }

        [Obsolete("Obsoleted, use 'GetTestClass(SPObjectTypes objectType)' instead.")]
        SPClass GetTestClass();
        SPClass GetTestClass(SPObjectTypes objectType);
        SPClass GetTestClass(SPObjectTypes objectType, string className);

        SPDomain GetTestDomain();
        SPDomain GetTestDomain(string domainCode);

        SPIdentifier GetTestIdentifier(SPObjectTypes objectType);
        SPIdentifier GetTestIdentifier(SPObjectTypes objectType, string identifierCode);
        SPIdentifier GetTestIdentifier(SPObjectTypes objectType, string domainCode, string identifierCode);

        SPPeople GetTestPeople();
        SPPeople GetTestPeople(string firstName);

        SPStatus GetTestStatus(SPObjectTypes objectType);
        SPStatus GetTestStatus(SPObjectTypes objectType, string statusName);

        SPPhase GetTestPhase(SPObjectTypes objectType);
        SPPhase GetTestPhase(SPObjectTypes objectType, string phaseName);
    }
}
