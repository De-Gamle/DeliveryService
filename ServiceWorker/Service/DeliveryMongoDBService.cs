using MongoDB.Driver;
using ServiceWorker.Models;
using ServiceWorker.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceWorker.Service
{
    public class DeliveryMongoDBService : IDeliveryDBRepository
    {
        private readonly IMongoCollection<ShippingRequest> _shippingRequests;
        private readonly ILogger<DeliveryMongoDBService> _logger;

        public DeliveryMongoDBService(IConfiguration configuration, ILogger<DeliveryMongoDBService> logger)
        {
            _logger = logger;
            try
            {
                var mongoClient = new MongoClient(
                    configuration["MongoDB:ConnectionString"]);
                    
                var mongoDatabase = mongoClient.GetDatabase(
                    configuration["MongoDB:DatabaseName"]);
                    
                _shippingRequests = mongoDatabase.GetCollection<ShippingRequest>(
                    configuration["MongoDB:CollectionName"]);
                    
                _logger.LogInformation("MongoDB connection initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing MongoDB connection");
                throw;
            }
        }

        public async Task<ShippingRequest> GetByIdAsync(string packageId)
        {
            return await _shippingRequests.Find(s => s.PackageId == packageId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShippingRequest>> GetAllAsync()
        {
            return await _shippingRequests.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(ShippingRequest shippingRequest)
        {
            await _shippingRequests.InsertOneAsync(shippingRequest);
            _logger.LogInformation($"ShippingRequest with ID {shippingRequest.PackageId} saved to MongoDB");
        }

        public async Task UpdateAsync(ShippingRequest shippingRequest)
        {
            await _shippingRequests.ReplaceOneAsync(s => s.PackageId == shippingRequest.PackageId, shippingRequest);
        }

        public async Task DeleteAsync(string packageId)
        {
            await _shippingRequests.DeleteOneAsync(s => s.PackageId == packageId);
        }
    }
}