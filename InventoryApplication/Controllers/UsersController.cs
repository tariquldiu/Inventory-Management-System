using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryApplication.Models;
using System.Net;
using System.Web.Security;
using System.Data.Entity;

namespace InventoryApplication.Controllers
{
    public class UsersController : Controller
    {
        InventoryBDEntities db = new InventoryBDEntities();
        // GET: Users
       
        public ActionResult Login()
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName");
            return View();
        }
        [HttpPost]
        public ActionResult Login(User u)
        {
            string pass = FormsAuthentication.HashPasswordForStoringInConfigFile(u.Password, "SHA1");
            if (ModelState.IsValid)
            {
                u.Password = pass;
                var user = db.Users.Where(x => x.UserName == u.UserName && x.Password == u.Password&&x.IsActive=="yes").Count();
                if (user != 0)
                {
                    FormsAuthentication.SetAuthCookie(u.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
       
        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName");
            return View();
        }
        [HttpPost]
        public ActionResult Create(User u)
        {
            string pass = FormsAuthentication.HashPasswordForStoringInConfigFile(u.Password, "SHA1");
            if (ModelState.IsValid)
            {
                u.Password = pass;
                db.Users.Add(u);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.ToList();
            ViewBag.us = user.Count();
            return View(user);
        }
        [Authorize]
        public ActionResult Edit(int? id)
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "EmployeeName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,Password,RoleName,EmployeeId,IsActive,Employee")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        

    }
}