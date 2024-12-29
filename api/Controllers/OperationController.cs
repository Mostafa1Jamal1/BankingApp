using api.Models.DTOs;
using api.Models;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class OperationController : Controller
    {
        private readonly BankingDbContext _db;

        public OperationController(BankingDbContext db)
        {
            _db = db;

        }

        [HttpGet("{id:int}/balance", Name = "GetBalanceById")]
        // GET api/accounts/{id}/balance
        public async Task<ActionResult<string>> GetBalanceByIdAsync(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = await _db.Accounts.Where(n => n.Id == id).FirstOrDefaultAsync();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok($"Balance: {account.Balance}");
        }

        [HttpPost("deposite")]
        // POST api/accounts/deposite
        public async Task<ActionResult> DepositeAsync([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var account = await _db.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefaultAsync();

            if (account == null) return NotFound();

            account.Balance += data.Amount;
            // ToDo: Create transaction and save it
            var transaction = new Transaction
            {
                Type = "Deposite",
                Status = "Done",
                ToAccountId = account.Id,
                Amount = data.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("withdraw")]
        // POST api/accounts/withdraw
        public async Task<ActionResult> WithdrawAsync([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var account = await _db.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefaultAsync();

            if (account == null) return NotFound();
            // ToDo: make logic for different types of accounts
            if (data.Amount > account.Balance) return BadRequest("No enough balance to withdraw");

            account.Balance -= data.Amount;
            // Create transaction and save it
            var transaction = new Transaction
            {
                Type = "Withdraw",
                Status = "Done",
                FromAccountId = account.Id,
                Amount = data.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("transfer")]
        // POST api/accounts/transfer
        public async Task<ActionResult> TransferAsync([FromBody] TransferDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var FromAccount = await _db.Accounts.Where(a => a.Id == data.From).FirstOrDefaultAsync();
            var ToAccount = await _db.Accounts.Where(a => a.Id == data.To).FirstOrDefaultAsync();

            if (FromAccount == null || ToAccount == null || data.Amount > FromAccount.Balance)
                return BadRequest();
            // ToDo: add logic for different types of accounts
            FromAccount.Balance -= data.Amount;
            ToAccount.Balance += data.Amount;

            var transaction = new Transaction
            {
                Type = "Transfer",
                Status = "Done",
                FromAccountId = FromAccount.Id,
                ToAccountId = ToAccount.Id,
                Amount = data.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    } 
}