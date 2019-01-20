using EBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Services
{
	public interface ILanguage
	{
		IEnumerable<Language> GetAllLanguages();
		Language GetById(int id);
		Language Create(Language language);
		Language Update(Language language);
		void Delete(int id);
	}
}
