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
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using EBook.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace EBook.Controllers
{
   public class BooksController : Controller
   {
		private IHostingEnvironment _hostingEnvironment;
		private IBook _bookManager;
		private ICategory _categoryManager;
		private ILanguage _languageManager;
		private IUser _userManager;

      public BooksController(IHostingEnvironment hostingEnvironment , IBook bookManager, 
			ICategory categoryManager, ILanguage languageManager, IUser userManager)
      {
			_hostingEnvironment = hostingEnvironment;
			_bookManager = bookManager;
			_categoryManager = categoryManager;
			_languageManager = languageManager;
			_userManager = userManager;
      }

      // GET: Books
		[Route("[controller]/{id?}")]
      public IActionResult Index(int? id)
      {
			var loggedInUserId = HttpContext.Session.GetString("LoggedInUserId");
			List<Book> books = new List<Book>();
			int categorySelectId = 1;
			if(id != null)
			{
				categorySelectId = int.Parse(id.ToString());
			}
			if (loggedInUserId == null)
			{
				if (id != null)
					books.AddRange(_bookManager.GetBooksByCategory(categorySelectId));
				else
					books.AddRange(_bookManager.GetAllBooks());
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", categorySelectId);
				return View(books);
			}
			User user = _userManager.GetById(int.Parse(loggedInUserId));
			
			if (user == null)
			{
				if (id != null)
					books.AddRange(_bookManager.GetBooksByCategory(categorySelectId));
				else
					books.AddRange(_bookManager.GetAllBooks());
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", categorySelectId);
				return View(books);
			}
			if(user.SubscribedCategorieId != null)
			{
				if (id != null)
					books.AddRange(_bookManager.GetBooksByCategory(categorySelectId));
				else
					books.AddRange(_bookManager.GetBooksByCategory(int.Parse(user.SubscribedCategorieId.ToString())));
				List<Category> categories = new List<Category>();
				Category category = _categoryManager.GetById(int.Parse(user.SubscribedCategorieId.ToString()));
				categories.Add(category);
				ViewData["Category"] = new SelectList(categories, "Id", "Name");
				return View(books);
			}
			else
			{
				if (id != null)
					books.AddRange(_bookManager.GetBooksByCategory(categorySelectId));
				else
					books.AddRange(_bookManager.GetAllBooks());
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", categorySelectId);
				return View(books);
			}
      }

      // GET: Books/Details/5
      public IActionResult Details(int? id)
      {
         if (id == null)
         {
               return NotFound();
         }

			var book = _bookManager.GetById(int.Parse(id.ToString()));
         if (book == null)
         {
               return NotFound();
         }

         return View(book);
      }

      // GET: Books/Create
      public IActionResult Create()
      {
         ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name");
         ViewData["Language"] = new SelectList(_languageManager.GetAllLanguages(), "Id", "Name");
         return View();
      }

      // POST: Books/Create
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create(BookViewModel bookView)
      {
         if (ModelState.IsValid)
         {
				Book book = new Book()
				{
					Author = bookView.Author,
					CategoryId = bookView.CategoryId,
					LanguageId = bookView.LanguageId,
					FileName = bookView.FileName,
					Keywords = bookView.Keywords,
					MIME = bookView.MIME,
					PublicationYear = bookView.PublicationYear,
					Title = bookView.Title
					
				};
				_bookManager.Create(book);
				ElasticsearchController elasticSearchController = new ElasticsearchController(_hostingEnvironment, _bookManager);
				await elasticSearchController.IndexBook(book);
            return RedirectToAction(nameof(Index));
         }
         ViewData["CategoryId"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", bookView.CategoryId);
         ViewData["LanguageId"] = new SelectList(_languageManager.GetAllLanguages(), "Id", "Name", bookView.LanguageId);
         return View(bookView);
      }

      // GET: Books/Edit/5
      public IActionResult Edit(int? id)
      {
         if (id == null)
         {
               return NotFound();
         }

			var book = _bookManager.GetAllBooks();
         if (book == null)
         {
               return NotFound();
			}
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name");
			ViewData["Language"] = new SelectList(_languageManager.GetAllLanguages(), "Id", "Name");
			return View(book);
      }

      // POST: Books/Edit/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public IActionResult Edit(int id, Book book)
      {
         if (id != book.Id)
         {
               return NotFound();
         }

         if (ModelState.IsValid)
         {
            _bookManager.Update(book);

				ElasticsearchController elasticSearchController = new ElasticsearchController(_hostingEnvironment, _bookManager);
				elasticSearchController.UpdateIndex(id);

				return RedirectToAction(nameof(Index));
			}
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name");
			ViewData["Language"] = new SelectList(_languageManager.GetAllLanguages(), "Id", "Name");
			return View(book);
      }

      // GET: Books/Delete/5
      public IActionResult Delete(int? id)
      {
         if (id == null)
         {
               return NotFound();
         }

			var book = _bookManager.GetAllBooks();
         if (book == null)
         {
               return NotFound();
         }

         return View(book);
      }

      // POST: Books/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public IActionResult DeleteConfirmed(int id)
      {
			_bookManager.Delete(id);

			ElasticsearchController elasticSearchController = new ElasticsearchController(_hostingEnvironment, _bookManager);
			elasticSearchController.DeleteDocument(id);

			return RedirectToAction(nameof(Index));
      }

		[HttpPost]
		public IActionResult ReadMetadata(IFormFile pdf)
		{
			string fileName = UploadPdf(pdf.FileName, pdf);
			string filePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads/" + fileName);

			PdfReader reader = new PdfReader(filePath);

			string info = reader.Info.ToString();
			string title = reader.Info["Title"];
			string authror = reader.Info["Author"];
			string keywords = reader.Info["Keywords"];
			string date = reader.Info["CreationDate"];
			DateTime calendar = PdfDate.Decode(date);
			string creationDate = calendar.ToString("yyyy-MM-dd");

			reader.Close();

			return Json(title + "^" + authror + "^" + keywords + "^" + creationDate + "^" + fileName + "^" + pdf.ContentType);
		}

		public async Task<IActionResult> Download(string filename)
		{
			if (filename == null)
				return Content("filename not present");

			var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads", filename);

			var memory = new MemoryStream();
			using (var stream = new FileStream(path, FileMode.Open))
			{
				await stream.CopyToAsync(memory);
			}
			memory.Position = 0;
			return File(memory, "application/pdf", System.IO.Path.GetFileName(path));
		}

		public string UploadPdf(string bookName, IFormFile pdf)
		{
			string fn = bookName.Replace(" ", "");
			var fileName = fn;
			var uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
			var filePath = System.IO.Path.Combine(uploads, fileName);
			FileStream fs = new FileStream(filePath, FileMode.Create);
			pdf.CopyTo(fs);
			fs.Close();
			return fileName;
		}
	}
}
