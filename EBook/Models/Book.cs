using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Models
{
	public class Book : Model
	{
		[Required, MaxLength(80)]
		public string  Title { get; set; }
		[Required, MaxLength(120)]
		public string Author { get; set; }
		[Required, MaxLength(120)]
		public string Keywords { get; set; }
		[Required]
		public DateTime PublicationYear { get; set; }
		[Required, MaxLength(200)]
		public string FileName { get; set; }
		[Required, MaxLength(100)]
		public string MIME { get; set; }
		[ForeignKey("CategoryId")]
		[NotMapped]
		public Category Category { get; set; }
		public int CategoryId { get; set; }
		[ForeignKey("LanguageId")]
		[NotMapped]
		public Language Language { get; set; }
		public int LanguageId { get; set; }
		public string Body { get; set; }
	}
}
