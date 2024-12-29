using System.Security.Principal;
using BankingAPI.Models.DTOs;
using api.Models;
using api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;

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
        public ActionResult<List<Transaction>> GetAllTransaction()
        {
            return Ok(_db.Transactions);
        }

        [HttpGet("{id:int}", Name = "GetTransactionById")]
        // GET api/v1/transactions/{id}
        public ActionResult<Transaction> GetTransactionById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var transaction = _db.Transactions.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (transaction == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok(transaction);
        }

        [HttpPost(Name = "CreateTransaction")]
        // POST api/v1/transactions
        public ActionResult CreateTransaction([FromBody] TransactionDTO data)
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
            _db.Transactions.Add(transaction);
            _db.SaveChanges();
            return Ok("Transaction created Successfuly");
        }

        [HttpPut("{id:int}", Name = "UpdateTransaction")]
        // PUT api/v1/transactions/{id}
        public ActionResult UpdateTransaction(int id, [FromBody] TransactionDTO data)
        {
            if (id <= 0)
                return BadRequest("Id can not be negative");
            if (data == null)
                return BadRequest("No data provided to update");
            var transaction = _db.Transactions.Where(_ => _.Id == id).FirstOrDefault();
            if (transaction == null) return NotFound($"No transaction found with Id: {id}");
            transaction.Type = data.Type;
            transaction.Status = data.Status;
            transaction.Amount = data.Amount;
            transaction.CreatedAt = DateTime.Now;
            transaction.UpdatedAt = DateTime.Now;
            transaction.FromAccountId = data.FromAccountId;
            transaction.ToAccountId = data.ToAccountId;
            _db.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteTransactionById")]
        // DELETE api/v1/transaction/{id}
        public ActionResult DeleteTransactionById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var transaction = _db.Transactions.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (transaction == null)
                return NotFound($"No transaction found with Id: {id}");
            // Removed Successfully
            _db.Transactions.Remove(transaction);
            _db.SaveChanges();
            return Ok($"Transaction with id: {id} deleted successfuly");
        }
    }
}

    