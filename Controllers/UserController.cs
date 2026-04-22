using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Data;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models.ViewModels;


namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Controllers
{
	public class UserController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _context;

		public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;

		}

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var userStats = await _userManager.Users.ToListAsync();
            var result = new List<UserStatsViewModel>();
            foreach (var user in userStats)
            {
                
                var subjectCount = await _context.Subjects
                    .CountAsync(s => s.UserId == user.Id && !s.IsDeleted);

                var messageCount = await _context.Messages
                    .CountAsync(m => m.UserId == user.Id && !m.IsDeleted);

                var lastActivityMessage = await _context.Messages
                    .Where(m => m.UserId == user.Id && !m.IsDeleted)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.CreatedAt)
                    .FirstOrDefaultAsync();

                var lastActivitySubject = await _context.Subjects
                    .Where(s => s.UserId == user.Id && !s.IsDeleted)
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => s.CreatedAt)
                    .FirstOrDefaultAsync();

                var lastActivity = lastActivityMessage > lastActivitySubject
                    ? lastActivityMessage
                    : lastActivitySubject;

                result.Add(new UserStatsViewModel
                {
                    Id = user.Id.GetHashCode(),
                    UserName = user.UserName,
                    Email = user.Email,
                    SubjectCount = subjectCount,
                    MessageCount = messageCount,
                    LastActivity = lastActivity
                });
            }
            return View(result);
        }

        
        public ActionResult Details(int id)
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		
		public async Task<ActionResult> Create()
		{
			return View();
		}
	[Authorize(Roles = "Admin")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> Create(createUserViewModel vm)
	{
		if (!ModelState.IsValid)
			return View(vm);

		var user = new IdentityUser { UserName = vm.UserName, Email = vm.Email };
		var result = await _userManager.CreateAsync(user, vm.Password);

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);
			return View(vm);
		}

		var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);

		if (!roleResult.Succeeded)
		{
			foreach (var error in roleResult.Errors)
				ModelState.AddModelError(string.Empty, error.Description);
			return View(vm);
		}

		return RedirectToAction(nameof(Index));
	}
		
		public ActionResult Edit(int id)
		{
			return View();
		}

		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		
		public ActionResult Delete(int id)
		{
			return View();
		}

		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
