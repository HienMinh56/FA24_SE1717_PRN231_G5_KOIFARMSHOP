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
        public async Task<List<KoiFish>> GetKois()
        {
            var kois = new List<KoiFish>();

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
                            kois = JsonConvert.DeserializeObject<List<KoiFish>>(result.Data.ToString());
                        }
                    }
                }
            }

            return kois;
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
                // Fetch the main Order data
                var orderResponse = await httpClient.GetAsync(Const.API_ENDPOINT + "Orders/" + id);
                if (orderResponse.IsSuccessStatusCode)
                {
                    var orderContent = await orderResponse.Content.ReadAsStringAsync();
                    var orderResult = JsonConvert.DeserializeObject<BusinessResult>(orderContent);
                    var order = JsonConvert.DeserializeObject<Order>(orderResult?.Data?.ToString() ?? "");

                    // Fetch OrderDetails for this specific Order ID
                    var detailsResponse = await httpClient.GetAsync(Const.API_ENDPOINT + "OrderDetails/" + id);
                    if (detailsResponse.IsSuccessStatusCode)
                    {
                        var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                        var detailsResult = JsonConvert.DeserializeObject<BusinessResult>(detailsContent);
                        var orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(detailsResult?.Data?.ToString() ?? "");

                        // Assign fetched OrderDetails to the Order object
                        order.OrderDetails = orderDetails;


                        return View(order);
                    }
                }
            }

            // Handle cases where either the order or details fetching fails
            return View(new Order());
        }

        // GET: OrderRazor/Create
        public async Task<IActionResult> Create()
        {
            // Initialize empty Order object
            var order = new CreateOrderRequest();

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
                            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "Email");
                            ViewData["VoucherId"] = new SelectList(await this.GetVouchers(), "VoucherId", "VoucherCode");
                            ViewData["KoiId"] = new SelectList(await this.GetKois(), "KoiId", "KoiName");
                            ViewBag.Kois = await this.GetKois();
                            return View(order);
                        }
                    }
                }
            }

            // Fallback if data fetching fails
            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "Email");
            ViewData["VoucherId"] = new SelectList(await this.GetVouchers(), "VoucherId", "VoucherCode");
            ViewData["KoiId"] = new SelectList(await this.GetKois(), "KoiId", "KoiName");
            ViewBag.Kois = await this.GetKois();
            return View(order);
        }

        // POST: OrderRazor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,TotalAmount,Quantity,Status,VoucherCode,ShippingAddress,PaymentMethod,DeliveryDate,Note,TotalWeight,OrderDetails")] CreateOrderRequest order)
        {
            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                ModelState.AddModelError("", "Order details are missing. Please add at least one item.");
            }
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Set API endpoint and send the full Order object, including OrderDetails
                    using (var response = await httpClient.PostAsJsonAsync(Const.API_ENDPOINT + "Orders", order))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result is not null && result.Status == Const.SUCCESS_CREATE_CODE)
                            {
                                // Success path - redirect to the index or success action
                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                // Handle API returned data or errors
                                ModelState.AddModelError("", "API returned an error: " + result?.Message);
                            }
                        }
                        else
                        {
                            // Handle HTTP errors
                            ModelState.AddModelError("", "Error creating order. Please try again.");
                        }
                    }
                }
            }

            // Repopulate ViewData for dropdowns if ModelState is invalid
            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "Email");
            ViewData["VoucherId"] = new SelectList(await this.GetVouchers(), "VoucherId", "VoucherCode");
            ViewData["KoiId"] = new SelectList(await this.GetKois(), "KoiId", "KoiName");
            ViewBag.Kois = await this.GetKois();
            return View(order);
        }


        // GET: OrderRazor/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            using (var httpClient = new HttpClient())
            {
                // Fetch the main Order data
                var orderResponse = await httpClient.GetAsync(Const.API_ENDPOINT + "Orders/" + id);
                if (orderResponse.IsSuccessStatusCode)
                {
                    var orderContent = await orderResponse.Content.ReadAsStringAsync();
                    var orderResult = JsonConvert.DeserializeObject<BusinessResult>(orderContent);
                    var order = JsonConvert.DeserializeObject<CreateOrderRequest>(orderResult?.Data?.ToString() ?? "");

                    // Fetch OrderDetails for this specific Order ID
                    var detailsResponse = await httpClient.GetAsync(Const.API_ENDPOINT + "OrderDetails/" + id);
                    if (detailsResponse.IsSuccessStatusCode)
                    {
                        var detailsContent = await detailsResponse.Content.ReadAsStringAsync();
                        var detailsResult = JsonConvert.DeserializeObject<BusinessResult>(detailsContent);
                        var orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(detailsResult?.Data?.ToString() ?? "");

                        // Assign fetched OrderDetails to the Order object
                        order.OrderDetails = orderDetails;

                        // Set ViewData for dropdowns
                        ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "Email");
                        ViewData["VoucherId"] = new SelectList(await this.GetVouchers(), "VoucherId", "VoucherCode");


                        return View(order);
                    }
                }
            }

            // Handle cases where either the order or details fetching fails
            return View();
        }


        // POST: OrderRazor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderId,UserId,TotalAmount,Quantity,Status,VoucherCode,ShippingAddress,PaymentMethod,DeliveryDate,ReceiveDate,Note,TotalWeight")] CreateOrderRequest order)
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
            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "Email", order.UserId);
            ViewData["VoucherId"] = new SelectList(await this.GetVouchers(), "VoucherId", "VoucherCode", order.VoucherCode);

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
