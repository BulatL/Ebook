using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.ViewModels
{
	public class BookViewModel : ViewModel
	{
		[Required, MaxLength(80)]
		public string Title { get; set; }
		[Required, MaxLength(120)]
		public string Author { get; set; }
		[Required, MaxLength(120)]
		public string Keywords { get; set; }
		[Required]
		public DateTime PublicationYear { get; set; }
		[Required]
		public string FileName { get; set; }
		[Required]
		public IFormFile BookFile { get; set; }
		[Required]
		public string MIME { get; set; }
		[Required]
		public int CategoryId { get; set; }
		[Required]
		public int LanguageId { get; set; }
		
	}
}
