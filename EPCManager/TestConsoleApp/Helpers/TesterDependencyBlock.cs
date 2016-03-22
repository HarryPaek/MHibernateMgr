using EPCManager.Domain.Abstract.Common;
using EPCManager.Domain.Entities;
using EPCManager.Domain.Exceptions;
using System;
using System.Linq;


namespace TestConsoleApp.Helpers
{
    public class TesterDependencyBlock : ITesterDependencyBlock
    {
        public TesterDependencyBlock(IClassRepository classRepository, IDomainRepository domainRepository, IIdentifierRepository identifierRepository,
                                     IPeopleRepository peopleRepository, IPhaseRepository phaseRepository, IStatusRepository statusRepository)
        {
            ClassRepository = classRepository;
            DomainRepository = domainRepository;
            IdentifierRepository = identifierRepository;
            PeopleRepository = peopleRepository;
            PhaseRepository = phaseRepository;
            StatusRepository = statusRepository;
        }

        public IClassRepository ClassRepository { get; private set; }
        public IDomainRepository DomainRepository { get; private set; }
        public IIdentifierRepository IdentifierRepository { get; private set; }
        public IPeopleRepository PeopleRepository { get; private set; }
        public IPhaseRepository PhaseRepository { get; private set; }
        public IStatusRepository StatusRepository { get; private set; }

        [Obsolete("Obsoleted, use 'GetTestClass(SPObjectTypes objectType)' instead.")]
        public SPClass GetTestClass()
        {
            return GetTestClass(SPObjectTypes.Document, "GeneralDocument");
        }

        public SPClass GetTestClass(SPObjectTypes objectType)
        {
            string className = string.Empty;

            switch (objectType)
            {
                case SPObjectTypes.Project:
                    className = "EPC";
                    break;
                case SPObjectTypes.Document:
                    className = "GeneralDocument";
                    break;
                case SPObjectTypes.Dataset:
                    break;
                case SPObjectTypes.Item:
                    className = "GeneralItem";
                    break;
                case SPObjectTypes.Work:
                    className = "General";
                    break;
                case SPObjectTypes.Process:
                    className = "GAP-4STEP";
                    break;
                case SPObjectTypes.Unknown:
                case SPObjectTypes.Activity:
                case SPObjectTypes.File:
                case SPObjectTypes.Folder:
                case SPObjectTypes.Template:
                case SPObjectTypes.People:
                case SPObjectTypes.Role:
                case SPObjectTypes.Organization:
                case SPObjectTypes.RelationshipType:
                case SPObjectTypes.ProjectRole:
                default:
                    className = "Unknown";
                    break;
            }

            return GetTestClass(objectType, className);
        }

        public SPClass GetTestClass(SPObjectTypes objectType, string className)
        {
            SPClass spClass = ClassRepository.Items.FirstOrDefault(cls => cls.ObjectType == objectType && cls.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

            if (spClass == null)
                throw new EPCManagerDBException("테스트를 위한 Class를 찾을 수 없습니다.");

            return spClass;
        }

        public SPDomain GetTestDomain()
        {
            return RetriveOrCreateTestDomain("TEST");
        }
        
        public SPDomain GetTestDomain(string domainCode)
        {
            return RetriveOrCreateTestDomain(domainCode);
        }

        private SPDomain RetriveOrCreateTestDomain(string domainCode)
        {
            SPDomain spDomain = DomainRepository.Items.FirstOrDefault(domain => domain.Code.Equals(domainCode, StringComparison.OrdinalIgnoreCase));

            if (spDomain == null)
            {
                spDomain = new SPDomain { Code = domainCode, Description = domainCode, CreatedBy = GetTestPeople(), CreatedDate = DateTime.Now, OId = 0 };
                DomainRepository.Add(spDomain);
            }

            return spDomain;
        }

        public SPIdentifier GetTestIdentifier(SPObjectTypes objectType)
        {
            string domainCode = "NONE";
            string idCode = string.Format("HXID-{0}", DateTime.Now.ToString("yyyyMMdd-HHmmss"));

            return GetTestIdentifier(objectType, domainCode, idCode);
        }

        public SPIdentifier GetTestIdentifier(SPObjectTypes objectType, string identifierCode)
        {
            string domainCode = "NONE";

            return GetTestIdentifier(objectType, domainCode, identifierCode);
        }

        public SPIdentifier GetTestIdentifier(SPObjectTypes objectType, string domainCode, string identifierCode)
        {
            return new SPIdentifier
            {
                OId = 0,
                ObjectType = objectType,
                Domain = GetTestDomain(domainCode),
                Code = identifierCode
            };
        }

        public SPPeople GetTestPeople()
        {
            return GetTestPeople("Harry");
        }

        public SPPeople GetTestPeople(string firstName)
        {
            SPPeople spPeople = PeopleRepository.Items.FirstOrDefault(p => p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));

            if (spPeople == null)
                spPeople = new SPPeople { OId = 1 }; // System

            return spPeople;
        }

        public SPStatus GetTestStatus(SPObjectTypes objectType)
        {
            int ordinal = 0;

            return GetTestStatus(objectType, ordinal);
        }

        public SPStatus GetTestStatus(SPObjectTypes objectType, string statusName)
        {
            SPStatus spStatus = StatusRepository.Items.FirstOrDefault(s => s.ObjectType == objectType && s.Name.Equals(statusName, StringComparison.OrdinalIgnoreCase));

            if (spStatus == null)
                spStatus = GetTestStatus(objectType);

            return spStatus;
        }

        private SPStatus GetTestStatus(SPObjectTypes objectType, int ordinal)
        {
            SPStatus spStatus = StatusRepository.Items.FirstOrDefault(s => s.ObjectType ==  objectType && s.Ordinal == ordinal);

            if (spStatus == null)
                spStatus = GetTestStatus(objectType, 0);

            return spStatus;
        }

        public SPPhase GetTestPhase(SPObjectTypes objectType)
        {
            return GetTestPhase(objectType, "START");
        }

        public SPPhase GetTestPhase(SPObjectTypes objectType, string phaseName)
        {
            SPPhase spPhase = PhaseRepository.Items.FirstOrDefault(s => s.ObjectType == objectType && s.Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase));

            if (spPhase == null)
                throw new EPCManagerDBException("테스트를 위한 Phase를 찾을 수 없습니다.");

            return spPhase;
        }
    }
}
