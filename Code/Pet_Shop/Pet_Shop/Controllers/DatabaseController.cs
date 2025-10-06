using Microsoft.AspNetCore.Mvc;
using Pet_Shop.Services;

namespace Pet_Shop.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly DatabaseService _databaseService;

        public DatabaseController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var isConnected = await _databaseService.TestConnectionAsync();
                var isCreated = await _databaseService.EnsureDatabaseCreatedAsync();
                
                if (isConnected && isCreated)
                {
                    await _databaseService.SeedInitialDataAsync();
                    ViewBag.Message = "Database connection successful and initial data seeded!";
                    ViewBag.IsSuccess = true;
                }
                else
                {
                    ViewBag.Message = "Database connection failed!";
                    ViewBag.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error: {ex.Message}";
                ViewBag.IsSuccess = false;
            }

            return View();
        }
    }
}
