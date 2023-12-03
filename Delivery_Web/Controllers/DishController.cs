using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models;
using Delivery_Models.Models.Enum;
using Delivery_Models.ViewModels;
//using System.Web.Mvc;

namespace Delivery_Web.Controllers
{
    public class DishController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        
        #region Dish Menu(home page)
        [HttpGet]
        public async Task<IActionResult> Index(int id, DishCategory[] category, bool vegetarian, DishSorting sorting)//, int page = 1)
        {
            string categories = "";
            foreach (var items in category)
            {
                categories = string.Join("&", category.Select(c => $"category={c}"));//"categories =" + items.ToString() + "&&";
            }



            DishPagedListDto dishList = new DishPagedListDto();
            DishFilterVM filterVM = new DishFilterVM();
            filterVM.categories = category;
            filterVM.sorting = sorting;
            //filterVM.page = page;
            filterVM.vegetarian = vegetarian;

            HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/dish?{categories}&&sorting={sorting}&&vegetarian={vegetarian}&&page={id}");
            switch (response.StatusCode)
            {
                case (HttpStatusCode)500:
                    {
                        return NotFound();
                    }
                case (HttpStatusCode)200:
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        dishList = JsonConvert.DeserializeObject<DishPagedListDto>(responseBody);
                        filterVM.dishPagedListDto = dishList;
                        return View(filterVM);
                    }
                default:
                    {
                        return NotFound();
                    }
            }

        }

        [HttpPost]
        public async Task<IActionResult> Index(DishCategory[] category, DishSorting sorting, bool vegetarian, int page = 1)
        {
            string categories = "";
            foreach (var items in category)
            {
                categories = string.Join("&", category.Select(c => $"categories={c}"));
            }

            DishPagedListDto dishList = new DishPagedListDto();
            DishFilterVM filterVM = new DishFilterVM();
            filterVM.categories = category;
            filterVM.sorting = sorting;
            filterVM.page = page;
            filterVM.vegetarian = vegetarian;
            
                try
                {
                    //TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                    //var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                    //client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                    HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/dish?{categories}&sorting={sorting}&vegetarian={vegetarian}");
                    switch (response.StatusCode)
                    {
                        case (HttpStatusCode)500:
                            {
                                return NotFound();
                            }
                        case (HttpStatusCode)200:
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                dishList = JsonConvert.DeserializeObject<DishPagedListDto>(responseBody);
                                filterVM.dishPagedListDto = dishList;
                                return View(filterVM);
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
        #endregion

        #region Dish Details
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            string responseBody = "";
            DishDto dishViewModel = new();

            //TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
            //var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
            //client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/dish/{id}");
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
        #endregion

        #region Add Dish To Basket
        public async Task<IActionResult> AddToBasket(string dishId)
        {
            Response statusMessage = new Response();

            TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            var jsonString = JsonConvert.SerializeObject(new { id = dishId });
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


            HttpResponseMessage response = await client.PostAsync($"https://localhost:7279/api/basket/dish/{dishId}", httpContent);
            if (response.StatusCode == HttpStatusCode.OK)
            {

                TempData["success"] = "One Dish is added to Cart Successfully";
                return RedirectToAction("Index", "Basket");
            }
            else
            {
                TempData["error"] = "Error Occurs...";
            }
            return View(null);
        }
        #endregion
    }
}
