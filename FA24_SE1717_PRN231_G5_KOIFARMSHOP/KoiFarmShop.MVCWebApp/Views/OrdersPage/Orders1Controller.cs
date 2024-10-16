using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Common;
using KoiFarmShop.Service.Base;
using Newtonsoft.Json;
using KoiFarmShop.Data.Request;

namespace KoiFarmShop.MVCWebApp.Views.OrdersPage
{
    public class Orders1Controller : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public Orders1Controller()
        {
        }
        public async Task<List<Order>> GetOrders()
        {
            var orders = new List<Order>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            orders = JsonConvert.DeserializeObject<List<Order>>(result.Data.ToString());
                        }
                    }
                }
            }
            
            return orders;
        }
        public async Task<List<User>> GetUsers()
        {
            var users = new List<User>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Users"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            users = JsonConvert.DeserializeObject<List<User>>(result.Data.ToString());
                        }
                    }
                }
            }

            return users;
        }
        public async Task<List<Voucher>> GetVouchers()
        {
            var vouchers = new List<Voucher>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Vouchers"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            vouchers = JsonConvert.DeserializeObject<List<Voucher>>(result.Data.ToString());
                        }
                    }
                }
            }

            return vouchers;
        }
        public async Task<List<KoiFish>> GetKoiFishes()
        {
            var koiFishes = new List<KoiFish>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "KoiFish"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            koiFishes = JsonConvert.DeserializeObject<List<KoiFish>>(result.Data.ToString());
                        }
                    }
                }
            }

            return koiFishes;
        }


        public async Task<IActionResult> Index()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<List<Order>>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }
            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "FullName");
            
            return View(new List<Order>());
        }

        public async Task<IActionResult> Details(string id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<Order>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new Order());
        }

        public async Task<IActionResult> Create()
        {
            var order = new Order();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var data = JsonConvert.DeserializeObject<List<Order>>(result.Data.ToString());

                            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "FullName");
                            ViewData["VoucherId"] = new SelectList(await this.GetVouchers(), "VoucherId", "VoucherCode");
                            ViewData["KoiId"] = new SelectList(await this.GetKoiFishes(), "KoiId", "KoiId");
                            return View();
                        }
                    }
                }
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string userId, string koiId, int quantity, string voucherCode)
        {
            var order = new OrderCreateRequest
            {
                UserId = userId,
                OrderItems = new List<OrderItem>
        {
            new OrderItem
            {
                KoiId = koiId,
                Quantity = quantity
            }
        },
                VoucherCode = voucherCode
            };

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsJsonAsync(Const.APIEndpoint + "Orders/", order);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Status == Const.SUCCESS_CREATE_CODE)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to create the order. Please try again later.");
                        }
                    }
                    else
                    {
                        // Thêm lỗi vào ModelState nếu phản hồi từ API không thành công
                        ModelState.AddModelError("", "An error occurred while communicating with the server.");
                    }
                }
            }

            // Nếu có lỗi hoặc ModelState không hợp lệ, trả lại view với Model và thông báo lỗi
            return View(order);
        }



        public async Task<IActionResult> Edit(string id)
        {
            var order = new Order();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders/" + id ))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            order = JsonConvert.DeserializeObject<Order>(result.Data.ToString());
                        }
                    }
                }
            }
            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "FullName");
            ViewData["VoucherId"] = new SelectList(await this.GetVouchers(), "VoucherId", "VoucherCode");
            return View(order);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Order order)
        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PutAsJsonAsync(Const.APIEndpoint + "Orders/" + id, order))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result != null && result.Status == Const.SUCCESS_UPDATE_CODE)
                            {
                                saveStatus = true;
                                // Success path logic here
                                return RedirectToAction(nameof(Index)); // Or another success action
                            }
                            else
                            {
                                saveStatus = false;
                            }
                        }
                    }
                }
            }

            if (saveStatus)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["OrderId"] = new SelectList(await this.GetOrders(), "OrderId", order.OrderId);
                return View(order);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<Order>(result.Data.ToString());

                            return View(data);
                        }
                    }
                }
            }

            return View(new Order());
        }

        // POST: Consignments/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            bool deleteStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.DeleteAsync(Const.APIEndpoint + "Orders/" + id))
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Status == Const.SUCCESS_DELETE_CODE)
                        {
                            deleteStatus = true;
                        }
                        else
                        {
                            deleteStatus = false;
                        }
                    }
                }
            }

            if (deleteStatus)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}
