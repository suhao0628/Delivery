using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Delivery_Models.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Delivery_Models.Models.Dto;
using System.Net;

namespace Delivery_Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
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

            UserEditModel userEditModel = new UserEditModel();
            userEditModel.FullName = userDto.FullName;
            //userEditModel.Email = userDto.Email;
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["success"] = "Profile Updated Successfully";
                    UserDto userProfile = new UserDto
                    {
                        Id = userDto.Id,
                        FullName = userDto.FullName,
                        Address = userDto.Address,
                        BirthDate = userDto.BirthDate,
                        PhoneNumber = userDto.PhoneNumber,
                        Gender = userDto.Gender
                    };

                    return View(userProfile);
                }
                else
                {
                    TempData["error"] = "Error Occurs...";
                    return View(response);
                }
                
               
            }
        }
        #endregion
    }
}
