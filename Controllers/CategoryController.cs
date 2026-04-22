using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Data;
using Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Models;

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CategoryController(ApplicationDbContext context)
		{
			_context = context;
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Index()
		{
			return View(await _context.Categories
				.Where(c => !c.IsDeleted)
				.ToListAsync());
		}
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var detailCategory = await _context.Categories
				.Include(c => c.Subjects)
					.ThenInclude(s => s.User)
				.Include(c => c.Subjects)
					.ThenInclude(s => s.Messages)
						.ThenInclude(m => m.User)
				.FirstOrDefaultAsync(c => c.Id == id);


			if (detailCategory == null)
			{
				return NotFound();
			}


			detailCategory.Subjects = detailCategory.Subjects
				.Where(s => !s.IsDeleted)
				.OrderByDescending(s => s.CreatedAt)
				.ToList();

			return View(detailCategory);
		}

		[Authorize(Roles = "Admin")]

		public IActionResult Create()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Create([Bind("Id,Name,Description,IsDeleted")] Category category)
		{
			if (ModelState.IsValid)
			{
				_context.Add(category);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index", "Home");


			}
			return View(category);
		}


		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,IsDeleted")] Category category)
		{
			if (id != category.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(category);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CategoryExists(category.Id))
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
			return View(category);
		}


		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = await _context.Categories
				.FirstOrDefaultAsync(m => m.Id == id);
			if (category == null)
			{
				return NotFound();
			}

			return View(category);
		}




		[Authorize(Roles = "Admin")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{

			var category = await _context.Categories
				.Include(c => c.Subjects)
					.ThenInclude(s => s.Messages)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (category != null)
			{

				category.IsDeleted = true;


				foreach (var subject in category.Subjects)
				{
					subject.IsDeleted = true;
					foreach (var message in subject.Messages)
					{
						message.IsDeleted = true;
					}
				}

				await _context.SaveChangesAsync();
			}


			return RedirectToAction("Index", "Home");
		}

		private bool CategoryExists(int id)
		{
			return _context.Categories.Any(e => e.Id == id);
		}
	}
}