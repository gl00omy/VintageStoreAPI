using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VintageStore.Data;

namespace VintageStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VintageStoreController : ControllerBase
    {
        private readonly DataContext _context;
        public VintageStoreController(DataContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<ActionResult<List<VintageStore>>> GetVintageStore()
        {
            return Ok(await _context.VintageStores.ToListAsync());
        }
        [HttpPost]
        public async Task<ActionResult<List<VintageStore>>> CreateVintageStore(VintageStore store)
        {
            _context.VintageStores.Add(store);
            await _context.SaveChangesAsync();
            return Ok(await _context.VintageStores.ToListAsync());
        }
        [HttpPut]
        public async Task<ActionResult<List<VintageStore>>> UpdateVintageStore(VintageStore store) 
        { 
             var dbStore = await _context.VintageStores.FindAsync(store.Id);
            if(dbStore == null) 
                return BadRequest("Item not found");
            
            dbStore.FirstName = store.FirstName;
            dbStore.LastName = store.LastName;  
            dbStore.address = store.address;
            dbStore.foodName = store.foodName;
            dbStore.totalPrice = store.totalPrice;

            await _context.SaveChangesAsync();
            return Ok(await _context.VintageStores.ToListAsync());
            
        }
        [HttpDelete("{id}")]
        
        public async Task<ActionResult<List<VintageStore>>> DeleteVintageStore( int id)
        {
            var dbStore = await _context.VintageStores.FindAsync(id);
            if (dbStore == null)
                return BadRequest("Item not found");
            _context.VintageStores.Remove(dbStore);
            await _context.SaveChangesAsync();
            return Ok(await _context.VintageStores.ToListAsync());
        }
    }
          
}

