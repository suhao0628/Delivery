using Delivery_Models.Models.Dto;
using Delivery_Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Delivery_Models.Models.Entity;
using Delivery_API.Services;

namespace Delivery_Web.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        #region Create Dish
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(DishDto dishDto)
        {
            string responseBody = "";
            using (var client = new HttpClient())
            {
                TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
                client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var jsonString = JsonConvert.SerializeObject(dishDto);
                HttpContent httpContent = new StringContent(jsonString);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync("https://localhost:7279/api/admin/create", httpContent);
                switch (response.StatusCode)
                {
                    case (HttpStatusCode)500:
                        {
                            return NotFound();
                        }
                    case (HttpStatusCode)200:
                        {
                            TempData["success"] = "Dish Created Successfully";
                            
                            return RedirectToAction("Index","Dish");
                        }
                    default:
                        {
                            return NotFound();
                        }
                }
            }
        }
        #endregion

        #region  Edit Dish
        [HttpGet]
        public async Task<IActionResult> Update(Guid dishId)
        {
            string responseBody = "";
            DishDto dishViewModel = new();

            TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/dish/{dishId}");
            switch (response.StatusCode)
            {
                case (HttpStatusCode)500:
                    {
                        return NotFound();
                    }
                case (HttpStatusCode)200:
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        dishViewModel = JsonConvert.DeserializeObject<DishDto>(responseBody);
                        return View(dishViewModel);
                    }
                default:
                    {
                        return NotFound();
                    }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(DishDto dishDto)
        {
            string responseBody = "";
            Response statusMessage = new Response();

            using (var client = new HttpClient())
            {
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var jsonString = JsonConvert.SerializeObject(dishDto);
                HttpContent httpContent = new StringContent(jsonString);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                HttpResponseMessage response = await client.PutAsync($"https://localhost:7279/api/admin/update/{dishDto.Id}", httpContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["success"] = "Dish Updated Successfully";
                    return RedirectToAction("Details","Dish", new { id = dishDto.Id });
                }
                else
                {
                    TempData["error"] = "Error Occurs...";
                    return NotFound();
                }
            }
        }
        #endregion

        #region Delete Dish
        public async Task<IActionResult> Delete(Guid dishId)
        {
            using (var client = new HttpClient())
            {
                TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(User.Claims.First(w => w.Type == "token").Value);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);

                var jsonString = JsonConvert.SerializeObject(new { dishId = dishId });
                HttpContent httpContent = new StringContent(jsonString);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7279/api/admin/delete/{dishId}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["success"] = "Dish Deleted Successfully";
                    return RedirectToAction("Index", "Dish");
                }
                else
                {
                    TempData["error"] = "Error Occurs...";
                }
                return NotFound();
            }
        }
        #endregion
    }
}
