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
    public class ConsignmentsRazorController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public ConsignmentsRazorController()
        {
        }

        public async Task<List<Consignment>> GetConsignments()
        {
            var consignments = new List<Consignment>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Consignments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            consignments = JsonConvert.DeserializeObject<List<Consignment>>(result.Data.ToString());
                        }
                    }
                }
            }

            return consignments;
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

        // GET: Consignments
        public async Task<IActionResult> Index()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Consignments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<List<Consignment>>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new List<Consignment>());
        }

        // GET: Consignments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Consignments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<Consignment>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new Consignment());
        }

        // GET: Consignments/Create
        public async Task<IActionResult> Create()
        {
            var consignmentTmp = new CreateConsignmentRequest();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Consignments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var consignment = JsonConvert.DeserializeObject<Consignment>(result.Data.ToString());

                            consignmentTmp.UserId = consignment.UserId;
                            consignmentTmp.KoiId = consignment.KoiId;
                            consignmentTmp.Type = consignment.Type;
                            consignmentTmp.DealPrice = consignment.DealPrice;
                            consignmentTmp.Method = consignment.Method;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "FullName");
            ViewData["KoiId"] = new SelectList(await this.GetKoiFishes(), "KoiId", "KoiId");
            return View(consignmentTmp);
        }

        // POST: Consignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateConsignmentRequest consignment)
        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PostAsJsonAsync(Const.APIEndpoint + "Consignments/", consignment))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result != null && result.Status == Const.SUCCESS_CREATE_CODE)
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

            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "FullName");
            ViewData["KoiId"] = new SelectList(await this.GetKoiFishes(), "KoiId", "KoiId");
            return View(consignment);
        }

        // GET: Consignments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var consignmentTmp = new UpdateConsignmentRequest();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Consignments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var consignment = JsonConvert.DeserializeObject<Consignment>(result.Data.ToString());
                            
                            consignmentTmp.ConsignmentId = consignment.ConsignmentId;
                            consignmentTmp.UserId = consignment.UserId;
                            consignmentTmp.KoiId = consignment.KoiId;
                            consignmentTmp.Type = consignment.Type;
                            consignmentTmp.DealPrice = consignment.DealPrice;
                            consignmentTmp.Method = consignment.Method;
                            consignmentTmp.Status = consignment.Status;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            
            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "UserId");
            ViewData["KoiId"] = new SelectList(await this.GetKoiFishes(), "KoiId", "KoiId");
            return View(consignmentTmp);
        }

        // POST: Consignments/Edit/5
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateConsignmentRequest consignment)
        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PutAsJsonAsync(Const.APIEndpoint + "Consignments/" + id, consignment))
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

            ViewData["ConsignmentId"] = new SelectList(await GetConsignments(), "ConsignmentId", consignment.ConsignmentId);
            return View(consignment);
        }

        // GET: Consignments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Consignments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<Consignment>(result.Data.ToString());

                            return View(data);
                        }
                    }
                }
            }

            return View(new Consignment());
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
                    using (var response = await httpClient.DeleteAsync(Const.APIEndpoint + "Consignments/" + id))
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