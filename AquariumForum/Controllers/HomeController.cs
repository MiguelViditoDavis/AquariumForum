using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquariumForum.Models;
using AquariumForum;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // Displays all discussions on the home page
    public async Task<IActionResult> Index()
    {
        var discussions = await _context.Discussions
            .Include(d => d.Comments)
            .OrderByDescending(d => d.CreateDate)
            .ToListAsync();

        return View(discussions);
    }

    // Displays a single discussion with details and sorted comments
    public async Task<IActionResult> GetDiscussion(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Include comments and order them by creation date (descending)
        var discussion = await _context.Discussions
            .Include(d => d.Comments.OrderByDescending(c => c.CreateDate))
            .FirstOrDefaultAsync(d => d.DiscussionId == id);

        if (discussion == null)
        {
            return NotFound();
        }

        return View(discussion);
    }

    // Handles errors and unexpected scenarios (optional)
    public IActionResult Error()
    {
        return View();
    }
}
