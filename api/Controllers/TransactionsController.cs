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
        [HttpGet("all")]
        // GET api/transactions/all
        public ActionResult<List<Transaction>> GetAllTransaction()
        {
            return Ok(BankRepositry.Transactions);
        }
    }

    [ApiController]
    [Route("api/accounts")]
    public class TransactionController : Controller
    {
        [HttpPost("deposite")]
        // POST api/accounts/deposite
        public ActionResult Deposite([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var account = BankRepositry.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefault();

            if (account == null) return NotFound();

            account.Balance += data.Amount;
            // ToDo: Create transaction and save it
            var transaction = new Transaction
            {
                Id = BankRepositry.Transactions.Count(),
                Type = "Deposite",
                DeductedFromId = "Cash",
                AddedToId = account.AccountNumber,
                Amount = data.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            BankRepositry.Transactions.Add(transaction);
            return NoContent();
        }

        [HttpPost("withdraw")]
        // POST api/accounts/withdraw
        public ActionResult Withdraw([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var account = BankRepositry.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefault();

            if (account == null) return NotFound();

            if (data.Amount > account.Balance) return BadRequest("No enough balance to withdraw");

            account.Balance -= data.Amount;
            // Create transaction and save it
            var transaction = new Transaction
            {
                Id = BankRepositry.Transactions.Count(),
                Type = "Withdraw",
                DeductedFromId = account.AccountNumber,
                AddedToId = "Cash",
                Amount = data.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt= DateTime.Now,
            };
            BankRepositry.Transactions.Add(transaction);
            return NoContent();
        }

        [HttpPost("transfer")]
        // POST api/accounts/transfer
        public ActionResult Transfer([FromBody] TransferDTO data)
        {
            if (data == null || data.Amount < 0) return BadRequest();

            var FromAccount = BankRepositry.Accounts.Where(a => a.AccountNumber == data.From).FirstOrDefault();
            var ToAccount = BankRepositry.Accounts.Where(a => a.AccountNumber == data.To).FirstOrDefault();

            if (FromAccount == null || ToAccount == null || data.Amount > FromAccount.Balance) return BadRequest();

            FromAccount.Balance -= data.Amount;
            ToAccount.Balance += data.Amount;
            // ToDo: Create a transaction and save it
            var transaction = new Transaction
            {
                Id = BankRepositry.Transactions.Count(),
                Type = "Transfer",
                DeductedFromId = FromAccount.AccountNumber,
                AddedToId = ToAccount.AccountNumber,
                Amount = data.Amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            BankRepositry.Transactions.Add(transaction);
            return NoContent();
        }
    }
}