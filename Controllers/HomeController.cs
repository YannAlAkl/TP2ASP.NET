using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Data;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context )
        {
         
            _context = context;
        }

		public async Task<IActionResult> Index(int? pageNumber)
		{
			var pageSize = 6;
			var currentPage = pageNumber ?? 1;

			var categoriesQuery = _context.Categories
				.Where(c => !c.IsDeleted)
				.OrderBy(c => c.Name)
				.AsNoTracking();

			var paginatedCategories = await PaginatedList<Category>.CreateAsync(categoriesQuery, currentPage, pageSize);

			foreach (var category in paginatedCategories)
			{
				category.Subjects = await _context.Subjects
					.Where(s => s.CategoryId == category.Id && !s.IsDeleted)
					.OrderByDescending(s => s.CreatedAt)
					.Take(3)
					.Include(s => s.User)
					.Include(s => s.Messages.Where(m => !m.IsDeleted))
					.ThenInclude(m => m.User)
					.AsNoTracking()
					.ToListAsync();
			}

			return View(paginatedCategories);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
