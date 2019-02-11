using System.Collections.Generic;
using System.Linq;
using EBook.Data;
using EBook.Models;
using Microsoft.EntityFrameworkCore;
namespace EBook.Services
{
	public class BookManager : IBook
	{
		private EbookDbContext _context;

		public BookManager(EbookDbContext context)
		{
			_context = context;
		}

		public Book Create(Book book)
		{
			if (book == null)
				return null;

			_context.Book.Add(book);
			_context.SaveChanges();
			return book;
		}

		public void Delete(int id)
		{
			if (id == 0)
				return;

			Book book = GetById(id);
			_context.Entry(book).State = EntityState.Deleted;
			_context.SaveChanges();
		}

		public IEnumerable<Book> GetAllBooks()
		{
			return _context.Book.OrderBy(b => b.Title).Include(b => b.Category).Include(b => b.Language);
		}

		public IEnumerable<Book> GetBooksByCategory(int id)
		{
			return _context.Book.Where(b => b.CategoryId == id).Include(b => b.Category).Include(b => b.Language);
		}

		public IEnumerable<Book> GetBooksByCategoryName(string name)
		{
			return _context.Book.Where(b => b.Category.Name == name).Include(b => b.Category).Include(b => b.Language);
		}

		public IEnumerable<Book> GetBooksByLanguage(int id)
		{
			return _context.Book.Where(b => b.LanguageId == id).Include(b => b.Category).Include(b => b.Language);
		}

		public Book GetById(int id)
		{
			return _context.Book.Include(b => b.Category).Include(b => b.Language).SingleOrDefault(b => b.Id == id);
		}

		public IEnumerable<Book> Search(string searchText, string ascDesc)
		{
			return _context.Book.Where(b => b.Author.Equals(searchText) ||
													  b.FileName.Equals(searchText) ||
													  b.Keywords.Equals(searchText) ||
													  b.PublicationYear.Equals(searchText) ||
													  b.Title.Equals(searchText))
										.Include(b => b.Category).Include(b => b.Language);
		}

		public void SetDefaultCateogry(int categoryId)
		{
			List<Book> books = _context.Book.Where(b => b.CategoryId == categoryId).ToList();
			foreach (Book book in books)
			{
				if(book.CategoryId == categoryId)
				{
					book.CategoryId = 1;
					Update(book);
				}
			}
		}

		public void setDefaultLangauge(int lanugageId)
		{
			List<Book> books = _context.Book.Where(b => b.LanguageId == lanugageId).ToList();
			foreach (Book book in books)
			{
				if(book.LanguageId == lanugageId)
				{
					book.LanguageId = 1;
					Update(book);
				}
			}
		}

		public Book Update(Book book)
		{
			var local = _context.Set<Book>()
			 .Local
			 .FirstOrDefault(entry => entry.Id.Equals(book.Id));

			// check if local is not null 
			if (local != null) // I'm using a extension method
			{
				// detach
				_context.Entry(local).State = EntityState.Detached;
			}
			// set Modified flag in your entry
			_context.Entry(book).State = EntityState.Modified;

			// save 
			_context.SaveChanges();
			return book;
		}
	}
}
