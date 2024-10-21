using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Common;
using KoiFarmShop.Service.Base;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using KoiFarmShop.Data.Request;

namespace KoiFarmShop.MVCWebApp.Controllers
{
    public class VouchersController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public VouchersController()
        {

        }
        // GET: Vouchers
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, double? DiscountAmount = null, string voucherId = null, int? Status = null)
        {
            List<Voucher> data = new List<Voucher>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Vouchers"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result is not null)
                            {
                                data = JsonConvert.DeserializeObject<List<Voucher>>(result.Data.ToString());

                            }
                        }
                    }
                }

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return View(new List<Voucher>());
            }
            if (!string.IsNullOrEmpty(voucherId))
                data = data.Where(x => x.VoucherId.Contains(voucherId)).ToList();
            if (DiscountAmount.HasValue && DiscountAmount >= 0 && DiscountAmount <= 100)
                data = data.Where(x => x.DiscountAmount.Equals(DiscountAmount)).ToList();
            if (Status.HasValue)
                data = data.Where(x => x.Status == Status).ToList();

            return View(data); // Fix: Pass the data as IEnumerable<Voucher> to the View
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


        // GET: Vouchers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Vouchers/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var data = JsonConvert.DeserializeObject<Voucher>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new Voucher());
        }

        // GET: Vouchers/Create
        public async Task<IActionResult> Create()
        {
            var Voucher = new CreateVoucherRequest();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Vouchers"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var vouchers = JsonConvert.DeserializeObject<List<CreateVoucherRequest>>(result.Data.ToString());
                            ViewData["VoucherCode"] = new SelectList(vouchers, "VoucherCode", "voucherCode");

                            return View();
                        }
                    }
                }
            }

            return View();
        }

        // POST: Vouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVoucherRequest voucher)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PostAsJsonAsync(Const.API_ENDPOINT + "Vouchers", voucher))
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
                                var data = JsonConvert.DeserializeObject<CreateVoucherRequest>(result.Data.ToString());

                                return View(data); // Display the returned voucher data
                            }
                        }
                        else
                        {
                            // Handle the error from the API here
                            ModelState.AddModelError("", "Error creating voucher.");
                        }
                    }
                }
            }

            // If we get here, something went wrong, so return the view with the voucher data
            return View(voucher);
        }


        // GET: Vouchers/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            var Voucher = new CreateVoucherRequest();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Vouchers/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var data = JsonConvert.DeserializeObject<CreateVoucherRequest>(result.Data.ToString());


                            return View(data);
                        }
                    }
                }
            }

            return View();
        }

        // POST: Vouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CreateVoucherRequest voucher)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PutAsJsonAsync(Const.API_ENDPOINT + "Vouchers", voucher))
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
                                var data = JsonConvert.DeserializeObject<CreateVoucherRequest>(result.Data.ToString());

                                return View(data); // Display the returned voucher data
                            }
                        }
                        else
                        {
                            // Handle the error from the API here
                            ModelState.AddModelError("", "Error edit voucher.");
                        }
                    }
                }
            }

            // If we get here, something went wrong, so return the view with the voucher data
            return View(voucher);
        }

        // GET: Vouchers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Vouchers/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var data = JsonConvert.DeserializeObject<Voucher>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new Voucher());
        }

        // POST: Vouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync(Const.API_ENDPOINT + "Vouchers/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(new Voucher());
        }

        private bool VoucherExists(string id)
        {
            return _context.Vouchers.Any(e => e.VoucherId == id);
        }
    }
}
