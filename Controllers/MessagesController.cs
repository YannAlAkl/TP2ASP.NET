using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Data;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Controllers
{
    public class MessagesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Messages
                .Where(m => !m.IsDeleted)
                .Include(m => m.Subject)
                .Include(m => m.User);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Subject)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }


        [Authorize]

        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,SubjectId")] Message message)
        {

            if (string.IsNullOrWhiteSpace(message.Content))
            {
                return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
            }


            var newMessage = new Message
            {
                Content = message.Content,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                SubjectId = message.SubjectId,
                UserId = _userManager.GetUserId(User)
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
        }
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var message = await _context.Messages.FindAsync(id);
            if (message == null) return NotFound();

           
            var currentUserId = _userManager.GetUserId(User);
            if (message.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Title", message.SubjectId);
            return View(message);
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content")] Message message)
        {
            if (id != message.Id) return NotFound();

            var existing = await _context.Messages.FindAsync(id);
            if (existing == null) return NotFound();

            // Ownership check
            var currentUserId = _userManager.GetUserId(User);
            if (existing.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            ModelState.Remove("User");
            ModelState.Remove("Subject");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    existing.Content = message.Content;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", "Subject", new { id = existing.SubjectId });
            }

            return View(message);
        }


        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Subject)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (message.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(message);
        }


        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return RedirectToAction("Index", "Home");
            }


            var currentUserId = _userManager.GetUserId(User);
            if (message.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }


            message.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> Like(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Forbid();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            var likedIds = (message.LikedByUserIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (!likedIds.Contains(userId))
            {
                likedIds.Add(userId);
                message.LikedByUserIds = string.Join(",", likedIds);
                message.LikeCount = likedIds.Count;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
        }
    }
    }
