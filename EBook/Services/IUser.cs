using EBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBook.Services
{
	public interface IUser
	{
		IEnumerable<User> GetAllUsers();
		User Login(string username, string password);
		User GetById(int id);
		User GetByUsername(string username);
		bool Exist(int id);
		User Create(User user);
		User Update(User user);
		void Delete(int id);
	}
}
