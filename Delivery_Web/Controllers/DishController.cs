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

        #region 菜品目录
        /// <summary>
        /// 菜品目录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id, DishCategory[] category, bool vegetarian, DishSorting sorting, int page = 1)
        {
            ViewBag.Category = category.ToString();
            ViewBag.Vegetarian = vegetarian;
            ViewBag.Sorting = sorting.ToString();
            string categories = "";
            foreach (var items in category)
            {
                 categories += string.Join("&", category.Select(c => $"category={c}"));//"categories =" + items.ToString() + "&&";
            }
             
            string responseBody = "";

            DishPagedListDto dishListModel = new DishPagedListDto();
            DishFilterVM filterVM = new DishFilterVM();
            filterVM.categories = category;
            filterVM.sorting = sorting;
                filterVM.page = page;
                filterVM.vegetarian = vegetarian;
            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/dish?{categories}&&sorting={sorting}&&vegetarian={vegetarian}&&page={id}");
                switch (response.StatusCode)
                {
                    case (HttpStatusCode)500:
                        {
                            return NotFound();
                        }
                    case (HttpStatusCode)200:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            dishListModel = JsonConvert.DeserializeObject<DishPagedListDto>(responseBody);
                            filterVM.dishPagedListDto = dishListModel;
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

        /// <summary>
        /// 菜品目录
        /// </summary>
        /// <param name="category"></param>
        /// <param name="vegetarian"></param>
        /// <param name="sorting"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(DishCategory[] category, bool vegetarian, DishSorting sorting, int page = 1)
        {
            ViewBag.Category = category.ToString();
            ViewBag.Vegetarian = vegetarian;
            ViewBag.Sorting = sorting.ToString();
            string categories = "";
            foreach (var items in category)
            {
                categories += string.Join("&", category.Select(c => $"category={c}"));
            }

            string responseBody = "";

            DishPagedListDto dishListModel = new DishPagedListDto();
            DishFilterVM filterVM = new DishFilterVM();
            filterVM.categories = category;
            filterVM.sorting = sorting;
            filterVM.page = page;
            filterVM.vegetarian = vegetarian;
            try
            {
                TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
                var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
                client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/dish?{categories}&sorting={sorting}&vegetarian={vegetarian}");
                switch (response.StatusCode)
                {
                    case (HttpStatusCode)500:
                        {
                            return NotFound();
                        }
                    case (HttpStatusCode)200:
                        {
                            responseBody = await response.Content.ReadAsStringAsync();
                            dishListModel = JsonConvert.DeserializeObject<DishPagedListDto>(responseBody);
                            filterVM.dishPagedListDto = dishListModel;
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

        #region 获取菜品详情
        /// <summary>
        /// 获取菜品详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            string responseBody = "";
            DishDto dishViewModel = new();

            try
            {
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
                            return View(dishViewModel);                        }
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

    }
}
