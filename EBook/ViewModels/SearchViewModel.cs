using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.ViewModels
{
	public class SearchViewModel
	{
		public string Title { get; set; }
		public string Filename { get; set; }
		public string Highlights { get; set; }

		public SearchViewModel()
		{
		}

		public SearchViewModel(string title, string filename, string highlights)
		{
			Title = title;
			Filename = filename;
			Highlights = highlights;
		}
	}
}
