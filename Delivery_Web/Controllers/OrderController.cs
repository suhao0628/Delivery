using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models;
using Delivery_Models.ViewModels;

namespace Delivery_Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        static readonly HttpClient client = new HttpClient();

        #region Order List
        public async Task<IActionResult> Index()
        {
            string responseBody = "";

            List<OrderDto> orderListViewModels = new List<OrderDto>();

            TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/order");
            switch (response.StatusCode)
            {
                case (HttpStatusCode)500:
                    {
                        return NotFound();
                    }
                case (HttpStatusCode)200:
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        orderListViewModels = JsonConvert.DeserializeObject<List<OrderDto>>(responseBody);
                        return View(orderListViewModels);
                    }
                default:
                    {
                        return NotFound();
                    }
            }

        }
        #endregion

        #region Order Details
        public async Task<IActionResult> Details(string id)
        {
            string responseBody = "";

            OrderDto orderDetailsViewModel = new OrderDto();


            TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7279/api/order/{id}");
            switch (response.StatusCode)
            {
                case (HttpStatusCode)500:
                    {
                        return NotFound();
                    }
                case (HttpStatusCode)200:
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        orderDetailsViewModel = JsonConvert.DeserializeObject<OrderDto>(responseBody);
                        return View(orderDetailsViewModel);
                    }
                default:
                    {
                        return NotFound();
                    }
            }

        }
        #endregion

        #region Confirm Delivery
        public async Task<IActionResult> ConfirmDelivery(Guid orderId)
        {
            Response statusMessage = new Response();

            TokenResponse tokenJsonViewModel = JsonConvert.DeserializeObject<TokenResponse>(HttpContext.User.Claims.Where(w => w.Type == "token").First().Value);
            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", tokenJsonViewModel.Token);
            client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

            var jsonString = JsonConvert.SerializeObject(new { id = orderId });
            HttpContent httpContent = new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


            HttpResponseMessage response = await client.PostAsync($"https://localhost:7279/api/order/{orderId}/status", httpContent);

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
        #endregion

    }
}
