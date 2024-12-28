using System.Security.Principal;
using BankingAPI.Models.DTOs;
using api.Models;
using api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private readonly BankingDbContext _db;
        public TransactionsController(BankingDbContext db)
        {
            _db = db;
        }
        [HttpGet("all")]
        // GET api/transactions/all
        public ActionResult<List<Transaction>> GetAllTransaction()
        {
            return Ok(_db.Transactions);
        }
    }

    [ApiController]
    [Route("api/accounts")]
    public class TransactionController : Controller
    {
        private readonly BankingDbContext _db;
        public TransactionController(BankingDbContext db)
        {
            _db = db;
        }
        [HttpPost("deposite")]
        // POST api/accounts/deposite
        public ActionResult Deposite([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var account = _db.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefault();

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
            _db.Transactions.Add(transaction);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPost("withdraw")]
        // POST api/accounts/withdraw
        public ActionResult Withdraw([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var account = _db.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefault();

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
                UpdatedAt= DateTime.Now,
            };
            _db.Transactions.Add(transaction);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPost("transfer")]
        // POST api/accounts/transfer
        public ActionResult Transfer([FromBody] TransferDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var FromAccount = _db.Accounts.Where(a => a.AccountNumber == data.From).FirstOrDefault();
            var ToAccount = _db.Accounts.Where(a => a.AccountNumber == data.To).FirstOrDefault();

            if (FromAccount == null || ToAccount == null || data.Amount > FromAccount.Balance) return BadRequest();
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
            _db.Transactions.Add(transaction);
            _db.SaveChanges();
            return NoContent();
        }
    }
}