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
			_context.Attach(category).State = EntityState.Modified;
			_context.SaveChanges();
			return category;
		}
	}
}
