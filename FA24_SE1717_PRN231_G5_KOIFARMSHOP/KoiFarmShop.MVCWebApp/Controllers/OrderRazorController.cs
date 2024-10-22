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
    public class OrderRazorController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public OrderRazorController(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
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

        // GET: OrderRazor
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string OrderId = null, string ShippingAddress = null, int? Status = null)
        {
            List<Order> data = new List<Order>();
            try
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
                                data = JsonConvert.DeserializeObject<List<Order>>(result.Data.ToString());
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return View(new List<Order>());
            }

            // Filter the data
            if (!string.IsNullOrEmpty(OrderId))
                data = data.Where(x => x.OrderId.Contains(OrderId)).ToList();
            if (!string.IsNullOrEmpty(ShippingAddress))
                data = data.Where(x => x.ShippingAddress.Contains(ShippingAddress)).ToList();
            if (Status.HasValue)
                data = data.Where(x => x.Status == Status).ToList();

            // Calculate pagination
            int totalItems = data.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass pagination and filter data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.OrderId = OrderId;
            ViewBag.ShippingAddress = ShippingAddress;
            ViewBag.Status = Status;

            return View(paginatedData);
        }

        // GET: OrderRazor/Details/5
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

        // GET: OrderRazor/Create
        public async Task<IActionResult> Create()
        {
            var Order = new Order();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Orders"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var orders = JsonConvert.DeserializeObject<List<Order>>(result.Data.ToString());
                            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
                            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "VoucherId", "VoucherCode");

                            return View();
                        }
                    }
                }
            }

            return View();
        }

        // POST: OrderRazor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,TotalAmount,Quantity,Status,VoucherId,ShippingAddress,PaymentMethod,DeliveryDate,Note,TotalWeight")] Order order)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PostAsJsonAsync(Const.API_ENDPOINT + "Orders", order))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result is not null && result.Status == Const.SUCCESS_CREATE_CODE)
                            {
                                // Success path logic here
                                return RedirectToAction(nameof(Index)); // Or another success action
                            }
                            else if (result is not null && result.Data != null)
                            {
                                var data = JsonConvert.DeserializeObject<Order>(result.Data.ToString());

                                return View(data); // Display the returned voucher data
                            }
                        }
                        else
                        {
                            // Handle the error from the API here
                            ModelState.AddModelError("", "Error creating order.");
                        }
                    }
                }
            }

            // If we get here, something went wrong, so return the view with the voucher data
            return View(order);
        }

        // GET: OrderRazor/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            var Order = new Order();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Orders/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var data = JsonConvert.DeserializeObject<Order>(result.Data.ToString());
                            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
                            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "VoucherId", "VoucherCode");
                            return View(data);
                        }
                    }
                }
            }

            return View();
        }

        // POST: OrderRazor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderId,UserId,TotalAmount,Quantity,Status,VoucherId,ShippingAddress,PaymentMethod,DeliveryDate,ReceiveDate,Note,TotalWeight")] Order order)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PutAsJsonAsync(Const.API_ENDPOINT + "Orders", order))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result is not null && result.Status == Const.SUCCESS_UPDATE_CODE)
                            {
                                // Success path logic here
                                return RedirectToAction(nameof(Index)); // Or another success action
                            }
                            else if (result is not null && result.Data != null)
                            {
                                var data = JsonConvert.DeserializeObject<Order>(result.Data.ToString());

                                return View(data); // Display the returned voucher data
                            }
                        }
                        else
                        {
                            // Handle the error from the API here
                            ModelState.AddModelError("", "Error edit order.");
                        }
                    }
                }
            }

            // If we get here, something went wrong, so return the view with the voucher data
            return View(order);
        }

        // GET: OrderRazor/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Orders/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var data = JsonConvert.DeserializeObject<Order>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new Order());
        }

        // POST: OrderRazor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync(Const.API_ENDPOINT + "Orders/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(new Order());
        }

        private bool OrderExists(string id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
