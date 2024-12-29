using System.Security.Principal;
using BankingAPI.Models.DTOs;
using api.Models;
using api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/transactions")]
    public class TransactionsCRUDController : Controller
    {
        private readonly BankingDbContext _db;
        public TransactionsCRUDController(BankingDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        // GET api/v1/transactions
        public async Task<ActionResult<List<Transaction>>> GetAllTransactionAsync()
        {
            return Ok(await _db.Transactions.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetTransactionById")]
        // GET api/v1/transactions/{id}
        public async Task<ActionResult<Transaction>> GetTransactionByIdAsync(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var transaction = await _db.Transactions.Where(n => n.Id == id).FirstOrDefaultAsync();
            // If account not found - Client Error
            if (transaction == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok(transaction);
        }

        [HttpPost(Name = "CreateTransaction")]
        // POST api/v1/transactions
        public async Task<ActionResult> CreateTransactionAsync([FromBody] TransactionDTO data)
        {
            if (data == null)
                return BadRequest("Error: Need to provide account data!");

            var transaction = new Transaction
            {
                Type = data.Type,
                Status = data.Status,
                Amount = data.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                FromAccountId = data.FromAccountId,
                ToAccountId = data.ToAccountId,
            };
            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
            return Ok("Transaction created Successfuly");
        }

        [HttpPut("{id:int}", Name = "UpdateTransaction")]
        // PUT api/v1/transactions/{id}
        public async Task<ActionResult> UpdateTransactionAsync(int id, [FromBody] TransactionDTO data)
        {
            if (id <= 0)
                return BadRequest("Id can not be negative");
            if (data == null)
                return BadRequest("No data provided to update");
            var transaction = await _db.Transactions.Where(_ => _.Id == id).FirstOrDefaultAsync();
            if (transaction == null) return NotFound($"No transaction found with Id: {id}");
            transaction.Type = data.Type;
            transaction.Status = data.Status;
            transaction.Amount = data.Amount;
            transaction.CreatedAt = DateTime.Now;
            transaction.UpdatedAt = DateTime.Now;
            transaction.FromAccountId = data.FromAccountId;
            transaction.ToAccountId = data.ToAccountId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteTransactionById")]
        // DELETE api/v1/transaction/{id}
        public async Task<ActionResult> DeleteTransactionByIdAsync(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var transaction = await _db.Transactions.Where(n => n.Id == id).FirstOrDefaultAsync();
            // If account not found - Client Error
            if (transaction == null)
                return NotFound($"No transaction found with Id: {id}");
            // Removed Successfully
            _db.Transactions.Remove(transaction);
            await _db.SaveChangesAsync();
            return Ok($"Transaction with id: {id} deleted successfuly");
        }
    }
}

    