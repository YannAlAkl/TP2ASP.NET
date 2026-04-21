using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Data;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Controllers
{
	public class SubjectController : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubjectController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Subjects
        // GET: Subjects - vue gestion réservée à l'admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var sujets = _context.Subjects
                .Where(s => !s.IsDeleted)
                .Include(s => s.Category)
                .Include(s => s.User);
            return View(await sujets.ToListAsync());
        }

        // GET: Subjects/Details/5
        // GET: Subjects/Details/5?page=1&pageSize=10
        public async Task<IActionResult> Details(int id, int page = 1, int pageSize = 10)
        {
            // On récupère le sujet avec son auteur et sa catégorie (sans les messages pour l'instant)
            var subject = await _context.Subjects
                .Include(s => s.User)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (subject == null)
            {
                return NotFound();
            }

            // *** Incrémentation du nombre de vues à chaque visite ***
            subject.ViewCount++;
            _context.Update(subject);
            await _context.SaveChangesAsync();

            // Validation des paramètres de pagination (valeurs par défaut sécuritaires)
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            // Nombre total de messages (non supprimés) pour ce sujet
            int totalMessages = await _context.Messages
                .CountAsync(m => m.SubjectId == id && !m.IsDeleted);

            int totalPages = (int)Math.Ceiling(totalMessages / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            // On charge uniquement la page de messages demandée, avec leur auteur
            var messages = await _context.Messages
                .Where(m => m.SubjectId == id && !m.IsDeleted)
                .Include(m => m.User)
                .OrderBy(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            subject.Messages = messages;

            // Calcul du nombre de messages par utilisateur (affiché dans la barre latérale de chaque message)
            var userPostCounts = await _context.Messages
                .Where(m => !m.IsDeleted && m.UserId != null)
                .GroupBy(m => m.UserId!)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            // On passe toutes les données de pagination à la vue
            ViewBag.MessageCount = totalMessages;
            ViewBag.UserPostCounts = userPostCounts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(subject);
        }
		        
        // GET: Subjects/Create
		public IActionResult Create()
		{
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
			return View();
		}

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,CategoryId")] Subject subject)
        {
            // On associe automatiquement le sujet à l'utilisateur connecté
            subject.UserId = _userManager.GetUserId(User);
            subject.CreatedAt = DateTime.Now;
            subject.ViewCount = 0;
            subject.IsDeleted = false;

            // On retire les erreurs de validation sur les propriétés de navigation
            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Subject", new { id = subject.Id });
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name", subject.CategoryId);
            return View(subject);
        }

        // GET: Subjects/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            // Vérification de propriété : seul le créateur (ou un admin) peut modifier
            var currentUserId = _userManager.GetUserId(User);
            if (subject.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name", subject.CategoryId);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CategoryId")] Subject subject)
        {
            if (id != subject.Id)
            {
                return NotFound();
            }

            // On charge le sujet original de la BD pour conserver les champs non modifiables
            var existing = await _context.Subjects.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            // Vérification de propriété
            var currentUserId = _userManager.GetUserId(User);
            if (existing.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    // On modifie uniquement les champs autorisés (on garde UserId, CreatedAt, ViewCount, IsDeleted intacts)
                    existing.Title = subject.Title;
                    existing.Content = subject.Content;
                    existing.CategoryId = subject.CategoryId;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Redirection vers la page de détails du sujet (et non vers la liste scaffoldée)
                return RedirectToAction("Details", "Subject", new { id = subject.Id });
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name", subject.CategoryId);
            return View(subject);
        }

        // GET: Subjects/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var subject = await _context.Subjects
				.Include(s => s.Category)
				.Include(s => s.User)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (subject == null)
			{
				return NotFound();
			}

			return View(subject);
		}

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // On charge les messages pour pouvoir les marquer supprimés en cascade
            var subject = await _context.Subjects
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject != null)
            {
                int categoryId = subject.CategoryId;
                subject.IsDeleted = true;

                // Suppression logique en cascade des messages du sujet
                foreach (var m in subject.Messages)
                {
                    m.IsDeleted = true;
                }

                await _context.SaveChangesAsync();

                // Retour vers la page de la catégorie (plus utile qu'une liste scaffoldée)
                return RedirectToAction("Details", "Category", new { id = categoryId });
            }

            return RedirectToAction("Index", "Home");
        }

        private bool SubjectExists(int id)
		{
			return _context.Subjects.Any(e => e.Id == id);
		}
	}
}
