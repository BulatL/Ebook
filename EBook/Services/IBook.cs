using EBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Services
{
	public interface IBook
	{
		IEnumerable<Book> GetAllBooks();
		IEnumerable<Book> GetBooksByCategory(int id);
		IEnumerable<Book> GetBooksByLanguage(int id);
		IEnumerable<Book> Search(string searchText, string ascDesc);
		Book GetById(int id);
		Book Create(Book book);
		Book Update(Book book);
		void Delete(int id);
	}
}
