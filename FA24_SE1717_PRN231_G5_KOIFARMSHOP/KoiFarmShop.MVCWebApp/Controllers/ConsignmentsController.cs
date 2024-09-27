using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;
using KoiFarmShop.APIService.Controllers;
using KoiFarmShop.Service;
using KoiFarmShop.Common;
using Newtonsoft.Json;
using KoiFarmShop.Service.Base;

namespace KoiFarmShop.MVCWebApp.Controllers
{
    public class ConsignmentsController : Controller
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public ConsignmentsController(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consignment = await _context.Consignments
                .Include(c => c.Koi)
                .Include(c => c.Payment)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consignment == null)
            {
                return NotFound();
            }

            return View(consignment);
        }

        // GET: Consignments/Create
        //public IActionResult Create()
        //{
        //    ViewData["KoiId"] = new SelectList(_context.KoiFishes, "KoiId", "Breed");
        //    ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId");
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
        //    return View();
        //}

        // POST: Consignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ConsignmentId,UserId,KoiId,Type,DealPrice,Method,PaymentId,Status,ConsignmentDate,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] Consignment consignment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(consignment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["KoiId"] = new SelectList(_context.KoiFishes, "KoiId", "Breed", consignment.KoiId);
        //    ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", consignment.PaymentId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", consignment.UserId);
        //    return View(consignment);
        //}

        // GET: Consignments/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var consignment = await _context.Consignments.FindAsync(id);
        //    if (consignment == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["KoiId"] = new SelectList(_context.KoiFishes, "KoiId", "Breed", consignment.KoiId);
        //    ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", consignment.PaymentId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", consignment.UserId);
        //    return View(consignment);
        //}

        // POST: Consignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ConsignmentId,UserId,KoiId,Type,DealPrice,Method,PaymentId,Status,ConsignmentDate,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] Consignment consignment)
        //{
        //    if (id != consignment.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(consignment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ConsignmentExists(consignment.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["KoiId"] = new SelectList(_context.KoiFishes, "KoiId", "Breed", consignment.KoiId);
        //    ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", consignment.PaymentId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", consignment.UserId);
        //    return View(consignment);
        //}

        // GET: Consignments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var consignment = await _context.Consignments
        //        .Include(c => c.Koi)
        //        .Include(c => c.Payment)
        //        .Include(c => c.User)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (consignment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(consignment);
        //}

        // POST: Consignments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var consignment = await _context.Consignments.FindAsync(id);
        //    if (consignment != null)
        //    {
        //        _context.Consignments.Remove(consignment);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
