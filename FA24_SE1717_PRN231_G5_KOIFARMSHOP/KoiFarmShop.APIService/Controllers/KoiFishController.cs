using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Models;

namespace KoiFarmShop.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiFishController : ControllerBase
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public KoiFishController(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context)
        {
            _context = context;
        }

        // GET: api/KoiFish
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KoiFish>>> GetKoiFishes()
        {
            return await _context.KoiFishes.ToListAsync();
        }

        // GET: api/KoiFish/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KoiFish>> GetKoiFish(int id)
        {
            var koiFish = await _context.KoiFishes.FindAsync(id);

            if (koiFish == null)
            {
                return NotFound();
            }

            return koiFish;
        }

        // PUT: api/KoiFish/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKoiFish(int id, KoiFish koiFish)
        {
            if (id != koiFish.Id)
            {
                return BadRequest();
            }

            _context.Entry(koiFish).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KoiFishExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/KoiFish
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<KoiFish>> PostKoiFish(KoiFish koiFish)
        {
            _context.KoiFishes.Add(koiFish);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKoiFish", new { id = koiFish.Id }, koiFish);
        }

        // DELETE: api/KoiFish/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKoiFish(int id)
        {
            var koiFish = await _context.KoiFishes.FindAsync(id);
            if (koiFish == null)
            {
                return NotFound();
            }

            _context.KoiFishes.Remove(koiFish);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KoiFishExists(int id)
        {
            return _context.KoiFishes.Any(e => e.Id == id);
        }
    }
}
