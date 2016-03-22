using EPCManager.Domain.Entities;

namespace TestConsoleApp.Helpers
{
    internal class SPDomainPresenter
    {
        SPDomain domain;

        public SPDomainPresenter(SPDomain domain)
        {
            this.domain = domain;
        }

        public override string ToString()
        {
            return string.Format("OID=[{0}], Code=[{1}], Description=[{2}], CreatedDate=[{3}], CreatedBy=[{4}]",
                                  domain.OId, domain.Code, domain.Description, domain.CreatedDate, domain.CreatedBy.Description);
        }
    }
}
