using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using api.Models;
using api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly BankingDbContext _db;

        public AccountsController(BankingDbContext db)
        {
            _db = db;
        }
        [HttpGet(Name = "GetAllAccounts")]
        // GET api/accounts
        public ActionResult<IEnumerable<Account>> GetAllAccounts()
        {
            return Ok(_db.Accounts);
        }

        [HttpGet("{id:int}", Name = "GetAccountById")]
        // GET api/accounts/{id}
        public ActionResult<Account> GetAccountById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = _db.Accounts.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok(account);
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

        [HttpPost("create", Name = "CreateAccount")]
        // POST api/accounts/create
        public ActionResult<Account> CreateAccount([FromBody] AccountDTO data)
        {
            if (data == null)
                return BadRequest("Error: Need to provide account data!");
            // No need for this as long as [APIController] attribute exist
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            var ExistAccount = _db.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefault();
            if (ExistAccount != null) return BadRequest($"Can bot use this Account number: {data.AccountNumber}");
            Account NewAccount;
            switch (data.AccountType.ToLower())
            {
                case "savings":
                    NewAccount = new SavingsAccount
                    {
                        Name = data.Name,
                        AccountNumber = data.AccountNumber,
                        Balance = data.Balance,
                        Currency = data.Currency,
                        Status = data.Status,
                        InterestRate = 0.15
                    };
                    break;

                case "checking":
                    NewAccount = new CheckingAccount
                    {
                        Name = data.Name,
                        AccountNumber = data.AccountNumber,
                        Balance = data.Balance,
                        Currency = data.Currency,
                        Status = data.Status,
                        OverdraftLimit = 500m
                    };
                    break;

                default:
                    throw new ArgumentException($"Invalid account type: {data.AccountType}");
            }
            _db.Accounts.Add(NewAccount);
            _db.SaveChanges();
            return Ok("Account created Successfuly");
        }

        [HttpPut("{id:int}/update", Name = "UpdateAccount")]
        // PUT api/accounts/{id}/update
        public ActionResult UpdateAccount(int id, [FromBody] AccountDTO data)
        {
            if (id <= 0)
                return BadRequest("Id can not be negative");
            if (data == null)
                return BadRequest("No data provided to update");
            var account = _db.Accounts.Where(n => n.Id == id).FirstOrDefault();
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            account.Balance = data.Balance;
            account.AccountNumber = data.AccountNumber;
            account.Name = data.Name;
            account.Status = data.Status;
            account.Currency = data.Currency;
            account.UpdatedAt = DateTime.Now;
            _db.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteAccountById")]
        // DELETE api/accounts/{id}
        public ActionResult DeleteAccountById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = _db.Accounts.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Removed Successfully
            _db.Accounts.Remove(account);
            _db.SaveChanges();
            return Ok($"Account with id: {id} deleted successfuly");
        }
    }
}
