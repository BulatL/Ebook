using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.ViewModels
{
	public class CategoryViewModel : ViewModel
	{
		[Required, MaxLength(30)]
		public string Name { get; set; }
	}
}
