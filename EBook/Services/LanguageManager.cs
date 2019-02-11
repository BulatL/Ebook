using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EBook.Data;
using EBook.Models;
using Microsoft.EntityFrameworkCore;

namespace EBook.Services
{
	public class LanguageManager : ILanguage
	{
		private EbookDbContext _context;

		public LanguageManager(EbookDbContext context)
		{
			_context = context;
		}
		public Language Create(Language language)
		{
			if (language == null)
				return null;

			_context.Language.Add(language);
			_context.SaveChanges();
			return language;
		}

		public void Delete(int id)
		{
			if (id == 0)
				return;

			Language language = new Language { Id = id };
			_context.Entry(language).State = EntityState.Deleted;
			_context.SaveChanges();
		}

		public IEnumerable<Language> GetAllLanguages()
		{
			return _context.Language.OrderBy(c => c.Name);
		}

		public Language GetById(int id)
		{
			return _context.Language.SingleOrDefault(c => c.Id == id);
		}

		public Language Update(Language language)
		{
			var local = _context.Set<Language>()
			 .Local
			 .FirstOrDefault(entry => entry.Id.Equals(language.Id));

			// check if local is not null 
			if (local != null) // I'm using a extension method
			{
				// detach
				_context.Entry(local).State = EntityState.Detached;
			}
			// set Modified flag in your entry
			_context.Entry(language).State = EntityState.Modified;

			// save 
			_context.SaveChanges();
			return language;
		}
	}
}
