using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _publicPath;

        public ImagesController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _publicPath = Path.Combine(_environment.ContentRootPath, "Public");
        }

        /// <summary>
        /// Get a specific image from a category folder
        /// </summary>
        /// <param name="category">The category folder (frontali, superiori)</param>
        /// <param name="filename">The image filename</param>
        /// <returns>The image file</returns>
        [HttpGet("{category}/{filename}")]
        public IActionResult GetImage(string category, string filename)
        {
            try
            {
                // Validate category
                var validCategories = new[] { "frontali", "superiori" };
                if (!validCategories.Contains(category.ToLower()))
                {
                    return BadRequest($"Invalid category. Valid categories are: {string.Join(", ", validCategories)}");
                }

                // Construct the file path
                var categoryPath = Path.Combine(_publicPath, char.ToUpper(category[0]) + category.Substring(1).ToLower());
                var filePath = Path.Combine(categoryPath, filename);

                // Security check: ensure the file is within the allowed directory
                if (!filePath.StartsWith(_publicPath))
                {
                    return BadRequest("Invalid file path");
                }

                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"Image '{filename}' not found in category '{category}'");
                }

                // Get file extension to determine content type
                var extension = Path.GetExtension(filename).ToLower();
                var contentType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                // Return the file for inline display (not download)
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                
                // Set headers for inline display
                Response.Headers["Content-Disposition"] = "inline";
                Response.Headers["Cache-Control"] = "public, max-age=3600"; // Cache for 1 hour
                
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving image: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all images in a specific category
        /// </summary>
        /// <param name="category">The category folder (frontali, superiori)</param>
        /// <returns>List of image filenames</returns>
        [HttpGet("{category}")]
        public IActionResult GetImagesInCategory(string category)
        {
            try
            {
                // Validate category
                var validCategories = new[] { "frontali", "superiori" };
                if (!validCategories.Contains(category.ToLower()))
                {
                    return BadRequest($"Invalid category. Valid categories are: {string.Join(", ", validCategories)}");
                }

                // Construct the category path
                var categoryPath = Path.Combine(_publicPath, char.ToUpper(category[0]) + category.Substring(1).ToLower());

                // Check if directory exists
                if (!Directory.Exists(categoryPath))
                {
                    return NotFound($"Category '{category}' not found");
                }

                // Get all image files
                var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp" };
                var imageFiles = Directory.GetFiles(categoryPath)
                    .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .Select(file => new
                    {
                        filename = Path.GetFileName(file),
                        size = new FileInfo(file).Length,
                        lastModified = new FileInfo(file).LastWriteTime,
                        url = $"/api/images/{category.ToLower()}/{Path.GetFileName(file)}",
                        staticUrl = $"/images/{char.ToUpper(category[0]) + category.Substring(1).ToLower()}/{Path.GetFileName(file)}"
                    })
                    .ToList();

                return Ok(new
                {
                    category = category.ToLower(),
                    count = imageFiles.Count,
                    images = imageFiles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving images: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all available categories
        /// </summary>
        /// <returns>List of available categories</returns>
        [HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                if (!Directory.Exists(_publicPath))
                {
                    return NotFound("Public directory not found");
                }

                var categories = Directory.GetDirectories(_publicPath)
                    .Select(dir => new
                    {
                        name = Path.GetFileName(dir).ToLower(),
                        displayName = Path.GetFileName(dir),
                        imageCount = Directory.GetFiles(dir)
                            .Count(file => new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp" }
                                .Contains(Path.GetExtension(file).ToLower())),
                        url = $"/api/images/{Path.GetFileName(dir).ToLower()}"
                    })
                    .ToList();

                return Ok(new
                {
                    totalCategories = categories.Count,
                    categories = categories
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving categories: {ex.Message}");
            }
        }
    }
}