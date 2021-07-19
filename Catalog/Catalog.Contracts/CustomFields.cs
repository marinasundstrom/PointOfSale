using System;
using System.Collections.Generic;

namespace Catalog.Contracts
{
    public class CreateCustomFieldDetails
    {
        public string CustomFieldId { get; set; } = null!;

        public string Value { get; set; } = null!;
    }

    public class AddCustomFieldToItemCommand
    {
        public AddCustomFieldToItemCommand(string itemId, CreateCustomFieldDetails customFieldDetails)
        {
            ItemId = itemId;
            CreateCustomFieldDetails = customFieldDetails;
        }

        public string ItemId { get; }

        public CreateCustomFieldDetails CreateCustomFieldDetails { get; }
    }

    public class AddCustomFieldToItemCommandResponse
    {

    }

    public class RemoveCustomFieldFromItemCommand
    {
        public RemoveCustomFieldFromItemCommand(
            string itemId,
            string customFieldId
        )
        {
            ItemId = itemId;
            CustomFieldId = customFieldId;
        }

        public string ItemId { get; }

        public string CustomFieldId { get; }
    }

    public class RemoveCustomFieldFromItemCommandResponse
    {

    }

    public class AddCustomFieldToItemItemCommand
    {
        public AddCustomFieldToItemItemCommand(string itemId, Guid orderItemId, CreateCustomFieldDetails customFieldDetails)
        {
            ItemId = itemId;
            ItemItemId = orderItemId;
            CreateCustomFieldDetails = customFieldDetails;
        }

        public string ItemId { get; }

        public Guid ItemItemId { get; }

        public CreateCustomFieldDetails CreateCustomFieldDetails { get; }
    }

    public class AddCustomFieldToItemItemCommandResponse
    {

    }

    public class RemoveCustomFieldFromItemItemCommand
    {
        public RemoveCustomFieldFromItemItemCommand(
            string itemId,
            Guid orderItemId,
            string customFieldId
        )
        {
            ItemId = itemId;
            ItemItemId = orderItemId;
            CustomFieldId = customFieldId;
        }

        public string ItemId { get; }

        public Guid ItemItemId { get; }

        public string CustomFieldId { get; }
    }

    public class RemoveCustomFieldFromItemItemCommandResponse
    {

    }
}