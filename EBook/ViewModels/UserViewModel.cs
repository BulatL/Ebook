using EBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.ViewModels
{
	public class UserViewModel : ViewModel
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
		[Required, Display(Name = "Subcribe to category")]
		public int SubscribedCategorieId { get; set; }

		[Required, Display(Name = "Subcribe to all categories")]
		public bool SubscribedCategorieAll { get; set; }
	}
}
