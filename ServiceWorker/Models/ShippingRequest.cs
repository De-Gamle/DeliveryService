namespace ServiceWorker.Models
{
    public class ShippingRequest
    {
        public string PackageId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        
        
        // For CSV format
        public string ToCsvString()
        {
            return $"{PackageId},{CustomerName},{PickupAddress},{DeliveryAddress}";
        }
        
        // CSV header
        public static string CsvHeader => "PackageId,CustomerName,PickupAddress,DeliveryAddress";
    }
}