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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string ConsignmentId = null, string Method = null, int? Status = null)
        {
            List<Consignment> data = new List<Consignment>();
            try
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
                                data = JsonConvert.DeserializeObject<List<Consignment>>(result.Data.ToString());
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return View(new List<Consignment>());
            }

            // Filter the data
            if (!string.IsNullOrEmpty(ConsignmentId))
                data = data.Where(x => x.ConsignmentId.Contains(ConsignmentId)).ToList();
            if (!string.IsNullOrEmpty(Method))
                data = data.Where(x => x.Method.Contains(Method)).ToList();
            if (Status.HasValue)
                data = data.Where(x => x.Status == Status).ToList();

            // Calculate pagination
            int totalItems = data.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass pagination and filter data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.ConsignmentId = ConsignmentId;
            ViewBag.Method = Method;
            ViewBag.Status = Status;

            return View(paginatedData);
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
                            // Deserialize result.Data as a list of Consignments
                            var consignments = JsonConvert.DeserializeObject<List<Consignment>>(result.Data.ToString());
                            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "Email");
                            ViewData["KoiId"] = new SelectList(await this.GetKoiFishes(), "KoiId", "KoiName");
                            return View();
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            
            return View(consignmentTmp);
        }

        // POST: Consignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateConsignmentRequest consignment)
        {
            if (ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Hoặc ghi vào log
                }

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.PostAsJsonAsync(Const.APIEndpoint + "Consignments", consignment))
                    {
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
                                ModelState.AddModelError("", "Failed to create the consignment. Please try again later.");
                            }
                        }
                        else
                        {
                            // Thêm lỗi vào ModelState nếu phản hồi từ API không thành công
                            ModelState.AddModelError("", "An error occurred while communicating with the server.");
                        }
                    }
                }
            }

            ViewData["UserId"] = new SelectList(await this.GetUsers(), "UserId", "Email");
            ViewData["KoiId"] = new SelectList(await this.GetKoiFishes(), "KoiId", "KoiName");
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
                            consignmentTmp.Note = consignment.Note;
                            consignmentTmp.CustomerContact = consignment.CustomerContact;
                            consignmentTmp.CustomerAddress = consignment.CustomerAddress;
                            consignmentTmp.TotalWeight = consignment.TotalWeight;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "Email");
            ViewData["KoiId"] = new SelectList(await this.GetKoiFishes(), "KoiId", "KoiName");
            return View(consignmentTmp);
        }

        // POST: Consignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateConsignmentRequest consignment)
        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    // Properly send the voucher in the body of the POST request
                    using (var response = await httpClient.PutAsJsonAsync(Const.APIEndpoint + "Consignments", consignment))
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
                                ModelState.AddModelError("", "Failed to edit the consignment. Please try again later.");
                            }
                        }
                        else
                        {
                            // Thêm lỗi vào ModelState nếu phản hồi từ API không thành công
                            ModelState.AddModelError("", "An error occurred while communicating with the server.");
                        }
                    }
                }
            }

            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "Email");
            ViewData["ConsignmentId"] = new SelectList(await GetConsignments(), "ConsignmentId", consignment.ConsignmentId);
            return View(consignment);
        }

        // GET: Consignments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                // Retrieve consignment information
                var response = await httpClient.GetAsync(Const.APIEndpoint + "Consignments/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                    if (result != null && result.Data != null)
                    {
                        var consignment = JsonConvert.DeserializeObject<Consignment>(result.Data.ToString());

                        return View(consignment);
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
                var response = await httpClient.DeleteAsync(Const.APIEndpoint + "Consignments/" + id);
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