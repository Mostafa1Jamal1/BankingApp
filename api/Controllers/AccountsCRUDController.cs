using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using api.Models;
using api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsCRUDController : ControllerBase
    {
        private readonly BankingDbContext _db;

        public AccountsCRUDController(BankingDbContext db)
        {
            _db = db;
        }
        [HttpGet(Name = "GetAllAccounts")]
        // GET api/v1/accounts
        public ActionResult<IEnumerable<AccountDTO>> GetAllAccounts()
        {
            var accounts = _db.Accounts.Select(a => new AccountDTO
            {
                AccountNumber = a.AccountNumber,
                AccountType = a.GetType().Name,
                Name = a.Name,
                Balance = a.Balance,
                Currency = a.Currency,
                Status = a.Status,
            });
            return Ok(accounts);
        }

        [HttpGet("{id:int}", Name = "GetAccountById")]
        // GET api/v1/accounts/{id}
        public ActionResult<AccountDTO> GetAccountById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = _db.Accounts.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            var accountDTO = new AccountDTO
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.GetType().Name,
                Name = account.Name,
                Balance = account.Balance,
                Currency = account.Currency,
                Status = account.Status,
            };
            return Ok(accountDTO);
        }

        [HttpPost(Name = "CreateAccount")]
        // POST api/v1/accounts/
        public ActionResult CreateAccount([FromBody] AccountDTO data)
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

        [HttpPut("{id:int}", Name = "UpdateAccount")]
        // PUT api/v1/accounts/{id}
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
        // DELETE api/v1/accounts/{id}
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
