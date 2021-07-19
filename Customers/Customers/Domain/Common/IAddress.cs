namespace Customers.Domain.Common
{
    public interface IAddress
    {
        string Thoroughfare { get; set; }
        string? Premises { get; set; }
        string? SubPremises { get; set; }
        string PostalCode { get; set; }
        string Locality { get; set; }
        string SubAdministrativeArea { get; set; }
        string AdministrativeArea { get; set; }
        string Country { get; set; }
    }
}