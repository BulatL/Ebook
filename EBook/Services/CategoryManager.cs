using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EBook.Data;
using EBook.Models;
using Microsoft.EntityFrameworkCore;

namespace EBook.Services
{
	public class CategoryManager : ICategory
	{
		private EbookDbContext _context;

		public CategoryManager(EbookDbContext context)
		{
			_context = context;
		}
		public Category Create(Category category)
		{
			if (category == null)
				return null;

			_context.Category.Add(category);
			_context.SaveChanges();
			return category;
		}

		public void Delete(int id)
		{
			if (id == 0)
				return;

			Category category = new Category { Id = id };
			_context.Entry(category).State = EntityState.Deleted;
			_context.SaveChanges();
			
		}

		public IEnumerable<Category> GetAllCategoris()
		{
			return _context.Category.OrderBy(c => c.Name);
		}

		public Category GetById(int id)
		{
			return _context.Category.SingleOrDefault(c => c.Id == id);
		}

		public Category Update(Category category)
		{
			var local = _context.Set<Category>()
			 .Local
			 .FirstOrDefault(entry => entry.Id.Equals(category.Id));

			// check if local is not null 
			if (local != null) // I'm using a extension method
			{
				// detach
				_context.Entry(local).State = EntityState.Detached;
			}
			// set Modified flag in your entry
			_context.Entry(category).State = EntityState.Modified;

			// save 
			_context.SaveChanges();
			return category;
		}
	}
}
