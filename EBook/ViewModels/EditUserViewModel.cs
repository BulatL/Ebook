using EBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.ViewModels
{
	public class EditUserViewModel : ViewModel
	{
		[Required, MaxLength(30)]
		public string Firstname { get; set; }
		[Required, MaxLength(30)]
		public string Lastname { get; set; }
		[Required, MaxLength(10)]
		public string Username { get; set; }
		[Required]
		public Role Role { get; set; }
		[Display(Name = "Subcribe to category")]
		public int SubscribedCategorieId { get; set; }

		[Display(Name = "Subcribe to all categories")]
		public bool SubscribedCategorieAll { get; set; }

		public string Password { get; set; }

		public EditUserViewModel()
		{
		}

		public static User ViewToUser(EditUserViewModel viewModel)
		{
			User user = new User();

			user.Id = viewModel.Id;
			user.Firstname = viewModel.Firstname;
			user.Lastname = viewModel.Lastname;
			user.Role = viewModel.Role;
			user.Username = viewModel.Username;
			user.Password = viewModel.Password;

			if (viewModel.SubscribedCategorieAll == true)
				user.SubscribedCategorieId = null;
			else
				user.SubscribedCategorieId = viewModel.SubscribedCategorieId;

			return user;
		}

		public EditUserViewModel(User user)
		{
			this.Id = user.Id;
			this.Firstname = user.Firstname;
			this.Lastname = user.Lastname;
			this.Role = user.Role;
			this.Username = user.Username;
			this.Password = user.Password;

			if (user.SubscribedCategorieId == null)
				this.SubscribedCategorieAll = true;
			else
				this.SubscribedCategorieId = int.Parse(user.SubscribedCategorieId.ToString());
			
		}
	}
}
