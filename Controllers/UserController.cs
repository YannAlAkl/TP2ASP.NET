
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models.ViewModels;


namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Controllers
{
	public class UserController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;

		public UserController(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		// GET: UserController
		public ActionResult Index()
		{
			return View();
		}

		// GET: UserController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: UserController/Create
		public async Task<ActionResult> Create()
		{
			return View();
		}

		// POST: UserController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult>	 Create(createUserViewModel vm)
		{
			var user = new IdentityUser { UserName = vm.Username, Email = vm.Email };
				var result = await _userManager.CreateAsync(user, vm.Password);
				if (result.Succeeded)
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
					return View(vm);
			}

			
			
		}

		// GET: UserController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: UserController/Edit/5
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

		// GET: UserController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: UserController/Delete/5
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
