using ServiceWorker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceWorker.Service
{
    public interface IDeliveryDBRepository
    {
        Task<ShippingRequest> GetByIdAsync(string packageId);
        Task<IEnumerable<ShippingRequest>> GetAllAsync();
        Task CreateAsync(ShippingRequest shippingRequest);
        Task UpdateAsync(ShippingRequest shippingRequest);
        Task DeleteAsync(string packageId);
    }
}