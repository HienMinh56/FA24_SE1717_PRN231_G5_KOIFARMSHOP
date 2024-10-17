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

namespace KoiFarmShop.MVCWebApp.Controllers
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

            return View(new List<Order>());
        }

        public async Task<IActionResult> Details(string id)
        {
            using (var httpClient = new HttpClient())
            {
                // Lấy thông tin đơn hàng
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var order = JsonConvert.DeserializeObject<Order>(result.Data.ToString());

                            // Lấy danh sách chi tiết đơn hàng
                            using (var detailsResponse = await httpClient.GetAsync(Const.APIEndpoint + "OrderDetails/" + id))
                            {
                                if (detailsResponse.IsSuccessStatusCode)
                                {
                                    var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                                    var detailsResult = JsonConvert.DeserializeObject<BusinessResult>(detailsContent);

                                    if (detailsResult != null && detailsResult.Data != null)
                                    {
                                        order.OrderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(detailsResult.Data.ToString());
                                    }
                                }
                            }

                            return View(order);
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

                            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "FullName");
                            ViewData["VoucherId"] = new SelectList(await GetVouchers(), "VoucherId", "VoucherCode");
                            ViewData["KoiId"] = new SelectList(await GetKoiFishes(), "KoiId", "KoiId");
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
            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "FullName", userId);
            ViewData["VoucherId"] = new SelectList(await GetVouchers(), "VoucherId", "VoucherCode", voucherCode);
            ViewData["KoiId"] = new SelectList(await GetKoiFishes(), "KoiId", "KoiId", koiId);
            return View(order);
        }



        public async Task<IActionResult> Edit(string id)
        {
            var updateOrderRequest = new UpdateOrderRequest();

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
                            var order = JsonConvert.DeserializeObject<Order>(result.Data.ToString());
                            // Chuyển đổi từ Order sang UpdateOrderRequest
                            updateOrderRequest.OrderId = order.OrderId;
                            updateOrderRequest.Status = order.Status;
                            updateOrderRequest.VoucherId = order.VoucherId;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "FullName");
            ViewData["VoucherId"] = new SelectList(await GetVouchers(), "VoucherId", "VoucherCode");

            return View(updateOrderRequest);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateOrderRequest updateOrderRequest)
        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Send the updateOrderRequest model in the PUT request
                    using (var response = await httpClient.PutAsJsonAsync(Const.APIEndpoint + "Orders", updateOrderRequest))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result != null && result.Status == Const.SUCCESS_UPDATE_CODE)
                            {
                                saveStatus = true;
                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                saveStatus = false;
                            }
                        }


                    }
                }
            }


            // Reload dropdowns if there is an error
            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "FullName");
            ViewData["VoucherId"] = new SelectList(await GetVouchers(), "VoucherId", "VoucherCode");

            return View(updateOrderRequest);
        }




        public async Task<IActionResult> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                // Retrieve order information
                var response = await httpClient.GetAsync(Const.APIEndpoint + "Orders/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                    if (result != null && result.Data != null)
                    {
                        var order = JsonConvert.DeserializeObject<Order>(result.Data.ToString());

                        // Retrieve order details
                        var detailsResponse = await httpClient.GetAsync(Const.APIEndpoint + "OrderDetails/" + id);
                        if (detailsResponse.IsSuccessStatusCode)
                        {
                            var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                            var detailsResult = JsonConvert.DeserializeObject<BusinessResult>(detailsContent);

                            if (detailsResult != null && detailsResult.Data != null)
                            {
                                order.OrderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(detailsResult.Data.ToString());
                            }
                        }

                        return View(order);
                    }
                }
            }

            // If not successful, redirect to a list or error page
            return RedirectToAction(nameof(Index));
        }




        // POST: Consignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(Const.APIEndpoint + "Orders/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                    if (result != null && result.Status == Const.SUCCESS_DELETE_CODE)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            // If delete fails, show the delete view again
            return RedirectToAction(nameof(Delete), new { id });
        }

    }
}
