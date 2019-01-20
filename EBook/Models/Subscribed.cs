using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Models
{
	public class Subscribed
	{
		[ForeignKey("UserId")]
		public User User { get; set; }
		[ForeignKey("CategoryId")]
		public Category Category { get; set; }
		public int UserId { get; set; }
		public int CategoryId { get; set; }
	}
}
