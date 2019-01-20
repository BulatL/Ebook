﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Models
{
	public class Language : Model
	{
		[Required, MaxLength(30)]
		public string Name { get; set; }
	}
}
