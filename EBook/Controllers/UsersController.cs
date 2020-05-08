using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EBook.Data;
using EBook.Models;
using EBook.Services;
using Microsoft.AspNetCore.Http;
using EBook.ViewModels;
using System.Linq.Expressions;

namespace EBook.Controllers
{
   public class UsersController : Controller
   {
		private IUser _manager;
		private ICategory _categoryManager;


		public UsersController(IUser manager, ICategory categoryManager)
      {
			_manager = manager;
			_categoryManager = categoryManager;
      }

      // GET: Users
      public IActionResult Index()
      {
         var loggedInUserId = HttpContext.Session.GetString("LoggedInUserId");

         if(loggedInUserId == null)
            return RedirectToAction("Index", "Home", new { area = "" });

         List<User> users = _manager.GetAllUsers().ToList();
         return View(users);
      }

      // GET: Users/Details/5
      public IActionResult Details(int id)
      {
			User user = _manager.GetById(id);

         if (user == null)
         {
               return NotFound();
         }

         return View(user);
      }
		
      public IActionResult Register()
      {
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name");
			return View();
      }

      // POST: Users/Create
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public IActionResult Register(UserViewModel model)
      {
         if (ModelState.IsValid)
         {
				User exist = _manager.GetByUsername(model.Username);
				if(exist != null)
				{
					ModelState.AddModelError("Username", "Username already taken");
					return View(model);
				}
				User user = new User()
				{
					Firstname = model.Firstname,
					Lastname = model.Lastname,
					Password = model.Password,
					Role = model.Role,
					SubscribedCategorieId = model.SubscribedCategorieId,
					Username = model.Username
				};
				if(model.SubscribedCategorieAll == true)
				{
					user.SubscribedCategorieId = null;
				}
            _manager.Create(user);
            return RedirectToAction("Index", "Home", new { area = "" });
         }
         return View(model);
      }

      // GET: Users/Edit/5
      public IActionResult Edit(int id)
      {
			User loggedInUser = GetLoggedInUser();
			if(loggedInUser == null)
			{
				return StatusCode(401);
			}

			if (loggedInUser.Id != id)
				if (loggedInUser.Role == Role.Subscriber)
					return StatusCode(401);

			User user = _manager.GetById(id);

			if (user == null)
         {
               return NotFound();
         }

			EditUserViewModel model = new EditUserViewModel(user);

			ViewData["Id"] = id;
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", model.SubscribedCategorieId);
			return View(model);
      }

      // POST: Users/Edit/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public IActionResult Edit(int id, EditUserViewModel model)
		{
			User loggedInUser = GetLoggedInUser();

			if (id != model.Id)
				return NotFound();
         
			if (loggedInUser == null)
				return StatusCode(401);

			if (loggedInUser.Id != id)
				if (loggedInUser.Role == Role.Subscriber)
					return StatusCode(401);

			if (ModelState.IsValid)
         {
				User exist = _manager.GetByUsername(model.Username);
				if (exist != null)	
				{
					if(exist.Id != id)
					{
						ModelState.AddModelError("Username", "Username already taken");
						ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", model.SubscribedCategorieId);
						return View(model);
				}
				}
				User user = EditUserViewModel.ViewToUser(model);
					
				_manager.Update(user);

            return RedirectToAction(nameof(Index));
         }
			ViewData["Category"] = new SelectList(_categoryManager.GetAllCategoris(), "Id", "Name", model.SubscribedCategorieId);
			return View(model);
      }

      // GET: Users/Delete/5
      public IActionResult Delete(int id)
      {
			User loggedInUser = GetLoggedInUser();

			if (loggedInUser == null)
				return StatusCode(401);

			if (loggedInUser.Id != id)
				if (loggedInUser.Role == Role.Subscriber)
					return StatusCode(401);

			User user = _manager.GetById(id);

			if (user == null)
         {
               return NotFound();
         }

         return View(user);
      }

      // POST: Users/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public IActionResult DeleteConfirmed(int id)
		{
			User loggedInUser = GetLoggedInUser();

			if (loggedInUser == null)
				return StatusCode(401);

			if (loggedInUser.Id != id)
				if (loggedInUser.Role == Role.Subscriber)
					return StatusCode(401);

			_manager.Delete(id);
         return RedirectToAction(nameof(Index));
      }
		

		[HttpGet]
		public IActionResult Login()
		{
			return View(new LoginViewModel());
		}

		[HttpPost]
		public IActionResult Login(LoginViewModel model)
		{
			User user = _manager.Login(model.Username, model.Password);
			if (user == null)
			{
				ModelState.AddModelError("Username", "Wrong username or password");
				return View(model);
			}
			HttpContext.Session.SetString("LoggedInUserUsername", user.Username);
			HttpContext.Session.SetString("LoggedInUserId", user.Id.ToString());
			HttpContext.Session.SetString("LoggedInUserRole", user.Role.ToString());
			
			return RedirectToAction(nameof(BooksController.Index));
		}

		[HttpGet]
		public IActionResult ChangePassword(int id)
		{
			if (!_manager.Exist(id))
				return NotFound();

			User loggedInUser = GetLoggedInUser();

			if (loggedInUser == null)
				return StatusCode(401);

			if (loggedInUser.Id != id)
				if (loggedInUser.Role == Role.Subscriber)
					return StatusCode(401);

			ViewData["Id"] = id;
			return View();
		}
		[HttpPost]
		public IActionResult ChangePassword(int id, string newPassword)
		{
			User user = _manager.GetById(id);
			User loggedInUser = GetLoggedInUser();

			if (user == null)
				return NotFound();
		
			if (loggedInUser == null)
				return StatusCode(401);

			if (loggedInUser.Id != id)
				if (loggedInUser.Role == Role.Subscriber)
					return StatusCode(401);

			user.Password = newPassword;
			_manager.Update(user);

			return Json("Success");
		}

		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Remove("LoggedInUserUsername");
			HttpContext.Session.Remove("LoggedInUserId");
			HttpContext.Session.Remove("LoggedInUserRole");

         return RedirectToAction("Index", "Home", new { area = "" });
      }

		[HttpGet]
		public User GetLoggedInUser()
		{
			var loggedInUserId = HttpContext.Session.GetString("LoggedInUserId");

			if (loggedInUserId == null)
			{
				return null;
			}
			User user = _manager.GetById(int.Parse(loggedInUserId));

			if (user == null)
			{
				return null;
			}
			return user;
		}
	}
}
