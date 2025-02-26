using System.Collections.Concurrent;
using ServiceWorker.Models;

namespace ServiceWorker.Service;

public class BookingService
{
    private readonly ConcurrentBag<ShippingRequest> _bookings = new();
    private readonly ILogger<BookingService> _logger;

    public BookingService(ILogger<BookingService> logger)
    {
        _logger = logger;
    }

    public void Put(ShippingRequest request)
    {
        _bookings.Add(request);
        _logger.LogInformation($"Added booking with ID: {request.Id}");
    }

    public IEnumerable<ShippingRequest> GetAll()
    {
        // Return sorted list with most pressing bookings first (based on delivery date)
        return _bookings.OrderBy(b => b.DeliveryDate).ToList();
    }
}
