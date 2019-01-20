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
      private readonly EbookDbContext _context;
		private IHostingEnvironment _hostingEnvironment;
		private IBook _bookManager;
		private ICategory _categoryManager;
		private ILanguage _languageManager;

      public BooksController(EbookDbContext context,IHostingEnvironment hostingEnvironment , IBook bookManager, 
			ICategory categoryManager, ILanguage languageManager)
      {
         _context = context;
			_hostingEnvironment = hostingEnvironment;
			_bookManager = bookManager;
			_categoryManager = categoryManager;
			_languageManager = languageManager;
      }

      // GET: Books
      public async Task<IActionResult> Index()
      {
         var ebookDbContext = _context.Book.Include(b => b.Category).Include(b => b.Language);
         return View(await ebookDbContext.ToListAsync());
      }

      // GET: Books/Details/5
      public async Task<IActionResult> Details(int? id)
      {
         if (id == null)
         {
               return NotFound();
         }

         var book = await _context.Book
               .Include(b => b.Category)
               .Include(b => b.Language)
               .FirstOrDefaultAsync(m => m.Id == id);
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
            _context.Add(book);
            await _context.SaveChangesAsync();
				ElasticSearchController elasticSearchController = new ElasticSearchController(_hostingEnvironment, _bookManager);
				await elasticSearchController.Index(book);
            return RedirectToAction(nameof(Index));
         }
         ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", bookView.CategoryId);
         ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Name", bookView.LanguageId);
         return View(bookView);
      }

      // GET: Books/Edit/5
      public async Task<IActionResult> Edit(int? id)
      {
         if (id == null)
         {
               return NotFound();
         }

         var book = await _context.Book.FindAsync(id);
         if (book == null)
         {
               return NotFound();
         }
         ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", book.CategoryId);
         ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Name", book.LanguageId);
         return View(book);
      }

      // POST: Books/Edit/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("Title,Author,Keywords,PublicationYear,FileName,MIME,CategoryId,LanguageId,Id")] Book book)
      {
         if (id != book.Id)
         {
               return NotFound();
         }

         if (ModelState.IsValid)
         {
               try
               {
                  _context.Update(book);
                  await _context.SaveChangesAsync();
               }
               catch (DbUpdateConcurrencyException)
               {
                  if (!BookExists(book.Id))
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
         ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", book.CategoryId);
         ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Name", book.LanguageId);
         return View(book);
      }

      // GET: Books/Delete/5
      public async Task<IActionResult> Delete(int? id)
      {
         if (id == null)
         {
               return NotFound();
         }

         var book = await _context.Book
               .Include(b => b.Category)
               .Include(b => b.Language)
               .FirstOrDefaultAsync(m => m.Id == id);
         if (book == null)
         {
               return NotFound();
         }

         return View(book);
      }

      // POST: Books/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id)
      {
         var book = await _context.Book.FindAsync(id);
         _context.Book.Remove(book);
         await _context.SaveChangesAsync();
         return RedirectToAction(nameof(Index));
      }

		[HttpPost]
		public IActionResult ReadMetadata(IFormFile pdf)
		{
			string fileName = UploadPdf(pdf.FileName, pdf);
			string filePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads/" + fileName);
			/*PdfDocument document = PdfReader.Open(filePath);
			string info = document.Info.ToString();
			string title = document.Info.Title.ToString();
			string authror = document.Info.Author.ToString();
			string keywords = document.Info.Keywords.ToString();
			string creationDate = document.Info.CreationDate.ToString("yyyy-MM-dd");*/

			PdfReader reader = new PdfReader(filePath);
			/*var bodyFromPdf = string.Empty;
			for (int c = 1; c <= reader.NumberOfPages; c++)
			{
				ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
				bodyFromPdf += PdfTextExtractor.GetTextFromPage(reader, c, strategy);
			}*/
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

      private bool BookExists(int id)
      {
         return _context.Book.Any(e => e.Id == id);
      }

		public async Task<IActionResult> Download(string filename)
		{
			/*var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/" + filename);
			FileStream fs = new FileStream(filePath, FileMode.Create);
			return File(fs, "blob");*/

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
