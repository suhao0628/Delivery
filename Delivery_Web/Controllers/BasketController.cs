using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models;
using Delivery_Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Delivery_Web.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        #region Basket List
        public async Task<IActionResult> Index()
        {
            List<DishBasketDto> dishInBasket = new();
            using (var client = new HttpClient())
            {
                var token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "token")?.Value;
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(User.Claims.FirstOrDefault(w => w.Type == "token").Value);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/basket");

                string responseBody = await response.Content.ReadAsStringAsync();
                dishInBasket = JsonConvert.DeserializeObject<List<DishBasketDto>>(responseBody);
                return View(dishInBasket);

            }
        }

        #endregion

        #region Remove/Decrease dish
        public async Task<IActionResult> DeleteDish(string dishId, bool increase)
        {
            using (var client = new HttpClient())
            {
                TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(User.Claims.First(w => w.Type == "token").Value);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);

                var jsonString = JsonConvert.SerializeObject(new { dishId = dishId, increase = true });
                HttpContent httpContent = new StringContent(jsonString);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7279/api/basket/dish/{dishId}?increase={increase}");

                if (response.StatusCode == HttpStatusCode.OK)
                {

                    TempData["success"] = "Delete Successfully";
                    return RedirectToAction("Index", "Basket");
                }
                else
                {
                    TempData["error"] = "Error Occurs...";
                }
                return View(null);
            }
        }
        #endregion

        #region Generate Order
        /// 
        [HttpGet]
        public async Task<IActionResult> GenerateOrder()
        {
            using (var client = new HttpClient())
            {
                GenerateOrderVM generateOrderVM = new GenerateOrderVM();
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(User.Claims.First(w => w.Type == "token").Value);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                HttpResponseMessage responseBasket = await client.GetAsync($"https://localhost:7279/api/basket");
                HttpResponseMessage responseProfile = await client.GetAsync("https://localhost:7279/api/account/profile");

                string responseBodyBasket = await responseBasket.Content.ReadAsStringAsync();
                generateOrderVM.DishBasketDtos = JsonConvert.DeserializeObject<List<DishBasketDto>>(responseBodyBasket);

                string responseBodyProfile = await responseProfile.Content.ReadAsStringAsync();
                generateOrderVM.UserDto = JsonConvert.DeserializeObject<UserDto>(responseBodyProfile);
                return View(generateOrderVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateOrder(DateTime deliveryTime, string address)
        {
            OrderCreateDto orderCreate = new OrderCreateDto();
            orderCreate.DeliveryTime = deliveryTime;
            orderCreate.Address = address;


            var jsonString = JsonConvert.SerializeObject(orderCreate);
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using (var client = new HttpClient())
            {
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(User.Claims.First(w => w.Type == "token").Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                HttpResponseMessage response = await client.PostAsync("https://localhost:7279/api/order", httpContent);

                switch (response.StatusCode)
                {
                    case (HttpStatusCode)500:
                        {
                            return NotFound();
                        }
                    case (HttpStatusCode)200:
                        {


                            return RedirectToAction("Index", "Order");
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
