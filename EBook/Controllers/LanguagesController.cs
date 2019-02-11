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
   public class LanguagesController : Controller
   {
		private ILanguage _languageManager;
		private IBook _bookManager;

		public LanguagesController(ILanguage languageManager, IBook bookManager)
		{
			_languageManager = languageManager;
			_bookManager = bookManager;
		}

      // GET: Languages
      public IActionResult Index()
      {
         return View(_languageManager.GetAllLanguages());
      }

      // GET: Languages/Details/5
      public IActionResult Details(int? id)
      {
         if (id == null)
				return NotFound();
			
			Language language = _languageManager.GetById(int.Parse(id.ToString()));

			if (language == null)
				return NotFound();

         return View(language);
      }

      // GET: Languages/Create
      public IActionResult Create()
      {
         return View();
      }

      // POST: Languages/Create
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public IActionResult Create(Language language)
      {
         if (ModelState.IsValid)
         {
				_languageManager.Create(language);
            return RedirectToAction(nameof(Index));
         }
         return View(language);
      }

      // GET: Languages/Edit/5
      public IActionResult Edit(int? id)
      {
			if (id == null)
				return NotFound();

			Language language = _languageManager.GetById(int.Parse(id.ToString()));

			if (language == null)
				return NotFound();
         
         return View(language);
      }

      // POST: Languages/Edit/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public IActionResult Edit(int id, Language language)
      {
         if (id != language.Id)
				return NotFound();
         

         if (ModelState.IsValid)
         {
				_languageManager.Update(language);
            return RedirectToAction(nameof(Index));
         }
         return View(language);
      }

      // GET: Languages/Delete/5
      public IActionResult Delete(int? id)
      {
			if (id == null)
				return NotFound();
			
			Language language = _languageManager.GetById(int.Parse(id.ToString()));

         if (language == null)
				return NotFound();
         

         return View(language);
      }

      // POST: Languages/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public IActionResult DeleteConfirmed(int id)
      {
			if(id == 1)
				return RedirectToAction(nameof(Index));

			_bookManager.setDefaultLangauge(id);
			_languageManager.Delete(id);

         return RedirectToAction(nameof(Index));
      }
   }
}
