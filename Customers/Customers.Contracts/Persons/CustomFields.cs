using System;
using System.Collections.Generic;

namespace Customers.Contracts.Persons
{
    public class AddCustomFieldToPersonCommand
    {
        public AddCustomFieldToPersonCommand(Guid personId, CreateCustomFieldDetails customFieldDetails)
        {
            PersonId = personId;
            CreateCustomFieldDetails = customFieldDetails;
        }

        public Guid PersonId { get; }

        public CreateCustomFieldDetails CreateCustomFieldDetails { get; }
    }

    public class AddCustomFieldToPersonCommandResponse
    {

    }

    public class RemoveCustomFieldFromPersonCommand
    {
        public RemoveCustomFieldFromPersonCommand(
            Guid personId,
            string customFieldId
        )
        {
            PersonId = personId;
            CustomFieldId = customFieldId;
        }

        public Guid PersonId { get; }

        public string CustomFieldId { get; }
    }

    public class RemoveCustomFieldFromPersonCommandResponse
    {

    }
}