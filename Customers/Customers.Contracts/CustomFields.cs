using System;
using System.Collections.Generic;

namespace Customers.Contracts
{
    public class CreateCustomFieldDetails
    {
        public string CustomFieldId { get; set; } = null!;

        public string Value { get; set; } = null!;
    }
}