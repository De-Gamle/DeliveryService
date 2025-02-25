using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DeliveryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<DeliveriesController> _logger;

    public DeliveriesController(IDeliveryRepository deliveryRepository, ILogger<DeliveriesController> logger)
    {
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ParcelDeliveryDTO>> GetAll()
    {
        _logger.LogInformation("Henter alle leveringer");
        var deliveries = _deliveryRepository.GetAll();
        return Ok(deliveries);
    }
}