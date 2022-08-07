using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using Dogs.Data.IdentityModels;
using Dogs.Data.DataTransferObjects.Account;

namespace Dogs.Identity.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly SignInManager<ApplicationUser> signInManager;
        readonly IConfiguration configuration;
        readonly ILogger<AccountController> logger;


        public AccountController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           IConfiguration configuration,
           ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.logger = logger;
        }


        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (!loginResult.Succeeded)
                {
                    return BadRequest("Incorrect login or password");
                }

                var user = await userManager.FindByNameAsync(loginModel.Username);

                return Ok(GetToken(user));
            }
            return BadRequest(ModelState);

        }

        [Authorize]
        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await userManager.FindByNameAsync(
                User.Identity.Name ??
                User.Claims.Where(c => c.Properties.ContainsKey("unique_name")).Select(c => c.Value).FirstOrDefault()
                );
            return Ok(GetToken(user));

        }


        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = registerModel.Username,
                        FirstName = registerModel.FirstName,
                        LastName = registerModel.LastName,
                        Email = registerModel.Email
                    };

                    var identityResult = await this.userManager.CreateAsync(user, registerModel.Password);
                    if (identityResult.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        var token = GetToken(user);
                        var url = this.configuration.GetValue<String>("ServerBaseUrl");
                        HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        HttpResponseMessage responseMessage = await client.GetAsync("guides/GetFreeGuideId");
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                            var freeId = JsonConvert.DeserializeObject<int>(responseData);
                            user.KgtId = freeId;
                            var updatedIdentityResult = await this.userManager.UpdateAsync(user);
                            if (updatedIdentityResult.Succeeded)
                                return Ok(GetToken(user));
                        }
                    }
                    else
                    {
                        return BadRequest(identityResult.Errors);
                    }
                }
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        private String GetToken(ApplicationUser user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new Claim[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString()),
                        new Claim("KgtId", user.KgtId.ToString()),
                        new Claim("IdentityId", user.Id)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.GetValue<String>("Tokens:Key")));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddSeconds(this.configuration.GetValue<int>("Tokens:Lifetime")),
                audience: this.configuration.GetValue<String>("Tokens:Audience"),
                issuer: this.configuration.GetValue<String>("Tokens:Issuer")
                );

            var handler = new JwtSecurityTokenHandler();
            var tkn = handler.WriteToken(jwt);
            SecurityToken validatedToken;

            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = signingKey,
                ValidateAudience = true,
                ValidAudience = this.configuration.GetValue<String>("Tokens:Audience"),
                ValidateIssuer = true,
                ValidIssuer = this.configuration.GetValue<String>("Tokens:Issuer"),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };

            new JwtSecurityTokenHandler().ValidateToken(tkn, validationParameters, out validatedToken);

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }

        [HttpPost]
        [Route("sendresetpasswordemail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] string userNameOrEmail)
        {
            try
            {
                var user = await userManager.FindByNameAsync(userNameOrEmail);

                if (user == null)
                {
                    user = await userManager.FindByEmailAsync(userNameOrEmail);
                }

                if (user == null)
                {
                    return NotFound();
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);


                var clientBaseUrl = this.configuration.GetValue<string>("ClientBaseUrl");

                var resetLink = clientBaseUrl + "Account/ResetPassword?token=" + token;


                SmtpClient client = new SmtpClient(
                    this.configuration.GetValue<String>("SendEmails:Host"),
                    this.configuration.GetValue<int>("SendEmails:Port"));
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(
                    this.configuration.GetValue<String>("SendEmails:MailAddressConfiguration"),
                    this.configuration.GetValue<String>("SendEmails:Password"));
                client.EnableSsl = true;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(this.configuration.GetValue<String>("SendEmails:MailAddressVisible"));
                mailMessage.To.Add(user.Email);
                mailMessage.Body = "Oto Twój link do zresetowania hasła. Kliknij w niego " +
                    "lub skopiuj go do paska adresu w przeglądarce, aby ustawić nowe hasło. \n" + resetLink;
                mailMessage.Subject = "Baza KGT - zmiana hasła";
                client.Send(mailMessage);

                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserNameOrEmail);

            if (user == null)
            {
                user = await userManager.FindByEmailAsync(model.UserNameOrEmail);
            }

            if (user == null)
            {
                return NotFound();
            }

            //because token consists of + signs which are turned into spaces in URL query string, so it has to be reversed
            var token = model.Token.Replace(" ", "+");

            IdentityResult result = userManager.ResetPasswordAsync
              (user, token, model.Password).Result;
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }
    }
}