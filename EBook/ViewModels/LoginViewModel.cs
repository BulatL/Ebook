using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.ViewModels
{
	public class LoginViewModel
	{
		[Required, MaxLength(10)]
		public string Username { get; set; }
		[Required, MaxLength(10)]
		public string Password { get; set; }
	}
}
