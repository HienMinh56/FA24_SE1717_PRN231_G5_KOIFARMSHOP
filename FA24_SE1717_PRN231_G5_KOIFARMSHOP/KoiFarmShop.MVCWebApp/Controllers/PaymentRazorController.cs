﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using KoiFarmShop.Data.Models;
using KoiFarmShop.Common;
using KoiFarmShop.Service.Base;
using KoiFarmShop.Data.Repository;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Request;
using System.Text;
using Newtonsoft.Json.Linq;

namespace KoiFarmShop.MVCWebApp.Controllers
{
    public class PaymentRazorController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;
        public PaymentRazorController()
        {
        }

        public async Task<List<Payment>> GetPayments()
        {
            var payments = new List<Payment>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Payments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            payments = JsonConvert.DeserializeObject<List<Payment>>(result.Data.ToString());
                        }
                    }
                }
            }

            return payments;
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

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string? PaymentId = null, string? UserId = null, 
                                               string? Type = null, string? PaymentMethod = null, string? Refundable = null, string? Currency = null)
        {
            List<Payment> data = new List<Payment>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Payments"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                            if (result != null && result.Data != null)
                            {
                                data = JsonConvert.DeserializeObject<List<Payment>>(result.Data.ToString());
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return View(new List<Payment>());
            }

            // Filter the data
            if (!string.IsNullOrEmpty(PaymentId))
                data = data.Where(x => x.PaymentId.Contains(PaymentId)).ToList();

            if (!string.IsNullOrEmpty(UserId))
                data = data.Where(x => x.UserId.Contains(UserId)).ToList();

            if (!string.IsNullOrEmpty(PaymentMethod))
                data = data.Where(x => x.PaymentMethod != null && x.PaymentMethod.Contains(PaymentMethod)).ToList();

            int? typeRefundable = Refundable?.ToLower() switch
            {
                "yes" => 1,
                "no" => 2,
                _ => null
            };

            if (typeRefundable.HasValue)
            {
                data = data.Where(x => x.Refundable == typeRefundable.Value).ToList();
            }

            if (!string.IsNullOrEmpty(Currency))
            {
                data = data.Where(x => x.Currency != null && x.Currency.Contains(Currency)).ToList();
            }

            int? typeValue = Type?.ToLower() switch
            {
                "order" => 1,
                "consignments" => 2,
                _ => null 
            };

            if (typeValue.HasValue)
                data = data.Where(x => x.Type == typeValue.Value).ToList();
           

            data = data.OrderByDescending(x => x.Id).ToList();

            // Calculate pagination
            int totalItems = data.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PaymentId = PaymentId;
            ViewBag.UserId = UserId;
            ViewBag.Type = Type;
            ViewBag.PaymentMethod = PaymentMethod;
            ViewBag.Refundable = Refundable;
            ViewBag.Currency = Currency;

            return View(paginatedData);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var order = new Order();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Payments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null)
                        {
                            var data = JsonConvert.DeserializeObject<List<Order>>(result.Data.ToString());

                            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "UserId");
                            ViewData["OrderId"] = new SelectList(await GetOrders(), "OrderId", "OrderId");
                            ViewData["ConsignmentId"] = new SelectList(await GetConsignments(), "ConsignmentId", "ConsignmentId");
                            return View();
                        }
                    }
                }
            }

            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePaymentRequest paymentRequest)
        {
            bool isValid = false;

            if (paymentRequest.Type == 1) // check payment
            {
                
                using (var httpClient = new HttpClient())
                {
                    var orderResponse = await httpClient.GetAsync($"{Const.APIEndpoint}Orders/{paymentRequest.OrderId}");
                    if (orderResponse.IsSuccessStatusCode)
                    {
                        var content = await orderResponse.Content.ReadAsStringAsync();
                        var order = JsonConvert.DeserializeObject<Order>(content);
                        isValid = true; 
                    }
                }
            } 
            else 
            {
                using (var httpClient = new HttpClient())
                {
                    var consignmentResponse = await httpClient.GetAsync($"{Const.APIEndpoint}Consignments/{paymentRequest.ConsignmentId}");
                    if (consignmentResponse.IsSuccessStatusCode)
                    {
                        var content = await consignmentResponse.Content.ReadAsStringAsync();
                        var consignment = JsonConvert.DeserializeObject<Consignment>(content);
                        isValid = true;
                    }
                }
            }

            if (!isValid)
            {
                ModelState.AddModelError("", "ID not found.");
                return View(paymentRequest);
            }

            using (var httpClient = new HttpClient())
            {
                var paymentToCreate = new CreatePaymentRequest
                {
                    Type = paymentRequest.Type,
                    OrderId = paymentRequest.Type == 1 ? paymentRequest.OrderId : null, // type 1
                    ConsignmentId = paymentRequest.Type == 2 ? paymentRequest.ConsignmentId : null, // type = 2
                    CreatedDate = paymentRequest.CreatedDate,
                    Status = paymentRequest.Status,
                    Currency = paymentRequest.Currency,
                    PaymentMethod = paymentRequest.PaymentMethod,
                    Refundable = paymentRequest.Refundable,
                    Note = paymentRequest.Note,
                };

                var jsonContent = JsonConvert.SerializeObject(paymentToCreate);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(Const.APIEndpoint + "Payments", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(responseContent);

                        if (result != null && result.Status == Const.SUCCESS_CREATE_CODE) // check success
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to create payment. Please try again later.");
                        }
                    }
                }
            }
            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "UserId");
            ViewData["OrderId"] = new SelectList(await GetOrders(), "OrderId", "OrderId");
            ViewData["ConsignmentId"] = new SelectList(await GetConsignments(), "ConsignmentId", "ConsignmentId");
            return View(paymentRequest);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var updatePayment = new UpdatePaymentRequest();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Payments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var payment = JsonConvert.DeserializeObject<Payment>(result.Data.ToString());

                            updatePayment.PaymentId = payment.PaymentId;
                            updatePayment.UserId = payment.UserId;
                            updatePayment.Amount = payment.Amount;
                            updatePayment.Status = payment.Type;
                            updatePayment.PaymentMethod = payment.PaymentMethod;
                            updatePayment.Currency = payment.Currency;
                            updatePayment.Refundable = payment.Refundable;
                            updatePayment.Note = payment.Note;
                            updatePayment.CreatedDate = (DateTime)payment.CreatedDate;
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "UserId");
            return View(updatePayment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdatePaymentRequest payment)
        {
            bool saveStatus = false;

            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.PutAsJsonAsync(Const.APIEndpoint + "Payments", payment))
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
                                ModelState.AddModelError("", "Failed to edit the payment. Please try again later.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "An error occurred while communicating with the server.");
                        }
                    }
                }
            }

            ViewData["UserId"] = new SelectList(await GetUsers(), "UserId", "UserId");
            ViewData["PaymentId"] = new SelectList(await GetPayments(), "PaymentId", payment.PaymentId);
            return View(payment);
        }

        public async Task<IActionResult> Details(string id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Payments/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                        if (result != null && result.Data != null)
                        {
                            var data = JsonConvert.DeserializeObject<Payment>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new Payment());
        }

        public async Task<IActionResult> Delete(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(Const.APIEndpoint + "Payments/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<BusinessResult>(content);

                    if (result != null && result.Data != null)
                    {
                        var payment = JsonConvert.DeserializeObject<Payment>(result.Data.ToString());
                        return View(payment);
                    }
                }
            }

            // If not successful, redirect to a list or error page
            TempData["ErrorMessage"] = "Failed to retrieve payment details.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(Const.APIEndpoint + "Payments/" + id);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Payment deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete the payment.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

    }
}
