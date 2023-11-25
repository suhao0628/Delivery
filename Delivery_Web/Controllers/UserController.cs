using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Delivery_Models.Models;
using Newtonsoft.Json;
//using System.Web.Mvc;
using System.Net.Http.Headers;
using Delivery_Models.Models.Dto;
using System.Net;

namespace Delivery_Web.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
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

                            var claims = new List<Claim>(){
                                new Claim("token",responseBody)
                            };

                            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
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
                    case (System.Net.HttpStatusCode)500:
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
                            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
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

        #region Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string responseBody = "";

            UserDto userProfile = new UserDto();

            using (var client = new HttpClient())
            {
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                HttpResponseMessage response = await client.GetAsync("https://localhost:7279/api/account/profile");
                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode)500:
                        {
                            return NotFound();
                        }
                    case (System.Net.HttpStatusCode)200:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            userProfile = JsonConvert.DeserializeObject<UserDto>(responseBody);
                            return View(userProfile);
                        }
                    default:
                        {
                            return NotFound();
                        }
                }


            }
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserDto userDto)
        {
            string responseBody = "";
            Response statusMessage = new Response();

            //序列化
            UserEditModel userEditModel = new UserEditModel();
            userEditModel.FullName = userDto.FullName;
            //userEditModel.Email = email;
            userEditModel.Address = userDto.Address;
            userEditModel.BirthDate = userDto.BirthDate;
            userEditModel.PhoneNumber = userDto.PhoneNumber;
            userEditModel.Gender = userDto.Gender;
            using (var client = new HttpClient())
            {
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var jsonString = JsonConvert.SerializeObject(userEditModel);
                HttpContent httpContent = new StringContent(jsonString);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                HttpResponseMessage response = await client.PutAsync("https://localhost:7279/api/account/profile", httpContent);

                //switch (response.StatusCode)
                //{
                //    case (System.Net.HttpStatusCode)500:
                //        {
                //            responseBody = await response.Content.ReadAsStringAsync();
                //            statusMessage = JsonConvert.DeserializeObject<Response>(responseBody);
                //            break;
                //        }
                //    case (System.Net.HttpStatusCode)200:
                //        {
                //            break;
                //        }
                //    default:
                //        {
                //            break;
                //        }
                //}
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["success"] = "Profile Updated Successfully";
                    
                    return View();
                }
                else
                {
                    TempData["error"] = "Error Occurs...";
                }
                
                UserDto userProfile = new UserDto();

                userProfile.Id = userDto.Id;
                userProfile.FullName = userDto.FullName;
                userProfile.Email = userDto.Email;
                userProfile.Address = userDto.Address;
                userProfile.BirthDate = userDto.BirthDate;
                userProfile.PhoneNumber = userDto.PhoneNumber;
                userProfile.Gender = userDto.Gender;

                return View(userProfile);
            }
        }

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
                            return RedirectToAction("Login", "User");
                        }
                    default:
                        {
                            return NotFound();
                        }
                }
            }
        }
        #endregion
    }
}
