using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ProductFunctionsApp.Application.Interfaces;

namespace ProductFunctionsApp.Api.Functions;

public class TimerProductFunctions
{
    private readonly ILogger<TimerProductFunctions> _logger;
    private readonly IProductService _productService;

    public TimerProductFunctions(
        ILogger<TimerProductFunctions> logger,
        IProductService productService
    )
    {
        _logger = logger;
        _productService = productService;
    }

    // This function runs every 5 minutes. The NCRONTAB expression is "{second} {minute} {hour} {day} {month} {day-of-week}"
    [Function("ProductCountReport")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        var products = await _productService.GetAllProductsAsync();
        int count = products.Count();

        _logger.LogInformation(
            $"Product summary report: There are currently {count} products in the system."
        );

        if (myTimer.IsPastDue)
        {
            _logger.LogInformation("Timer is running late!");
        }
    }
}
