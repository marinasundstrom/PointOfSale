using System.Collections.Generic;

namespace Customers.Contracts.Persons
{
    public class GetPersonsQuery
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;

        public bool IncludeCustomFields { get; set; } = true;
    }

    public class GetPersonsQueryResponse
    {
        public IEnumerable<PersonDto> Persons { get; set; } = null!;

        public int Total { get; set; }
    }

    public class GetPersonByCustomerNoQuery
    {
        public Guid PersonId { get; set; }

        public bool IncludeCustomFields { get; set; } = true;
    }
}