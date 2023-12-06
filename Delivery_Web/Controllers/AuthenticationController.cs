using Delivery_Models.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace Delivery_Web.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        #region Register
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterModel register)
        {
            string responseBody = "";
            Response statusMessage = new Response();

            var jsonString = JsonConvert.SerializeObject(register);
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync("https://localhost:7279/api/account/register", httpContent);

                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode)400:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            statusMessage = JsonConvert.DeserializeObject<Response>(responseBody);
                            return View(statusMessage);
                        }
                    case (System.Net.HttpStatusCode)200:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();

                            var claims = new List<Claim>(){
                                new Claim("token",responseBody)
                            };
                            //if (register.Role=="Admin")
                            //{
                            //    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                            //}
                            //else
                            //{
                            //    claims.Add(new Claim(ClaimTypes.Role, "User"));
                            //}
                            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, register.Role));
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                                IsPersistent = false,
                                AllowRefresh = true
                            });

                            return RedirectToAction("Index", "Dish");
                        }
                    default:
                        {
                            break;
                        }
                }

                return View();
            }
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginCredentials login)
        {
            string responseBody = "";
            Response statusMessage = new Response();

            var jsonString = JsonConvert.SerializeObject(login);
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync("https://localhost:7279/api/account/login", httpContent);

                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode)500:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            statusMessage = JsonConvert.DeserializeObject<Response>(responseBody);
                            return View(statusMessage);
                        }
                    case (System.Net.HttpStatusCode)200:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();

                            //var claims = new List<Claim>(){
                            //    new Claim("token",responseBody)
                            //};

                            //var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme));
                            // Parse the token response
                            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                            //// Extract claims from the token
                            var handler = new JwtSecurityTokenHandler();
                            var token = handler.ReadJwtToken(tokenResponse.Token);
                            var claims = token.Claims.ToList();


                            // Get user role from claims
                            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                            var role = roleClaim?.Value ?? string.Empty;

                            // Create claims list for user authentication
                            claims.Add(new Claim(ClaimTypes.Role, role));
                            var userClaims = new List<Claim>{
                                new Claim("token",responseBody),
                    new Claim(ClaimTypes.Role, role) // Assign the role extracted from the token
                };

                            // Create the ClaimsIdentity and ClaimsPrincipal
                            var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var userPrincipal = new ClaimsPrincipal(identity);



                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                                {
                                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                                    IsPersistent = false,
                                    AllowRefresh = true
                                });
                            return RedirectToAction("Index", "Dish");
                        }
                    default:
                        {
                            break;
                        }
                }

                return View();
            }
        }

        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            using (var client = new HttpClient())
            {
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                HttpResponseMessage response = await client.PostAsync("https://localhost:7279/api/account/logout", null);
                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode)500:
                        {
                            return NotFound();
                        }
                    case (System.Net.HttpStatusCode)200:
                        {
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            return RedirectToAction("Login", "Authentication");
                        }
                    default:
                        {
                            return NotFound();
                        }
                }
            }
        }
        # endregion
    }
}
