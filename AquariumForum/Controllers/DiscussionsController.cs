using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquariumForum;
using AquariumForum.Models;

namespace AquariumForum.Controllers
{
    public class DiscussionsController : Controller
    {
        private readonly AppDbContext _context;

        public DiscussionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Discussions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Discussions.ToListAsync());
        }

        // GET: Discussions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        // GET: Discussions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discussions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscussionId,Title,Content,CreateDate")] Discussion discussion, IFormFile imageFile)
        {
            // Step 1: Log any ModelState validation errors
            ModelState.Remove("ImageFilename");  // Remove ImageFilename from validation

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                return View(discussion); // If invalid, return early
            }

            Console.WriteLine("ModelState is valid. Proceeding with file saving...");

            // Step 2: Debug file saving with a simple test file
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    var testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "test_debug.txt");
                    Console.WriteLine($"DEBUG: Attempting to write to {testFilePath}");

                    // Write a test file to check if saving works
                    await System.IO.File.WriteAllTextAsync(testFilePath, "This is a debug test file.");
                    Console.WriteLine("DEBUG: Test file written successfully!");

                    // Proceed with image saving logic
                    var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    Console.WriteLine($"DEBUG: Saving image to {filePath}");
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    Console.WriteLine("DEBUG: Image saved successfully!");

                    discussion.ImageFilename = fileName;
                    discussion.CreateDate = DateTime.Now;

                    _context.Add(discussion);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: Could not save file or discussion. Exception: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No image file uploaded.");
            }

            return View(discussion);
        }

        // GET: Discussions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions.FindAsync(id);
            if (discussion == null)
            {
                return NotFound();
            }
            return View(discussion);
        }

        // POST: Discussions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscussionId,Title,Content,ImageFilename,CreateDate")] Discussion discussion)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionExists(discussion.DiscussionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }

        // GET: Discussions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        // POST: Discussions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussion = await _context.Discussions.FindAsync(id);
            if (discussion != null)
            {
                _context.Discussions.Remove(discussion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionExists(int id)
        {
            return _context.Discussions.Any(e => e.DiscussionId == id);
        }
    }
}
