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
        public async Task<IActionResult> Index()
        {
            //return View(await _context.KoiFishes.ToListAsync());
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Const.APIEndpoint + "KoiFish"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<BusinessResult>(content);
                        if (result is not null && result.Data is not null)
                        {
                            var data = JsonConvert.DeserializeObject<List<KoiFish>>
                                (result.Data.ToString());

                            return View(data);
                        }
                    }
                }
            }
            return View(new KoiFish());
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
        public async Task<IActionResult> Create(CreateKoiFishRequest koiFish)
        {
            bool saveStatus = false;
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.PostAsJsonAsync(Const.APIEndpoint + $"KoiFish", koiFish))
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

            ViewBag.Images = new SelectList(await this.GetImages(), "Url", "ImageId");

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
                    using (var response = await httpClient.PutAsJsonAsync(Const.APIEndpoint + $"KoiFish", koiFish))
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
            return View(id);
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
    }
}
