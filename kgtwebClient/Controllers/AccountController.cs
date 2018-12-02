using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kgtwebClient.Models.Account;

namespace kgtwebClient.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // call to identity api to token endpoint
            // if token isnt empty, add token to the cookie, set session?,  and redirect somewhere
            System.Web.HttpContext.Current.Session["firstName"] = "a"; //some magic with getting data from token
            return Redirect(returnUrl);
            //else return some error
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (Request.IsAuthenticated)
                RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                //http request to identity api / account / register with register model data
                //token comes back, if its not empty then add it to the cookie 
                //create User model with Id taken from token and other data (without password) taken from register model
                //send user model to kgt server api / account / register
                //if success redirect to the page of new user
                //email confirmation??


                //var user = new User
                //{
                //    RegistrationDate = DateTime.Now,
                //    UserName = model.Username,
                //    Email = model.Email,
                //    FirstName = model.FirstName,
                //    LastName = model.LastName,
                //    ProfileImage = random.GetRandomProfileImage()
                //};
                return Redirect("profile of newly created user");
            }
            // If we got this far, something failed, redisplay form
            else return View(model);
        }
    }
}