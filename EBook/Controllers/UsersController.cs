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

namespace EBook.Controllers
{
   public class UsersController : Controller
   {
		private IUser _manager;

      public UsersController(IUser manager)
      {
			_manager = manager;
      }

      // GET: Users
      public IActionResult Index()
      {
         return View(_manager.GetAllUsers());
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
         return View();
      }

      // POST: Users/Create
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public IActionResult Register(User user)
      {
         if (ModelState.IsValid)
         {
				User exist = _manager.GetByUsername(user.Username);
				if(exist != null)
				{
					ModelState.AddModelError("Username", "Username already taken");
					return View(user);
				}
            _manager.Create(user);
            return RedirectToAction(nameof(Index));
         }
         return View(user);
      }

      // GET: Users/Edit/5
      public IActionResult Edit(int id)
      {
			User loggedInUser = GetLoggedInUser();
			if(loggedInUser == null)
			{
				return StatusCode(401);
			}

			if(loggedInUser.Id != id)
			{
				if(loggedInUser.Role == Role.Subscriber)
					return StatusCode(401);
			}
			User user = _manager.GetById(id);

			if (user == null)
         {
               return NotFound();
         }
         return View(user);
      }

      // POST: Users/Edit/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
      // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public IActionResult Edit(int id, User user)
		{
			User loggedInUser = GetLoggedInUser();

			if (id != user.Id)
				return NotFound();
         
			if (loggedInUser == null)
				return StatusCode(401);
			
			if (loggedInUser.Role == Role.Subscriber || loggedInUser.Id != id)
				return StatusCode(401);
			
			if (ModelState.IsValid)
         {
            try
            {
					User exist = _manager.GetByUsername(user.Username);
					if (exist != null)
					{
						if(exist.Id == id)
						{
							ModelState.AddModelError("Username", "Username already taken");
							return View(user);
					}
					}
				_manager.Update(user);
            }
            catch (DbUpdateConcurrencyException)
            {
               if (!_manager.Exist(user.Id))
               {
                  return NotFound();
               }
               else
               {
                  throw;
               }
            }
            return RedirectToAction(nameof(Index));
         }
         return View(user);
      }

      // GET: Users/Delete/5
      public IActionResult Delete(int id)
      {
			User loggedInUser = GetLoggedInUser();

			if (loggedInUser == null)
				return StatusCode(401);

			if (loggedInUser.Role == Role.Subscriber || loggedInUser.Id != id)
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

			if (loggedInUser.Role == Role.Subscriber || loggedInUser.Id != id)
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
			
			return RedirectToAction(nameof(HomeController.Index));
		}

		[HttpGet]
		public IActionResult ChangePassword(int id)
		{
			if (!_manager.Exist(id))
				return NotFound();

			User loggedInUser = GetLoggedInUser();

			if (loggedInUser == null)
				return StatusCode(401);

			if (loggedInUser.Role == Role.Subscriber || loggedInUser.Id != id)
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

			if (loggedInUser.Role == Role.Subscriber || loggedInUser.Id != id)
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

			return RedirectToAction(nameof(HomeController.Index));
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
