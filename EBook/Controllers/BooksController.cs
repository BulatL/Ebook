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
using Newtonsoft.Json;
using EBook.viewModel;

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
		[Route("[controller]")]
      public IActionResult Index()
      {
			var loggedInUserId = HttpContext.Session.GetString("LoggedInUserId");

			string downloadCategory = "none";

			if (loggedInUserId != null)
			{
				User user = _userManager.GetById(int.Parse(loggedInUserId));

				if (user != null)
				{
					if (user.Role == Role.Admin)
						downloadCategory = "all";
					else
					{
						if (user.SubscribedCategorieId == null)
							downloadCategory = "all";
						else
							downloadCategory = user.SubscribedCategorieId.ToString();
					}	
				}
			}

			List<BookIndexViewModel> books = BookIndexViewModel.CopyList(_bookManager.GetAllBooks().ToList(), downloadCategory);
			
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name");
			return View(books);


			/*List<Book> books = new List<Book>();
	
			if (loggedInUserId == null)
			{
				books.AddRange(_bookManager.GetAllBooks());
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name");
				return View(books);
			}
			User user = _userManager.GetById(int.Parse(loggedInUserId));
			
			if (user == null)
			{
				books.AddRange(_bookManager.GetAllBooks());
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name");
				return View(books);
			}
			if(user.SubscribedCategorieId != null)
			{
				int categoryId = 1;
				if (user.SubscribedCategorieId != null)
				{
					categoryId = int.Parse(user.SubscribedCategorieId.ToString());
					books.AddRange(_bookManager.GetBooksByCategory(categoryId));
				}
				else
					books.AddRange(_bookManager.GetBooksByCategory(int.Parse(user.SubscribedCategorieId.ToString())));
				List<Category> categories = new List<Category>();
				Category category = _categoryManager.GetById(int.Parse(user.SubscribedCategorieId.ToString()));
				categories.Add(category);
				ViewData["Category"] = new SelectList(categories, "Name", "Name", categoryId);
				return View(books);
			}
			else
			{
				books.AddRange(_bookManager.GetAllBooks());
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name");
				return View(books);
			}*/

		}

		[Route("[controller]/{name}")]
		public IActionResult GetByCategory(string name)
		{
			var loggedInUserId = HttpContext.Session.GetString("LoggedInUserId");

			string downloadCategory = "none";

			if (loggedInUserId != null)
			{
				User user = _userManager.GetById(int.Parse(loggedInUserId));

				if (user != null)
				{
					if (user.SubscribedCategorieId == null)
						downloadCategory = "all";
					else
						downloadCategory = user.SubscribedCategorieId.ToString();
				}
			}

			List<BookIndexViewModel> books = BookIndexViewModel.CopyList(_bookManager.GetBooksByCategoryName(name).ToList(), downloadCategory);

			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name", name);
			return View("Index", books);

			/*List<Book> books = new List<Book>();
			string categorySelectName = "";

			if (loggedInUserId == null)
			{
				books.AddRange(_bookManager.GetBooksByCategoryName(name));
				categorySelectName = books.FirstOrDefault(b => b.Category.Name == name).Category.Name;
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name", categorySelectName);
				return View("Index", books);
			}
			User user = _userManager.GetById(int.Parse(loggedInUserId));

			if (user == null)
			{
				books.AddRange(_bookManager.GetAllBooks());
				categorySelectName = books.FirstOrDefault(b => b.Category.Name == name).Category.Name;
				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name", categorySelectName);
				return View("Index", books);
			}
			else if (user.SubscribedCategorieId != null)
			{
				books.AddRange(_bookManager.GetBooksByCategoryName(name));
				categorySelectName = books.FirstOrDefault(b => b.Category.Name == name).Category.Name;

				List<Category> categories = new List<Category>();
				Category category = _categoryManager.GetById(int.Parse(user.SubscribedCategorieId.ToString()));
				categories.Add(category);
				ViewData["Category"] = new SelectList(categories, "Name", "Name", categorySelectName);
				return View("Index", books);
			}
			else
			{
				books.AddRange(_bookManager.GetBooksByCategoryName(name));
				categorySelectName = books.FirstOrDefault(b => b.Category.Name == name).Category.Name;

				ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Name", "Name", categorySelectName);
				return View("Index",books);
			}*/
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
      public IActionResult Create(BookViewModel bookView)
      {
         if (ModelState.IsValid)
         {
				UploadPdf(bookView.FileName, bookView.BookFile, true);
				DeletePdf(_hostingEnvironment.WebRootPath + "/uploads/" + bookView.OldfFilename);
				
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
				elasticSearchController.IndexBook(book);
            return RedirectToAction(nameof(Index));
         }
         ViewData["CategoryId"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", bookView.CategoryId);
         ViewData["LanguageId"] = new SelectList(_languageManager.GetAllLanguages(), "Id", "Name", bookView.LanguageId);
         return View(bookView);
      }

      // GET: Books/Edit/5
      public IActionResult Edit(int id)
      {
			Book book = _bookManager.GetById(id);
         if (book == null)
         {
               return NotFound();
			}
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", book.CategoryId);
			ViewData["Language"] = new SelectList(_languageManager.GetAllLanguages(), "Id", "Name", book.LanguageId);
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
				Book oldBook = _bookManager.GetById(id);
				if(oldBook.FileName != book.FileName)
				{
					DeletePdf(_hostingEnvironment.WebRootPath + "uploads/" + oldBook.FileName);
				}
            _bookManager.Update(book);

				ElasticsearchController elasticSearchController = new ElasticsearchController(_hostingEnvironment, _bookManager);
				elasticSearchController.UpdateIndex(id);

				return RedirectToAction(nameof(Index));
			}
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", book.CategoryId);
			ViewData["Language"] = new SelectList(_languageManager.GetAllLanguages(), "Id", "Name", book.LanguageId);
			return View(book);
      }

      // GET: Books/Delete/5
      public IActionResult Delete(int? id)
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

      // POST: Books/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public IActionResult DeleteConfirmed(int id)
      {
			Book book = _bookManager.GetById(id);
			_bookManager.Delete(id);

			ElasticsearchController elasticSearchController = new ElasticsearchController(_hostingEnvironment, _bookManager);
			elasticSearchController.DeleteDocument(id);
			
			string path = _hostingEnvironment.WebRootPath + "/uploads/" + book.FileName;
			DeletePdf(path);

			return RedirectToAction(nameof(Index));
      }

		[HttpPost]
		public IActionResult ReadMetadata(IFormFile pdf)
		{
			string bookName = UploadPdf(pdf.FileName, pdf, false);
			string randomFilname = bookName.Split("&")[0];
			string filename = bookName.Split("&")[1];

			string filePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads/" + randomFilname);

			PdfReader reader = new PdfReader(filePath);

			string info = reader.Info.ToString();
			string title = reader.Info["Title"];
			string authror = reader.Info["Author"];
			string keywords = reader.Info["Keywords"];
			string date = reader.Info["CreationDate"];
			DateTime calendar = PdfDate.Decode(date);
			string creationDate = calendar.ToString("yyyy-MM-dd");

			reader.Close();

			return Json(title + "^" + authror + "^" + keywords + "^" + creationDate + "^" + filename + "^" + pdf.ContentType + "^" + randomFilname);
		}

		public async Task<IActionResult> Download(int id)
		{
			Book book = _bookManager.GetById(id);

			if (book == null)
				return NotFound();

			if (book.FileName == null)
				return NotFound();

			var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads", book.FileName);

			var memory = new MemoryStream();
			using (var stream = new FileStream(path, FileMode.Open))
			{
				await stream.CopyToAsync(memory);
			}
			memory.Position = 0;
			return File(memory, "application/pdf", System.IO.Path.GetFileName(path));
		}

		public string UploadPdf(string bookName, IFormFile pdf, bool createPdf)
		{
			string filename = "";
			if (createPdf == true)
				filename = bookName;
			else
				filename = System.IO.Path.GetRandomFileName();
			var uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
			var filePath = System.IO.Path.Combine(uploads, filename);
			FileStream fs = new FileStream(filePath, FileMode.Create);
			pdf.CopyTo(fs);
			fs.Close();
			string returnString = filename + "&" + bookName;
			return returnString;
		}

		public void DeletePdf(string path)
		{
			System.IO.File.Delete(path);
			return;
		}
	}
}
