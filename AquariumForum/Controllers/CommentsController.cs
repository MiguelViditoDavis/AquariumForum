using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquariumForum.Models;
using System.Threading.Tasks;
using System.Linq;

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
        public IActionResult Create(int discussionId)
        {
            // Pass the discussionId to the view for the hidden input and back link
            ViewData["DiscussionId"] = discussionId;
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Content,CreateDate,DiscussionId")] Comment comment)
        {
            Console.WriteLine($"DEBUG: DiscussionId = {comment.DiscussionId}");
            Console.WriteLine($"DEBUG: Content = {comment.Content}");

            if (ModelState.IsValid)
            {
                Console.WriteLine("DEBUG: Adding comment to database...");
                comment.CreateDate = DateTime.Now;  // Set the current date and time

                _context.Add(comment);
                await _context.SaveChangesAsync();

                Console.WriteLine("DEBUG: Comment successfully saved.");
                // Redirect to the discussion details page after adding a comment
                return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
            }

            Console.WriteLine("DEBUG: ModelState is not valid. Errors:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            // Preserve discussionId if there's a validation error
            ViewData["DiscussionId"] = comment.DiscussionId;
            return View(comment);
        }
    }
}
