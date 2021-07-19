using System.Threading.Tasks;

using Checkout.Contracts;

namespace Checkout.Hubs
{
    public interface ICatalogClient
    {
        Task CatalogItemUpdated(CatalogItemEvent dto);
    }
}