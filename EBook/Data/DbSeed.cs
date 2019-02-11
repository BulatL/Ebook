using EBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Data
{
	public class DbSeed
	{
		public static void SeedData(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Default" },
				new Category { Id = 2, Name = "Action and Adventure"},
				new Category { Id = 3, Name = "Art" },
				new Category { Id = 4, Name = "Biography" },
				new Category { Id = 5, Name = "Comic book" },
				new Category { Id = 6, Name = "Crime" },
				new Category { Id = 7, Name = "Drama" },
				new Category { Id = 8, Name = "Fantasy" },
				new Category { Id = 9, Name = "History" },
				new Category { Id = 10, Name = "Horror" },
				new Category { Id = 11, Name = "Mystery" },
				new Category { Id = 12, Name = "Review" },
				new Category { Id = 13, Name = "Political thriller" },
				new Category { Id = 14, Name = "Science" },
				new Category { Id = 15, Name = "Romance" },
				new Category { Id = 16, Name = "Science fiction" },
				new Category { Id = 17, Name = "True crime" },
				new Category { Id = 18, Name = "Short story	" },
				new Category { Id = 19, Name = "Thriller	" }
				);
			
			modelBuilder.Entity<User>().HasData(
				new User { Id = 1, Firstname = "Marko", Lastname = "Markovic", Username = "marko", Password = "123", Role = Role.Admin},
				new User { Id = 2, Firstname = "Nikola", Lastname = "Nikolic", Username = "nikola", Password = "123", Role = Role.Admin, SubscribedCategorieId = 1 },
				new User { Id = 3, Firstname = "Ivan", Lastname = "Ivanovic", Username = "ivan", Password = "123", Role = Role.Subscriber, SubscribedCategorieId = 2 }
				);

			modelBuilder.Entity<Language>().HasData(
				new Language { Id = 1, Name = "Default" },
				new Language { Id = 2, Name = "Spanish" },
				new Language { Id = 3, Name = "English" },
				new Language { Id = 4, Name = "Russian" },
				new Language { Id = 5, Name = "Japanese" },
				new Language { Id = 6, Name = "German" },
				new Language { Id = 7, Name = "French" },
				new Language { Id = 8, Name = "Turkish" },
				new Language { Id = 9, Name = "Italian" },
				new Language { Id = 10, Name = "Polish" },
				new Language { Id = 11, Name = "Ukrainian" },
				new Language { Id = 12, Name = "Serbian" },
				new Language { Id = 13, Name = "Hungarian" },
				new Language { Id = 14, Name = "Greek" },
				new Language { Id = 15, Name = "Czech" }
				);
		}
	}
}
