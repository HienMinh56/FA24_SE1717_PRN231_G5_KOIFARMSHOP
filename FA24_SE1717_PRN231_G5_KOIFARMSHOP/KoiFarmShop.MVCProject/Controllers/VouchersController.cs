using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using static System.Net.WebRequestMethods;
using KoiFarmShop.Common;
using KoiFarmShop.Service.Base;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace KoiFarmShop.MVCProject.Controllers
{
    public class VouchersController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public VouchersController()
        {
            
        }
        //public VouchersController(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        //{
        //    _context = context;
        //}

        // GET: Vouchers
        public async Task<IActionResult> Index()
        {
            //return View(await _context.KoiFishes.ToListAsync());
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
                            var data = JsonConvert.DeserializeObject<List<Voucher>>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }
            return View(new Voucher());
        }

        // GET: Vouchers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Vouchers/"+id))
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
        public async Task <IActionResult> Create()
        {
            var Voucher = new Voucher();
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
                            var vouchers = JsonConvert.DeserializeObject<List<Voucher>>(result.Data.ToString());
                            ViewData["VoucherCode"] = new SelectList(vouchers, "VoucherCode","voucherCode");

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
        public async Task<IActionResult> Create([Bind("Id,VoucherId,VoucherCode,DiscountAmount,MinOrderAmount,Status,ValidityStartDate,ValidityEndDate,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] Voucher voucher)
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
                                var data = JsonConvert.DeserializeObject<Voucher>(result.Data.ToString());

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
            var Voucher = new Voucher();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.API_ENDPOINT + "Vouchers/"+id))
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

            return View();
        }

        // POST: Vouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,VoucherId,VoucherCode,DiscountAmount,MinOrderAmount,Status,ValidityStartDate,ValidityEndDate,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] Voucher voucher)
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
                                var data = JsonConvert.DeserializeObject<Voucher>(result.Data.ToString());

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
                using (var response = await httpClient.DeleteAsync(Const.API_ENDPOINT + "Vouchers/"+ id))
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
