using System;
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

        public async Task<IActionResult> Index()
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
                            var data = JsonConvert.DeserializeObject<List<Payment>>(result.Data.ToString());
                            return View(data);
                        }
                    }
                }
            }

            return View(new List<Payment>());
        }
      

        [HttpGet]
        public IActionResult Create()
        {
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
                    ConsignmentId = paymentRequest.Type == 2 ? paymentRequest.ConsignmentId : null // type = 2
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
                            updatePayment.Amount = payment.Amount;
                            updatePayment.Status = payment.Status;
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
                // Retrieve payment information
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
                // Gửi yêu cầu xóa đến API
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
