using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Dogs.ViewModels.Data.Models;
using Dogs.ViewModels.Data.Models.Account;
using kgtwebClient.Helpers;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static kgtwebClient.Helpers.LoginHelper;

namespace kgtwebClient.Controllers
{
    public class AccountController : Controller
    {
        private static readonly HttpClient serverHttpClient = new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["ServerBaseUrl"]) };
        private static readonly HttpClient identityApiHttpClient = new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["IdentityApiBaseUrl"]) };


        // GET: Account
        [HttpGet]
        public ActionResult Index()
        {
            if(!LoginHelper.IsAuthenticated())
                return RedirectToAction("Login", "Account", new { returnUrl = this.Request.Url.AbsoluteUri });
            return RedirectToAction("Guide", "Guides", new { id = 
                System.Web.HttpContext.Current.Session["CurrentUserId"]});
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (LoginHelper.IsAuthenticated())
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string userNameOrEmail)
        {
            if (LoginHelper.IsAuthenticated())
                return RedirectToAction("Index", "Home");
            identityApiHttpClient.DefaultRequestHeaders.Accept.Clear();
            identityApiHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage identityApiMessage = 
                new HttpRequestMessage(HttpMethod.Post, identityApiHttpClient.BaseAddress + "account/sendresetpasswordemail");

            var modelSerialized = JsonConvert.SerializeObject(userNameOrEmail);
            identityApiMessage.Content = new StringContent(modelSerialized, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage identityApiResponseMessage = identityApiHttpClient.SendAsync(identityApiMessage).Result;
            if (identityApiResponseMessage.IsSuccessStatusCode)    //200 OK
            {
                return View("ResetPasswordEmailSent");
            }
            else
            {
                ViewBag.Message = "Kod błędu: " + identityApiResponseMessage.StatusCode;
                return View("Error");
            }
        }

        public ActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (LoginHelper.IsAuthenticated())
                return RedirectToAction("Index", "Home");
            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.Message = "Hasła nie są takie same";
                return View("Error");
            }
            if (model.Password.Length < 6 || !model.Password.Any(char.IsDigit))
            {
                ViewBag.Message = "Hasło musi mieć minimum 6 znaków, w tym 1 cyfrę.";
                return View("Error");
            }
            identityApiHttpClient.DefaultRequestHeaders.Accept.Clear();
            identityApiHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage identityApiMessage =
                new HttpRequestMessage(HttpMethod.Post, identityApiHttpClient.BaseAddress + "account/resetpassword");

            var modelSerialized = JsonConvert.SerializeObject(model);
            identityApiMessage.Content = new StringContent(modelSerialized, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage identityApiResponseMessage = identityApiHttpClient.SendAsync(identityApiMessage).Result;
            if (identityApiResponseMessage.IsSuccessStatusCode)    //200 OK
            {
                return View("ResetPasswordSuccessful");
            }
            else
            {
                ViewBag.Message = "Kod błędu: " + identityApiResponseMessage.StatusCode;
                return View("Error");
            }
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (LoginHelper.IsAuthenticated())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //if user came from error page or reset password page, we shouldnt redirect back to those pages
            if (returnUrl.Contains("Error") || returnUrl.Contains("ResetPassword"))
                returnUrl = null;

            identityApiHttpClient.DefaultRequestHeaders.Accept.Clear();
            identityApiHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage identityApiMessage = new HttpRequestMessage(HttpMethod.Post, identityApiHttpClient.BaseAddress + "account/token");

            var modelSerialized = JsonConvert.SerializeObject(model);
            identityApiMessage.Content = new StringContent(modelSerialized, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage identityApiResponseMessage = identityApiHttpClient.SendAsync(identityApiMessage).Result;
            if (identityApiResponseMessage.IsSuccessStatusCode)    //200 OK
            {
                var responseIdentityApiData = identityApiResponseMessage.Content.ReadAsStringAsync().Result;
                var token = responseIdentityApiData;
                token = token.Replace("\"", "");


                SecurityToken validatedToken;
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Tokens:Key"]));
                var validationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = signingKey,
                    ValidateAudience = true,
                    ValidAudience = ConfigurationManager.AppSettings["Tokens:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = ConfigurationManager.AppSettings["Tokens:Issuer"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };

                var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out validatedToken);
                var claims = principal.Claims;
                var userId = Int32.Parse(principal.Claims.Where(c => c.Type == "KgtId").Select(c => c.Value).FirstOrDefault());
                serverHttpClient.DefaultRequestHeaders.Accept.Clear();
                serverHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseMessage = serverHttpClient.GetAsync("guides/" + userId).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                    var guide = JsonConvert.DeserializeObject<GuideModel>(responseData);
                    System.Web.HttpContext.Current.Session.Timeout = 30;
                    System.Web.HttpContext.Current.Session["CurrentUserId"] = userId;
                    System.Web.HttpContext.Current.Session["token"] = token;
                    System.Web.HttpContext.Current.Session["isMember"] = guide.IsMember;
                    System.Web.HttpContext.Current.Session["isAdmin"] = guide.IsAdmin;
                }
            }
            else if (identityApiResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return RedirectToAction("Error", "Home", new { error = "Błędny login lub hasło (lub inny błąd ale raczej to :D)." });
            }
            else
            {
                return RedirectToAction("Error", "Home", new { error = "Błędne żądanie." });
            }
            if(String.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Home");
            return Redirect(returnUrl);
            //else return some error
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if(LoginHelper.IsAuthenticated())
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (LoginHelper.IsAuthenticated())
                RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {

                //email confirmation??
                identityApiHttpClient.DefaultRequestHeaders.Accept.Clear();
                identityApiHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpRequestMessage identityApiMessage = new HttpRequestMessage(HttpMethod.Post, identityApiHttpClient.BaseAddress + "account/register");

                var modelSerialized = JsonConvert.SerializeObject(model);
                identityApiMessage.Content = new StringContent(modelSerialized, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage identityApiResponseMessage = identityApiHttpClient.SendAsync(identityApiMessage).Result;
                if (identityApiResponseMessage.IsSuccessStatusCode)    //200 OK
                {
                    var responseIdentityApiData = identityApiResponseMessage.Content.ReadAsStringAsync().Result;
                    var token = responseIdentityApiData;
                    token = token.Replace("\"", "");

                    serverHttpClient.DefaultRequestHeaders.Accept.Clear();
                    serverHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    serverHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpRequestMessage serverMessage = new HttpRequestMessage(HttpMethod.Post, serverHttpClient.BaseAddress + "guides/register");
                    serverMessage.Content = new StringContent(modelSerialized, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage responseServerMessage = serverHttpClient.SendAsync(serverMessage).Result;
                    if (responseServerMessage.IsSuccessStatusCode)    //200 OK
                    {
                        var responseServerData = responseServerMessage.Content.ReadAsStringAsync().Result;
                        System.Web.HttpContext.Current.Session.Timeout = 30;
                        System.Web.HttpContext.Current.Session["CurrentUserId"] = Int32.Parse(responseServerData);
                        System.Web.HttpContext.Current.Session["token"] = token;
                        System.Web.HttpContext.Current.Session["isMember"] = false;
                        System.Web.HttpContext.Current.Session["isAdmin"] = false;


                    }
                }
                else if (identityApiResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return RedirectToAction("Error", "Home", new { error = "Błędne dane - takie konto może już istnieć lub pola formularza zostały nieprawidłowo wypełnione." });
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { error = "Błędne żądanie." });
                }
                return RedirectToAction("UpdateGuide", "Guides", new { id = System.Web.HttpContext.Current.Session["CurrentUserId"] });
            }
            // If we got this far, something failed, redisplay form
            else return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }

}