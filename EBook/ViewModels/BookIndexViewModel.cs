using EBook.Models;
using EBook.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.viewModel
{
	public class BookIndexViewModel : ViewModel
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public string Keywords { get; set; }
		[Display(Name = "Publication year")]
		public DateTime PublicationYear { get; set; }
		[Display(Name = "File name")]
		public string FileName { get; set; }
		public string MIME { get; set; }
		public Category Category { get; set; }
		public Language Language { get; set; }
		public bool Download { get; set; }

		public BookIndexViewModel()
		{
		}

		public BookIndexViewModel(Book book)
		{
			this.Id = book.Id;
			this.Title = book.Title;
			this.Author = book.Author;
			this.Keywords = book.Keywords;
			this.PublicationYear = book.PublicationYear;
			this.FileName = book.FileName;
			this.MIME = book.MIME;
			this.Category = book.Category;
			this.Language = book.Language;
		}

		public static List<BookIndexViewModel> CopyList(List<Book> books, string downloadCategory)
		{
			List<BookIndexViewModel> viewModels = new List<BookIndexViewModel>();
			foreach (Book book in books)
			{
				BookIndexViewModel viewModel = new BookIndexViewModel(book);

				if (downloadCategory.Equals("none"))
					viewModel.Download = false;
				else if (downloadCategory.Equals("all"))
					viewModel.Download = true;
				else
				{
					int categoryId = int.Parse(downloadCategory);
					if(book.CategoryId == categoryId)
						viewModel.Download = true;
				}

				viewModels.Add(viewModel);
			}
			return viewModels;
		}
	}
}
