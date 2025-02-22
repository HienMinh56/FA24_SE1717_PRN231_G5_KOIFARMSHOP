﻿using System;
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

namespace KoiFarmShop.MVC.Controllers
{
    public class KoiFishController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;
        public KoiFishController()
        {

        }

        public async Task<List<Image>> GetImages()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Images"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null && result.Data is not null)
                        {
                            var data = JsonConvert.DeserializeObject<List<Image>>
                                (result.Data.ToString());

                            return data;
                        }
                    }
                    return null;
                }
            }
        }
        public async Task<List<KoiFish>> GetKoiFish()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "Koifish"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null && result.Data is not null)
                        {
                            var data = JsonConvert.DeserializeObject<List<KoiFish>>
                                (result.Data.ToString());

                            return data;
                        }
                    }
                    return null;
                }
            }
        }

        //public KoiFishController(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        //{
        //    _context = context;
        //}

        // GET: KoiFish
        public async Task<IActionResult> Index(string koiname, string gender, double minPrice, double maxPrice)
        {
            //return View(await _context.KoiFishes.ToListAsync());
            using (var httpClient = new HttpClient())
            {
                var url = $"{Const.APIEndpoint}Koifish/odata?$filter="
                    + (koiname == null ? "" : $"contains(KoiName, '{koiname}') and ")
                    + (gender == null ? "" : $"Gender eq '{gender}' and ")
                    + (minPrice == 0 ? "" : $"Price ge {minPrice} and ");
                if (maxPrice == 0)
                {
                    if (url.EndsWith("="))
                    {
                        url = $"{Const.APIEndpoint}Koifish/odata";
                    }
                    else
                    {
                        url = url.Trim().Remove(url.Length - 4);
                    }
                }
                else
                {
                    url += $"Price le {maxPrice}";
                }
                using (var response = await httpClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<List<KoiFish>>(content);
                        if (data is not null)
                        {
                            return View(data);
                        }

                        //var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        //if (result is not null && result.Data is not null)
                        //{
                        //    var data = JsonConvert.DeserializeObject<List<KoiFish>>
                        //        (result.Data.ToString());

                        //    return View(data);
                        //}
                    }
                }
            }
            return View(new List<KoiFish>());
        }

        // GET: KoiFish/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var koiFish = await _context.KoiFishes
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (koiFish == null)
            //{
            //    return NotFound();
            //}

            //return View(koiFish);

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + $"KoiFish/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null && result.Data is not null)
                        {
                            var data = JsonConvert.DeserializeObject<KoiFish>
                                (result.Data.ToString());

                            return View(data);
                        }
                    }
                }
            }
            return View(new KoiFish());
        }

        // GET: KoiFish/Create
        public async Task<IActionResult> Create()
        {
            //return View();

            ViewBag.Images = new SelectList(await this.GetImages(), "Id", "ImageId");
            return View();
        }

        // POST: KoiFish/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateKoiFishRequest koiFish)
        {
            bool saveStatus = false;
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    var formData = new MultipartFormDataContent();

                    formData.Add(new StringContent(koiFish.KoiName ?? ""), "KoiName");
                    formData.Add(new StringContent(koiFish.Origin ?? ""), "Origin");
                    formData.Add(new StringContent(koiFish.Gender ?? ""), "Gender");
                    formData.Add(new StringContent(koiFish.Age.ToString()), "Age");
                    formData.Add(new StringContent(koiFish.Size.ToString()), "Size");
                    formData.Add(new StringContent(koiFish.Breed ?? ""), "Breed");
                    formData.Add(new StringContent(koiFish.Type ?? ""), "Type");
                    formData.Add(new StringContent(koiFish.Price.ToString()), "Price");
                    formData.Add(new StringContent(koiFish.Quantity.ToString()), "Quantity");
                    formData.Add(new StringContent(koiFish.OwnerType.ToString()), "OwnerType");
                    formData.Add(new StringContent(koiFish.Description ?? ""), "Description");

                    if (koiFish.Image != null && koiFish.Image.Count > 0)
                    {
                        foreach (var image in koiFish.Image)
                        {
                            if (image.Length > 0)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await image.CopyToAsync(memoryStream);
                                    var imageContent = new ByteArrayContent(memoryStream.ToArray());
                                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                                    formData.Add(imageContent, "Image", image.FileName);
                                }
                            }
                        }
                    }
                    using (var response = await httpClient.PostAsync(Const.APIEndpoint + $"KoiFish", formData))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                            if (result is not null && result.Data is not null)
                            {
                                saveStatus = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine(response);
                        }
                    }
                }
            }
            if (saveStatus)
            {
                return RedirectToAction(nameof(Index));

            }
            return View(new CreateKoiFishRequest());
        }

        // GET: KoiFish/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var koiFish = await _context.KoiFishes.FindAsync(id);
            //if (koiFish == null)
            //{
            //    return NotFound();
            //}
            //return View(koiFish);

            //var availableImages = new List<SelectListItem>
            //{

            //}

            ViewBag.Images = new SelectList((await this.GetImages()).Where(i => i.KoiId == id), "Url", "ImageId");

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + $"KoiFish/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null && result.Data is not null)
                        {
                            var data = JsonConvert.DeserializeObject<UpdateKoiFishRequest>
                                (result.Data.ToString());

                            return View(data);
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: KoiFish/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateKoiFishRequest koiFish)
        {
            //if (id != koiFish.Id)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(koiFish);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!KoiFishExists(koiFish.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(koiFish);

            bool saveStatus = false;
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    var formData = new MultipartFormDataContent();

                    formData.Add(new StringContent(koiFish.KoiId ?? ""), "KoiId");
                    formData.Add(new StringContent(koiFish.KoiName ?? ""), "KoiName");
                    formData.Add(new StringContent(koiFish.Origin ?? ""), "Origin");
                    formData.Add(new StringContent(koiFish.Gender ?? ""), "Gender");
                    formData.Add(new StringContent(koiFish.Age.ToString()), "Age");
                    formData.Add(new StringContent(koiFish.Size.ToString()), "Size");
                    formData.Add(new StringContent(koiFish.Breed ?? ""), "Breed");
                    formData.Add(new StringContent(koiFish.Type ?? ""), "Type");
                    formData.Add(new StringContent(koiFish.Price.ToString()), "Price");
                    formData.Add(new StringContent(koiFish.Quantity.ToString()), "Quantity");
                    formData.Add(new StringContent(koiFish.OwnerType.ToString()), "OwnerType");
                    formData.Add(new StringContent(koiFish.Description ?? ""), "Description");

                    if (koiFish.AddedImage != null && koiFish.AddedImage.Count > 0)
                    {
                        foreach (var image in koiFish.AddedImage)
                        {
                            if (image.Length > 0)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await image.CopyToAsync(memoryStream);
                                    var imageContent = new ByteArrayContent(memoryStream.ToArray());
                                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                                    formData.Add(imageContent, "AddedImage", image.FileName);
                                }
                            }
                        }
                    }
                    if (koiFish.RemovedImage != null && koiFish.RemovedImage.Count > 0)
                    {
                        foreach (var image in koiFish.RemovedImage)
                        {
                            formData.Add(new StringContent(image), "RemovedImage");
                        }
                    }
                    using (var response = await httpClient.PutAsync(Const.APIEndpoint + $"KoiFish", formData))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                            if (result is not null && result.Data is not null)
                            {
                                saveStatus = true;
                            }
                        }
                    }
                }
            }
            if (saveStatus)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Edit), new { id });
        }


        // GET: KoiFish/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var koiFish = await _context.KoiFishes
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (koiFish == null)
            //{
            //    return NotFound();
            //}

            //return View(koiFish);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + $"KoiFish/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null && result.Data is not null)
                        {
                            var data = JsonConvert.DeserializeObject<KoiFish>
                                (result.Data.ToString());

                            return View(data);
                        }
                    }
                }
            }
            return View(new KoiFish());
        }

        // POST: KoiFish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //var koiFish = await _context.KoiFishes.FindAsync(id);
            //if (koiFish != null)
            //{
            //    _context.KoiFishes.Remove(koiFish);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync(Const.APIEndpoint + $"KoiFish/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(new KoiFish());
        }

        private bool KoiFishExists(int id)
        {
            return _context.KoiFishes.Any(e => e.Id == id);
        }

        //public async Task<IActionResult> Search(string koiname, string gender, double minPrice, double maxPrice)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var response = await httpClient.GetAsync($"{Const.APIEndpoint}Koifish/odata?$  "
        //            + (koiname == null ? "" : $"filter=contains(Name, '{koiname}') and ")
        //            + (gender == null ? "" : $"Gender eq '{gender}' and ")
        //            + (minPrice == 0 ? "" : $"and Price ge {minPrice} and ")
        //            + (maxPrice == 0 ? "\b\b\b\b" : $"Price le {maxPrice}")))
        //        {
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                var result = JsonConvert.DeserializeObject<BusinessResult>(content);
        //                if (result is not null && result.Data is not null)
        //                {
        //                    var data = JsonConvert.DeserializeObject<List<KoiFish>>
        //                        (result.Data.ToString());

        //                    return RedirectToAction(nameof(Index),data);
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //}
    }
}
