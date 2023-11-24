using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Delivery_Models.Models;
using Newtonsoft.Json;
using Delivery_Models.Models.Enum;
//using System.Web.Mvc;
using Delivery_Models.ViewModels;
using System.Net.Http.Headers;
using Delivery_Models.Models.Dto;

namespace Delivery_Web.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private static readonly HttpClient client = new();
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Info = "注册成功后，直接进入个人中心";
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

            try
            {
                HttpResponseMessage response = await client.PostAsync("https://localhost:7279/api/account/login", httpContent);

                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode)500:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            statusMessage = JsonConvert.DeserializeObject<Response>(responseBody);
                            ViewBag.Info = statusMessage.Message;
                            break;
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
                                IsPersistent = false,  //浏览器关闭后，是否保持登录状态。
                                AllowRefresh = true
                            });
                            return RedirectToAction("Index", "Dish");
                        }
                    default:
                        {
                            ViewBag.Info = "系统异常";
                            break;
                        }
                }
            }
            catch
            {
                ViewBag.Info = "系统异常";
            }
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Info = "注册成功后，直接进入个人中心";
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

            try
            {
                HttpResponseMessage response = await client.PostAsync("https://localhost:7279/api/account/register", httpContent);

                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode)500:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            statusMessage = JsonConvert.DeserializeObject<Response>(responseBody);
                            ViewBag.Info = statusMessage.Message;
                            break;
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
                                IsPersistent = false,  //浏览器关闭后，是否保持登录状态。
                                AllowRefresh = true
                            });
                            return RedirectToAction("Index", "Dish");
                        }
                    default:
                        {
                            ViewBag.Info = "系统异常";
                            break;
                        }
                }
            }
            catch
            {
                ViewBag.Info = "系统异常";
            }
            
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            ViewBag.Info = "欢迎";
            string responseBody = "";

            UserDto showUserProfileModel = new UserDto();

            try
            {
                TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
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
                            showUserProfileModel = JsonConvert.DeserializeObject<UserDto>(responseBody);
                            return View(showUserProfileModel);
                        }
                    default:
                        {
                            return NotFound();
                        }
                }
            }
            catch
            {
                return NotFound();
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

            TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            var jsonString = JsonConvert.SerializeObject(userEditModel);
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                HttpResponseMessage response = await client.PutAsync("https://localhost:7279/api/account/profile", httpContent);

                switch (response.StatusCode)
                {
                    case (System.Net.HttpStatusCode)500:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            statusMessage = JsonConvert.DeserializeObject<Response>(responseBody);
                            ViewBag.Info = statusMessage.Message;
                            break;
                        }
                    case (System.Net.HttpStatusCode)200:
                        {
                            ViewBag.Info = "修改成功";
                            break;
                        }
                    default:
                        {
                            ViewBag.Info = "系统异常";
                            break;
                        }
                }
            }
            catch
            {
                ViewBag.Info = "系统异常";
            }
            UserDto showUserProfileModel = new UserDto();

            showUserProfileModel.Id = userDto.Id;
            showUserProfileModel.FullName = userDto.FullName;
            showUserProfileModel.Email = userDto.Email;
            showUserProfileModel.Address = userDto.Address;
            showUserProfileModel.BirthDate = userDto.BirthDate;
            showUserProfileModel.PhoneNumber = userDto.PhoneNumber;
            showUserProfileModel.Gender = userDto.Gender;

            return View(showUserProfileModel);
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
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
            catch
            {
                return NotFound();
            }
        }
    }
}
