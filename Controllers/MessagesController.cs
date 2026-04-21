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

        // GET: Messages
        // GET: Messages - vue gestion réservée à l'admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Messages
                .Where(m => !m.IsDeleted)
                .Include(m => m.Subject)
                .Include(m => m.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
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

        // GET: Messages/Create
        [Authorize]

        public IActionResult Create()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Messages/Create - L'utilisateur doit être connecté pour répondre
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,SubjectId")] Message message)
        {
            // Si le contenu est vide, on retourne à la page du sujet sans créer de message
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
            }

            // Création du message avec les infos serveur (non modifiables par le formulaire)
            var newMessage = new Message
            {
                Content = message.Content,
                CreatedAt = DateTime.Now,          // date définie côté serveur
                IsDeleted = false,                 // jamais supprimé à la création
                SubjectId = message.SubjectId,
                UserId = _userManager.GetUserId(User)  // association automatique à l'utilisateur connecté
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
        }
        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Title", message.SubjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", message.UserId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,CreatedAt,IsDeleted,SubjectId,UserId")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Title", message.SubjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", message.UserId);
            return View(message);
        }

        // GET: Messages/Delete/5
        // GET: Messages/Delete/5 - Propriétaire ou Admin
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

            // Vérification de propriété : seul l'auteur du message (ou un admin) peut le supprimer
            var currentUserId = _userManager.GetUserId(User);
            if (message.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        // POST: Messages/Delete/5 - Suppression LOGIQUE
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

            // Vérification de propriété
            var currentUserId = _userManager.GetUserId(User);
            if (message.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Suppression LOGIQUE (la consigne interdit la suppression définitive)
            message.IsDeleted = true;
            await _context.SaveChangesAsync();

            // Retour à la page du sujet
            return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
        [Authorize]
        // Action "J'aime" - renommée de LikeCount en Like pour correspondre au bouton de la vue
        // On stocke les Ids des usagers qui ont aimé dans le champ LikedByUserIds (séparés par des virgules)
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

            // On récupère la liste des Ids des usagers qui ont déjà aimé ce message
            var likedIds = (message.LikedByUserIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (!likedIds.Contains(userId))
            {
                // L'usager n'a pas encore aimé : on ajoute son Id
                likedIds.Add(userId);
                message.LikedByUserIds = string.Join(",", likedIds);
                message.LikeCount = likedIds.Count;
                await _context.SaveChangesAsync();
            }
            // Si déjà aimé, on ne fait rien (pas de double-like)

            // Retour vers la page du sujet
            return RedirectToAction("Details", "Subject", new { id = message.SubjectId });
        }
    }
    }
