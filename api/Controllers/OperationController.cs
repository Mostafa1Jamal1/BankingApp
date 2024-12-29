using api.Models.DTOs;
using api.Models;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<string> GetBalanceById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = _db.Accounts.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok($"Balance: {account.Balance}");
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
                UpdatedAt = DateTime.Now,
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

            var FromAccount = _db.Accounts.Where(a => a.Id == data.From).FirstOrDefault();
            var ToAccount = _db.Accounts.Where(a => a.Id == data.To).FirstOrDefault();

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