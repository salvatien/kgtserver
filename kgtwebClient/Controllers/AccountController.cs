using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Dogs.ViewModels.Data.Models.Account;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static kgtwebClient.Helpers.LoginHelper;

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
        //[ValidateAntiForgeryToken]
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



                //string url = "https://localhost:44350/api/";
                //HttpClient client = new HttpClient { BaseAddress = new Uri(url) };
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "account/register");


                //var dogSerialized = JsonConvert.SerializeObject(model);
                //message.Content = new StringContent(dogSerialized, System.Text.Encoding.UTF8, "application/json");
                //HttpResponseMessage responseMessage = client.SendAsync(message).Result;
                //if (responseMessage.IsSuccessStatusCode)    //200 OK
                //{
                //    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                //    var stream = responseData;


                if(true)
                {
                    var stream = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJmMGU3ZDAyYy1jNzA3LTRkOGMtOGNmNS1lMTY1NTAzZDBiNjQiLCJ1bmlxdWVfbmFtZSI6InRlc3RlcjEwIiwianRpIjoiZmMxYTg0MDgtMjFhYS00ZWUzLTkyMzAtMGViYjJhYWQ3YmE2IiwiaWF0IjoiMjAxOC0xMi0wMyAyMDo1MDoyNyIsIktndElkIjoiMiIsIm5iZiI6MTU0Mzg3MDIyNywiZXhwIjoxNTQzOTU2NjI3LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjQwMDAifQ.rMKzHgm_sCvaIHbK-oeivdUORMcSIAbSNQOJ6l8AiNY";
                    SecurityToken validatedToken;
                    TokenValidationParameters validationParameters = new TokenValidationParameters();
                    validationParameters.IssuerSigningKey = 
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1516CBA666B4F0EFD8C487A132DA2E64A4838D2DB04AD9A0DC4436262411E146"));

                    new JwtSecurityTokenHandler().ValidateToken("stream", validationParameters, out validatedToken);

                    var token = new JwtSecurityToken(jwtEncodedString: stream);
                    Console.WriteLine("email => " + token.Claims.First(c => c.Type == "email").Value);
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
                    var jti = tokenS.Claims.First(claim => claim.Type == "jti").Value;
                    var ticket = new FormsAuthenticationTicket(1, model.Email, DateTime.Now, tokenS.ValidTo, true, tokenS.RawPayload);
                    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);

                    //var user = new User
                    //{
                    //    RegistrationDate = DateTime.Now,
                    //    UserName = model.Username,
                    //    Email = model.Email,
                    //    FirstName = model.FirstName,
                    //    LastName = model.LastName,
                    //    ProfileImage = random.GetRandomProfileImage()
                    //};
                }
                return Redirect("profile of newly created user");
            }
            // If we got this far, something failed, redisplay form
            else return View(model);
        }
    }
}