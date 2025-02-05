using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquariumForum;
using AquariumForum.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AquariumForum.Controllers
{
    public class CommentsController : Controller
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["DiscussionId"] = new SelectList(_context.Discussions, "DiscussionId", "DiscussionId");
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Content,CreateDate,DiscussionId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Discussions", new { id = comment.DiscussionId });
            }

            ViewData["DiscussionId"] = new SelectList(_context.Discussions, "DiscussionId", "DiscussionId", comment.DiscussionId);
            return View(comment);
        }
    }
}
