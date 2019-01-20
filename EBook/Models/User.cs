using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Models
{
	public class User : Model
	{
		[Required, MaxLength(30)]
		public string Firstname { get; set; }
		[Required, MaxLength(30)]
		public string Lastname { get; set; }
		[Required, MaxLength(10)]
		public string Username { get; set; }
		[Required, MaxLength(10)]
		public string Password { get; set; }
		[Required]
		public Role Role { get; set; }
		public List<Subscribed> SubscribedCategories { get; set; }
	}
}
