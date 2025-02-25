using System.Collections.Generic;
using DeliveryService.Models;

namespace DeliveryService.Repositories
{
    public interface IDeliveryRepository
    {
        void Put(ParcelDeliveryDTO delivery);
        IEnumerable<ParcelDeliveryDTO> GetAll();
    }
}