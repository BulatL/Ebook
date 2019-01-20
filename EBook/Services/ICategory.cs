using EBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Services
{
	public interface ICategory
	{
		IEnumerable<Category> GetAllCategoris();
		Category GetById(int id);
		Category Create(Category category);
		Category Update(Category category);
		void Delete(int id);
	}
}
