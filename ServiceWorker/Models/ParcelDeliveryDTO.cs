using System;

namespace DeliveryService;

public class ParcelDeliveryDTO
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string DeliveryAddress { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string PackageDescription { get; set; }
    public bool IsPriority { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}