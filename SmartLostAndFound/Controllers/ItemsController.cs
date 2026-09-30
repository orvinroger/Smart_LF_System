using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLostAndFound.Data;
using SmartLostAndFound.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SmartLostAndFound.Controllers
{
    public class ItemsController : Controller
    {
        private const long MaxImageSize = 5 * 1024 * 1024;
        private static readonly Dictionary<string, string> ImageTypes = new Dictionary<string, string>
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var items = await _context.LostFoundItems.ToListAsync();
            return View(items);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LostFoundItem item, IFormFile image)
        {
            var extension = image == null ? null : Path.GetExtension(image.FileName).ToLowerInvariant();
            if (image != null && image.Length > 0 &&
                (image.Length > MaxImageSize || !ImageTypes.TryGetValue(extension, out var contentType) ||
                 !string.Equals(image.ContentType, contentType, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Image", "Choose a JPG, PNG, or WebP image smaller than 5 MB.");
            }

            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                    var uploadDirectory = Path.Combine(webRoot, "images", "items");
                    Directory.CreateDirectory(uploadDirectory);
                    var fileName = $"{Guid.NewGuid():N}{extension}";
                    var filePath = Path.Combine(uploadDirectory, fileName);
                    await using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    item.ImagePath = $"/images/items/{fileName}";
                }

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Claim(int id)
        {
            var item = await _context.LostFoundItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (item.IsClaimed)
            {
                TempData["ClaimMessage"] = "This item has already been claimed.";
            }
            else
            {
                item.IsClaimed = true;
                await _context.SaveChangesAsync();
                TempData["ClaimMessage"] = "Claim recorded. Use the pickup details below to arrange collection.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.LostFoundItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
    }
}