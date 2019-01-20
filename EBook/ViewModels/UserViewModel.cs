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
		public RoleViewModel Role { get; set; }
	}
}
