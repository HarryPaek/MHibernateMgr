using EPCManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.Helpers
{
    internal class SPPeoplePresenter
    {
        SPPeople people;

        public SPPeoplePresenter(SPPeople people)
        {
            this.people = people;
        }

        public override string ToString()
        {
            return string.Format("OID=[{0}], FirstName=[{1}], LastName=[{2}], Description=[{3}], Inactive=[{4}], Identifier=[{5}], CreatedDate=[{6}], CreatedBy=[{7}]",
                                  people.OId, people.FirstName, people.LastName, people.Description, people.Inactive, people.Identifier.Code, people.CreatedDate, people.CreatedBy.Description);
        }
    }
}
