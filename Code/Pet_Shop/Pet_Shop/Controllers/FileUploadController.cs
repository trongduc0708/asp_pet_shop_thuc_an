using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Pet_Shop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(IWebHostEnvironment environment, ILogger<FileUploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, string type = "banner")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Không có file được chọn" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, message = "Định dạng file không được hỗ trợ. Chỉ chấp nhận JPG, PNG, GIF" });
                }

                // Validate file size (5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "Kích thước file không được vượt quá 5MB" });
                }

                // Determine upload directory based on type
                var uploadDir = type.ToLower() switch
                {
                    "product" => "products",
                    "banner" => "banners",
                    _ => "uploads"
                };

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", uploadDir);
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return file URL
                var fileUrl = $"/uploads/{uploadDir}/{fileName}";
                
                _logger.LogInformation($"File uploaded successfully: {fileName} to {uploadDir}");
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Upload thành công",
                    fileUrl = fileUrl,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi upload file" });
            }
        }

        [HttpPost("upload-product-images")]
        public async Task<IActionResult> UploadProductImages(List<IFormFile> files)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    return BadRequest(new { success = false, message = "Không có file được chọn" });
                }

                var uploadedFiles = new List<object>();
                var errors = new List<string>();

                foreach (var file in files)
                {
                    try
                    {
                        if (file.Length == 0) continue;

                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            errors.Add($"File {file.FileName}: Định dạng không được hỗ trợ");
                            continue;
                        }

                        // Validate file size (5MB)
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            errors.Add($"File {file.FileName}: Kích thước vượt quá 5MB");
                            continue;
                        }

                        // Create uploads directory if it doesn't exist
                        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "products");
                        if (!Directory.Exists(uploadsPath))
                        {
                            Directory.CreateDirectory(uploadsPath);
                        }

                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadsPath, fileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Add to uploaded files
                        uploadedFiles.Add(new
                        {
                            fileName = fileName,
                            fileUrl = $"/uploads/products/{fileName}",
                            originalName = file.FileName
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"File {file.FileName}: {ex.Message}");
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = $"Upload thành công {uploadedFiles.Count} file(s)",
                    files = uploadedFiles,
                    errors = errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading product images: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi upload file" });
            }
        }

        [HttpDelete("delete-image")]
        public IActionResult DeleteImage([FromBody] DeleteImageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FileUrl))
                {
                    return BadRequest(new { success = false, message = "URL file không hợp lệ" });
                }

                // Extract filename from URL
                var fileName = Path.GetFileName(request.FileUrl);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", "banners", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation($"File deleted successfully: {fileName}");
                    return Ok(new { success = true, message = "Xóa file thành công" });
                }
                else
                {
                    return NotFound(new { success = false, message = "File không tồn tại" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xóa file" });
            }
        }
    }

    public class DeleteImageRequest
    {
        public string FileUrl { get; set; } = string.Empty;
    }
}
