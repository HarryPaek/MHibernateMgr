using EPCManager.Domain.Entities;

namespace TestConsoleApp.Helpers
{
    internal class SPIdentifierPresenter
    {
        SPIdentifier identifier;

        public SPIdentifierPresenter(SPIdentifier identifier)
        {
            this.identifier = identifier;
        }

        public override string ToString()
        {
            return string.Format("OID=[{0}], Domain=[{1}], Code=[{2}], ObjectType=[{3}]",
                                  identifier.OId, identifier.Domain.Code, identifier.Code, identifier.ObjectType);
        }
    }
}
