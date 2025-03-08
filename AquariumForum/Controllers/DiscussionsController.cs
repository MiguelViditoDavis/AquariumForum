using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquariumForum.Models;

namespace AquariumForum.Controllers
{
    public class DiscussionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DiscussionsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Discussions (Shows ALL discussions)
        public async Task<IActionResult> Index()
        {
            var discussions = await _context.Discussions
                .Include(d => d.User)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            return View(discussions);
        }

        // GET: My Threads (Shows ONLY the logged-in user's discussions)
        [Authorize]
        public async Task<IActionResult> MyThreads()
        {
            var userId = _userManager.GetUserId(User);
            var myThreads = await _context.Discussions
                .Where(d => d.UserId == userId)
                .Include(d => d.User)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            return View("Index", myThreads); // Uses the same Index.cshtml view
        }

        // GET: Discussions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var discussion = await _context.Discussions
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);

            if (discussion == null) return NotFound();

            return View(discussion);
        }

        // GET: Discussions/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discussions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] Discussion discussion, IFormFile imageFile)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("ImageFilename");

            if (!ModelState.IsValid)
            {
                return View(discussion);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "You must be logged in to create a discussion.");
                return View(discussion);
            }

            discussion.UserId = userId;
            discussion.CreateDate = DateTime.Now;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                discussion.ImageFilename = fileName;
            }
            else
            {
                ModelState.AddModelError("ImageFilename", "An image is required.");
                return View(discussion);
            }

            _context.Add(discussion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Discussions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var discussion = await _context.Discussions.FindAsync(id);
            if (discussion == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (discussion.UserId != userId)
            {
                return Forbid(); // Prevents users from editing other users' posts
            }

            return View(discussion);
        }

        // POST: Discussions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("DiscussionId,Title,Content,ImageFilename")] Discussion discussion)
        {
            if (id != discussion.DiscussionId) return NotFound();

            var existingDiscussion = await _context.Discussions.AsNoTracking().FirstOrDefaultAsync(d => d.DiscussionId == id);
            if (existingDiscussion == null) return NotFound();

            if (existingDiscussion.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Prevents unauthorized edits
            }

            discussion.CreateDate = existingDiscussion.CreateDate;
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (!ModelState.IsValid) return View(discussion);

            try
            {
                _context.Update(discussion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscussionExists(discussion.DiscussionId)) return NotFound();
                throw;
            }
        }

        // GET: Discussions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var discussion = await _context.Discussions.FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null) return NotFound();

            if (discussion.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Prevents unauthorized deletion
            }

            return View(discussion);
        }

        // POST: Discussions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussion = await _context.Discussions.FindAsync(id);
            if (discussion == null) return NotFound();

            if (discussion.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Prevents unauthorized deletion
            }

            _context.Discussions.Remove(discussion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionExists(int id)
        {
            return _context.Discussions.Any(e => e.DiscussionId == id);
        }
    }
}
