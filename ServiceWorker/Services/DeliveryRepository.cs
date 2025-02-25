using System;
using System.Collections.Generic;
using System.Linq;
using DeliveryService.Models;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly List<ParcelDeliveryDTO> _deliveries = new();
        private readonly object _lock = new();
        private readonly ILogger<DeliveryRepository> _logger;

        public DeliveryRepository(ILogger<DeliveryRepository> logger)
        {
            _logger = logger;
        }

        public void Put(ParcelDeliveryDTO delivery)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException(nameof(delivery));
            }

            lock (_lock)
            {
                // Tjek om levering allerede eksisterer (opdatering)
                var existingIndex = _deliveries.FindIndex(d => d.Id == delivery.Id);
                if (existingIndex >= 0)
                {
                    _deliveries[existingIndex] = delivery;
                    _logger.LogInformation("Levering opdateret: {Id}", delivery.Id);
                }
                else
                {
                    _deliveries.Add(delivery);
                    _logger.LogInformation("Ny levering tilføjet: {Id}", delivery.Id);
                }
            }
        }

        public IEnumerable<ParcelDeliveryDTO> GetAll()
        {
            lock (_lock)
            {
                // Sortér listen med mest presserende leveringer først
                // Prioritér først efter IsPriority flag, derefter efter leveringsdato
                return _deliveries
                    .OrderByDescending(d => d.IsPriority)
                    .ThenBy(d => d.DeliveryDate)
                    .ToList();
            }
        }
    }
}