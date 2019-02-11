using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EBook.Data;
using EBook.Models;
using EBook.Services;

namespace EBook.Controllers
{
	public class CategoriesController : Controller
	{
		private ICategory _categoryManager;
		private IUser _userManager;
		private IBook _bookManager;

		public CategoriesController(ICategory categoryManager, IUser userManager, IBook bookManager)
		{
			_categoryManager = categoryManager;
			_userManager = userManager;
			_bookManager = bookManager;
		}

		// GET: Categories
		public IActionResult Index()
		{
			return View(_categoryManager.GetAllCategoris());
		}

		// GET: Categories/Details/5
		public IActionResult Details(int? id)
		{
			if (id == null)
				return NotFound();

			Category category = _categoryManager.GetById(int.Parse(id.ToString()));

			if (category == null)
				return NotFound();
			
			return View(category);
		}

		// GET: Categories/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Categories/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Category category)
		{
			if (ModelState.IsValid)
			{
				_categoryManager.Create(category);
				return RedirectToAction(nameof(Index));
			}
			return View(category);
		}

		// GET: Categories/Edit/5
		public IActionResult Edit(int? id)
		{
			if (id == null)
				return NotFound();

			Category category = _categoryManager.GetById(int.Parse(id.ToString()));
			if (category == null)
				return NotFound();
		
			return View(category);
		}

		// POST: Categories/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int id, Category category)
		{
			if (id != category.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				_categoryManager.Update(category);
				
				return RedirectToAction(nameof(Index));
			}
			return View(category);
		}

		// GET: Categories/Delete/5
		public IActionResult Delete(int? id)
		{
			if (id == null)
				return NotFound();

			Category category = _categoryManager.GetById(int.Parse(id.ToString()));
			if (category == null)
				return NotFound();

			return View(category);
		}

		// POST: Categories/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(int id)
		{
			if(id == 1)
				return RedirectToAction(nameof(Index));

			_userManager.SetDefaultCategory(id);
			_bookManager.SetDefaultCateogry(id);
			_categoryManager.Delete(id);

			return RedirectToAction(nameof(Index));
		}
	}
}
