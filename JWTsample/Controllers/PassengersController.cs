using JWTsample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTsample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class PassengersController : ControllerBase
    {
        private readonly AddDbContext _context;

        public PassengersController(AddDbContext context)
        {
            _context = context;
        }

        // GET: api/Passengers
        [HttpGet]
        [Authorize(Roles ="Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Passengers>>> GetPassengers()
        {
            return await _context.Passengers.ToListAsync();
        }

        // GET: api/Passengers/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<Passengers>> GetPassengers(int id)
        {
            var passengers = await _context.Passengers.FindAsync(id);

            if (passengers == null)
            {
                return NotFound();
            }

            return passengers;
        }

        // PUT: api/Passengers/5
        [Authorize(Roles = "Admin")]
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPassengers(int id, Passengers passengers)
        {
            if (id != passengers.PassengersId)
            {
                return BadRequest();
            }

            _context.Entry(passengers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PassengersExists(id))
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

        // POST: api/Passengers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Passengers>> PostPassengers(Passengers passengers)
        {
            _context.Passengers.Add(passengers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassengers", new { id = passengers.PassengersId }, passengers);
        }

        // DELETE: api/Passengers/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassengers(int id)
        {
            var passengers = await _context.Passengers.FindAsync(id);
            if (passengers == null)
            {
                return NotFound();
            }

            _context.Passengers.Remove(passengers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PassengersExists(int id)
        {
            return _context.Passengers.Any(e => e.PassengersId == id);
        }
    }
}
