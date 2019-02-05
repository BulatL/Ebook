using EBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Data
{
	public class EbookDbContext : DbContext
	{
		public EbookDbContext(DbContextOptions options)
				: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Book>().HasOne(b => b.Category);
			modelBuilder.Entity<Book>().HasOne(b => b.Language);
			modelBuilder.Entity<User>().HasOne(u => u.SubscribedCategorie);
			DbSeed.SeedData(modelBuilder);
			foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<User> User { get; set; }
		public DbSet<Book> Book { get; set; }
		public DbSet<Category> Category { get; set; }
		public DbSet<Language> Language { get; set; }
	}
}
