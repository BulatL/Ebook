﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EBook.Data;
using EBook.Models;
using Microsoft.EntityFrameworkCore;

namespace EBook.Services
{
	public class UserManager : IUser
	{
		private EbookDbContext _context;

		public UserManager(EbookDbContext context)
		{
			_context = context;
		}

		public User Create(User user)
		{
			if (user == null)
				return null;

			_context.User.Add(user);
			_context.SaveChanges();
			return user;
		}

		public void Delete(int id)
		{
			if (id == 0)
				return;

			User user = new User { Id = id };
			_context.Entry(user).State = EntityState.Deleted;
			_context.SaveChanges();
		}

		public IEnumerable<User> GetAllUsers()
		{
			return _context.User.OrderBy(c => c.Username).Include(c => c.SubscribedCategorie);
		}

		public User GetById(int id)
		{
			return _context.User.Include(c => c.SubscribedCategorie).SingleOrDefault(u => u.Id == id);
		}

		public User GetByUsername(string username)
		{
			return _context.User.Include(c => c.SubscribedCategorie).SingleOrDefault(u => u.Username == username);
		}

		public bool Exist(int id)
		{
			return _context.User.Any(u => u.Id == id);
		}

		public User Login(string username, string password)
		{
			return _context.User.SingleOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));
		}

		public User Update(User user)
		{
			var local = _context.Set<User>()
			 .Local
			 .FirstOrDefault(entry => entry.Id.Equals(user.Id));

			// check if local is not null 
			if (local != null) // I'm using a extension method
			{
				// detach
				_context.Entry(local).State = EntityState.Detached;
			}
			// set Modified flag in your entry
			_context.Entry(user).State = EntityState.Modified;

			// save 
			_context.SaveChanges();
			return user;
		}

		public void SetDefaultCategory(int categoryId)
		{
			List<User> users = _context.User.Where(u => u.SubscribedCategorieId == categoryId).ToList();
			foreach (User user in users)
			{
				if(user.SubscribedCategorieId == categoryId)
				{
					user.SubscribedCategorieId = 1;
					Update(user);
				}
			}
		}
	}
}
