using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Data;
using Pet_Shop.Scripts;

namespace Pet_Shop.Controllers
{
    /// <summary>
    /// Controller để export data cho ML training
    /// Chỉ nên dùng trong development hoặc với authentication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExportDataController : ControllerBase
    {
        private readonly PetShopDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ExportDataController(PetShopDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("export-all")]
        public async Task<IActionResult> ExportAll()
        {
            try
            {
                var exporter = new ExportDataForML(_context);
                
                // Export to AI_chatbot_train folder
                var basePath = Path.Combine(_env.ContentRootPath, "..", "..", "AI_chatbot_train");
                var interactionsPath = Path.Combine(basePath, "interactions_cvae_cf_large.csv");
                var productsPath = Path.Combine(basePath, "items_cvae_cbf.csv");

                var (interactionsResult, productsResult) = await exporter.ExportAllAsync(
                    interactionsPath, 
                    productsPath);

                return Ok(new
                {
                    success = true,
                    message = "Data exported successfully",
                    interactions = interactionsResult,
                    products = productsResult
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("export-interactions")]
        public async Task<IActionResult> ExportInteractions()
        {
            try
            {
                var exporter = new ExportDataForML(_context);
                var basePath = Path.Combine(_env.ContentRootPath, "..", "..", "AI_chatbot_train");
                var outputPath = Path.Combine(basePath, "interactions_cvae_cf_large.csv");

                var result = await exporter.ExportInteractionsAsync(outputPath);

                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("export-products")]
        public async Task<IActionResult> ExportProducts()
        {
            try
            {
                var exporter = new ExportDataForML(_context);
                var basePath = Path.Combine(_env.ContentRootPath, "..", "..", "AI_chatbot_train");
                var outputPath = Path.Combine(basePath, "items_cvae_cbf.csv");

                var result = await exporter.ExportProductsAsync(outputPath);

                return Ok(new
                {
                    success = true,
                    message = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}

